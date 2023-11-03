﻿using AutoMapper;
using DailyShop.Business.Features.Products.Dtos;
using DailyShop.Business.Features.Products.Models;
using DailyShop.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyShop.Business.Features.Products.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, GetListProductDto>().ReverseMap();
            CreateMap<Product, InsertedProductDto>()
                .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => src.Colors))
                .ForMember(dest => dest.Sizes, opt => opt.MapFrom(src => src.Sizes))
                .ForMember(dest => dest.ProductImages, opt => opt.MapFrom(src => src.ProductImages))
                .ReverseMap();
            CreateMap<Product, InsertProductViewModel>().ReverseMap();
            CreateMap<ProductImage, InsertedProductImageDto>().ReverseMap();
            CreateMap<Color, InsertedProductColorDto>().ReverseMap();
            CreateMap<Size, InsertedProductSizeDto>().ReverseMap();
        }
    }
}
