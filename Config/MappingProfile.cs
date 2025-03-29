using AutoMapper;
using DAM_Upload.DTO;
using DAM_Upload.Models;

namespace DAM_Upload.Config
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Permission mappings
            CreateMap<PermissionFile, PermissionDto>()
                .ForMember(dest => dest.ResourceType, opt => opt.MapFrom(src => "File"))
                .ForMember(dest => dest.ResourceId, opt => opt.MapFrom(src => src.FileId))
                .ForMember(dest => dest.ResourceName, opt => opt.MapFrom(src => src.File != null ? src.File.Name : "Unknown"));

            CreateMap<PermissionFolder, PermissionDto>()
                .ForMember(dest => dest.ResourceType, opt => opt.MapFrom(src => "Folder"))
                .ForMember(dest => dest.ResourceId, opt => opt.MapFrom(src => src.FolderId))
                .ForMember(dest => dest.ResourceName, opt => opt.MapFrom(src => src.Folder != null ? src.Folder.Name : "Unknown"));

            // AccessRequest mapping
            CreateMap<AccessRequest, AccessRequestDto>()
                .ForMember(dest => dest.RequesterEmail, opt => opt.MapFrom(src => src.Requester != null ? src.Requester.Email : string.Empty))
                .ForMember(dest => dest.RequesterUsername, opt => opt.MapFrom(src => src.Requester != null ? src.Requester.Username : string.Empty))
                .ForMember(dest => dest.OwnerEmail, opt => opt.MapFrom(src => src.Owner != null ? src.Owner.Email : string.Empty))
                .ForMember(dest => dest.ResourceType, opt => opt.Ignore())
                .ForMember(dest => dest.ResourceName, opt => opt.Ignore());
        }
    }
}
