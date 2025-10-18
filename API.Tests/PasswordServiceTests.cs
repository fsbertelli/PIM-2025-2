using System;
using Xunit;
using API.Services;

namespace API.Tests;

public class PasswordServiceTests
{
    [Fact]
    public void HashPassword_Generates64CharHexHash_And_VerifyPassword_ReturnsTrueForCorrectPassword()
    {
        var password = "TestPassword123!";
        var hash = PasswordService.HashPassword(password);

        Assert.False(string.IsNullOrEmpty(hash));
        Assert.Equal(64, hash.Length); // SHA256 hex has 64 chars
        Assert.True(PasswordService.VerifyPassword(password, hash));
        Assert.False(PasswordService.VerifyPassword("wrongpass", hash));
    }

    [Fact]
    public void HashPassword_NullOrEmpty_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => PasswordService.HashPassword(null!));
        Assert.Throws<ArgumentNullException>(() => PasswordService.HashPassword(string.Empty));
    }

    [Fact]
    public void VerifyPassword_ReturnsFalse_WhenPasswordOrHashIsNullOrEmpty()
    {
        var password = "AnotherPass123";
        var hash = PasswordService.HashPassword(password);

        Assert.False(PasswordService.VerifyPassword(null!, hash));
        Assert.False(PasswordService.VerifyPassword(password, null!));
        Assert.False(PasswordService.VerifyPassword(string.Empty, hash));
        Assert.False(PasswordService.VerifyPassword(password, string.Empty));
    }
}

