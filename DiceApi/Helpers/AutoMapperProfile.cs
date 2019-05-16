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

            CreateMap<UserInfoDto, UserDto>();
            CreateMap<UserDto, UserInfoDto>();

            CreateMap<UserRoom, UserInfoDto>()
                .ForMember(dest => dest.Id,
                opt =>
                {
                    opt.MapFrom(src => src.User.Id);
                })
                .ForMember(dest => dest.Username,
                opt =>
                {
                    opt.MapFrom(src => src.User.Username);
                });

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

            CreateMap<Room, RoomDetailsDto>()
                .ForMember(dest => dest.Owner,
                opt =>
                {
                    opt.PreCondition(src => src.RoomUsers != null);

                    opt.MapFrom(src => src.RoomUsers.SingleOrDefault(x => x.Owner).User);
                })
                .ForMember(dest => dest.Users,
                opt =>
                {
                    opt.MapFrom(src => src.RoomUsers.Where(x => x.RoomId == src.Id));
                });

            CreateMap<Roll, RollDto>()
                .ForMember(dest => dest.Username,
                opt =>
                {
                    opt.PreCondition(src => src.User != null);
                    opt.MapFrom(src => src.User.Username);
                });
            CreateMap<RollDto, Roll>();

            CreateMap<RollValue, RollValueDto>();
            CreateMap<RollValueDto, RollValue>();
        }
    }
}
