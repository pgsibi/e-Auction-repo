using EAuction.Auction.Endpoint.Handlers;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EAuction.Auction.Endpoint.Infrastructure
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

            services.AddMassTransit(x =>
            {
                x.AddConsumer<AddAuctionItemCommandHandler>();
                x.AddConsumer<DeleteAuctionItemCommandHandler>();
                x.AddConsumer<AddBidCommandHandler>();
                x.AddConsumer<UpdateBidAmountCommandHandler>();
                x.SetKebabCaseEndpointNameFormatter();
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
