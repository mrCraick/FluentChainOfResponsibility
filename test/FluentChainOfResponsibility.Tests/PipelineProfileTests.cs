using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Moq;
using Shouldly;
using Xunit;

namespace FluentChainOfResponsibility.Tests
{
    public class PipelineProfileTests
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
        public void AddNext_RequestPipelineHandlers_ShouldReturnCollectionWithAddedItem()
        {
            // arrange

            var builder = new ContainerBuilder();

            builder.Register<InstanceFactory>(componentContext =>
            {
                var context = componentContext.Resolve<IComponentContext>();
                return serviceType => context.Resolve(serviceType);
            });


            var container = builder.Build();

            var profile = new Mock<PipelineProfile<Ping, Pong>>(container.Resolve<InstanceFactory>())
            {
                CallBase = true,
            }.Object;

            // act

            var actual = profile.AddNext(new PingPipelineHandler())
                .RequestPipelineHandlers
                .FirstOrDefault();

            // assert

            actual.ShouldNotBeNull();
            actual.GetType().ShouldBe(typeof(PingPipelineHandler));
        }

        [Fact]
        public void AddOptionNext_RequestPipelineHandlers_ShouldReturnCollectionWithAddedItem()
        {
            // arrange

            var builder = new ContainerBuilder();

            builder.Register<InstanceFactory>(componentContext =>
            {
                var context = componentContext.Resolve<IComponentContext>();
                return serviceType => context.Resolve(serviceType);
            });

            var container = builder.Build();

            var profile = new Mock<PipelineProfile<Ping, Pong>>(container.Resolve<InstanceFactory>())
            {
                CallBase = true,
            }.Object;

            // act

            var actual = profile.AddOptionNext(new PingPipelineHandler())
                .RequestPipelineHandlers
                .FirstOrDefault();

            // assert

            actual.ShouldNotBeNull();
            actual.GetType().ShouldBe(typeof(PingPipelineHandler));
        }

        [Fact]
        public void AddOptionNext_RequestPipelineHandlers_ShouldReturnCollectionWithZeroItem()
        {
            // arrange

            var builder = new ContainerBuilder();

            builder.Register<InstanceFactory>(componentContext =>
            {
                var context = componentContext.Resolve<IComponentContext>();
                return serviceType => context.Resolve(serviceType);
            });

            var container = builder.Build();

            var profile = new Mock<PipelineProfile<Ping, Pong>>(container.Resolve<InstanceFactory>())
            {
                CallBase = true,
            }.Object;

            // act

            var actual = profile.AddOptionNext(default)
                .RequestPipelineHandlers;

            // assert

            actual.ShouldNotBeNull();
            actual.Count.ShouldBe(0);
        }

        [Fact]
        public void AddNext_FromContainer_RequestPipelineHandlers_ShouldReturnCollectionWithAddedItem()
        {
            // arrange

            var builder = new ContainerBuilder();

            builder.Register<InstanceFactory>(componentContext =>
            {
                var context = componentContext.Resolve<IComponentContext>();    
                return serviceType => context.Resolve(serviceType);
            });

            builder.RegisterType<PingPipelineHandler>()
                .As<IRequestPipelineHandler<Ping, Pong>>()
                .AsSelf();

            var container = builder.Build();

            var profile = new Mock<PipelineProfile<Ping, Pong>>(container.Resolve<InstanceFactory>())
            {
                CallBase = true,
            }.Object;

            // act

            var actual = profile.AddNext<PingPipelineHandler>()
                .RequestPipelineHandlers
                .FirstOrDefault();

            // assert

            actual.ShouldNotBeNull();
            actual.GetType().ShouldBe(typeof(PingPipelineHandler));
        }


