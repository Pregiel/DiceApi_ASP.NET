using AutoMapper;
using DiceApi.Dtos;
using DiceApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<Room, RoomDto>();
            CreateMap<RoomDto, Room>();

            CreateMap<Room, RoomInfoDto>()
                .ForMember(dest => dest.Owner,
                opt =>
                {
                    opt.PreCondition(src => src.RoomUsers != null);

                    opt.MapFrom(src => src.RoomUsers.SingleOrDefault(x => x.Owner).User);
                })
                .ForMember(dest => dest.ClientAmount,
                opt =>
                {
                    opt.MapFrom(src => src.RoomUsers.Count);
                });
            CreateMap<RoomInfoDto, Room>();

            CreateMap<Roll, RollDto>();
            CreateMap<RollDto, Roll>();
        }
    }
}
