using Microsoft.Extensions.Configuration;

namespace Snippy.Data;

public class DataConfiguration : IDataConfiguration
{
    public DataConfiguration(IConfiguration Configuration)
    {
        Configuration.Bind("DBConfig", this);
        var section = Configuration.GetSection("DBConfig");
    }

    public string ConnectionString { get; }
}
