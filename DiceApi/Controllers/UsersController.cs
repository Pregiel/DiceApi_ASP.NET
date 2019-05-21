using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DiceApi.Dtos;
using DiceApi.Entities;
using DiceApi.Helpers;
using DiceApi.Services;
using DiceApi.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DiceApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IUserRoomService _userRoomService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(IUserService userService, 
            IUserRoomService userRoomService, 
            IMapper mapper, 
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _userRoomService = userRoomService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        // POST: api/users/authenticate
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] UserDto userDto)
        {
            try
            {
                var user = _userService.Authenticate(userDto.Username, userDto.Password);

                if (user == null)
                    return Unauthorized();

                var tokenString = ReceiveToken(user);

                return Ok(new
                {
                    user.Id,
                    user.Username,
                    Token = tokenString
                });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/users
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register([FromBody]UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);

            var validator = new UserValidator();
            var result = validator.Validate(userDto);

            try
            {
                if (!result.IsValid)
                    throw new ApplicationException(string.Join(",", result.Errors));

                _userService.Create(user, userDto.Password);

                var tokenString = ReceiveToken(user);

                return Ok(new
                {
                    user.Id,
                    user.Username,
                    Token = tokenString
                });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/users
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            var userDtos = _mapper.Map<IList<UserDto>>(users);
            return Ok(userDtos);
        }

        // GET: api/users/info
        [HttpGet("info")]
        public IActionResult GetInfo()
        {
            var user = _userService.GetById(Int32.Parse(User.Identity.Name));

            if (user == null)
                return Unauthorized();

            var rooms = _userRoomService.GetRoomsByUserId(user.Id);
            var roomDtos = _mapper.Map<IList<RoomInfoDto>>(rooms);

            return Ok(new
            {
                id = user.Id,
                username = user.Username,
                rooms = roomDtos
            });
        }

        private string ReceiveToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
    }
}
