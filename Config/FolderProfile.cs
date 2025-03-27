using AutoMapper;
using DAM_Upload.DTO;
using DAM_Upload.Models;

namespace DAM_Upload.Config
{
    public class FolderProfile : Profile
    {
        public FolderProfile()
        {
            CreateMap<Folder, FolderDTO>();
        }
    }

}
