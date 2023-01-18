using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Rabobank.TechnicalTest.GCOB.Repositories;
using Rabobank.TechnicalTest.GCOB.Services;
using Rabobank.TechnicalTest.GCOB.Validators;

namespace Rabobank.TechnicalTest.GCOB
{
    public class Startup
    {
        private readonly IWebHostEnvironment _currentEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _currentEnvironment = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (_currentEnvironment.IsDevelopment())
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "GCOB Service",
                        Version = "v1"
                    });
                });
            }

            // data repositories
            services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
            services.AddSingleton<IAddressRepository, InMemoryAddressRepository>();
            services.AddSingleton<ICountryRepository, InMemoryCountryRepository>();

            // services
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<ICountryService, CountryService>();

            // validation
            services.AddValidatorsFromAssemblyContaining<CustomerValidator>();

            // infrastructure
            services.AddControllers()
                .AddNewtonsoftJson();

            services.AddSingleton(sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger("DefaultLogger"));
            services.AddLogging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();

                app.UseSwaggerUI(setup =>
                {
                    setup.SwaggerEndpoint("/swagger/v1/swagger.json", "GCOB Service");
                    setup.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
