using System.Net;

using AutoResolveDns.Abstraction;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoResolveDns.Core
{
    public class AutoResolveDnsService : IHostedService, IDisposable, IAsyncDisposable
    {
        private CancellationTokenSource? _cancellationTokenSource;

        private readonly IDomainHostVaildateService _vaildateService;
        private readonly IDomainDNSOperator _operator;
        private readonly ILogger _logger;
        private readonly IOptionsSnapshot<ServiceSettings> _settings;

        public AutoResolveDnsService ( IDomainHostVaildateService vaildateService , IDomainDNSOperator dNSOperator , ILogger<AutoResolveDnsService> logger , IOptionsSnapshot<ServiceSettings> settings )
        {
            _vaildateService = vaildateService;
            _operator = dNSOperator;
            _logger = logger;
            _settings = settings;
        }

        public void Dispose ()
        {
            _logger.LogInformation ( $"Service Dispose" );
            _cancellationTokenSource?.Cancel ();
            _cancellationTokenSource?.Dispose ();
        }

        public ValueTask DisposeAsync ()
        {
            Dispose ();
            return ValueTask.CompletedTask;
        }

        public Task StartAsync ( CancellationToken cancellationToken )
        {
            _logger.LogInformation ( $"Service Start" );
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource ( cancellationToken );
            return Task.Run ( async () =>
            {
                do
                {
                    foreach ( DomainStruct domain in _settings.Value.Domains )
                    {
                        _logger.LogDebug ( $"Validation Domain:{domain}" );
                        if ( !IPHelper.TryGetCurrentIP ( out IPAddress? currentIp ) )
                        {
                            _logger.LogWarning ( $"Get current machine ip failure." );
                            break;
                        }
                        try
                        {
                            if ( !_vaildateService.GetDomainHostIsVaildate ( domain.Host , out IPAddress [ ]? ipAddresses ) )
                            {
                                _logger.LogDebug ( $"Domain and host validation faliure." );
                                _logger.LogTrace ( $"Domain {domain} actual need resolve to :{currentIp?.MapToIPv4 ()?.ToString ()}" );
                                foreach ( var item in ipAddresses )
                                {
                                    DomainOperationContext context = new DomainOperationContext(domain,currentIp?.MapToIPv4 ()?.ToString ());
                                    await _operator.ExecuteAsync ( context , _cancellationTokenSource.Token );
                                }
                            }
                            else
                            {
                                _logger.LogDebug ( $"Domain and host not need to resolve dns" );
                                _logger.LogTrace ( $"Domain {domain} is already map to :{currentIp?.MapToIPv4 ()?.ToString ()}" );
                            }
                        }
                        catch ( Exception ex )
                        {
                            _logger.LogCritical ( $"Raising some exception when execute services:{ex.Message}." );
                            _logger.LogDebug ( $"Stacktrace:{ex.StackTrace}" );
                        }
                        finally
                        {
                            _logger.LogInformation ( $"Domain {domain} resolves task finish." );
                        }
                    }
                    await Task.Delay ( _settings.Value.Delay );
                } while ( !_cancellationTokenSource.IsCancellationRequested );
            } , _cancellationTokenSource.Token );
        }

        public Task StopAsync ( CancellationToken cancellationToken )
        {
            _logger.LogInformation ( $"Service Stop" );
            _cancellationTokenSource?.Cancel ();
            return Task.CompletedTask;
        }
    }
}
