using System.Threading;
using System.Threading.Tasks;

namespace FluentChainOfResponsibility
{
    /// <summary>
    /// Defines a mediator to encapsulate request/response interaction patterns
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Asynchronously send a request to a pipeline
        /// </summary>
        /// <typeparam name="TRequest">The type of request being handled</typeparam>
        /// <typeparam name="TResponse">The type of response from the handler</typeparam>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task<IPipelineContext<TRequest, TResponse>> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest<TResponse>
            where TResponse : class;
    }
}