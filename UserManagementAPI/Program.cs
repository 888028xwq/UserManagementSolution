using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;

// 20250630 mod by jimmy for 使用者資料串聯資料庫
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 20250630 mod by jimmy for 使用者資料串聯資料庫
// ****** 添加資料庫連接和 DbContext 註冊 Start ******
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//var mysqlPassword = Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD");

//// 檢查密碼是否為空，如果為空可能會有問題
//if (string.IsNullOrEmpty(mysqlPassword))
//{
//    // 暫時允許它為空，但實際運行可能導致連接失敗
//    Console.WriteLine("警告：未找到環境變數 MYSQL_ROOT_PASSWORD。資料庫連接可能失敗。");
//}

//var fullConnectionString = $"{connectionString}Pwd={mysqlPassword};";
builder.Services.AddDbContext<UserManagementDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)) // 接收DB資料與檢測 MySQL Server 版本
);
// ****** 添加資料庫連接和 DbContext 註冊 End ******

// ******* 添加 CORS 策略 Start *******
// Angular 前端 (http://localhost:4200) 與 ASP.NET Core 後端 API (https://localhost:7278)
// 他們位於不同的埠號，所以會被視為不同的「來源」(Origin)。所以需要依賴CORS來控制與轉換
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200") // 前端的URL
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
