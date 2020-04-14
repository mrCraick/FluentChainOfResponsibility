using System;
using System.Threading;
using System.Threading.Tasks;

namespace FluentChainOfResponsibility
{
    /// <inheritdoc />
    public class PipelineContext<TRequest, TResponse> : IPipelineContext<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : class
    {
        private readonly IPipelines<TRequest, TResponse> _pipelines;

        /// <summary>
        /// Create an instance of <see cref="PipelineContext{TRequest,TResponse}"/>
        /// </summary>
        /// <param name="pipelines">Pipelines</param>
        public PipelineContext(IPipelines<TRequest, TResponse> pipelines)
        {
            _pipelines = pipelines;
        }

        /// <inheritdoc />
        public bool IsFaulted { get; private set; }

        /// <inheritdoc />
        public Exception? Exception { get; private set; }

        /// <inheritdoc />
        public TResponse? Response { get; private set; }

        /// <inheritdoc />
        public async Task Execute(TRequest request, CancellationToken cancellationToken)
        {
            Response = await Next(request, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<TResponse?> Next(TRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var pipelineHandler = _pipelines.GetNextPipelineHandler();

                return await pipelineHandler.HandleAsync(request, this, cancellationToken);
            }
            catch (Exception exception)
            {
                if (IsFaulted && Exception != null)
                    throw;
                IsFaulted = true;
                Exception = exception;

                await _pipelines.HandleException(exception, this, cancellationToken);

                return default;
            }
        }
    }
}