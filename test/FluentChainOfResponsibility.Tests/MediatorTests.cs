using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Shouldly;
using Xunit;

namespace FluentChainOfResponsibility.Tests
{
    public class MediatorTests
    {
        public class Ping : IRequest<Pong>
        {
            public string Message { get; set; }
        }

        public class Pong
        {
            public string Message { get; set; }
        }

        public class PingPipelineHandler : IRequestPipelineHandler<Ping, Pong>
        {
            public Task<Pong> HandleAsync(
                Ping request, 
                IPipelineContext<Ping, Pong> pipelineContext, 
                CancellationToken cancellationToken)
            {
                return Task.FromResult(
                    new Pong
                    {
                        Message = $"{request.Message} Pong!",
                    });
            }
        }

        [Fact]
        public async Task Send_ShouldReturnCorrectResponse()
        {
            // arrange

            var builder = new ContainerBuilder();

            builder.Register<InstanceFactory>(componentContext =>
            {
                var context = componentContext.Resolve<IComponentContext>();
                return serviceType => context.Resolve(serviceType);
            });

            builder.RegisterType<Mediator>()
                .As<IMediator>();
            builder.RegisterType<PipelineContextFactory>()
                .As<IPipelineContextFactory>();
            builder
                .RegisterGeneric(typeof(PipelinesFactory<,>))
                .As(typeof(IPipelinesFactory<,>));

            builder.RegisterType<PingPipelineHandler>()
                .As<IRequestPipelineHandler<Ping, Pong>>();

            var container = builder.Build();

            var request = new Ping
            {
                Message = "Ping"
            };

            // act

            var actual = await container.Resolve<IMediator>().Send<Ping, Pong>(request, CancellationToken.None);

            // assert

            actual.ShouldNotBeNull();
            actual.IsFaulted.ShouldBeFalse();
            actual.Exception.ShouldBeNull();
            actual.Response.ShouldNotBeNull();
            actual.Response.Message.ShouldBe("Ping Pong!");
        }
            
        public class PingPipelineProfile : PipelineProfile<Ping, Pong>
        {
            public PingPipelineProfile(InstanceFactory instanceFactory) : base(instanceFactory)
            {
                AddNext<PingPipelineHandler>();
            }
        }

        [Fact]
        public async Task SendWithProfile_ShouldReturnCorrectResponse()
        {
            // arrange

            var builder = new ContainerBuilder();

            builder.Register<InstanceFactory>(componentContext =>
            {
                var context = componentContext.Resolve<IComponentContext>();
                return serviceType => context.Resolve(serviceType);
            });

            builder.RegisterType<Mediator>()
                .As<IMediator>();

            builder.RegisterType<PipelineContextFactory>()
                .As<IPipelineContextFactory>();

            builder.RegisterGeneric(typeof(PipelinesFactory<,>))
                .As(typeof(IPipelinesFactory<,>));

            builder.RegisterType<PingPipelineHandler>()
                .As<IRequestPipelineHandler<Ping, Pong>>()
                .AsSelf();

            builder.RegisterType<PingPipelineProfile>()
                .As<IPipelineProfile<Ping, Pong>>();

            var container = builder.Build();

            var request = new Ping
            {
                Message = "Ping"
            };

            // act

            var actual = await container.Resolve<IMediator>().Send<Ping, Pong>(request, CancellationToken.None);

            // assert

            actual.ShouldNotBeNull();
            actual.IsFaulted.ShouldBeFalse();
            actual.Exception.ShouldBeNull();
            actual.Response.ShouldNotBeNull();
            actual.Response.Message.ShouldBe("Ping Pong!");
        }
    }
}