using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DiceApi.Dtos;
using DiceApi.Entities;
using DiceApi.Helpers;
using DiceApi.Services;
using DiceApi.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DiceApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private IRoomService _roomService;
        private IUserService _userService;
        private IUserRoomService _userRoomService;
        private IMapper _mapper;
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

        // GET: api/rooms
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _roomService.GetAll();
            var roomDtos = _mapper.Map<IList<RoomInfoDto>>(rooms);

            return Ok(roomDtos);
        }

        // POST: api/rooms
        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody]RoomDto roomDto)
        {
            var room = _mapper.Map<Room>(roomDto);
            var user = await _userService.GetById(Int32.Parse(User.Identity.Name));

            if (user == null)
                return Unauthorized();

            var validator = new RoomValidator();
            var result = validator.Validate(roomDto);

            try
            {
                if (!result.IsValid)
                    throw new ApplicationException(string.Join(",", result.Errors));

                await _roomService.Create(room, roomDto.Password);

                await _userRoomService.Create(user, room, true);

                return Ok();
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/rooms/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetById(Int32.Parse(User.Identity.Name));

            if (user == null)
                return Unauthorized();

            var room = await _roomService.GetById(id);

            if (_userRoomService.GetByIds(user.Id, id) == null)
                return Unauthorized();

            if (room == null)
                return BadRequest(Properties.resultMessages.RoomNotFound);

            var roomDetails = _mapper.Map<RoomDetailsDto>(room);

            return Ok(roomDetails);
        }

        // POST: api/rooms/5
        [HttpPost("{id}")]
        public async Task<IActionResult> Join(int id, [FromBody]RoomDto roomDto)
        {
            var user = await _userService.GetById(Int32.Parse(User.Identity.Name));
            if (user == null)
                return Unauthorized();

            var room = await _roomService.GetById(id);
            if (room == null)
                return BadRequest(Properties.resultMessages.RoomNotFound);

            try
            {
                var userRoom = await _userRoomService.GetByIds(user.Id, id);
                if (userRoom == null)
                {
                    if (roomDto.Password == null)
                        return BadRequest(Properties.resultMessages.PasswordNull);

                    room = await _roomService.Authenticate(id, roomDto.Password);

                    await _userRoomService.Create(user, room, false);
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
