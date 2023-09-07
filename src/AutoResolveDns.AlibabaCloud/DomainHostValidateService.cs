using System.Diagnostics.CodeAnalysis;
using System.Net;

using AutoResolveDns.Abstraction;

namespace AutoResolveDns.AlibabaCloud
{
    internal class DomainHostValidateService : IDomainHostVaildateService
    {
        public bool GetDomainHostIsVaildate ( string domain , [NotNullWhen ( false )] out IPAddress [ ]? actualIp )
        {
            actualIp = IPHelper.GetDomainIps ( domain );
            if ( !IPHelper.TryGetCurrentIP ( out IPAddress? address ) )
            {
                return false;
            }
            string[] actualIpv4s = actualIp?.Select(p=>p.MapToIPv4().ToString()).ToArray()??Array.Empty<string>();
            string? currentIpv4 = address?.MapToIPv4().ToString();
            if ( string.IsNullOrWhiteSpace ( currentIpv4 ) )
            {
                return false;
            }
            for ( int i = 0; i < actualIpv4s.Length; i++ )
            {
                string ipv4 = actualIpv4s.ElementAt(i);
                if ( !ipv4.Equals ( currentIpv4 ) )
                {
                    return false;
                }
            }
            return true;
        }
    }
}
