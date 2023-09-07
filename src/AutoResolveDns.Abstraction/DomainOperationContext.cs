namespace AutoResolveDns.Abstraction
{
    /// <summary>
    /// 域名操作上下文
    /// </summary>
    public class DomainOperationContext
    {
        /// <summary>
        /// 域名
        /// </summary>
        public DomainStruct Domain { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string? IP { get; set; }

        public DomainOperationContext ()
        {

        }

        public DomainOperationContext ( DomainStruct domain , string ip )
        {
            Domain = domain;
            IP = ip;
        }
    }
}
