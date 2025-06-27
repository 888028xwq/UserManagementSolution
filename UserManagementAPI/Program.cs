var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ******* 添加 CORS 策略 Start *******
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                   // 允許所有標頭和所有 HTTP 方法 (GET, POST, PUT, DELETE 等)
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
// ******* 添加 CORS 策略 End *******

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ******* 使用 CORS 中介層 Start *******
app.UseCors("AllowSpecificOrigin");
// ******* 使用 CORS 中介層 End *******

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
