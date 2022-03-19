using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Refit;
using RefitLab.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace RefitLab
{
  public class Startup
  {
    public Startup (IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices (IServiceCollection services)
    {

      services.AddControllers ();
      services.AddSwaggerGen (c =>
      {
        c.SwaggerDoc ("v1", new OpenApiInfo { Title = "RefitLab", Version = "v1" });
      });

      // Console.WriteLine(Path.Combine (Directory.GetCurrentDirectory(), "ssl/https.pfx"));
      // Console.WriteLine(Directory.GetCurrentDirectory());
      var clientCertificateHandler = new HttpClientHandler ();
      // clientCertificateHandler.ClientCertificates.Add (new X509Certificate2 (Path.Combine (Directory.GetCurrentDirectory (), "ssl/https.pfx"), "1234"));
      clientCertificateHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
      services.AddRefitClient<IpobxApi> ().ConfigureHttpClient (c => c.BaseAddress = new Uri ("https://localhost:5001"))
        .ConfigurePrimaryHttpMessageHandler (_ => clientCertificateHandler);
      // .ConfigurePrimaryHttpMessageHandler (_ => new HttpClientHandler
      // {
      //   ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
      // });

      services.AddAuthentication ()
        .AddCookie (CookieAuthenticationDefaults.AuthenticationScheme + "-CreatedByPobx", options =>
        {
          options.ExpireTimeSpan = TimeSpan.FromMinutes (5);
          options.SlidingExpiration = true;
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure (IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment ())
      {
        app.UseDeveloperExceptionPage ();
        app.UseSwagger ();
        app.UseSwaggerUI (c => c.SwaggerEndpoint ("/swagger/v1/swagger.json", "RefitLab v1"));
      }

      app.UseHttpsRedirection ();

      app.UseRouting ();

      app.UseAuthorization ();

      app.UseEndpoints (endpoints =>
      {
        endpoints.MapControllers ();
      });
    }
  }
}
