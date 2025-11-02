using AutoMapper;
using FindFi.Bll.Mapping;
using Xunit;

namespace FindFi.Tests.AutoMapper;

public class AutoMapperConfigurationTests
{
    [Fact]
    public void ProfilesConfiguration_IsValid()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductProfile>();
        });
        config.AssertConfigurationIsValid();
    }
}
