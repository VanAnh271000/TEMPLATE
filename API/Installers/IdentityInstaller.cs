using Application.Services.Identity;
using Domain.Entities.Identity;
using Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Installers
{
    public class IdentityInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            //For Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(opts =>
            {
                opts.SignIn.RequireConfirmedEmail = false;
                opts.Password.RequireDigit = true;
                opts.Password.RequireLowercase = true;
                opts.Password.RequireNonAlphanumeric = true;
                opts.Password.RequireUppercase = true;
                opts.Password.RequiredLength = 8;
                opts.Password.RequiredUniqueChars = 0;
            });

            services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(10));

            //Adding Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero,

                        ValidIssuer = configuration["JWT:ValidIssuer"],
                        ValidAudience = configuration["JWT:ValidAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"])),
                    };
                });

            services.AddAuthorization(options =>
            {
                #region Account
                options.AddPolicy("ReadAccount", policy =>
                    policy.Requirements.Add(new PermissionRequirement("account.read")));

                options.AddPolicy("WriteAccount", policy =>
                    policy.Requirements.Add(new PermissionRequirement("account.write")));

                options.AddPolicy("DeleteAccount", policy =>
                    policy.Requirements.Add(new PermissionRequirement("account.delete")));
                #endregion

            });

        }
    }
}
