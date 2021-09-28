using EAuction.Buyer.Domain.Aggregate;
using EAuction.Buyer.Domain.Aggregate.BuyerAggregate;
using EAuction.Buyer.Endpoint.Infrastructure;
using EAuction.Buyer.Infrastucture;
using EAuction.Buyer.Infrastucture.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace EAuction.Buyer.Endpoint
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
            services.AddScoped<IBuyerRepository, BuyerRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
