namespace AutoResolveDns.Abstraction
{
    /// <summary>
    /// 服务设置项
    /// </summary>
    public class ServiceSettings
    {
        /// <summary>
        /// 要观察且处理的域名
        /// </summary>
        public List<DomainStruct> Domains { get; set; } = new List<DomainStruct> ();

        /// <summary>
        /// 服务循环观察周期
        /// </summary>
        public TimeSpan Delay { get; set; } = TimeSpan.FromHours ( 1 );
    }

    /// <summary>
    /// 域名结构
    /// </summary>
    public record struct DomainStruct
    {
        /// <summary>
        /// 域名
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// RRType
        /// </summary>
        /// <remarks>阿里云的RRType，暂时不知道对应Windows DNS Server的哪个属性</remarks>
        public List<string> RRType { get; set; }
    }
}
