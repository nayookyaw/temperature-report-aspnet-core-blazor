
using AutoMapper;
using BackendAspNetCore.Dtos.Sensor;
using BackendAspNetCore.Dtos.SensorLog;
using BackendAspNetCore.Models;

namespace BackendAspNetCore.Mappers;

public class SensorProfile : Profile
{
    public SensorProfile()
    {
        CreateMap<SensorLog, SensorLogDto>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        CreateMap<Sensor, SensorDto>()
            .ForMember(s => s.MacAddress, m => m.MapFrom(s => s.MacAddress != null ? s.MacAddress.ToString() : "N/A"))
            .ForMember(s => s.Temperature, m => m.MapFrom(s => s.Temperature != null ? s.Temperature.ToString() : "N/A"))
            .ForMember(s => s.Humidity, m => m.MapFrom(s => s.Humidity != null ? s.Humidity.ToString() : "N/A"))
            .ForMember(s => s.LastUpdatedUtc, m => m.MapFrom(s => s.LastUpdatedUtc))
            .ForMember(s => s.SensorLogs, m => m.MapFrom(s => s.SensorLogs));
    }
}