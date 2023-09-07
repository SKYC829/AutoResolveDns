using AutoResolveDns.Abstraction;
using AutoResolveDns.AlibabaCloud;
using AutoResolveDns.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

HostApplicationBuilder? hostBuilder = Host.CreateApplicationBuilder();
hostBuilder.Configuration.AddUserSecrets<Program> ();
hostBuilder.Logging.AddConsole ()
                   .AddDebug ()
                   .AddFileLogger ()
                   .AddTraceSource ( "AutoResolveDns" );
if ( OperatingSystem.IsWindows () )
{
    hostBuilder.Logging.AddEventLog ()
                       .AddEventSourceLogger ();
}
hostBuilder.Services.Configure<ServiceSettings> ( c =>
{
    hostBuilder.Configuration.GetSection ( "ServiceSettings" ).Bind ( c );
} );
hostBuilder.Services.AddAlibabaCloudDnsService ();
hostBuilder.Services.AddHostedService<AutoResolveDnsService> ();
IHost appHost = hostBuilder.Build();
await appHost.RunAsync ();