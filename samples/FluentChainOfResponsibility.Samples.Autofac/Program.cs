using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Autofac;

namespace FluentChainOfResponsibility.Samples.Autofac
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var ping = new Ping
            {
                Message = "Ping",
            };

            var mediator = CreateMediator();

            var pipelineContext = await mediator.Send<Ping, Pong>(ping);

            Console.WriteLine($"Response: {pipelineContext.Response.Message}");
        }

        private static IMediator CreateMediator()
        {
            var builder = new ContainerBuilder();

            builder.Register(x => Console.Out);

            builder.Register<InstanceFactory>(componentContext =>
            {
                var context = componentContext.Resolve<IComponentContext>();
                return serviceType => context.Resolve(serviceType);
            });

            builder.RegisterType<Mediator>()
                .As<IMediator>()
                .SingleInstance();

            builder.RegisterType<PipelineContextFactory>()
                .As<IPipelineContextFactory>();

            builder.RegisterGeneric(typeof(PipelinesFactory<,>))
                .As(typeof(IPipelinesFactory<,>));

            builder.RegisterGeneric(typeof(DefaultPipelineProfile<,>))
                .As(typeof(PipelineProfile<,>))
                .As(typeof(IPipelineProfile<,>));

            builder.RegisterType<PingLogPipelineHandler>()
                .As<ILogPipelineHandler<Ping, Pong>>();

            builder.RegisterType<PingValidationPipelineHandler>()
                .As<IValidationPipelineHandler<Ping, Pong>>();

            builder.RegisterType<PingEndPipelineHandler>()
                .As<IEndPipelineHandler<Ping, Pong>>();

            builder.RegisterType<PingExceptionHandle>()
                .As<IExceptionHandle<Ping, Pong>>();

            var container = builder.Build();

            return container.Resolve<IMediator>();
        }
    }

    public class Ping : IRequest<Pong>
    {
        public string Message { get; set; }
    }

    public class Pong
    {
        public string Message { get; set; }
    }

    public interface ILogPipelineHandler<TRequest, TResponse> : IRequestPipelineHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {

    }

    public interface IValidationPipelineHandler<TRequest, TResponse> : IRequestPipelineHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {

    }

    public interface IOtherPipelineHandler<TRequest, TResponse> : IRequestPipelineHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {

    }

    public interface IEndPipelineHandler<TRequest, TResponse> : IRequestPipelineHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {

    }

    public class PingLogPipelineHandler : ILogPipelineHandler<Ping, Pong>
    {
        private readonly TextWriter _logger;

        public PingLogPipelineHandler(TextWriter logger)
        {
            _logger = logger;
        }

        public async Task<Pong> HandleAsync(Ping request, IPipelineContext<Ping, Pong> pipelineContext, CancellationToken cancellationToken)
        {
            _logger.WriteLine("--Begin handle Ping request");
            var result = await pipelineContext.Next(request, cancellationToken);
            _logger.WriteLine("--End handle Ping request");

            return result;
        }
    }

    public class PingValidationPipelineHandler : IValidationPipelineHandler<Ping, Pong>
    {
        public async Task<Pong> HandleAsync(Ping request, IPipelineContext<Ping, Pong> pipelineContext, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(request.Message))
                throw new InvalidOperationException("Message can't be null or empty!");
            return await pipelineContext.Next(request, cancellationToken);
        }
    }

    public class PingEndPipelineHandler : IEndPipelineHandler<Ping, Pong>
    {
        public Task<Pong> HandleAsync(Ping request, IPipelineContext<Ping, Pong> pipelineContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Pong
            {
                Message = request.Message + " Pong!",
            });
        }
    }

    public class PingExceptionHandle : IExceptionHandle<Ping, Pong>
    {
        private readonly TextWriter _logger;

        public PingExceptionHandle(TextWriter logger)
        {
            _logger = logger;
        }

        public Task Handle(Exception exception, IPipelineContext<Ping, Pong> pipelineContext, CancellationToken cancellationToken)
        {
            _logger.WriteLine($"An error occurred while processing the request!\nError message: {exception.Message}");
            _logger.WriteLine(exception.StackTrace);

            return Task.CompletedTask;
        }
    }


    public class DefaultPipelineProfile<TRequest, TResponse> : PipelineProfile<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {
        public DefaultPipelineProfile(InstanceFactory instanceFactory) : base(instanceFactory)
        {
            AddNext<ILogPipelineHandler<TRequest, TResponse>>();
            AddNext<IValidationPipelineHandler<TRequest, TResponse>>();
            AddOptionNext<IOtherPipelineHandler<TRequest, TResponse>>();
            AddNext<IEndPipelineHandler<TRequest, TResponse>>();
        }
    }
}
