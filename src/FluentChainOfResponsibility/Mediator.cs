using System.Threading;
using System.Threading.Tasks;

namespace FluentChainOfResponsibility
{
    /// <inheritdoc />
    public class Mediator : IMediator
    {
        private readonly IPipelineContextFactory _contextFactory;

        /// <summary>
        /// Create an instance of <see cref="Mediator"/>
        /// </summary>
        /// <param name="contextFactory">Context factory</param>
        public Mediator(IPipelineContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <inheritdoc />
        public async Task<IPipelineContext<TRequest, TResponse>> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest<TResponse>
            where TResponse : class
        {
            var context = _contextFactory.CreatePipelineContext<TRequest, TResponse>();

            await context.Execute(request, cancellationToken);

            return context;
        }
    }
}