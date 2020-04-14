using System;
using System.Threading;
using System.Threading.Tasks;

namespace FluentChainOfResponsibility
{
    /// <summary>
    /// Defines a pipeline context for a request handlers
    /// </summary>
    /// <typeparam name="TRequest">The type of request being handled</typeparam>
    /// <typeparam name="TResponse">The type of response from the handler</typeparam>
    public interface IPipelineContext<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {
        /// <summary>
        /// Whether an exception was thrown at time handled
        /// </summary>
        bool IsFaulted { get; }

        /// <summary>
        /// Instance of <see cref="System.Exception"/> 
        /// </summary>
        Exception? Exception { get; }

        /// <summary>
        /// Request response
        /// </summary>
        TResponse? Response { get; }

        /// <summary>
        /// Execute request processing
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        Task Execute(TRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Call a next pipeline to handled a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Request response</returns>
        Task<TResponse?> Next(TRequest request, CancellationToken cancellationToken);
    }
}