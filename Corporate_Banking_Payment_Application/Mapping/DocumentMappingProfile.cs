using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Mapping
{
    public class DocumentMappingProfile : Profile
    {
        public DocumentMappingProfile()
        {

            CreateMap<Document, DocumentDto>();


            CreateMap<CreateDocumentDto, Document>();


            CreateMap<UpdateDocumentDto, Document>()

                .ForMember(dest => dest.IsActive,
                           opt => opt.Condition(src => src.IsActive.HasValue));
        }

    }
}
