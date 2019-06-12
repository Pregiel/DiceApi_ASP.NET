using DiceApi.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DiceApi.Services;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using FluentValidation.AspNetCore;
using Newtonsoft.Json.Serialization;
using DiceApi.Hubs;

namespace DiceApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment CurrentEnvironment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment currentEnvironment)
        {
            Configuration = configuration;
            CurrentEnvironment = currentEnvironment;
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {

            services.AddCors();

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();

            if (CurrentEnvironment.IsEnvironment("Testing"))
            {
                services.AddDbContext<DataContext>(x => x.UseInMemoryDatabase("TestingDB"));
            }
            else
            {
                services.AddDbContext<DataContext>(x => x.UseMySql(appSettings.Database));
            }

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                })
                .AddFluentValidation(fv =>
                {
                    fv.ImplicitlyValidateChildProperties = true;
                });
            services.AddAutoMapper(typeof(Startup));

            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                        var userId = int.Parse(context.Principal.Identity.Name);
                        var user = userService.GetById(userId);
                        if (user == null)
                        {
                            context.Fail("Unauthorized");
                        }
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        if (string.IsNullOrEmpty(accessToken) == false)
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IUserRoomService, UserRoomService>();
            services.AddScoped<IRollService, RollService>();
            services.AddScoped<IRollValueService, RollValueService>();

            services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();

            app.UseMvc();

            app.UseStaticFiles();

            app.UseSignalR(routes =>
            {
                routes.MapHub<RoomHub>("/hub");
            });
        }
    }
}
