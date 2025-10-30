using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Mapping
{
    public class BatchTransactionMappingProfile : Profile
    {
        //public BatchTransactionMappingProfile()
        //{
        //    CreateMap<BatchTransaction, BatchTransactionDto>();
        //    CreateMap<CreateBatchTransactionDto, BatchTransaction>();
        //}
        public BatchTransactionMappingProfile()
        {
            // BatchTransaction → DTO
            //CreateMap<BatchTransaction, BatchTransactionDto>()
            //    .ForMember(dest => dest.SalaryDisbursements,
            //        opt => opt.MapFrom(src => src.SalaryDisbursements));

            //// DTO → Model
            //CreateMap<CreateBatchTransactionDto, BatchTransaction>()
            //    .ForMember(dest => dest.TotalTransactions, opt => opt.Ignore())
            //    .ForMember(dest => dest.TotalAmount, opt => opt.Ignore());

            CreateMap<BatchTransaction, BatchTransactionDto>();
            CreateMap<CreateBatchTransactionDto, BatchTransaction>();
        }
    }
}