        [Fact]
        public void AddOptionNext_FromContainer_RequestPipelineHandlers_ShouldReturnCollectionWithAddedItem()
        {
            // arrange

            var builder = new ContainerBuilder();

            builder.Register<InstanceFactory>(componentContext =>
            {
                var context = componentContext.Resolve<IComponentContext>();
                return serviceType => context.Resolve(serviceType);
            });

            builder.RegisterType<PingPipelineHandler>()
                .As<IRequestPipelineHandler<Ping, Pong>>()
                .AsSelf();

            var container = builder.Build();

            var profile = new Mock<PipelineProfile<Ping, Pong>>(container.Resolve<InstanceFactory>())
            {
                CallBase = true,
            }.Object;

            // act

            var actual = profile.AddOptionNext<PingPipelineHandler>()
                .RequestPipelineHandlers
                .FirstOrDefault();

            // assert

            actual.ShouldNotBeNull();
            actual.GetType().ShouldBe(typeof(PingPipelineHandler));
        }

        [Fact]
        public void AddOptionNext_FromContainer_RequestPipelineHandlers_ShouldReturnCollectionWithZeroItem()
        {
            // arrange

            var builder = new ContainerBuilder();

            builder.Register<InstanceFactory>(componentContext =>
            {
                var context = componentContext.Resolve<IComponentContext>();
                return serviceType => context.Resolve(serviceType);
            });

            var container = builder.Build();

            var profile = new Mock<PipelineProfile<Ping, Pong>>(container.Resolve<InstanceFactory>())
            {
                CallBase = true,
            }.Object;

            // act

            var actual = profile.AddOptionNext<PingPipelineHandler>()
                .RequestPipelineHandlers;

            // assert

            actual.ShouldNotBeNull();
            actual.Count.ShouldBe(0);
        }

        public class OtherPipelineHandler : IRequestPipelineHandler<Ping, Pong>
        {
            public async Task<Pong> HandleAsync(Ping request, IPipelineContext<Ping, Pong> pipelineContext, CancellationToken cancellationToken)
            {
                request.Message += " other";

                return await pipelineContext.Next(request, cancellationToken);
            }
        }

        public class OtherPipelineProfile : PipelineProfile<Ping, Pong>
        {
            public OtherPipelineProfile(InstanceFactory instanceFactory) : base(instanceFactory)
            {
                AddNext<OtherPipelineHandler>();
            }
        }

        [Fact]
        public void AddPipelineProfileAndAddNext_RequestPipelineHandlers_ShouldReturnCollectionWithAddedItems()
        {
            // arrange

            var builder = new ContainerBuilder();

            builder.Register<InstanceFactory>(componentContext =>
            {
                var context = componentContext.Resolve<IComponentContext>();
                return serviceType => context.Resolve(serviceType);
            });

            builder.RegisterType<PingPipelineHandler>()
                .As<IRequestPipelineHandler<Ping, Pong>>()
                .AsSelf();

            builder.RegisterType<OtherPipelineHandler>()
                .As<IRequestPipelineHandler<Ping, Pong>>()
                .AsSelf();


            var container = builder.Build();

            var profile = new Mock<PipelineProfile<Ping, Pong>>(container.Resolve<InstanceFactory>())
            {
                CallBase = true,
            }.Object;

            // act

            var actual = profile
                .AddPipelineProfile(new OtherPipelineProfile(container.Resolve<InstanceFactory>()))
                .AddNext<PingPipelineHandler>()
                .RequestPipelineHandlers
                .FirstOrDefault();

            // assert

            actual.ShouldNotBeNull();
            actual.GetType().ShouldBe(typeof(OtherPipelineHandler));
        }

