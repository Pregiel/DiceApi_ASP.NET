using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DiceApi.Dtos;
using DiceApi.Entities;
using DiceApi.Helpers;
using DiceApi.Hubs;
using DiceApi.Services;
using DiceApi.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace DiceApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IUserService _userService;
        private readonly IUserRoomService _userRoomService;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public RoomsController(IRoomService roomService,
            IUserService userService,
            IUserRoomService userRoomService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _roomService = roomService;
            _userService = userService;
            _userRoomService = userRoomService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        // GET: api/rooms?page=1&limit=5
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll([FromQuery]int page, [FromQuery]int limit)
        {
            if (page < 1)
                page = 1;

            IEnumerable<Room> rooms = _roomService.GetAll();
            var size = rooms.Count();
            if (limit > 0)
            {
                rooms = rooms
                    .Skip((page - 1) * limit)
                    .Take(limit);
            }

            var roomDtos = _mapper.Map<IList<RoomInfoDto>>(rooms);
            foreach (RoomInfoDto roomInfo in roomDtos)
            {
                roomInfo.OnlineClientAmount = RoomHub.GetOnlineGroupUsersAmount(roomInfo.Id);
            }

            return Ok(new
            {
                rooms = roomDtos,
                size
            });
        }

        // POST: api/rooms
        [HttpPost]
        public IActionResult CreateRoom([FromBody]RoomDto roomDto)
        {
            var room = _mapper.Map<Room>(roomDto);
            var user = _userService.GetById(Int32.Parse(User.Identity.Name));

            if (user == null)
                return Unauthorized();

            var validator = new RoomValidator();
            var result = validator.Validate(roomDto);

            try
            {
                if (!result.IsValid)
                    throw new ApplicationException(string.Join(",", result.Errors));

                var returnedRoom = _roomService.Create(room, roomDto.Password);

                _userRoomService.Create(user, room, true);

                return Ok(returnedRoom);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/rooms/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(Int32.Parse(User.Identity.Name));

            if (user == null)
                return Unauthorized();

            var room = _roomService.GetById(id);

            if (room == null)
                return BadRequest(Properties.resultMessages.RoomNotFound);

            if (_userRoomService.GetByIds(user.Id, id) == null)
                return BadRequest(Properties.resultMessages.UserRoomNotFound);

            var roomDetails = _mapper.Map<RoomDetailsDto>(room);

            return Ok(roomDetails);
        }

        // POST: api/rooms/5
        [HttpPost("{id}")]
        public IActionResult Join(int id, [FromBody]RoomDto roomDto)
        {
            var user = _userService.GetById(Int32.Parse(User.Identity.Name));
            if (user == null)
                return Unauthorized();

            var room = _roomService.GetById(id);
            if (room == null)
                return BadRequest(Properties.resultMessages.RoomNotFound);

            try
            {
                var userRoom = _userRoomService.GetByIds(user.Id, id);
                if (userRoom == null)
                {
                    if (roomDto.Password == null)
                        return BadRequest(Properties.resultMessages.PasswordNull);

                    room = _roomService.Authenticate(id, roomDto.Password);

                    _userRoomService.Create(user, room, false);
                }
            }
            catch (ApplicationException ex)
            {
                if (ex.Message != Properties.resultMessages.UserRoomExists)
                    return BadRequest(ex.Message);
            }

            if (room == null)
                return BadRequest(Properties.resultMessages.RoomNotFound);

            var roomDetails = _mapper.Map<RoomDetailsDto>(room);
            return Ok(roomDetails);
        }
    }
}
