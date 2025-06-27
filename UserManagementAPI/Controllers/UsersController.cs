using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models; // 引入我們剛剛建立的 User 模型

namespace UserManagementAPI.Controllers
{
    [Route("api/[controller]")] // 定義路由前綴
    [ApiController] // 標記為 API 控制器
    public class UsersController : ControllerBase
    {
        private static List<User> _users = new List<User>
        {
            new User { Id = 1, Name = "王小明", Email = "xiaoming@example.com" },
            new User { Id = 2, Name = "陳美麗", Email = "meili@example.com" },
            new User { Id = 3, Name = "張大衛", Email = "david@example.com" }
        };

        // GET: api/Users
        [HttpGet] // 告訴 ASP.NET Core 這個方法將處理 HTTP GET 請求。
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return Ok(_users); // 返回 200 OK 和使用者列表，並序列化為 JSON 格式作為回應主體。
        }

    }
}