        [Fact]
        public void AddPipelineProfileAndAddNext_FromContainer_RequestPipelineHandlers_ShouldReturnCollectionWithAddedItems()
        {
            // arrange

            var builder = new ContainerBuilder();

            builder.Register<InstanceFactory>(componentContext =>
            {
                var context = componentContext.Resolve<IComponentContext>();
                return serviceType => context.Resolve(serviceType);
            });

            builder.RegisterType<PingPipelineHandler>()
                .As<IRequestPipelineHandler<Ping, Pong>>()
                .AsSelf();

            builder.RegisterType<OtherPipelineHandler>()
                .As<IRequestPipelineHandler<Ping, Pong>>()
                .AsSelf();

            builder.RegisterType<OtherPipelineProfile>()
                .As<PipelineProfile<Ping, Pong>>()
                .As<IPipelineProfile<Ping, Pong>>()
                .AsSelf();

            var container = builder.Build();

            var profile = new Mock<PipelineProfile<Ping, Pong>>(container.Resolve<InstanceFactory>())
            {
                CallBase = true,
            }.Object;

            // act

            var actual = profile
                .AddPipelineProfile<OtherPipelineProfile>()
                .AddNext<PingPipelineHandler>()
                .RequestPipelineHandlers
                .FirstOrDefault();

            // assert

            actual.ShouldNotBeNull();
            actual.GetType().ShouldBe(typeof(OtherPipelineHandler));
        }

        [Fact]
        public void AddOptionPipelineProfile_RequestPipelineHandlers_ShouldReturnCollectionWithZeroItems()
        {
            // arrange

            var builder = new ContainerBuilder();

            builder.Register<InstanceFactory>(componentContext =>
            {
                var context = componentContext.Resolve<IComponentContext>();
                return serviceType => context.Resolve(serviceType);
            });


            var container = builder.Build();

            var profile = new Mock<PipelineProfile<Ping, Pong>>(container.Resolve<InstanceFactory>())
            {
                CallBase = true,
            }.Object;

            // act

            var actual = profile
                .AddOptionPipelineProfile(default)
                .RequestPipelineHandlers;

            // assert

            actual.ShouldNotBeNull();
            actual.Count.ShouldBe(0);
        }

        [Fact]
        public void AddOptionPipelineProfile_RequestPipelineHandlers_ShouldReturnCollectionWithAddedItem()
        {
            // arrange

            var builder = new ContainerBuilder();

            builder.Register<InstanceFactory>(componentContext =>
            {
                var context = componentContext.Resolve<IComponentContext>();
                return serviceType => context.Resolve(serviceType);
            });

            builder.RegisterType<OtherPipelineHandler>()
                .As<IRequestPipelineHandler<Ping, Pong>>()
                .AsSelf();


            var container = builder.Build();

            var profile = new Mock<PipelineProfile<Ping, Pong>>(container.Resolve<InstanceFactory>())
            {
                CallBase = true,
            }.Object;

            // act

            var actual = profile
                .AddOptionPipelineProfile(new OtherPipelineProfile(container.Resolve<InstanceFactory>()))
                .RequestPipelineHandlers
                .FirstOrDefault();

            // assert

            actual.ShouldNotBeNull();
            actual.GetType().ShouldBe(typeof(OtherPipelineHandler));
        }

        [Fact]
        public void AddOptionPipelineProfile_FromContainer_RequestPipelineHandlers_ShouldReturnCollectionWithAddedItems()
        {
            // arrange

            var builder = new ContainerBuilder();

            builder.Register<InstanceFactory>(componentContext =>
            {
                var context = componentContext.Resolve<IComponentContext>();
                return serviceType => context.Resolve(serviceType);
            });

            builder.RegisterType<PingPipelineHandler>()
                .As<IRequestPipelineHandler<Ping, Pong>>()
                .AsSelf();

            var container = builder.Build();

            var profile = new Mock<PipelineProfile<Ping, Pong>>(container.Resolve<InstanceFactory>())
            {
                CallBase = true,
            }.Object;

            // act

            var actual = profile
                .AddOptionPipelineProfile<OtherPipelineProfile>()
                .RequestPipelineHandlers;

            // assert

            actual.ShouldNotBeNull();
            actual.Count.ShouldBe(0);
        }

