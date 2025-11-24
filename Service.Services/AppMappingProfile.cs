using AutoMapper;
using Service.Domain.Models;
using Service.Domain.ModelsDb;
using Service.Domain.ViewModels.LoginAndRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<User, UserDb>().ReverseMap();
            CreateMap<User, LoginViewModel>().ReverseMap();
            CreateMap<User, RegisterViewModel>().ReverseMap();

            // Новая строка для 18 главы
            CreateMap<RegisterViewModel, ConfirmEmailViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserPassword, opt => opt.MapFrom(src => src.Password))
                .ReverseMap();
        }
    }
}
