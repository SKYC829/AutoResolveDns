using System.Net;

namespace AutoResolveDns.Abstraction
{
    public class IPHelper
    {
        public static bool TryGetCurrentIP ( out IPAddress? ip )
        {
            ip = IPAddress.None;
            HttpClientHandler handler = new HttpClientHandler ()
            {
                Proxy = null,
                UseProxy = false
            };
            HttpClient httpClient = new HttpClient (handler);
            try
            {
                HttpResponseMessage response = httpClient.GetAsync("https://api64.ipify.org?format=text").GetAwaiter().GetResult();
                if ( response.IsSuccessStatusCode )
                {
                    string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return IPAddress.TryParse ( content , out ip );
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static IPAddress [ ] GetDomainIps ( string domain )
        {
            return Dns.GetHostAddresses ( domain );
        }
    }
}
