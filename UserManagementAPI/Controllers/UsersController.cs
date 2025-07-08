using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
// 20250630 mod by jimmy for 使用者資料串聯資料庫
using Microsoft.EntityFrameworkCore; // 引入 EF Core 相關功能
using UserManagementAPI.Data; // 引入 DbContext 命名空間
using UserManagementAPI.Models; // 引入我們剛剛建立的 User 模型

// 20250703 mod by jimmy for JWT驗證
using Microsoft.AspNetCore.Authorization;

namespace UserManagementAPI.Controllers
{
    [Route("api/[controller]")] // 定義路由前綴
    [ApiController] // 標記為 API 控制器

    // 20250703 mod by jimmy for JWT驗證
    [Authorize] // 整個控制器都需要驗證
    public class UsersController : ControllerBase
    {
        // 20250630 mod by jimmy for 使用者資料串聯資料庫
        //private static List<User> _users = new List<User>
        //{
        //    new User { Id = 1, Name = "王小明", Email = "xiaoming@example.com" },
        //    new User { Id = 2, Name = "陳美麗", Email = "meili@example.com" },
        //    new User { Id = 3, Name = "張大衛", Email = "david@example.com" }
        //};
        private readonly UserManagementDbContext _context; // 注入 DbContext，資料庫的項目

        // 構造函數，透過依賴注入接收 DbContext 實例
        public UsersController(UserManagementDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers() // 允許返回各種 IActionResult (如 Ok(), NotFound())。
        {
            // 從資料庫非同步獲取所有使用者
            return Ok(await _context.Users.ToListAsync()); // 生成 SQL 語句來查詢 Users 表格中的所有數據 ( SELECT )
        }

        // GET: api/Users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            // 從資料庫非同步查找指定 ID 的使用者
            var user = await _context.Users.FindAsync(id); // FindAsync 適用於主鍵查詢

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> AddUser([FromBody] User newUser)
        {
            // 自動增長 ID 由資料庫處理，不需要手動設定
            _context.Users.Add(newUser); // 將新使用者添加到 DbSet ( _context ) 中
            await _context.SaveChangesAsync(); // 將更改保存到資料庫，在此可能是INSERT操作

            // 返回 201 Created 狀態碼，並包含新使用者的資料和其資源的 URI
            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
        }

        // PUT: api/Users/{id}
        [HttpPut("{id}")] // 根據前端選到的id將特定使用者做編輯
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (id != updatedUser.Id)
            {
                return BadRequest("User ID mismatch.");
            }

            // 通知 EF Core 這個實體是處於修改狀態
            _context.Entry(updatedUser).State = EntityState.Modified; // 讓 SaveChangesAsync() 知道要生成 UPDATE 語句。

            try
            {
                await _context.SaveChangesAsync(); // 保存更改到資料庫
            }
            catch (DbUpdateConcurrencyException)
            {
                // 如果在保存時發生並發衝突（例如，使用者在被修改前已被刪除）
                if (!await UserExists(id)) // 檢查使用者是否存在
                {
                    return NotFound();
                }
                else
                {
                    throw; // 其他並發衝突，重新拋出異常
                }
            }

            return NoContent(); // HTTP 204 No Content
        }

        // DELETE: api/Users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id); // 從資料庫查找使用者

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user); // 從 DbSet 中移除
            await _context.SaveChangesAsync(); // 將更改保存到資料庫

            return NoContent(); // HTTP 204 No Content
        }

        // 輔助方法：檢查使用者是否存在（用於 UpdateUser 錯誤處理）
        private async Task<bool> UserExists(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }



        //// GET: api/Users
        //[HttpGet] // 告訴 ASP.NET Core 這個方法將處理 HTTP GET 請求。
        //public ActionResult<IEnumerable<User>> GetUsers()
        //{
        //    return Ok(_users); // 返回 200 OK 和使用者列表，並序列化為 JSON 格式作為回應主體。
        //}

        //// 20250627 mod by jimmy for API新增使用者功能
        //[HttpPost]
        //public ActionResult<User> AddUser([FromBody] User newUser)
        //{
        //    newUser.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1; // ID是在後端自動生成的，以前端找到的最大的那個ID+1產生

        //    _users.Add(newUser); // 將前端創建的新使用者添加到列表中

        //    return CreatedAtAction(nameof(GetUsers), new { id = newUser.Id }, newUser); // 作為回應主體返回的新創建使用者物件，並生成 Location URL。
        //}

        //// 20250628 mod by jimmy for  API編輯使用者功能
        //[HttpPut("{id}")] // 定義了一個路由參數。這表示當請求的 URL 匹配 使用者ID的話，方法就會被調用
        //public IActionResult UpdateUser(int id, [FromBody] User updateUser)
        //{
        //    // 初步防呆；前端選擇的使用者ID與路徑中的使用者ID不匹配就報錯
        //    if (id != updateUser.Id)
        //    {
        //        return BadRequest("User id mismatch.");
        //    }

        //    var index = _users.FindIndex(u => u.Id == id);
        //    if (index == -1)
        //    {
        //        // 如果找不到使用者，返回 404 Not Found
        //        return NotFound();
        //    }

        //    _users[index] = updateUser; // 從後端更新使用者
        //    return NoContent(); // 這是 RESTful API 中 PUT 成功的回傳方式

        //}

        //// GET: api/Users/{id}
        //[HttpGet("{id}")] // {id} 表示這是路由參數
        //public ActionResult<User> GetUser(int id)
        //{
        //    var user = _users.FirstOrDefault(u => u.Id == id); // 找到列表中第一個匹配 ID 的使用者

        //    if (user == null)
        //    {
        //        return NotFound(); // 如果找不到使用者，返回 404 Not Found
        //    }

        //    return Ok(user); // 返回 200 OK 和找到的使用者資料
        //}

        //// 20250629 mod by jimmy for API刪除使用者功能
        //// DELETE: api/Users/{id}
        //[HttpDelete("{id}")] // {id} 表示這是路由參數

        //public IActionResult DeleteUser(int id)
        //{
        //    var user = _users.FirstOrDefault(u => u.Id == id);

        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    _users.Remove(user);

        //    return NoContent(); // RESTful API 中 DELETE 操作成功後的推薦回傳方式
        //}
    }
}
