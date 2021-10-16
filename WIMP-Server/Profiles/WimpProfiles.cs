using AutoMapper;
using WIMP_Server.Dtos;
using WIMP_Server.Dtos.Esi;
using WIMP_Server.Models;

namespace WIMP_Server.Profiles
{
    public class WimpProfiles : Profile
    {
        public WimpProfiles()
        {
            CreateMap<ReadCharacterDto, Character>();
            CreateMap<ReadShipDto, Ship>();
            CreateMap<ReadStarSystemDto, StarSystem>();

            CreateMap<Character, EsiNameIdPairDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CharacterId));
            CreateMap<StarSystem, EsiNameIdPairDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.StarSystemId));
            CreateMap<Ship, EsiNameIdPairDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ShipId));

            CreateMap<Intel, ReadIntelDto>();
            CreateMap<Ship, ReadShipDto>();

            CreateMap<EsiNameIdPairDto, ReadCharacterDto>()
                .ForMember(dest => dest.CharacterId, opt => opt.MapFrom(src => src.Id));
            CreateMap<EsiNameIdPairDto, ReadShipDto>()
                .ForMember(dest => dest.ShipId, opt => opt.MapFrom(src => src.Id));
            CreateMap<EsiNameIdPairDto, ReadStarSystemDto>()
                .ForMember(dest => dest.StarSystemId, opt => opt.MapFrom(src => src.Id));

            CreateMap<EsiNameIdPairDto, Ship>()
                .ForMember(dest => dest.ShipId, opt => opt.MapFrom(src => src.Id));

            CreateMap<EsiReadSystemDto, StarSystem>()
                .ForMember(dest => dest.StarSystemId, opt => opt.MapFrom(src => src.SystemId));

            CreateMap<EsiReadStargateDto, Stargate>()
                .ForMember(dest => dest.SrcStarSystemId, opt => opt.MapFrom(src => src.SrcSystemId))
                .ForMember(dest => dest.DstStarSystemId, opt => opt.MapFrom(src => src.Destination.SystemId));

            CreateMap<EsiSearchCharacterDto, Character>()
                .ForMember(dest => dest.CharacterId, opt => opt.MapFrom(src => src.Id));
            CreateMap<EsiSearchSystemDto, StarSystem>()
                .ForMember(dest => dest.StarSystemId, opt => opt.MapFrom(src => src.Id));
        }
    }
}