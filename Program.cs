#region Usings
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Services;
using minimal_api.Domain.ViewsModal;
using minimal_api.Infrastructure.Db;
using minimal_api.Infrastructure.Interfaces;
#endregion

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administrator
app.MapPost("/administrators/login", ([FromBody]LoginDTO loginDTO, IAdministratorService administratorService) => 
{
    if(administratorService.Login(loginDTO) != null)
        return Results.Ok("Login realizado com sucesso!");
    else
        return Results.Unauthorized();       
}).WithTags("Administrators");
#endregion

#region Vehicle
app.MapPost("/vehicles", ([FromBody]VehicleDTO vehicleDTO, IVehicleService vehicleService) => 
{
    var vehicles = new Vehicle
    {
        Name = vehicleDTO.Name,
        Brand = vehicleDTO.Brand,
        Year = vehicleDTO.Year,
    };
    vehicleService.PostVehicle(vehicles);     

    return Results.Created($"/vehicle/{vehicles.Id}", vehicles);  
}).WithTags("Vehicles");

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) =>{
    var vehicles = vehicleService.GetAllVehicles(page);

    return Results.Ok(vehicles);
}).WithTags("Vehicles");

app.MapGet("/vehicles{id}", ([FromRoute] int id, IVehicleService vehicleService) =>{
    var vehicle = vehicleService.GetById(id);
    
    if(vehicle == null) return Results.NotFound();

    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapPut("/vehicles{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>{
    var vehicle = vehicleService.GetById(id);
    
    if(vehicle == null) return Results.NotFound();

    vehicle.Name = vehicleDTO.Name;
    vehicle.Brand = vehicleDTO.Brand;
    vehicle.Year = vehicleDTO.Year;

    vehicleService.UpdateVehicle(vehicle);

    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapDelete("/vehicles{id}", ([FromRoute] int id, IVehicleService vehicleService) =>{
    var vehicle = vehicleService.GetById(id);
    
    if(vehicle == null) return Results.NotFound();

    vehicleService.DeleteVehicle(vehicle);

    return Results.NoContent();
}).WithTags("Vehicles");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion