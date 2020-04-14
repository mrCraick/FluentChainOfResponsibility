using System;
using System.Threading;
using System.Threading.Tasks;

namespace FluentChainOfResponsibility
{
    /// <summary>
    /// Defines a pipelines
    /// </summary>
    /// <typeparam name="TRequest">The type of request being handled</typeparam>
    /// <typeparam name="TResponse">The type of response from the handler</typeparam>
    public interface IPipelines<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {
        /// <summary>
        /// Get a next pipeline handler
        /// </summary>
        /// <returns>Next pipeline handler</returns>
        IRequestPipelineHandler<TRequest, TResponse> GetNextPipelineHandler();

        /// <summary>
        /// Handle an exception
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <param name="pipelineContext">The pipeline context</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        Task HandleException(
            Exception exception,
            IPipelineContext<TRequest, TResponse> pipelineContext,
            CancellationToken cancellationToken);
    }
}