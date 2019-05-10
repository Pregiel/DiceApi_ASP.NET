using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DiceApi.Dtos;
using DiceApi.Entities;
using DiceApi.Helpers;
using DiceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DiceApi.Controllers
{
    [Authorize]
    [Route("api/rooms/{roomId}/[controller]")]
    [ApiController]
    public class RollsController : ControllerBase
    {
        private IUserService _userService;
        private IRoomService _roomService;
        private IRollService _rollService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public RollsController(IRollService rollService,
            IRoomService roomService,
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _rollService = rollService;
            _roomService = roomService;
            _userService = userService;
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
                return BadRequest(new
                {
                    result = Properties.resultMessages.Failure,
                    error = Properties.resultMessages.RoomNotFound
                });

            var rolls = _rollService.GetRoomRolls(room);
            var rollDtos = _mapper.Map<IList<RollDto>>(rolls);

            return Ok(rollDtos);
        }

        // POST: api/rooms/5/rolls
        [HttpPost]
        public IActionResult NewRoll(int roomId, [FromBody]int maxValue)
        {
            var user = _userService.GetById(Int32.Parse(User.Identity.Name));
            if (user == null)
                return Unauthorized();

            var room = _roomService.GetById(roomId);
            if (room == null)
                return BadRequest(new
                {
                    result = Properties.resultMessages.Failure,
                    error = Properties.resultMessages.RoomNotFound
                });

            var random = new Random();
            int value = random.Next(1, maxValue + 1);

            var roll = new Roll(user, room, value, maxValue);
            _rollService.Create(roll);

            var rollDto = _mapper.Map<RollDto>(roll);

            return Ok(new
            {
                result = Properties.resultMessages.Success,
                roll = rollDto
            });
        }
    }
}
