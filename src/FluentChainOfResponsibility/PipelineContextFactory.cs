using System.Collections.Generic;

namespace FluentChainOfResponsibility
{
    /// <inheritdoc />
    public class PipelineContextFactory : IPipelineContextFactory
    {
        private readonly InstanceFactory _instanceFactory;

        public PipelineContextFactory(InstanceFactory instanceFactory)
        {
            _instanceFactory = instanceFactory;
        }

        /// <inheritdoc />
        public IPipelineContext<TRequest, TResponse> CreatePipelineContext<TRequest, TResponse>()
            where TRequest : IRequest<TResponse>
            where TResponse : class
        {
            var profile = _instanceFactory.GetOptionInstance<IPipelineProfile<TRequest, TResponse>>();
            var pipelinesFactory = _instanceFactory.GetInstance<IPipelinesFactory<TRequest, TResponse>>();

            IPipelines<TRequest, TResponse> pipelines;

            if (profile != null)
            {
                pipelines = pipelinesFactory.Create(profile);
            }
            else
            {
                var enumerator = _instanceFactory.GetInstance<IEnumerable<IRequestPipelineHandler<TRequest, TResponse>>>();
                var exceptionHandle = _instanceFactory.GetOptionInstance<IExceptionHandle<TRequest, TResponse>>() ??
                                      new ExceptionHandle<TRequest, TResponse>();

                pipelines = pipelinesFactory.Create(enumerator, exceptionHandle);
            }

            var pipelineContext = new PipelineContext<TRequest, TResponse>(pipelines);

            return pipelineContext;
        }
    }
}