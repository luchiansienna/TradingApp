using AutoMapper;
using Ferovinum.Domain;
using Ferovinum.Services.DTO;

namespace Ferovinum.Services.Mappings
{
    public class OrderTypeConverter : ITypeConverter<string, OrderType>
    {
        public OrderType Convert(string source, OrderType destination, ResolutionContext context)
        {
            if (!Enum.TryParse<OrderType>(source.ToLower(), out var parsedResult))
                return OrderType.buy;
            return parsedResult;
        }
    }
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<string, OrderType>().ConvertUsing(new OrderTypeConverter());
            CreateMap<Transaction, TransactionDTO>().ForMember(x => x.OrderType, opt => opt.MapFrom(src => src.OrderType.ToString().ToLower()));
            CreateMap<TransactionDTO, Transaction>();
            CreateMap<Transaction, TransactionWithIdDTO>().ForMember(x => x.OrderType, opt => opt.MapFrom(src => src.OrderType.ToString().ToLower()));
            CreateMap<TransactionWithIdDTO, Transaction>();
        }
    }
}