        [Fact]
        public void AddOptionPipelineProfile_FromContainer_RequestPipelineHandlers_ShouldReturnCollectionWithAddedItem()
        {
            // arrange

            var builder = new ContainerBuilder();

            builder.Register<InstanceFactory>(componentContext =>
            {
                var context = componentContext.Resolve<IComponentContext>();
                return serviceType => context.Resolve(serviceType);
            });

            builder.RegisterType<OtherPipelineHandler>()
                .As<IRequestPipelineHandler<Ping, Pong>>()
                .AsSelf();

            builder.RegisterType<OtherPipelineProfile>()
                .As<PipelineProfile<Ping, Pong>>()
                .As<IPipelineProfile<Ping, Pong>>()
                .AsSelf();

            var container = builder.Build();

            var profile = new Mock<PipelineProfile<Ping, Pong>>(container.Resolve<InstanceFactory>())
            {
                CallBase = true,
            }.Object;

            // act

            var actual = profile
                .AddOptionPipelineProfile<OtherPipelineProfile>()
                .RequestPipelineHandlers
                .FirstOrDefault();

            // assert

            actual.ShouldNotBeNull();
            actual.GetType().ShouldBe(typeof(OtherPipelineHandler));
        }

        public class PingExceptionHandler : IExceptionHandle<Ping, Pong>
        {
            public const string IT_HAS_BEEN_HANDLED = "It has been handled.";
            public static string Message { get; set; }

            public Task Handle(Exception exception, IPipelineContext<Ping, Pong> pipelineContext, CancellationToken cancellationToken)
            {
                Message = IT_HAS_BEEN_HANDLED;

                return Task.CompletedTask;
            }
        }

        public class BadPingPipelineHandler : IRequestPipelineHandler<Ping, Pong>
        {
            public Task<Pong> HandleAsync(Ping request, IPipelineContext<Ping, Pong> pipelineContext, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public async Task SetExceptionHandler_FromContainer_RequestPipelineHandlers_ShouldBeHandled()
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


            builder.Register(x =>
            {
                var profile = new Mock<PipelineProfile<Ping, Pong>>(x.Resolve<InstanceFactory>())
                    {
                        CallBase = true,
                    }
                    .Object
                    .AddNext(new BadPingPipelineHandler())
                    .SetExceptionHandler(new PingExceptionHandler());

                return profile;

            }).As<IPipelineProfile<Ping, Pong>>();

            var container = builder.Build();


            // act

            var actual = await container.Resolve<IMediator>().Send<Ping, Pong>(new Ping(), CancellationToken.None);

            // assert

            actual.ShouldNotBeNull();
            actual.IsFaulted.ShouldBeTrue();
            actual.Exception.ShouldNotBeNull();
            PingExceptionHandler.Message.ShouldBe(PingExceptionHandler.IT_HAS_BEEN_HANDLED);
        }

        [Fact]
        public async Task SetExceptionHandler_FromContainer_FromContainer_RequestPipelineHandlers_ShouldBeHandled()
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

            builder.RegisterType<PingExceptionHandler>()
                .AsSelf();


            builder.Register(x =>
            {
                var profile = new Mock<PipelineProfile<Ping, Pong>>(x.Resolve<InstanceFactory>())
                    {
                        CallBase = true,
                    }
                    .Object
                    .AddNext(new BadPingPipelineHandler())
                    .SetExceptionHandler<PingExceptionHandler>();

                return profile;

            }).As<IPipelineProfile<Ping, Pong>>();

            var container = builder.Build();


            // act

            var actual = await container.Resolve<IMediator>().Send<Ping, Pong>(new Ping(), CancellationToken.None);

            // assert

            actual.ShouldNotBeNull();
            actual.IsFaulted.ShouldBeTrue();
            actual.Exception.ShouldNotBeNull();
            PingExceptionHandler.Message.ShouldBe(PingExceptionHandler.IT_HAS_BEEN_HANDLED);
        }
    }


}