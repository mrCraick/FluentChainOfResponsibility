using System.Collections.Generic;

namespace FluentChainOfResponsibility
{
    /// <summary>
    /// Defines a <see cref="IPipelines{TRequest,TResponse}"/> factory
    /// </summary>
    /// <typeparam name="TRequest">The type of request being handled</typeparam>
    /// <typeparam name="TResponse">The type of response from the handler</typeparam>
    public interface IPipelinesFactory<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {
        /// <summary>
        /// Create an instance of <see cref="IPipelineProfile{TRequest,TResponse}"/>
        /// </summary>
        /// <param name="pipelineProfile">The pipeline profile</param>
        /// <returns>An instance of <see cref="IPipelineProfile{TRequest,TResponse}"></see></returns>
        IPipelines<TRequest, TResponse> Create(IPipelineProfile<TRequest, TResponse> pipelineProfile);

        /// <summary>
        /// Create an instance of <see cref="IPipelineProfile{TRequest,TResponse}"/>
        /// </summary>
        /// <param name="handlers">Collection of handlers</param>
        /// <param name="exceptionHandle">The exception handle</param>
        /// <returns>An instance of <see cref="IPipelineProfile{TRequest,TResponse}"></see></returns>
        IPipelines<TRequest, TResponse> Create(
            IEnumerable<IRequestPipelineHandler<TRequest, TResponse>> handlers,
            IExceptionHandle<TRequest, TResponse> exceptionHandle);
    }
}