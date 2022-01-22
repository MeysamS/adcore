using ADCore.ConfigSupplier.Models;

namespace ADCore.ConfigSupplier.Services
{
    public interface IConfigService
    {
        ApiReaderConfig ReadApiReaderConfig(string id);
        MapperConfig ReadMapperConfig(string id);
        void SaveDefaultBaseConfig();
    }
}
