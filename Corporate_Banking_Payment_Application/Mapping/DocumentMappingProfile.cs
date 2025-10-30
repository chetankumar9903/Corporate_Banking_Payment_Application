using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Mapping
{
    public class DocumentMappingProfile : Profile
    {
        public DocumentMappingProfile()
        {
            // Document -> DocumentDto (Read Operation)
            // Maps the Document model to the read-only DTO.
            CreateMap<Document, DocumentDto>();

            // CreateDocumentDto -> Document (Write Operation)
            // Only DocumentType and CustomerId are mapped here.
            // Other fields (Name, Size, IDs, URL, Dates) are populated in the service layer
            // after the file is successfully uploaded to Cloudinary.
            CreateMap<CreateDocumentDto, Document>();

            // UpdateDocumentDto -> Document (Update Operation)
            // This is used for updating metadata and status, but not file references.
            CreateMap<UpdateDocumentDto, Document>()
                // Apply condition for IsActive (value type) to allow partial updates.
                // If IsActive is null in the DTO, the existing value on the model is preserved.
                .ForMember(dest => dest.IsActive,
                           opt => opt.Condition(src => src.IsActive.HasValue));
        }

    }
}
