using Microsoft.Extensions.Options;
using NSwag.AspNetCore;

namespace LambdaReportsApp;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // このメソッドはランタイムによって呼び出されます。このメソッドを使用してコンテナにサービスを追加します
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApiDocument(config =>
        {
            config.PostProcess = document =>
            {
                document.Info.Title = "LambdaReportsAppAPI";
                document.Info.Version = "v1";
                document.Info.Description = "API Documentation";
            };
        });


    }

    // このメソッドはランタイムによって呼び出されます。このメソッドを使用してHTTPリクエストパイプラインを構成します
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseOpenApi();
        app.UseSwaggerUi(config =>
        {
            // カスタムファイルを追加
            config.CustomStylesheetPath = "/swagger-ui/custom.css";
            config.CustomJavaScriptPath = "/swagger-ui/custom.js";
        });

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }
}