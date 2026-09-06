using ServiceFlow.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container.
builder.Services.AddControllers();

// 2. Register Swagger tools
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Register our In-Memory Domain services as Singletons
builder.Services.AddSingleton<CustomerService>();
builder.Services.AddSingleton<EmployeeService>();
builder.Services.AddSingleton<TicketService>();

var app = builder.Build();

// 4. Configure the HTTP request pipeline (Middleware)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
