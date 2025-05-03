using AutoMapper;

namespace Maraudr.User.Application.Mappers
{
    public class Mapper 
    {
        private readonly IMapper _mapper;

        public Mapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public TDestination Map<TDestination>(object source)
        {
            return _mapper.Map<TDestination>(source);
        }

        public void Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            _mapper.Map(source, destination);
        }
    }
}