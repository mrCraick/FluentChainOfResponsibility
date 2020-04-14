namespace FluentChainOfResponsibility
{
    /// <summary>
    /// Defines a <see cref="IPipelineContext{TRequest,TResponse}"/> factory
    /// </summary>
    public interface IPipelineContextFactory
    {
        /// <summary>
        /// Create an instance <see cref="IPipelineContext{TRequest,TResponse}"/>
        /// </summary>
        /// <typeparam name="TRequest">The type of request being handled</typeparam>
        /// <typeparam name="TResponse">The type of response from the handler</typeparam>
        /// <returns>An instance <see cref="IPipelineContext{TRequest,TResponse}"></see></returns>
        IPipelineContext<TRequest, TResponse> CreatePipelineContext<TRequest, TResponse>()
            where TRequest : IRequest<TResponse>
            where TResponse : class;
    }
}