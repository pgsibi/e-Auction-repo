using EAuction.Auction.Domain.Aggregate;
using EAuction.Auction.Domain.Aggregate.AuctionAggregate;
using EAuction.Auction.Endpoint.Infrastructure;
using EAuction.Auction.Infrastructure;
using EAuction.Auction.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAuction.Bidding
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddCustomMassTransit(Configuration);

            services.Configure<MongoDbConfiguration>(Configuration.GetSection(nameof(MongoDbConfiguration)));
            services.AddSingleton<MongoDbConfiguration>(sp => sp.GetRequiredService<IOptions<MongoDbConfiguration>>().Value);
            services.AddScoped<IMongoContext, MongoContext>();
            services.AddScoped<IAuctionRepository, AuctionRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

           // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
