namespace AutoResolveDns.Abstraction
{
    /// <summary>
    /// 域名DNS操作器接口
    /// </summary>
    public interface IDomainDNSOperator
    {
        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="context">上下文数据</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作的异步任务</returns>
        Task ExecuteAsync ( DomainOperationContext context , CancellationToken cancellationToken = default );
    }
}
