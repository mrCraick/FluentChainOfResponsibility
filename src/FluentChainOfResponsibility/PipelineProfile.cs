using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FluentChainOfResponsibility
{
    /// <inheritdoc />
    public abstract class PipelineProfile<TRequest, TResponse> : IPipelineProfile<TRequest, TResponse>
        where TRequest : IRequest<TResponse> where TResponse : class
    {
        private readonly InstanceFactory _instanceFactory;
        private readonly Queue<Func<IRequestPipelineHandler<TRequest, TResponse>>> _queue;
        private IExceptionHandle<TRequest, TResponse>? _exceptionHandle;

        protected PipelineProfile(InstanceFactory instanceFactory)
        {
            _instanceFactory = instanceFactory;
            _queue = new Queue<Func<IRequestPipelineHandler<TRequest, TResponse>>>();
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IRequestPipelineHandler<TRequest, TResponse>> RequestPipelineHandlers
            => new ReadOnlyCollection<IRequestPipelineHandler<TRequest, TResponse>>(_queue.Select(x => x.Invoke()).ToList());

        /// <inheritdoc />
        public IPipelineProfile<TRequest, TResponse> AddNext(IRequestPipelineHandler<TRequest, TResponse> pipeline)
        {
            if (pipeline == null) throw new ArgumentNullException(nameof(pipeline));

            return AddNextInner(() => pipeline);
        }

        /// <inheritdoc />
        public IPipelineProfile<TRequest, TResponse> AddNext<TRequestPipelineHandler>()
            where TRequestPipelineHandler : IRequestPipelineHandler<TRequest, TResponse>
        {
            IRequestPipelineHandler<TRequest, TResponse> GetPipelineHandler() => _instanceFactory.GetInstance<TRequestPipelineHandler>();

            return AddNextInner(GetPipelineHandler);
        }

        /// <inheritdoc />
        public IPipelineProfile<TRequest, TResponse> AddOptionNext(
            IRequestPipelineHandler<TRequest, TResponse>? pipeline)
        {
            return pipeline == null ? this : AddNextInner(() => pipeline);
        }

        /// <inheritdoc />
        public IPipelineProfile<TRequest, TResponse> AddOptionNext<TRequestPipelineHandler>()
            where TRequestPipelineHandler : class, IRequestPipelineHandler<TRequest, TResponse>
        {
            IRequestPipelineHandler<TRequest, TResponse>? GetOptionPipelineHandler() => _instanceFactory.GetOptionInstance<TRequestPipelineHandler>();

            return AddOptionNext(GetOptionPipelineHandler());
        }

        /// <inheritdoc />
        public IPipelineProfile<TRequest, TResponse> SetExceptionHandler(
            IExceptionHandle<TRequest, TResponse> exceptionHandle)
        {
            if (exceptionHandle == null) throw new ArgumentNullException(nameof(exceptionHandle));

            return SetExceptionHandlerInner(exceptionHandle);
        }

        /// <inheritdoc />
        public IPipelineProfile<TRequest, TResponse> SetExceptionHandler<TExceptionHandle>()
            where TExceptionHandle : IExceptionHandle<TRequest, TResponse>
        {
            var exceptionHandle = _instanceFactory.GetInstance<TExceptionHandle>();

            return SetExceptionHandlerInner(exceptionHandle);
        }

        /// <inheritdoc />
        public IPipelines<TRequest, TResponse> BuildPipelines()
        {
            var handlers = _queue
                .Select(func => func())
                .Where(pipelineHandler => pipelineHandler != null)
                .ToList();

            var pipelines = new Pipelines
                <TRequest, TResponse>
                (GetExceptionHandle(),
                    new Queue<IRequestPipelineHandler<TRequest, TResponse>>(handlers));

            return pipelines;
        }

        /// <inheritdoc />
        public IPipelineProfile<TRequest, TResponse> AddOptionPipelineProfile(
            IPipelineProfile<TRequest, TResponse>? pipelineProfile)
        {
            return pipelineProfile == null ? this : AddPipelineProfileInner(pipelineProfile);
        }

        /// <inheritdoc />
        public IPipelineProfile<TRequest, TResponse> AddOptionPipelineProfile<TPipelineProfile>()
            where TPipelineProfile : class, IPipelineProfile<TRequest, TResponse>
        {
            var profile = _instanceFactory.GetOptionInstance<TPipelineProfile>();

            return AddOptionPipelineProfile(profile);
        }

        private IPipelineProfile<TRequest, TResponse> SetExceptionHandlerInner(
            IExceptionHandle<TRequest, TResponse> exceptionHandle)
        {
            _exceptionHandle = exceptionHandle;

            return this;
        }

        private IPipelineProfile<TRequest, TResponse> AddNextInner(
            Func<IRequestPipelineHandler<TRequest, TResponse>> pipeline)
        {
            _queue.Enqueue(pipeline);

            return this;
        }

        private IPipelineProfile<TRequest, TResponse> AddPipelineProfileInner(
            IPipelineProfile<TRequest, TResponse> pipelineProfile)
        {
            var pipelineHandlers = pipelineProfile.RequestPipelineHandlers;

            foreach (var pipelineHandler in pipelineHandlers) AddNextInner(() => pipelineHandler);

            return this;
        }

        private IExceptionHandle<TRequest, TResponse> GetExceptionHandle()
        {
            return _exceptionHandle ?? new ExceptionHandle<TRequest, TResponse>();
        }
    }
}