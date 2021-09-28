using EAuction.Seller.Product.Endpoint.Handlers;
using EAuction.Seller.Product.Endpoint.Saga.AddProduct;
using EAuction.Seller.Product.Endpoint.Saga.DeleteProduct;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using System;

namespace EAuction.Seller.Product.Endpoint.Infrastructure
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
            var sagaConfig = configuration.GetSection("SellerProductSaga").Get<SagaDataBaseSettings>();
            services.AddSingleton<BsonClassMap<AddProductRequestState>, AddProductRequestClassMap>();
            services.AddMassTransit(x =>
            {
                x.AddConsumer<AddSellerProductCommandHandler>();
                x.AddConsumer<AddSellerCommandHandler>();
                x.AddConsumer<GetSellerIdQueryHandler>();
                x.AddConsumer<DeleteProductCommandHandler>();
                x.AddConsumer<GetSellerProductsQueryHandler>();
                x.SetKebabCaseEndpointNameFormatter();
                x.AddSagaStateMachine<AddProductRequestStateMachine, AddProductRequestState>()
                .MongoDbRepository(r =>
                {
                    r.Connection = sagaConfig.ConnectionString;
                    r.DatabaseName = sagaConfig.DatabaseName;
                });

                x.AddSagaStateMachine<DeleteProductRequestStateMachine, DeleteProductRequestState>()
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
