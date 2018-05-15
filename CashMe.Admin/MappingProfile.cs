using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using CashMe.Shared.Models;

namespace CashMe.Admin
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            Mapper.CreateMap<ApplicationUser, RegisterViewModel>();
        }
    }
    public static class AutoMapperExtension
    {
        public static RegisterViewModel MapTo(this ApplicationUser obj)
        {
            return Mapper.Map<ApplicationUser, RegisterViewModel>(obj);
        }
    }
}