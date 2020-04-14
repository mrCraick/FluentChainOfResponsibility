using System;
using System.Collections.Generic;

namespace FluentChainOfResponsibility
{
    /// <inheritdoc />
    public class PipelinesFactory<TRequest, TResponse> : IPipelinesFactory<TRequest, TResponse> 
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {
        /// <inheritdoc />
        public IPipelines<TRequest, TResponse> Create(IPipelineProfile<TRequest, TResponse> pipelineProfile)
        {
            if (pipelineProfile == null) throw new ArgumentNullException(nameof(pipelineProfile));

            return pipelineProfile.BuildPipelines();
        }

        /// <inheritdoc />
        public IPipelines<TRequest, TResponse> Create(IEnumerable<IRequestPipelineHandler<TRequest, TResponse>> handlers, IExceptionHandle<TRequest, TResponse> exceptionHandle)
        {
            if (handlers == null) throw new ArgumentNullException(nameof(handlers));
            if (exceptionHandle == null) throw new ArgumentNullException(nameof(exceptionHandle));

            var requestPipelineHandlers = new Queue<IRequestPipelineHandler<TRequest, TResponse>>(handlers);
            return new Pipelines<TRequest, TResponse>(exceptionHandle, requestPipelineHandlers);
        }
    }
}