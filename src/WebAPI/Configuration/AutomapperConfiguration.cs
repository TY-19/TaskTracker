using AutoMapper;
using TaskTracker.Application.Mapping;

namespace TaskTracker.WebAPI.Configuration
{
    public class AutomapperConfiguration
    {
        public IMapper GetMapper()
        {
            return new MapperConfiguration(ConfigureMapper).CreateMapper();
        }

        private void ConfigureMapper(IMapperConfigurationExpression config)
        {
            config.AddProfile<AutomapperProfile>();
        }
    }
}
