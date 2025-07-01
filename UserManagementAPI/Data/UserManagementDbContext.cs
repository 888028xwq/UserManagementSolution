// 20250630 mod by jimmy for 使用者資料串聯資料庫
using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Models;

namespace UserManagementAPI.Data
{
    public class UserManagementDbContext : DbContext
    {
        // 構造函數，用於接收 DbContextOptions，這在 Program.cs 中配置
        public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options)
            : base(options)
        {
        }

        // 定義 DbSet，代表資料庫中的 Users 表格
        // 這裡的屬性名稱 (Users) 會對應到資料庫的表格名稱
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
