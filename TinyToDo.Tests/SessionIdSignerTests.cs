using TinyToDo.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting; 

namespace TinyToDo.Tests;

[TestClass]
public class SessionIdSignerTests
{
    // テスト用の固定シークレット
    private const ulong TestSecret = 0x1234567890ABCDEF;
    
    // テスト対象のインスタンス（各テストで共有）
    private SessionIdSigner _signer = null!;

    // 各テストの前に実行される
    [TestInitialize]
    public void Setup()
    {
        _signer = new SessionIdSigner(TestSecret);
    }

    // テスト1: 生成したセッションIDが検証に成功する
    [TestMethod]
    public void GenerateSignedSessionId_ShouldCreateVerifiableSessionId()
    {
        // Arrange（準備）- Setupで完了済み

        // Act（実行）
        var sessionId = _signer.GenerateSignedSessionId();

        // Assert（検証）
        Assert.IsTrue(_signer.VerifySessionId(sessionId));
    }

    // テスト2: 改ざんされたセッションIDが検証に失敗する
    [TestMethod]
    public void VerifySessionId_WithTamperedId_ShouldReturnFalse()
    {
        // Arrange
        var sessionId = _signer.GenerateSignedSessionId();
        
        // セッションIDの一部を改ざん
        var tamperedId = "X" + sessionId.Substring(1);

        // Act
        var result = _signer.VerifySessionId(tamperedId);

        // Assert
        Assert.IsFalse(result);
    }

    // テスト3: 異なるシークレットで生成されたIDが検証に失敗する
    [TestMethod]
    public void VerifySessionId_WithDifferentSecret_ShouldReturnFalse()
    {
        // Arrange
        var differentSecret = 0xFEDCBA0987654321UL;
        var otherSigner = new SessionIdSigner(differentSecret);
        var sessionId = otherSigner.GenerateSignedSessionId();

        // Act（元のsignerで検証）
        var result = _signer.VerifySessionId(sessionId);

        // Assert
        Assert.IsFalse(result);
    }

    // テスト4: 無効な形式のセッションIDが検証に失敗する
    [TestMethod]
    public void VerifySessionId_WithInvalidFormat_ShouldReturnFalse()
    {
        // Arrange
        var invalidSessionId = "invalid-session-id";

        // Act
        var result = _signer.VerifySessionId(invalidSessionId);

        // Assert
        Assert.IsFalse(result);
    }

    // テスト5: 空文字のセッションIDが検証に失敗する
    [TestMethod]
    public void VerifySessionId_WithEmptyString_ShouldReturnFalse()
    {
        // Act
        var result = _signer.VerifySessionId(string.Empty);

        // Assert
        Assert.IsFalse(result);
    }
}