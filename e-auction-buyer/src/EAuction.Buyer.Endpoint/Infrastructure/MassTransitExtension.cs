using EAuction.Buyer.Endpoint.Handlers;
using EAuction.Buyer.Endpoint.Saga.AddBid;
using EAuction.Buyer.Endpoint.Saga.UpdateBid;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using System;

namespace EAuction.Buyer.Endpoint.Infrastructure
{
    public static class MassTransitExtension
    {
        /// <summary>
        /// AddCustomMassTransit
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomMassTransit(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitmqConfig = configuration.GetSection(nameof(RabbitMqConfiguration)).Get<RabbitMqConfiguration>();
            var sagaConfig = configuration.GetSection("AuctionSaga").Get<SagaDataBaseSettings>();
            services.AddSingleton<BsonClassMap<AddBidRequestState>, AddBidRequestClassMap>();
            services.AddMassTransit(x =>
            {
                x.AddConsumer<AddBidCommandHandler>();
                x.AddConsumer<AddBuyerCommandHandler>();
                x.AddConsumer<UpdateBidCommandHandler>();
                x.AddConsumer<GetBuyerIdByEmailIdQueryHandler>();

                x.SetKebabCaseEndpointNameFormatter();
                x.AddSagaStateMachine<AddBidRequestStateMachine, AddBidRequestState>()
                .MongoDbRepository(r =>
                {
                    r.Connection = sagaConfig.ConnectionString;
                    r.DatabaseName = sagaConfig.DatabaseName;
                });

                x.AddSagaStateMachine<UpdateBidRequestStateMachine, UpdateBidRequestState>()
                .MongoDbRepository(r =>
                {
                    r.Connection = sagaConfig.ConnectionString;
                    r.DatabaseName = sagaConfig.DatabaseName;
                });
                x.AddPublishMessageScheduler();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri(rabbitmqConfig.RabbitMqRootUri), h =>
                    {
                        h.Username(rabbitmqConfig.UserName);
                        h.Password(rabbitmqConfig.Password);
                    });
                    cfg.ConfigureEndpoints(context);

                });

            });

            services.AddMassTransitHostedService();
            services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());
            return services;
        }

        /// <summary>
        ///  EnableCustomMassTransit
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static IApplicationBuilder EnableCustomMassTransit(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            return app;
        }
    }
}
