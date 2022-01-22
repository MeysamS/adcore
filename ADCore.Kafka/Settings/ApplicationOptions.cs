using System.Reflection;
using Microsoft.Extensions.Options;

namespace ADCore.Kafka.Settings
{
    public class ApplicationOptions : IOptions<ApplicationOptions>
    {
        public Assembly[] AppAssemblies { get; set; }
        public ApplicationOptions Value => this;

    }
}
