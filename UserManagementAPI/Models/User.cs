namespace UserManagementAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        // 初始化為空字串，避免 null 警告
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;


    }
}
