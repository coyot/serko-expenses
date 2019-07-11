using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serko.Expenses.Core;
using Serko.Expenses.Core.Calculators;
using Serko.Expenses.Core.ValueFinders;

namespace Serko.Expenses
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
            services.AddSingleton<IValueFinder, TotalValueFinder>()
                    .AddSingleton<IValueFinder, CostCentreValueFinder>()
                    .AddSingleton<IValueFinder, DateValueFinder>()
                    .AddSingleton<IValueFinder, DescriptionValueFinder>()
                    .AddSingleton<IValueFinder, ExpenseValueFinder>()
                    .AddSingleton<IValueFinder, PaymentMethodValueFinder>()
                    .AddSingleton<IValueFinder, VendorValueFinder>();
            services.AddSingleton<ICalculator, TaxCalculator>();
            services.AddSingleton<IValuesExtractor, ValuesExtractor>();
            services.AddSingleton<IEngine, SerkoEngine>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddOpenApiDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
