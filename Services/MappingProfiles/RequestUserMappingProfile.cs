using AutoMapper;
using Data.Entities;
using Domain.User;
using Services.UsersApi.ResponseModel;

namespace Services.MappingProfiles;
public class RequestUserMappingProfile : Profile
{
    public RequestUserMappingProfile()
    {
        CreateMap<RequestUser, User>()
            .ForMember(u => u.StreetAddress, opt => opt.MapFrom(r => r.Address.Street))
            .ForMember(u => u.ApartmentSuite, opt => opt.MapFrom(r => r.Address.Suite))
            .ForMember(u => u.City, opt => opt.MapFrom(r => r.Address.City))
            .ForMember(u => u.ZipCode, opt => opt.MapFrom(r => r.Address.Zipcode))
            .ForMember(u => u.Latitude, opt => opt.MapFrom(r => r.Address.Geo.Lat))
            .ForMember(u => u.Longitude, opt => opt.MapFrom(r => r.Address.Geo.Lng))
            .ForMember(u => u.CompanyName, opt => opt.MapFrom(r => r.Company.Name))
            .ForMember(u => u.CompanyCatchPhrase, opt => opt.MapFrom(r => r.Company.CatchPhrase))
            .ForMember(u => u.CompanyBs, opt => opt.MapFrom(r => r.Company.Bs))
            .ReverseMap();
    }
}
