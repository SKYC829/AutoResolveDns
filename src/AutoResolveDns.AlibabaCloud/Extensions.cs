using AutoResolveDns.Abstraction;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AutoResolveDns.AlibabaCloud
{
    public static class Extensions
    {
        public static IServiceCollection AddAlibabaCloudDnsService ( this IServiceCollection services )
        {
            services.TryAddEnumerable ( ServiceDescriptor.Scoped<IDomainHostVaildateService , DomainHostValidateService> () );
            services.TryAddEnumerable ( ServiceDescriptor.Scoped<IDomainDNSOperator , AliCloudDnsOperator> () );
            return services;
        }
    }
}
