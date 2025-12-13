using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyToDo.Models;

namespace TinyToDo.Tests;

[TestClass]
public class UserAccountTests
{
    // テスト用の固定値
    private const string TestUserId = "testuser";
    private const string TestPassword = "password123";

    #region Constructor Tests

    // テスト1: コンストラクタでIdが正しく設定される
    [TestMethod]
    public void Constructor_ShouldSetId()
    {
        // Arrange
        var expires = DateTime.UtcNow.AddMinutes(60);

        // Act
        var account = new UserAccount(TestUserId, TestPassword, expires);

        // Assert
        Assert.AreEqual(TestUserId, account.Id);
    }

    // テスト2: コンストラクタでExpiresが正しく設定される
    [TestMethod]
    public void Constructor_ShouldSetExpires()
    {
        // Arrange
        var expires = DateTime.UtcNow.AddMinutes(60);

        // Act
        var account = new UserAccount(TestUserId, TestPassword, expires);

        // Assert
        Assert.AreEqual(expires, account.Expires);
    }

    // テスト3: コンストラクタでToDoListが空のリストとして初期化される
    [TestMethod]
    public void Constructor_ShouldInitializeEmptyToDoList()
    {
        // Arrange
        var expires = DateTime.UtcNow.AddMinutes(60);

        // Act
        var account = new UserAccount(TestUserId, TestPassword, expires);

        // Assert
        Assert.IsNotNull(account.ToDoList);
        Assert.AreEqual(0, account.ToDoList.Count);
    }

    // テスト4: コンストラクタでパスワードがハッシュ化される
    [TestMethod]
    public void Constructor_ShouldHashPassword()
    {
        // Arrange
        var expires = DateTime.UtcNow.AddMinutes(60);

        // Act
        var account = new UserAccount(TestUserId, TestPassword, expires);

        // Assert（ハッシュ化されているので元のパスワードと異なる）
        Assert.AreNotEqual(TestPassword, account.HashedPassword);
        Assert.IsTrue(account.HashedPassword.StartsWith("$2")); // BCryptのプレフィックス
    }

    #endregion

    #region VerifyPassword Tests

    // テスト5: 正しいパスワードで検証成功
    [TestMethod]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var expires = DateTime.UtcNow.AddMinutes(60);
        var account = new UserAccount(TestUserId, TestPassword, expires);

        // Act
        var result = account.VerifyPassword(TestPassword);

        // Assert
        Assert.IsTrue(result);
    }

    // テスト6: 間違ったパスワードで検証失敗
    [TestMethod]
    public void VerifyPassword_WithWrongPassword_ShouldReturnFalse()
    {
        // Arrange
        var expires = DateTime.UtcNow.AddMinutes(60);
        var account = new UserAccount(TestUserId, TestPassword, expires);

        // Act
        var result = account.VerifyPassword("wrongpassword");

        // Assert
        Assert.IsFalse(result);
    }

    // テスト7: 空文字パスワードで検証失敗
    [TestMethod]
    public void VerifyPassword_WithEmptyString_ShouldReturnFalse()
    {
        // Arrange
        var expires = DateTime.UtcNow.AddMinutes(60);
        var account = new UserAccount(TestUserId, TestPassword, expires);

        // Act
        var result = account.VerifyPassword("");

        // Assert
        Assert.IsFalse(result);
    }

    // テスト8: 大文字小文字が異なるパスワードで検証失敗
    [TestMethod]
    public void VerifyPassword_WithDifferentCase_ShouldReturnFalse()
    {
        // Arrange
        var expires = DateTime.UtcNow.AddMinutes(60);
        var account = new UserAccount(TestUserId, TestPassword, expires);

        // Act
        var result = account.VerifyPassword("PASSWORD123");

        // Assert
        Assert.IsFalse(result);
    }

    #endregion
}