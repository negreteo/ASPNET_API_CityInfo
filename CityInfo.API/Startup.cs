using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Entities;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;

namespace CityInfo.API
{
   public class Startup
   {
      public static IConfiguration Configuration;

      public Startup (IHostingEnvironment env)
      {
         var builder = new ConfigurationBuilder ()
            .SetBasePath (env.ContentRootPath)
            .AddJsonFile ("appsettings.json", optional : false, reloadOnChange : true)
            .AddJsonFile ($"appsettings.{env.EnvironmentName}.json", optional : true, reloadOnChange : true);

         Configuration = builder.Build ();
      }

      // This method gets called by the runtime. Use this method to add services to the container. 
      public void ConfigureServices (IServiceCollection services)
      {
         services.AddMvc ();

         // AddTransient services are created everytime they are requested. Best for lightweight services.
         // AddScoped services are created once per request.  
         // AddSingleton services are created the first time they are requested.

         // Using concrete implementation for a service.
         //services.AddTransient<LocalMailServices> ();  

#if DEBUG
         services.AddTransient<IMailServices, LocalMailServices> ();
#else
         services.AddTransient<IMailServices, CloudMailServices> ();
#endif

         // Injecting DbContext
         //var connectionString = @"Server=(localdb)\mssqllocaldb;Database=CityInfoDB;Trusted_Connection=True;";
         //var connectionString = "server=localhost;database=CityInfoDB;user id=sa;password=LocalSQL123!;multipleactiveresultsets=true;";
         var connectionString = Startup.Configuration["connectionStrings:cityInfoDBConnectionString"];
         services.AddDbContext<CityInfoContext> (o => o.UseSqlServer (connectionString));

         services.AddScoped<ICityInfoRepository, CityInfoRepository> ();
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure (IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, CityInfoContext cityInfoContext)
      {
         //loggerFactory.AddConsole ();
         //loggerFactory.AddDebug ();

         //loggerFactory.AddProvider (new NLog.Extensions.Logging.NLogLoggerProvider ());
         //loggerFactory.AddNLog ();

         if (env.IsDevelopment ())
         {
            app.UseDeveloperExceptionPage ();
         }

         cityInfoContext.EnsureSeedDataForContext ();

         // Status codes handler (404, 202, etc)
         app.UseStatusCodePages ();

         AutoMapper.Mapper.Initialize (
            cfg =>
            {
               cfg.CreateMap<Entities.City, Models.CityWithoutPointsOfInterest> ();
               cfg.CreateMap<Entities.City, Models.City> ();
               cfg.CreateMap<Entities.PointOfInterest, Models.PointOfInterest> ();
               // Creation DTO Mapping
               cfg.CreateMap<Models.PointOfInterestForCreation, Entities.PointOfInterest> ();
               // Update DTO Mapping
               cfg.CreateMap<Models.PointOfInterestForUpdate, Entities.PointOfInterest> ();
               // Patch Entity Mappping
               cfg.CreateMap<Entities.PointOfInterest, Models.PointOfInterestForUpdate> ();
            }
         );

         app.UseMvc ();

         // app.Run (async (context) =>
         // {
         //    //throw new Exception ("Example exception");
         //    await context.Response.WriteAsync ("Hello World!");
         // });
      }

   }
}
