// 20250708 mod by jimmy for CI/CD測試
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // 引入 EntityFrameworkCore 以模擬 DbSet
using Moq; // 引入 Moq 命名空間
using System.Collections.Generic;

// 20250708 mod by jimmy for CI/CD測試 => moq模擬
using System.Linq;
using System.Threading.Tasks; // For async methods
using UserManagementAPI.Controllers; // 控制器
using UserManagementAPI.Models; // 模型
using UserManagementAPI.Data; // 引入 DbContext 命名空間
using Xunit;

namespace UserManagementAPI.Tests;

public class UsersControllerTests
{
    // 20250708 mod by jimmy for CI/CD測試 => moq模擬
    //[Fact]
    //public void GetUsers_ReturnsListOfUsers()
    //{
    //    // Arrange (準備測試數據或依賴)
    //    var controller = new UsersController(null); // 臨時傳入 null，如果控制器有 DbContext 依賴，這會報錯

    //    // Act (執行測試動作)
    //    var result = controller.GetUsers().Result; // 如果是 Task<ActionResult<...>> 需要 .Result

    //    // Assert (斷言結果)
    //    Assert.IsType<ActionResult<IEnumerable<User>>>(result); // 確保返回類型正確
    //    // 可以添加更多斷言，例如檢查返回的列表是否為空，或包含預期的項目
    //    // var okResult = Assert.IsType<OkObjectResult>(result.Result);
    //    // var users = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
    //    // Assert.Empty(users); // 假設初始化時為空
    //}

    // 1. 測試 GetUsers 方法是否返回一個包含用戶的列表 (當有用戶存在時)
    [Fact]
    public async Task GetUsers_ReturnsOkResultWithListOfUsers()
    {
        // Arrange (準備)
        // 創建一些測試用戶數據
        var testUsers = new List<User>
        {
            new User { Id = 1, Name = "User One", Email = "one@example.com" },
            new User { Id = 2, Name = "User Two", Email = "two@example.com" }
        }.AsQueryable(); // 轉換為 IQueryable 才能模擬 DbSet

        // 模擬 DbSet<User>
        var mockDbSet = new Mock<DbSet<User>>();
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(testUsers.Provider);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(testUsers.Expression);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(testUsers.ElementType);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(testUsers.GetEnumerator());

        // 模擬 UserManagementDbContext
        var mockDbContext = new Mock<UserManagementDbContext>();
        mockDbContext.Setup(c => c.Users).Returns(mockDbSet.Object); // 當訪問 DbContext.Users 時，返回我們模擬的 DbSet

        var controller = new UsersController(mockDbContext.Object); // 將模擬的 DbContext 傳入控制器

        // Act (執行)
        var result = await controller.GetUsers(); // 使用 await 因為 GetUsers 可能是 async Task<...>

        // Assert (斷言)
        // 確保返回的是 OkObjectResult
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        // 確保返回的值是一個 IEnumerable<User>
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
        // 斷言返回的用戶數量是否正確
        Assert.Equal(2, returnedUsers.Count());
        // 斷言包含特定的用戶
        Assert.Contains(returnedUsers, u => u.Name == "User One");
        Assert.Contains(returnedUsers, u => u.Name == "User Two");
    }

    // 2. 測試 GetUsers 方法是否返回空列表 (當沒有用戶時)
    [Fact]
    public async Task GetUsers_ReturnsOkResultWithEmptyListWhenNoUsers()
    {
        // Arrange
        var emptyUsers = new List<User>().AsQueryable();

        var mockDbSet = new Mock<DbSet<User>>();
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(emptyUsers.Provider);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(emptyUsers.Expression);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(emptyUsers.ElementType);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(emptyUsers.GetEnumerator());

        var mockDbContext = new Mock<UserManagementDbContext>();
        mockDbContext.Setup(c => c.Users).Returns(mockDbSet.Object);

        var controller = new UsersController(mockDbContext.Object);

        // Act
        var result = await controller.GetUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
        Assert.Empty(returnedUsers); // 斷言返回的列表是空的
    }

    // 3. 測試 GetUser(id) 方法當用戶存在時
    [Fact]
    public async Task GetUser_ExistingId_ReturnsOkResultWithUser()
    {
        // Arrange
        var testUser = new User { Id = 1, Name = "Test User", Email = "test@example.com" };
        var users = new List<User> { testUser }.AsQueryable();

        var mockDbSet = new Mock<DbSet<User>>();
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());
        // 模擬 FindAsync 方法，當調用 FindAsync(1) 時返回 testUser
        // FindAsync 接受 object[] 作為參數，所以需要這樣模擬
        mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                 .ReturnsAsync((object[] ids) => users.FirstOrDefault(u => u.Id == (int)ids[0]));


        var mockDbContext = new Mock<UserManagementDbContext>();
        mockDbContext.Setup(c => c.Users).Returns(mockDbSet.Object);

        var controller = new UsersController(mockDbContext.Object);

        // Act
        var result = await controller.GetUser(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal(testUser.Id, returnedUser.Id);
        Assert.Equal(testUser.Name, returnedUser.Name);
    }

    // 4. 測試 GetUser(id) 方法當用戶不存在時
    [Fact]
    public async Task GetUser_NonExistingId_ReturnsNotFoundResult()
    {
        // Arrange
        var emptyUsers = new List<User>().AsQueryable();

        var mockDbSet = new Mock<DbSet<User>>();
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(emptyUsers.Provider);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(emptyUsers.Expression);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(emptyUsers.ElementType);
        mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(emptyUsers.GetEnumerator());
        // 模擬 FindAsync 方法，當調用 FindAsync(任何 int) 時返回 null
        mockDbSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                 .ReturnsAsync((User)null); // 返回 null 表示找不到

        var mockDbContext = new Mock<UserManagementDbContext>();
        mockDbContext.Setup(c => c.Users).Returns(mockDbSet.Object);

        var controller = new UsersController(mockDbContext.Object);

        // Act
        var result = await controller.GetUser(999); // 測試一個不存在的 ID

        // Assert
        Assert.IsType<NotFoundResult>(result.Result); // 斷言返回的是 NotFound 結果
    }
}
