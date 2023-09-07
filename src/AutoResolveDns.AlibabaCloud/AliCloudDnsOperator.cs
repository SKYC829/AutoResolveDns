using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.SDK.Alidns20150109;
using AlibabaCloud.SDK.Alidns20150109.Models;
using AlibabaCloud.TeaUtil.Models;

using AutoResolveDns.Abstraction;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using static AlibabaCloud.SDK.Alidns20150109.Models.DescribeDomainRecordsResponseBody.DescribeDomainRecordsResponseBodyDomainRecords;

namespace AutoResolveDns.AlibabaCloud
{
    internal class AliCloudDnsOperator : IDomainDNSOperator
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AliCloudDnsOperator> _logger;
        private readonly Client _aliCloudClient;

        public AliCloudDnsOperator ( IConfiguration configuration , ILogger<AliCloudDnsOperator> logger )
        {
            _configuration = configuration;
            _logger = logger;
            Config config = new Config()
            {
                AccessKeyId = configuration.GetValue<string> ( "AliCloud:DNS:Ali_Dns_AK" ),
                AccessKeySecret = configuration.GetValue<string> ( "AliCloud:DNS:Ali_Dns_SK" ),
                Endpoint = configuration.GetValue<string>("AliCloud:DNS:Ali_Dns_Url")
            };
            _aliCloudClient = new Client ( config );
        }

        public async Task ExecuteAsync ( DomainOperationContext context , CancellationToken cancellationToken = default )
        {
            foreach ( var item in context.Domain.RRType )
            {
                await UpdateDnsResolveRecord ( context.Domain.Host , context.IP , rrType: item );
                if ( cancellationToken.IsCancellationRequested )
                {
                    _logger.LogWarning ( $"Task cancel when updating dns resolves at {item}.{context.Domain.Host} => {context.IP}" );
                    return;
                }
            }
            _logger.LogInformation ( $"The domain {context.Domain.Host} had all resolves to {context.IP}" );
        }

        private async Task UpdateDnsResolveRecord ( string host , string? ip , string rrType )
        {
            _logger.LogDebug ( $"Resolving domain:{rrType}.{host}" );
            _logger.LogTrace ( $"The domain {rrType}.{host} is resolving to :{ip}" );
            List<DescribeDomainRecordsResponseBodyDomainRecordsRecord> records = await GetAlibabaDnsResolveRecordIds ( host , rrType );
            if ( records == null || records.Count == 0 )
            {
                _logger.LogDebug ( $"There are not has any record for {rrType}.{host}" );
                return;
            }
            foreach ( DescribeDomainRecordsResponseBodyDomainRecordsRecord item in records )
            {
                if ( item.Value != ip )
                {
                    item.Value = ip;
                    await InternalUpdateDnsResolveRecordAsync ( item );
                }
            }
        }

        private async Task InternalUpdateDnsResolveRecordAsync ( DescribeDomainRecordsResponseBodyDomainRecordsRecord record )
        {
            UpdateDomainRecordRequest request = new UpdateDomainRecordRequest()
            {
                RecordId = record.RecordId,
                RR = record.RR,
                Type = record.Type,
                Value = record.Value
            };
            RuntimeOptions runtime = new RuntimeOptions();
            try
            {
                UpdateDomainRecordResponse response = await _aliCloudClient.UpdateDomainRecordWithOptionsAsync( request , runtime );
                _logger.LogInformation ( $"The domain {record.RR}.{record.DomainName} had resolves" );
                _logger.LogTrace ( $"The domain {record.RR}.{record.DomainName} had resolves to {record.Value} with status code :{response.StatusCode}" );
            }
            catch ( Exception ex )
            {
                _logger.LogCritical ( $"Raising an exception when {nameof ( InternalUpdateDnsResolveRecordAsync )}:{ex.Message}" );
                _logger.LogTrace ( $"Stacktrace:{ex.StackTrace}" );
            }
        }

        private async Task<List<DescribeDomainRecordsResponseBodyDomainRecordsRecord>> GetAlibabaDnsResolveRecordIds ( string host , string rrType )
        {
            DescribeDomainRecordsRequest request = new DescribeDomainRecordsRequest()
            {
                DomainName = host,
                RRKeyWord = rrType
            };
            RuntimeOptions runtime = new RuntimeOptions();
            List<DescribeDomainRecordsResponseBodyDomainRecordsRecord> recordIds = new List<DescribeDomainRecordsResponseBodyDomainRecordsRecord>();
            try
            {
                DescribeDomainRecordsResponse response = await _aliCloudClient.DescribeDomainRecordsWithOptionsAsync ( request , runtime );
                if ( response?.Body?.DomainRecords?.Record != null )
                {
                    foreach ( DescribeDomainRecordsResponseBodyDomainRecordsRecord? item in response?.Body?.DomainRecords?.Record! )
                    {
                        recordIds.Add ( item );
                    }
                }
            }
            catch ( Exception ex )
            {
                _logger.LogCritical ( $"Raising an exception when {nameof ( GetAlibabaDnsResolveRecordIds )}:{ex.Message}" );
                _logger.LogTrace ( $"Stacktrace:{ex.StackTrace}" );
            }
            return recordIds;
        }
    }
}
