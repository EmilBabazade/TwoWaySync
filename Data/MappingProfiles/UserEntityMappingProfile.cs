﻿using AutoMapper;
using Data.Entities;
using Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.MappingProfiles;
public class UserEntityMappingProfile : Profile
{
    public UserEntityMappingProfile()
    {
        CreateMap<UserEntity, User>().ReverseMap();
    }
}
