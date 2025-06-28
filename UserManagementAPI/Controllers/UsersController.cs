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

        // 20250627 mod by jimmy for API新增使用者功能
        [HttpPost]
        public ActionResult<User> AddUser([FromBody] User newUser)
        {
            newUser.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1; // ID是在後端自動生成的，以前端找到的最大的那個ID+1產生

            _users.Add(newUser); // 將前端創建的新使用者添加到列表中

            return CreatedAtAction(nameof(GetUsers), new { id = newUser.Id }, newUser); // 作為回應主體返回的新創建使用者物件，並生成 Location URL。
        }

        // 20250628 mod by jimmy for  API編輯使用者功能
        [HttpPut("{id}")] // 定義了一個路由參數。這表示當請求的 URL 匹配 使用者ID的話，方法就會被調用
        public IActionResult UpdateUser(int id, [FromBody] User updateUser)
        {
            // 初步防呆；前端選擇的使用者ID與路徑中的使用者ID不匹配就報錯
            if (id != updateUser.Id)
            {
                return BadRequest("User id mismatch.");
            }

            var index = _users.FindIndex(u => u.Id == id);
            if (index == -1)
            {
                // 如果找不到使用者，返回 404 Not Found
                return NotFound();
            }

            _users[index] = updateUser; // 從後端更新使用者
            return NoContent(); // 這是 RESTful API 中 PUT 成功的回傳方式

        }

        // GET: api/Users/{id}
        [HttpGet("{id}")] // {id} 表示這是路由參數
        public ActionResult<User> GetUser(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id); // 找到列表中第一個匹配 ID 的使用者

            if (user == null)
            {
                return NotFound(); // 如果找不到使用者，返回 404 Not Found
            }

            return Ok(user); // 返回 200 OK 和找到的使用者資料
        }


    }
}
