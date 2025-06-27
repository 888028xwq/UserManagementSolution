var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ******* �K�[ CORS ���� Start *******
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                   // ���\�Ҧ����Y�M�Ҧ� HTTP ��k (GET, POST, PUT, DELETE ��)
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
// ******* �K�[ CORS ���� End *******

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ******* �ϥ� CORS �����h Start *******
app.UseCors("AllowSpecificOrigin");
// ******* �ϥ� CORS �����h End *******

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
