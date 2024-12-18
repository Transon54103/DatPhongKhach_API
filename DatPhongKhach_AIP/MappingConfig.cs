﻿using AutoMapper;
using DatPhongKhach_AIP.Models;
using DatPhongKhach_AIP.Models.Dto;

namespace DatPhongKhach_AIP
{
    public class MappingConfig : Profile
    {
        public MappingConfig() 
        {
            CreateMap<Villa,VillaDTO>();
            CreateMap<VillaDTO,Villa>();
            CreateMap<Villa, VillaCreateDTO>().ReverseMap();
            CreateMap<Villa,VillaUpdateDTO>().ReverseMap();
        }
    }
}
