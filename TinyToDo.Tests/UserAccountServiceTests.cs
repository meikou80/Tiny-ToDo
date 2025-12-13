using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyToDo.Services;

namespace TinyToDo.Tests;

[TestClass]
public class UserAccountServiceTests
{
    // シングルトンインスタンスを取得
    private UserAccountService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _service = UserAccountService.Instance;
    }

    #region ValidateUserId Tests

    // テスト1: 有効な英数字のみのユーザーID
    [TestMethod]
    public void ValidateUserId_WithAlphanumeric_ShouldReturnTrue()
    {
        // Arrange
        var userId = "user123";

        // Act
        var result = _service.ValidateUserId(userId);

        // Assert
        Assert.IsTrue(result);
    }

    // テスト2: 有効な特殊文字を含むユーザーID
    [TestMethod]
    public void ValidateUserId_WithValidSpecialChars_ShouldReturnTrue()
    {
        // Arrange（許可される特殊文字: _ . + @ -）
        var userId = "user.name+tag@test-1";

        // Act
        var result = _service.ValidateUserId(userId);

        // Assert
        Assert.IsTrue(result);
    }

    // テスト3: 最小長（1文字）のユーザーID
    [TestMethod]
    public void ValidateUserId_WithMinLength_ShouldReturnTrue()
    {
        // Arrange
        var userId = "a";

        // Act
        var result = _service.ValidateUserId(userId);

        // Assert
        Assert.IsTrue(result);
    }

    // テスト4: 最大長（32文字）のユーザーID
    [TestMethod]
    public void ValidateUserId_WithMaxLength_ShouldReturnTrue()
    {
        // Arrange
        var userId = new string('a', 32);

        // Act
        var result = _service.ValidateUserId(userId);

        // Assert
        Assert.IsTrue(result);
    }

    // テスト5: 長すぎる（33文字）ユーザーID
    [TestMethod]
    public void ValidateUserId_WithTooLongId_ShouldReturnFalse()
    {
        // Arrange
        var userId = new string('a', 33);

        // Act
        var result = _service.ValidateUserId(userId);

        // Assert
        Assert.IsFalse(result);
    }
    // テスト6: 空文字のユーザーID
    [TestMethod]
    public void ValidateUserId_WithEmptyString_ShouldReturnFalse()
    {
        // Arrange
        var userId = "";

        // Act
        var result = _service.ValidateUserId(userId);

        // Assert
        Assert.IsFalse(result);
    }

    // テスト7: 無効な特殊文字を含むユーザーID
    [TestMethod]
    public void ValidateUserId_WithInvalidSpecialChars_ShouldReturnFalse()
    {
        // Arrange
        var userId = "user!name";

        // Act
        var result = _service.ValidateUserId(userId);

        // Assert
        Assert.IsFalse(result);
    }

    // テスト8: スペースを含むユーザーID
    [TestMethod]
    public void ValidateUserId_WithSpace_ShouldReturnFalse()
    {
        // Arrange
        var userId = "user name";

        // Act
        var result = _service.ValidateUserId(userId);

        // Assert
        Assert.IsFalse(result);
    }

    #endregion
}