using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
    [Route("api/user")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(IUserService userService, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        // POST: api/user/authenticate
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] UserDto userDto)
        {
            var user = _userService.Authenticate(userDto.Username, userDto.Password);

            if (user == null)
                return BadRequest(new
                {
                    result = Properties.resultMessages.Failure,
                    error = Properties.resultMessages.WrongCredentials
                });

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

            return Ok(new
            {
                result = Properties.resultMessages.Success,
                user.Id,
                user.Username,
                Token = tokenString
            });
        }

        // POST: api/user/register
        [AllowAnonymous]
        [HttpPost("register")]
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

                return Ok(new
                {
                    result = Properties.resultMessages.Success
                });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new
                {
                    result = Properties.resultMessages.Failure,
                    error = ex.Message
                });
            }
        }

        // GET: api/user
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            var userDtos = _mapper.Map<IList<UserDto>>(users);
            return Ok(userDtos);
        }

        // GET: api/user/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
    }
}
