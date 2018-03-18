﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Linq;
using TarokScoreBoard.Api.Middleware;
using TarokScoreBoard.Api.Swagger;
using TarokScoreBoard.Core;
using TarokScoreBoard.Core.Entities;
using TarokScoreBoard.Infrastructure.Repositories;
using TarokScoreBoard.Infrastructure.Services;

namespace TarokScoreBoard.Api
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
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
      services.AddCors();

      // my dependencies
      services.AddScoped<GameRepository>();
      services.AddScoped<GamePlayerRepository>();
      services.AddScoped<RoundRepository>();
      services.AddScoped<RoundResultRepository>();
      services.AddScoped<RoundModifierRepository>();
      services.AddScoped<TeamRepository>();
      services.AddScoped<TeamPlayerRepository>();
      services.AddScoped<GameService>();
      services.AddScoped<ScoreBoardService>();
      services.AddScoped<TeamService>();
      services.AddScoped<StatisticsService>();
      services.AddScoped<AuthorizationService>();
      services.AddScoped<RequestContext>();
      services.AddScoped((ser) =>  new NpgsqlConnection(ser.GetService<IConfiguration>().GetConnectionString("tarok")));

      services.ConfigureValidationModel();

      services.AddSingleton(Configuration);      

       services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new Info { Title = "Tarok Scoreboard", Version = "v1"});
        c.SchemaFilter<SwaggerExampleFilter>();
        c.AddSecurityDefinition("access-token", new ApiKeyScheme()
        {
          Description = "Access token authentication",
          Name = "access-token",
          In = "header",
          Type = "apiKey"
        });
        c.OperationFilter<AuthResponsesOperationFilter>();
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      var allowedOrigins = Configuration.GetSection("CORS:AllowedOrigins").GetChildren().Select(c => c.Value).ToArray();

      app.UseCors(a =>
      a.WithOrigins(allowedOrigins)
      .SetPreflightMaxAge(TimeSpan.FromSeconds(3600))
      .AllowAnyHeader()
      .AllowAnyMethod());

      app.UseErrorHandling();

      app.UseMvc();
      app.UseSwagger(c =>
      {
        c.RouteTemplate = "api-docs/{documentName}/scoreboard.json";
      });

      app.UseSwaggerUI(c =>
      {
        c.DocumentTitle = "Tarok Scoreboard API";
        c.SwaggerEndpoint("/api-docs/v1/scoreboard.json", "Tarok Scoreboard API");
        c.DefaultModelsExpandDepth(0);
        c.EnableFilter();
        c.EnableDeepLinking();
        c.RoutePrefix = "";
      });

      DapperMapping.ConfigureColumnMapping();
    }
  }
}
