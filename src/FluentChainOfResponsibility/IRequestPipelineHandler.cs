using System.Threading;
using System.Threading.Tasks;

namespace FluentChainOfResponsibility
{
    /// <summary>
    /// Defines a request pipeline handler
    /// </summary>
    /// <typeparam name="TRequest">The type of request being handled</typeparam>
    /// <typeparam name="TResponse">The type of response from the handler</typeparam>
    public interface IRequestPipelineHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse> 
        where TResponse : class
    {
        /// <summary>
        /// Asynchronously handle a request
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="pipelineContext">Pipeline context</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task<TResponse> HandleAsync(TRequest request, IPipelineContext<TRequest, TResponse> pipelineContext,
            CancellationToken cancellationToken);
    }
}