using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Domain.Repository.User;
using Domain.Service.User;
using Commons.ConfigModel;
using WebApi.Controllers;
using Swashbuckle.AspNetCore;
using System.Reflection;
using System.IO;

namespace WebApi
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
#if DEBUG
      services.AddMvc();
#else
        // トークンキーを発行
        services.AddMvc(options =>
            options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute()));
#endif

      // トークン設定
      services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

      // DIの設定
      services.AddScoped<IUserRepository, UserRepository>();
      services.AddScoped<IUserService, UserService>();

      // Configを専用Modelに設定
      services.Configure<DatabaseConfigModel>(this.Configuration.GetSection("DB"));

      // セッションの設定
      // Adds a default in-memory implementation of IDistributedCache.
      services.AddDistributedMemoryCache();

      // session
      services.AddSession(options =>
      {
        // Set a short timeout for easy testing.
        // options.IdleTimeout = TimeSpan.FromSeconds(10);
        // options.Cookie.HttpOnly = true;

        options.Cookie.Name = ControllerBase.SessionCookieName;
      });

#if DEBUG
      // SwaggerGenサービスの登録
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "My API", Version = "v1" });

        // XMLコメントを反映
        var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory,xmlFile);
        c.IncludeXmlComments(xmlPath);
      });
#endif

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, IAntiforgery antiforgery)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      // session有効化
      app.UseSession();

      app.UseMvc();
      // ルートアクセス時にトークン発行
      app.Use(next => context =>
      {
        if (
            string.Equals(context.Request.Path.Value, "/", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(context.Request.Path.Value, "/index.html", StringComparison.OrdinalIgnoreCase))
        {
          // We can send the request token as a JavaScript-readable cookie, and Angular will use it by default.
          var tokens = antiforgery.GetAndStoreTokens(context);
          context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken,
                    new CookieOptions() { HttpOnly = false });
        }

        return next(context);
      });

      // 静的ファイルのデフォルト設定を有効にする
      app.UseDefaultFiles();

      // 静的ファイルを使用する
      app.UseStaticFiles();

#if DEBUG
      // Swaggerミドルウェアの登録
      app.UseSwagger();
      // SwaggerUIミドルウェアの登録
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
      });
#endif

    }
  }
}
