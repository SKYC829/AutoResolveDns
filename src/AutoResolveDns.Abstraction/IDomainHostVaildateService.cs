using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace AutoResolveDns.Abstraction
{
    /// <summary>
    /// 域名和主机IP校验服务接口
    /// </summary>
    public interface IDomainHostVaildateService
    {
        /// <summary>
        /// 判断传入的域名映射的IP和主机IP是否一致
        /// </summary>
        /// <param name="domain">域名</param>
        /// <param name="actualIp">当前域名实际上应该映射的IP</param>
        /// <returns>是否一致</returns>
        bool GetDomainHostIsVaildate ( string domain , [NotNullWhen ( false )] out IPAddress [ ]? actualIp );
    }
}
