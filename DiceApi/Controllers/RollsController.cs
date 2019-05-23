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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace DiceApi.Controllers
{
    [Authorize]
    [Route("api/rooms/{roomId}/[controller]")]
    [ApiController]
    public class RollsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoomService _roomService;
        private readonly IRollService _rollService;
        private readonly IRollValueService _rollValueService;
        private readonly IHubContext<RoomHub> _roomHub;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public RollsController(IRollService rollService,
            IRollValueService rollValueService,
            IRoomService roomService,
            IUserService userService,
            IHubContext<RoomHub> roomHub,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _rollService = rollService;
            _rollValueService = rollValueService;
            _roomService = roomService;
            _userService = userService;
            _roomHub = roomHub;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        // GET: api/rooms/5/rolls
        [HttpGet]
        public IActionResult GetRolls(int roomId)
        {
            var user = _userService.GetById(Int32.Parse(User.Identity.Name));
            if (user == null)
                return Unauthorized();

            var room = _roomService.GetById(roomId);
            if (room == null)
                return BadRequest(Properties.resultMessages.RoomNotFound);

            var rolls = _rollService.GetRoomRolls(room);
            var rollDtos = _mapper.Map<IList<RollDto>>(rolls);

            return Ok(rollDtos);
        }

        // POST: api/rooms/5/rolls
        [HttpPost]
        public IActionResult NewRoll(int roomId, [FromBody]RollDto rollDto)
        {
            var user = _userService.GetById(Int32.Parse(User.Identity.Name));
            if (user == null)
                return Unauthorized();

            var room = _roomService.GetById(roomId);
            if (room == null)
                return BadRequest(Properties.resultMessages.RoomNotFound);                

            var random = new Random();
            foreach (RollValueDto rollValue in rollDto.RollValues)
            {
                rollValue.Value = random.Next(1, Math.Abs(rollValue.MaxValue) + 1);
                if (rollValue.MaxValue < 0)
                    rollValue.Value = -rollValue.Value;
            }

            var rollValues = _mapper.Map<IList<RollValue>>(rollDto.RollValues);

            var roll = new Roll()
            {
                User = user,
                Room = room,
                Modifier = rollDto.Modifier,
                RollValues = rollValues
            };

            _rollService.Create(roll);
            rollDto = _mapper.Map<RollDto>(roll);

            var roomGroup = "room_" + roomId;
            _roomHub.Clients.Group(roomGroup).SendAsync("NewRoll", rollDto);

            return Ok(rollDto);
        }
    }
}
