using GraphiQl;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Udata;
using Udata.Graphqlcore;
using Udata.Interface;
using Udata.Models;
using Udata.Repository;

namespace TrailgraphQL
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
            //****Very Important Code for Json serialization****
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );


            var sqlConnectionString = Configuration["Data:DefaultConnection:ConnectionString"];
            services.AddDbContext<TechEventDBContext>(options => options.UseNpgsql(sqlConnectionString));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TrailgraphQL", Version = "v1" });
            });

            services.AddScoped<ITechEventRepository, TechEventRepository>();

            services.AddScoped<TechEventInfoType>();
            services.AddScoped<ParticipantType>();
            services.AddScoped<TechEventQuery>();

            services.AddScoped<TechEventInputType>();
            services.AddScoped<TechEventMutation>();

            var sp = services.BuildServiceProvider();
            services.AddSingleton<ISchema>
                (new TechEventSchema(new FuncDependencyResolver(type => sp.GetService(type))));

            services.AddScoped<IDocumentExecuter, DocumentExecuter>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseGraphiQl("/graphql");
                //app.UseSwagger();
               // app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TrailgraphQL v1"));
          
            }

            app.UseGraphiQl("/graphql");

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
////https://www.c-sharpcorner.com/article/building-api-in-net-core-with-graphql2/

/////https://www.c-sharpcorner.com/article/preforming-crud-operations-using-graphql-in-asp-net-core/