using System;
using System.Reflection;
using Dotflik.Application.Repositories.Settings;
using Xunit;
using Moq;

namespace Dotflik.Application.Repositories.UnitTests
{
  public class RepositoryTests
  {
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(" ")]
    public void Constructor_EmptyConnectionString_ThrowsArgumentException(string connectionString)
    {
      var dbSettingsMock = new Mock<IDatabaseSettings>();
      dbSettingsMock.SetupGet(m => m.ConnectionString).Returns(connectionString);

      // Outer exception is thrown by Xunit to assert the inner exception instead
      var ex = Assert.Throws<TargetInvocationException>(
        () => new Mock<Repository>(dbSettingsMock.Object).Object);

      Assert.IsType<ArgumentException>(ex.InnerException);
    }

    [Theory]
    [InlineData("  x  ")]
    [InlineData("x")]
    [InlineData("x ")]
    [InlineData("x 0")]
    public void Constructor_NonEmptyConnectionString_RepositoryCanBeConstructed(string connectionString)
    {
      var dbSettingsMock = new Mock<IDatabaseSettings>();
      dbSettingsMock.SetupGet(m => m.ConnectionString).Returns(connectionString);

      _ = new Mock<Repository>(dbSettingsMock.Object).Object;
    }

  }
}
