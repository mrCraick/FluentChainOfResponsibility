using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FluentChainOfResponsibility
{
    /// <inheritdoc />
    public class Pipelines<TRequest, TResponse> : IPipelines<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {
        private readonly IExceptionHandle<TRequest, TResponse> _exceptionHandle;
        private readonly Queue<IRequestPipelineHandler<TRequest, TResponse>> _queue;

        public Pipelines(
            IExceptionHandle<TRequest, TResponse> exceptionHandle,
            Queue<IRequestPipelineHandler<TRequest, TResponse>> queue)
        {
            _exceptionHandle = exceptionHandle;
            _queue = queue;
        }

        /// <inheritdoc />
        public IRequestPipelineHandler<TRequest, TResponse> GetNextPipelineHandler()
        { 
            var next = _queue.Dequeue();

            return next;
        }

        /// <inheritdoc />
        public async Task HandleException(
            Exception exception, 
            IPipelineContext<TRequest, TResponse> pipelineContext,
            CancellationToken cancellationToken)
        {
            await _exceptionHandle.Handle(exception, pipelineContext, cancellationToken);
        }
    }
}