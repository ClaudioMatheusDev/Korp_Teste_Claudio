using Estoque.Application.Interfaces;
using Estoque.Application.Services;
using Estoque.Infrastructure;
using Estoque.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapControllers();



app.Run();


