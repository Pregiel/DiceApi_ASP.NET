using AutoMapper;
using DiceApi.Dtos;
using DiceApi.Services;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceApi.Hubs
{
    public class RoomHub : Hub
    {
        private readonly IRoomService _roomService;
        private readonly IUserService _userService;
        private readonly IRollService _rollService;
        private IMapper _mapper;

        public RoomHub(IRoomService roomService,
            IUserService userService,
            IRollService rollService,
            IMapper mapper)
        {
            _roomService = roomService;
            _userService = userService;
            _rollService = rollService;
            _mapper = mapper;
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
            var room = await _roomService.GetById(roomId);

            if (room == null)
            {
                await Clients.Caller.SendAsync("InvalidRoom", roomId);
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, roomGroup);
            await Clients.OthersInGroup(roomGroup).SendAsync("UserJoined", Context.ConnectionId);

            var roomDetails = _mapper.Map<RoomDetailsDto>(room);
            await Clients.Caller.SendAsync("RoomDetails", roomDetails);

            var rolls = await _rollService.GetRoomRolls(room);
            var rollDtos = _mapper.Map<IList<RollDto>>(rolls);

            await Clients.Caller.SendAsync("RollList", rollDtos);
        }

        public async Task LeaveRoom(int roomId)
        {
            var roomGroup = "room_" + roomId;
            var room = await _roomService.GetById(roomId);

            if (room == null)
            {
                await Clients.Caller.SendAsync("InvalidRoom", roomId);
                return;
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomGroup);
            await Clients.OthersInGroup(roomGroup).SendAsync("UserLeaved", Context.ConnectionId);
        }

        public async Task UpdateRollList(int roomId)
        {
            var roomGroup = "room_" + roomId;
            var room = await _roomService.GetById(roomId);

            if (room == null)
            {
                await Clients.Caller.SendAsync("InvalidRoom", roomId);
                return;
            }

            var rolls = await _rollService.GetRoomRolls(room);
            var rollDtos = _mapper.Map<IList<RollDto>>(rolls);

            await Clients.Group(roomGroup).SendAsync("RollList", rollDtos);
        }


    }
}
