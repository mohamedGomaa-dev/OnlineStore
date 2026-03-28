using AutoMapper;
using Store.Models.Entities;
using Store.Services.Dtos.CateogryDtos;
using Store.Services.Dtos.ImageDtos;
using Store.Services.Dtos.ProductDtos;
using Store.Services.Dtos.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Services.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<UserCreateDto, User>().ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            CreateMap<UserUpdateDto, User>().ReverseMap();

            // category maps
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryUpdateDto, Category>();

            //product maps
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>();

            // image dtos
            CreateMap<ProductImage, ProductImageDto>().ReverseMap();
        }
    }
}
