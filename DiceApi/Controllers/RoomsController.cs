using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DiceApi.Helpers;
using DiceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DiceApi.Controllers
{
    [Authorize]
    [Route("api/room")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private IRoomService _roomService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public RoomsController(IRoomService roomService, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _roomService = roomService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }
    }
}
