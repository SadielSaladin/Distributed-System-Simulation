using Distributed_System_Simulation.Services;
using Distributed_System_Simulation.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

List<int> nodeIds = new List<int> { 1, 2, 3, 4, 5 };

builder.Services.AddSingleton<INetworkService, NetworkService>();
builder.Services.AddSingleton<INodeServices>(sp =>
    new NodeServices(sp.GetRequiredService<INetworkService>())
);
builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();


}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
