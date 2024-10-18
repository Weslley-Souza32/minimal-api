#region Usings
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Enums;
using minimal_api.Domain.Services;
using minimal_api.Domain.ViewsModal;
using minimal_api.Infrastructure.Db;
using minimal_api.Infrastructure.Interfaces;
#endregion

#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if(string.IsNullOrEmpty(key)) key = "123456";

builder.Services.AddAuthentication(opton => {
    opton.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opton.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option => {
    option.TokenValidationParameters = new TokenValidationParameters{
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => 
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insirar o token JWT aqui"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
#endregion

#region Administrator
#region Metodo para autenficar
string GenerateTokenJWT(Administrator administrator)
{
    if(string.IsNullOrEmpty(key)) return string.Empty;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", administrator.Email),
        new Claim("Profile", administrator.Profile),
        new Claim(ClaimTypes.Role, administrator.Profile)
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
    );
    return new JwtSecurityTokenHandler().WriteToken(token);
}
#endregion
app.MapPost("/administrators/login", ([FromBody]LoginDTO loginDTO, IAdministratorService administratorService) => 
{
    var adm = administratorService.Login(loginDTO);
    if(adm != null)
    {
        string token = GenerateTokenJWT(adm);
        return Results.Ok(new AdministratorLoggedIn
        {
            Email = adm.Email,
            Profile = adm.Profile,
            Token = token
        });
    }
    else
        return Results.Unauthorized();       
}).AllowAnonymous().WithTags("Administrators");

app.MapPost("/administrators",([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) =>
{
    var validacao = new ValidationErrors
    {
        Messages = []
    };

    if(string.IsNullOrEmpty(administratorDTO.Email))
        validacao.Messages.Add("Email não pode ser nulo ou vazio!");
    if(string.IsNullOrEmpty(administratorDTO.Password))
        validacao.Messages.Add("Senha não pode ser nula ou vazia!");
    if(administratorDTO.Profile == null)
        validacao.Messages.Add("O perfil não pode ser nulo ou vazio!");
    
    if(validacao.Messages.Count > 0)
        return Results.BadRequest(validacao);
    
    var administrator = new Administrator
    {
        Email = administratorDTO.Email,
        Password = administratorDTO.Password,
        Profile = administratorDTO.Profile.ToString() ?? Profile.Editor.ToString(),
    };
    administratorService.PostAdministrator(administrator);

    return Results.Created($"/administrator/{administrator.Id}", new AdministratorViewsModel
    {
        Id = administrator.Id,
        Email = administrator.Email,
        Profile = administrator.Profile
    });
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
.WithTags("Administrators");;

app.MapGet("/administrators", ([FromQuery] int? page, IAdministratorService administratorService) =>
{
    var adms = new List<AdministratorViewsModel>();
    var administrators = administratorService.GetAllAdministrators(page);

    foreach(var adm in administrators)
    {
        adms.Add(new AdministratorViewsModel
        {
            Id = adm.Id,
            Email = adm.Email,
            Profile = adm.Profile
        });
    }
    return Results.Ok(adms);
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
.WithTags("Administrators");

app.MapGet("administrator{id}",([FromRoute] int id, IAdministratorService administratorService) =>
{
    var administrator = administratorService.GetById(id);
    if(administrator == null) return Results.NotFound();
    return Results.Ok(new AdministratorViewsModel
    {
        Id = administrator.Id,
        Email = administrator.Email,
        Profile = administrator.Profile
    });
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
.WithTags("Administrators");
#endregion

#region Metodo de validação
ValidationErrors validaDTO(VehicleDTO vehicleDTO)
{
     var validacao = new ValidationErrors
     {
        Messages = new List<string>()
     };

    if(string.IsNullOrEmpty(vehicleDTO.Name))
        validacao.Messages.Add("O nome não pode ser nulo ou vazio!");

    if(string.IsNullOrEmpty(vehicleDTO.Brand))
        validacao.Messages.Add("A marca não pode ser nula ou vazia!");

    if(vehicleDTO.Year < 1950)
        validacao.Messages.Add("Veículo muito antigo, aceitamos veículos superiores ao ano de 1950!");

    return validacao;
}
#endregion

#region Vehicle
app.MapPost("/vehicles", ([FromBody]VehicleDTO vehicleDTO, IVehicleService vehicleService) => 
{
    var validacao = validaDTO(vehicleDTO);

    if(validacao.Messages.Count > 0)
        return Results.BadRequest(validacao);
    
    var vehicles = new Vehicle
    {
        Name = vehicleDTO.Name,
        Brand = vehicleDTO.Brand,
        Year = vehicleDTO.Year,
    };
    vehicleService.PostVehicle(vehicles);     

    return Results.Created($"/vehicle/{vehicles.Id}", vehicles);  
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute {Roles = "Adm, Editor"})
.WithTags("Vehicles");

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) =>{
    var vehicles = vehicleService.GetAllVehicles(page);

    return Results.Ok(vehicles);
}).RequireAuthorization().WithTags("Vehicles");

app.MapGet("/vehicles{id}", ([FromRoute] int id, IVehicleService vehicleService) =>{
    var vehicle = vehicleService.GetById(id);
    
    if(vehicle == null) return Results.NotFound();

    return Results.Ok(vehicle);
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute {Roles = "Adm, Editor"})
.WithTags("Vehicles");

app.MapPut("/vehicles{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.GetById(id);
    if(vehicle == null) return Results.NotFound();

    var validacao = validaDTO(vehicleDTO);
    if(validacao.Messages.Count > 0)
        return Results.BadRequest(validacao);

    vehicle.Name = vehicleDTO.Name;
    vehicle.Brand = vehicleDTO.Brand;
    vehicle.Year = vehicleDTO.Year;

    vehicleService.UpdateVehicle(vehicle);

    return Results.Ok(vehicle);
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
.WithTags("Vehicles");

app.MapDelete("/vehicles{id}", ([FromRoute] int id, IVehicleService vehicleService) =>{
    var vehicle = vehicleService.GetById(id);
    
    if(vehicle == null) return Results.NotFound();

    vehicleService.DeleteVehicle(vehicle);

    return Results.NoContent();
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
.WithTags("Vehicles");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion