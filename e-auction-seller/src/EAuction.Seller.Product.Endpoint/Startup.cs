using EAuction.Seller.Product.Domain.Aggregate;
using EAuction.Seller.Product.Domain.Aggregate.SellerAggregate;
using EAuction.Seller.Product.Endpoint.Infrastructure;
using EAuction.Seller.Product.Infrastructure;
using EAuction.Seller.Product.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace EAuction.Seller.Product.Endpoint
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
            services.AddScoped<ISellerRepository, SellerRepository>();            
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
