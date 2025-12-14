using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyToDo.Models;
using TinyToDo.Services;

namespace TinyToDo.Tests;
[TestClass]
public class TodoServiceTests
{
    // テスト用のUserAccountを作成するヘルパーメソッド
    private static UserAccount CreateTestAccount()
    {
        return new UserAccount("testuser", "password123", DateTime.UtcNow.AddMinutes(60));
    }

    #region GetAll Tests

    // テスト1: nullアカウントで空リストを返す
    [TestMethod]
    public void GetAll_WithNullAccount_ShouldReturnEmptyList()
    {
        // Act
        var result = TodoService.GetAll(null);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    // テスト2: ToDoがある場合、リストを返す
    [TestMethod]
    public void GetAll_WithTodos_ShouldReturnList()
    {
        // Arrange
        var account = CreateTestAccount();
        TodoService.Add(account, "Test Todo 1");
        TodoService.Add(account, "Test Todo 2");

        // Act
        var result = TodoService.GetAll(account);

        // Assert
        Assert.AreEqual(2, result.Count);
    }

    #endregion

    #region Add Tests

    // テスト3: ToDoを追加できる
    [TestMethod]
    public void Add_WithValidContent_ShouldAddTodo()
    {
        // Arrange
        var account = CreateTestAccount();

        // Act
        TodoService.Add(account, "New Todo");

        // Assert
        Assert.AreEqual(1, account.ToDoList.Count);
        Assert.AreEqual("New Todo", account.ToDoList[0].Content);
    }

    // テスト4: 空文字は追加されない
    [TestMethod]
    public void Add_WithEmptyContent_ShouldNotAddTodo()
    {
        // Arrange
        var account = CreateTestAccount();

        // Act
        TodoService.Add(account, "");

        // Assert
        Assert.AreEqual(0, account.ToDoList.Count);
    }

    // テスト5: スペースのみは追加されない（trimされる）
    [TestMethod]
    public void Add_WithWhitespaceOnly_ShouldNotAddTodo()
    {
        // Arrange
        var account = CreateTestAccount();

        // Act
        TodoService.Add(account, "   ");

        // Assert
        Assert.AreEqual(0, account.ToDoList.Count);
    }

    // テスト6: nullアカウントでは何も起こらない
    [TestMethod]
    public void Add_WithNullAccount_ShouldDoNothing()
    {
        // Act（例外が発生しないことを確認）
        TodoService.Add(null, "New Todo");

        // Assert - 例外が発生しなければ成功
        Assert.IsTrue(true);
    }

    #endregion

    #region Get Tests

    // テスト7: 存在するIDでToDoを取得できる
    [TestMethod]
    public void Get_WithValidId_ShouldReturnTodo()
    {
        // Arrange
        var account = CreateTestAccount();
        TodoService.Add(account, "Test Todo");
        var todoId = account.ToDoList[0].Id;

        // Act
        var result = TodoService.Get(account, todoId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Test Todo", result.Content);
    }

    // テスト8: 存在しないIDでnullを返す
    [TestMethod]
    public void Get_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var account = CreateTestAccount();
        TodoService.Add(account, "Test Todo");

        // Act
        var result = TodoService.Get(account, "nonexistent-id");

        // Assert
        Assert.IsNull(result);
    }

    #endregion

    #region Delete Tests

    // テスト9: ToDoを削除できる
    [TestMethod]
    public void Delete_WithValidId_ShouldRemoveTodo()
    {
        // Arrange
        var account = CreateTestAccount();
        TodoService.Add(account, "Test Todo");
        var todoId = account.ToDoList[0].Id;

        // Act
        TodoService.Delete(account, todoId);

        // Assert
        Assert.AreEqual(0, account.ToDoList.Count);
    }

    // テスト10: 存在しないIDでは何も起こらない
    [TestMethod]
    public void Delete_WithInvalidId_ShouldDoNothing()
    {
        // Arrange
        var account = CreateTestAccount();
        TodoService.Add(account, "Test Todo");

        // Act
        TodoService.Delete(account, "noexistent-id");

        // Assert
        Assert.AreEqual(1, account.ToDoList.Count);
    }

    #endregion

    #region Toggle Tests

    // テスト11: 未完了→完了に切り替えられる
    [TestMethod]
    public void Toggle_WithValidId_ShouldChangeFromIncompleteToComplete()
    {
        // Arrange
        var account = CreateTestAccount();
        TodoService.Add(account, "Test Todo");
        var todoId = account.ToDoList[0].Id;
        Assert.IsFalse(account.ToDoList[0].IsCompleted); // 初期状態は未完了

        // Act
        TodoService.Toggle(account, todoId);

        // Assert
        Assert.IsTrue(account.ToDoList[0].IsCompleted);
    }

    // テスト12: 完了→未完了に切り替えられる
    [TestMethod]
    public void Toggle_ShouldChangeFromCompleteToIncomplete()
    {
        // Arrange
        var account = CreateTestAccount();
        TodoService.Add(account, "Test Todo");
        var todoId = account.ToDoList[0].Id;
        TodoService.Toggle(account, todoId); // 完了にする
        Assert.IsTrue(account.ToDoList[0].IsCompleted);

        // Act
        TodoService.Toggle(account, todoId); // 再度トグル

        // Assert
        Assert.IsFalse(account.ToDoList[0].IsCompleted);
    }

    #endregion

    #region Update Tests

    // テスト13: 内容を更新できる
    [TestMethod]
    public void Update_WithValidContent_ShouldUpdateTodo()
    {
        // Arrange
        var account = CreateTestAccount();
        TodoService.Add(account, "Test Todo");
        var todoId = account.ToDoList[0].Id;

        // Act
        var result = TodoService.Update(account, todoId, "Updated Content");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Updated Content", result.Content);
    }

    // テスト14: 空文字では更新されない
    [TestMethod]
    public void Update_WithEmptyContent_ShouldNotUpdate()
    {
        // Arrange
        var account = CreateTestAccount();
        TodoService.Add(account, "Original Content");
        var todoId = account.ToDoList[0].Id;

        // Act
        var result = TodoService.Update(account, todoId, "");
        
        // Assert
        Assert.AreEqual("Original Content", account.ToDoList[0].Content);

    }

    // テスト15: 存在しないIDではnullを返す
    [TestMethod]
    public void Update_WithNullAccount_ShouldReturnNull()
    {
        // Arrange
        var account = CreateTestAccount();
        TodoService.Add(account, "Test Todo");

        // Act
        var result = TodoService.Update(account, "noexistent-id", "New Content");

        // Assert
        Assert.IsNull(result);  
    }

    #endregion
}