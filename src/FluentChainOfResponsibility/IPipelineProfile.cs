using System.Collections.Generic;

namespace FluentChainOfResponsibility
{
    /// <summary>
    /// Defines a pipeline profile
    /// </summary>
    /// <typeparam name="TRequest">The type of request being handled</typeparam>
    /// <typeparam name="TResponse">The type of response from the handler</typeparam>
    public interface IPipelineProfile<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {
        /// <summary>
        /// Get a collection of request handlers
        /// </summary>
        IReadOnlyCollection<IRequestPipelineHandler<TRequest, TResponse>> RequestPipelineHandlers { get; }

        /// <summary>
        /// Add a new next pipeline
        /// </summary>
        /// <param name="pipeline">Instance of the new handler</param>
        /// <returns>Pipeline profile</returns>
        IPipelineProfile<TRequest, TResponse> AddNext(
            IRequestPipelineHandler<TRequest, TResponse> pipeline);

        /// <summary>
        /// Add a new next pipeline
        /// </summary>
        /// <typeparam name="TRequestPipelineHandler">The type of new handler</typeparam>
        /// <returns>Pipeline profile</returns>
        IPipelineProfile<TRequest, TResponse> AddNext<TRequestPipelineHandler>()
            where TRequestPipelineHandler : IRequestPipelineHandler<TRequest, TResponse>;

        /// <summary>
        /// Add a new next pipeline or ignore if it is null
        /// </summary>
        /// <param name="pipeline">Instance of the new handler or null</param>
        /// <returns>Pipeline profile</returns>
        IPipelineProfile<TRequest, TResponse> AddOptionNext(
            IRequestPipelineHandler<TRequest, TResponse>? pipeline);

        /// <summary>
        /// Add a new next pipeline or ignore if can't resolve
        /// </summary>
        /// <typeparam name="TRequestPipelineHandler">The type of new handler</typeparam>
        /// <returns>Pipeline profile</returns>
        IPipelineProfile<TRequest, TResponse> AddOptionNext<TRequestPipelineHandler>()
            where TRequestPipelineHandler : class, IRequestPipelineHandler<TRequest, TResponse>;

        /// <summary>
        /// Set a custom exception handler
        /// </summary>
        /// <param name="exceptionHandle">Instance of the exception handle</param>
        /// <returns>Pipeline profile</returns>
        IPipelineProfile<TRequest, TResponse> SetExceptionHandler(
            IExceptionHandle<TRequest, TResponse> exceptionHandle);

        /// <summary>
        /// Set a custom exception handler
        /// </summary>
        /// <typeparam name="TExceptionHandle">The type of exception handle</typeparam>
        /// <returns>Pipeline profile</returns>
        IPipelineProfile<TRequest, TResponse> SetExceptionHandler<TExceptionHandle>()
            where TExceptionHandle : IExceptionHandle<TRequest, TResponse>;

        /// <summary>
        /// Add a new other pipeline profile or ignore if it is null
        /// </summary>
        /// <param name="pipelineProfile">Instance of the new pipeline profile</param>
        /// <returns>Pipeline profile</returns>
        IPipelineProfile<TRequest, TResponse> AddOptionPipelineProfile(
            IPipelineProfile<TRequest, TResponse>? pipelineProfile);


        /// <summary>
        /// Add a new other pipeline profile or ignore if can't resolve
        /// </summary>
        /// <typeparam name="TPipelineProfile">The type of pipeline profile</typeparam>
        /// <returns>Pipeline profile</returns>
        IPipelineProfile<TRequest, TResponse> AddOptionPipelineProfile<TPipelineProfile>()
            where TPipelineProfile : class, IPipelineProfile<TRequest, TResponse>;

        /// <summary>
        /// Build a pipelines
        /// </summary>
        IPipelines<TRequest, TResponse> BuildPipelines();
    }
}