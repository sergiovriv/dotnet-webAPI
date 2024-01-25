using taskmanagementAPI.Data;
using Microsoft.EntityFrameworkCore;
using taskmanagementAPI.Services;
using Microsoft.AspNetCore.Authentication;
using taskmanagementAPI.Security;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddLogging();
builder.Services.AddHostedService<MachineStatsService>();
builder.Services.AddHostedService<DataBackupService>();

//Add the basic auth scheme
builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

//swagger configs
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.ToString()); //solve name conflicts

    //Use basic auth
    options.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic Authentication header using the Bearer scheme."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            Array.Empty<string>()
        }
    });
});

//conecction to the db setup
builder.Services.AddDbContext<TaskManagementContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TaskManagementContext")));

var app = builder.Build();


//http request pipeline configure
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication(); //important that authentication called before authorization!
app.UseAuthorization();

app.MapControllers();

app.Run();
