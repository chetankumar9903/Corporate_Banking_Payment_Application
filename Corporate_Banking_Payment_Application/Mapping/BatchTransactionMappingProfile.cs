using AutoMapper;
using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Mapping
{
    public class BatchTransactionMappingProfile : Profile
    {

        public BatchTransactionMappingProfile()
        {
            CreateMap<BatchTransaction, BatchTransactionDto>();
            CreateMap<CreateBatchTransactionDto, BatchTransaction>();
        }
    }
}
