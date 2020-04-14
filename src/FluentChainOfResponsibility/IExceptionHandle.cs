using System;
using System.Threading;
using System.Threading.Tasks;

namespace FluentChainOfResponsibility
{
    /// <summary>
    /// Defines a handler for an exception
    /// </summary>
    /// <typeparam name="TRequest">The type of request being handled</typeparam>
    /// <typeparam name="TResponse">The type of response from the handler</typeparam>
    public interface IExceptionHandle<out TRequest, TResponse>
        where TRequest : IRequest<TResponse> where TResponse : class
    {
        /// <summary> 
        /// Handles an exception
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <param name="pipelineContext">The current context</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="Task"/></returns>
        Task Handle(
            Exception exception,
            IPipelineContext<TRequest, TResponse> pipelineContext,
            CancellationToken cancellationToken);
    }

    /// <summary>
    /// Default <see cref="IExceptionHandle{TRequest, TResponse}"/> implementation
    /// </summary>
    /// <typeparam name="TRequest">The type of request being handled</typeparam>
    /// <typeparam name="TResponse">The type of response from the handler</typeparam>
    public class ExceptionHandle<TRequest, TResponse> : IExceptionHandle<TRequest, TResponse>
        where TRequest : IRequest<TResponse> where TResponse : class
    {
        /// <inheritdoc />
        public Task Handle(
            Exception exception, 
            IPipelineContext<TRequest, TResponse> pipelineContext,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}