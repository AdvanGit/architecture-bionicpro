using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var authority = builder.Configuration["Identity:Authority"];
var clientId = builder.Configuration["Identity:ClientId"];
var clientSecret = builder.Configuration["Identity:ClientSecret"];

var serializeOptions = new JsonSerializerOptions() { WriteIndented = true };

builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = authority;
    options.Audience = clientId;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = false,

        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
    };
    options.RequireHttpsMetadata = false;

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            if (context.Principal?.Identity is ClaimsIdentity identity)
            {
                var realmAccessClaim = identity.FindFirst("realm_access");
                if (realmAccessClaim != null)
                {
                    var realmAccess = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(realmAccessClaim.Value, serializeOptions);

                    if (realmAccess?.ContainsKey("roles") == true)
                    {
                        foreach (var role in realmAccess["roles"])
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, role));
                        }
                    }
                }
            }
        }
        
    };
});


var app = builder.Build();
app.UseCors(x => x
    .WithOrigins("http://localhost:3000")
    .AllowAnyHeader()
    .AllowAnyMethod()
  )
  .UseAuthentication()
  .UseAuthorization();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();