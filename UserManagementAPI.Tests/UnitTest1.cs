// 20250708 mod by jimmy for CI/CD測試
using Xunit;
using UserManagementAPI.Controllers; // 控制器
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using UserManagementAPI.Models; // 模型


namespace UserManagementAPI.Tests;

public class UsersControllerTests
{
    [Fact]
    public void GetUsers_ReturnsListOfUsers()
    {
        // Arrange (準備測試數據或依賴)
        var controller = new UsersController(null); // 臨時傳入 null，如果控制器有 DbContext 依賴，這會報錯

        // Act (執行測試動作)
        var result = controller.GetUsers().Result; // 如果是 Task<ActionResult<...>> 需要 .Result

        // Assert (斷言結果)
        Assert.IsType<ActionResult<IEnumerable<User>>>(result); // 確保返回類型正確
        // 可以添加更多斷言，例如檢查返回的列表是否為空，或包含預期的項目
        // var okResult = Assert.IsType<OkObjectResult>(result.Result);
        // var users = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
        // Assert.Empty(users); // 假設初始化時為空
    }
}
