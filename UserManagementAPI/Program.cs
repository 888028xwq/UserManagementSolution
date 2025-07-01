using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;

// 20250630 mod by jimmy for �ϥΪ̸�Ʀ��p��Ʈw
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 20250630 mod by jimmy for �ϥΪ̸�Ʀ��p��Ʈw
// ****** �K�[��Ʈw�s���M DbContext ���U Start ******
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//var mysqlPassword = Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD");

//// �ˬd�K�X�O�_���šA�p�G���ťi��|�����D
//if (string.IsNullOrEmpty(mysqlPassword))
//{
//    // �Ȯɤ��\�����šA����ڹB��i��ɭP�s������
//    Console.WriteLine("ĵ�i�G����������ܼ� MYSQL_ROOT_PASSWORD�C��Ʈw�s���i�ॢ�ѡC");
//}

//var fullConnectionString = $"{connectionString}Pwd={mysqlPassword};";
builder.Services.AddDbContext<UserManagementDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)) // ����DB��ƻP�˴� MySQL Server ����
);
// ****** �K�[��Ʈw�s���M DbContext ���U End ******

// ******* �K�[ CORS ���� Start *******
// Angular �e�� (http://localhost:4200) �P ASP.NET Core ��� API (https://localhost:7278)
// �L�̦�󤣦P���𸹡A�ҥH�|�Q�������P���u�ӷ��v(Origin)�C�ҥH�ݭn�̿�CORS�ӱ���P�ഫ
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200") // �e�ݪ�URL
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
