using AutoMapper;
using DiceApi.Dtos;
using DiceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceApi.Hubs
{
    [Authorize]
    public class RoomHub : Hub
    {
        private readonly IRoomService _roomService;
        private readonly IUserService _userService;
        private readonly IRollService _rollService;
        private readonly IMapper _mapper;

        private static Dictionary<int, ICollection<UserInfoDto>> _onlineGroupUsers;


        public RoomHub(IRoomService roomService,
            IUserService userService,
            IRollService rollService,
            IMapper mapper)
        {
            _roomService = roomService;
            _userService = userService;
            _rollService = rollService;
            _mapper = mapper;

            if (_onlineGroupUsers == null)
                _onlineGroupUsers = new Dictionary<int, ICollection<UserInfoDto>>();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = _userService.GetById(Int32.Parse(Context.User.Identity.Name));

            foreach (var entry in _onlineGroupUsers)
            {
                var userInGroup = entry.Value.SingleOrDefault(x => x.Id == user.Id);
                if (userInGroup != null)
                {
                    var roomGroup = "room_" + entry.Key;

                    _onlineGroupUsers[entry.Key].Remove(userInGroup);
                    await Clients.OthersInGroup(roomGroup).SendAsync("UserLeaved", userInGroup);

                    var onlineUsers = _onlineGroupUsers.GetValueOrDefault(entry.Key, new List<UserInfoDto>());
                    await Clients.OthersInGroup(roomGroup).SendAsync("UsersOnlineList", onlineUsers);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendRoom(int roomId, string message)
        {
            var roomGroup = "room_" + roomId;
            var room = _roomService.GetById(roomId);

            if (room == null)
            {
                await Clients.Caller.SendAsync("InvalidRoom", roomId);
                return;
            }

            await Clients.Group(roomGroup).SendAsync("Send", message);
        }

        public async Task JoinRoom(int roomId)
        {
            var roomGroup = "room_" + roomId;
            var room = _roomService.GetById(roomId);

            if (room == null)
            {
                await Clients.Caller.SendAsync("InvalidRoom", roomId);
                return;
            }

            var user = _userService.GetById(Int32.Parse(Context.User.Identity.Name));

            if (user == null)
            {
                await Clients.Caller.SendAsync("Unauthorized");
                return;
            }

            var userInfo = _mapper.Map<UserInfoDto>(user);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomGroup);
            await Clients.OthersInGroup(roomGroup).SendAsync("UserJoined", userInfo);

            var roomDetails = _mapper.Map<RoomDetailsDto>(room);
            await Clients.Caller.SendAsync("RoomDetails", roomDetails);

            var rolls = _rollService.GetRoomRolls(room);
            var rollDtos = _mapper.Map<IList<RollDto>>(rolls);

            await Clients.Caller.SendAsync("RollList", rollDtos);

            var onlineUsers = _onlineGroupUsers.GetValueOrDefault(roomId, new List<UserInfoDto>());
            if (!onlineUsers.Any(x => x.Id == userInfo.Id))
                onlineUsers.Add(userInfo);

            if (_onlineGroupUsers.ContainsKey(roomId))
                _onlineGroupUsers[roomId] = onlineUsers;
            else
                _onlineGroupUsers.Add(roomId, onlineUsers);

            await Clients.Group(roomGroup).SendAsync("UsersOnlineList", onlineUsers);
        }

        public async Task LeaveRoom(int roomId)
        {
            var roomGroup = "room_" + roomId;
            var room = _roomService.GetById(roomId);

            if (room == null)
            {
                await Clients.Caller.SendAsync("InvalidRoom", roomId);
                return;
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomGroup);

            if (_onlineGroupUsers.ContainsKey(roomId))
            {
                var user = _userService.GetById(Int32.Parse(Context.User.Identity.Name));

                if (user == null)
                {
                    await Clients.Caller.SendAsync("Unauthorized");
                    return;
                }

                var userInfo = _mapper.Map<UserInfoDto>(user);
                await Clients.OthersInGroup(roomGroup).SendAsync("UserLeaved", userInfo);

                var onlineUsers = _onlineGroupUsers.GetValueOrDefault(roomId, new List<UserInfoDto>());
                var userToRemove = onlineUsers.SingleOrDefault(x => x.Id == user.Id);
                if (userToRemove == null)
                    return;

                onlineUsers.Remove(userToRemove);
                _onlineGroupUsers[roomId] = onlineUsers;

                await Clients.OthersInGroup(roomGroup).SendAsync("UsersOnlineList", onlineUsers);

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomGroup);
            }
        }

        public async Task UpdateRollList(int roomId)
        {
            var roomGroup = "room_" + roomId;
            var room = _roomService.GetById(roomId);

            if (room == null)
            {
                await Clients.Caller.SendAsync("InvalidRoom", roomId);
                return;
            }

            var rolls = _rollService.GetRoomRolls(room);
            var rollDtos = _mapper.Map<IList<RollDto>>(rolls);

            await Clients.Group(roomGroup).SendAsync("RollList", rollDtos);
        }

        public async Task UserOnline(int roomId)
        {
            var roomGroup = "room_" + roomId;
            var room = _roomService.GetById(roomId);
            if (room == null)
            {
                await Clients.Caller.SendAsync("InvalidRoom", roomId);
                return;
            }

            var user = _userService.GetById(Int32.Parse(Context.User.Identity.Name));
            if (user == null)
            {
                await Clients.Caller.SendAsync("Unauthorized");
                return;
            }
            var userInfo = _mapper.Map<UserInfoDto>(user);

            var onlineUsers = _onlineGroupUsers.GetValueOrDefault(roomId, new List<UserInfoDto>());
            if (onlineUsers.Any(x => x.Id == userInfo.Id))
                return;
            else
                onlineUsers.Add(userInfo);

            if (_onlineGroupUsers.ContainsKey(roomId))
                _onlineGroupUsers[roomId] = onlineUsers;
            else
                _onlineGroupUsers.Add(roomId, onlineUsers);

            await Clients.Group(roomGroup).SendAsync("UsersOnlineList", onlineUsers);
        }
    }
}
