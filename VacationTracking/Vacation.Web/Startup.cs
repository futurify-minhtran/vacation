using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace Vacation.Web
{
  public class Startup
  {
    public Startup(IHostingEnvironment env)
    {
    }

    public IConfigurationRoot Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddRouting();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseDefaultFiles();
      app.UseStaticFiles();

      var trackPackageRouteHandler = new RouteHandler(context =>
      {
        var file = env.WebRootFileProvider.GetFileInfo("index.html");
        return context.Response.SendFileAsync(file);
      });

      var routeBuilder = new RouteBuilder(app, trackPackageRouteHandler);
      routeBuilder.MapRoute(
       "vacationTracking",
       @"{*url:regex(^(?!api/).*(?<!\..*)$)}");
      var routes = routeBuilder.Build();
      app.UseRouter(routes);

    }
  }
}
