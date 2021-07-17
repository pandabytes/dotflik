using System;
using Microsoft.Extensions.DependencyInjection;

using Dotflik.Application.Paginations;
using Dotflik.Application.Paginations.Args;
using Dotflik.Application.Repositories;
using Dotflik.Application.Repositories.Settings;
using Dotflik.Application.Validation;

using Xunit;
using Moq;

namespace Dotflik.Infrastructure.UnitTests
{
  public class DependencyInjectionTests
  {
    public class DummyPaginationTokenArgs : PaginationTokenArgs
    {}

    [Fact]
    public void AddMovieRepository_SupportDatabase_MovieRepositoryIsAdded()
    {
      // Arrange
      var dbSettingsMock = new Mock<DatabaseSettings>();
      dbSettingsMock.SetupGet(m => m.ConnectionString).Returns("dummy");
      var services = new ServiceCollection();

      // Act
      services.AddMovieRepository(Database.PostgresSQL);
      services.AddSingleton(dbSettingsMock.Object);
      var provider = services.BuildServiceProvider();

      // Assert
      // No need to call any Xunit.Assert because GetRequiredService
      // would throw an exception if the service is not registered
      _ = provider.GetRequiredService<IMovieRepository>();
    }

    [Fact]
    public void AddMovieRepository_NotSupportDatabase_ThrowsNotSupportedException()
    {
      // Use an arbitrarily number that is not in the enum to "fake" a not supported db
      var notSupportedDb = (Database)int.MaxValue;
      var services = new ServiceCollection();

      Assert.Throws<NotSupportedException>(() => services.AddMovieRepository(notSupportedDb));     
    }

    [Fact]
    public void AddGenreRepository_SupportDatabase_GenreRepositoryIsAdded()
    {
      // Arrange
      var dbSettingsMock = new Mock<DatabaseSettings>();
      dbSettingsMock.SetupGet(m => m.ConnectionString).Returns("dummy");
      var services = new ServiceCollection();

      // Act
      services.AddGenreRepository(Database.PostgresSQL);
      services.AddSingleton(dbSettingsMock.Object);
      var provider = services.BuildServiceProvider();

      // Assert
      // No need to call any Xunit.Assert because GetRequiredService
      // would throw an exception if the service is not registered
      _ = provider.GetRequiredService<IGenreRepository>();
    }

    [Fact]
    public void AddGenreRepository_NotSupportDatabase_ThrowsNotSupportedException()
    {
      // Use an arbitrarily number that is not in the enum to "fake" a not supported db
      var notSupportedDb = (Database)int.MaxValue;
      var services = new ServiceCollection();

      Assert.Throws<NotSupportedException>(() => services.AddGenreRepository(notSupportedDb));
    }

    [Fact]
    public void AddDataAnnotationValidator_SimpleCall_DataAnnationationValidatorIsAdded()
    {
      // Arrange
      var services = new ServiceCollection();

      // Act
      services.AddDataAnnotationValidator();
      var provider = services.BuildServiceProvider();

      // Assert
      // No need to call any Xunit.Assert because GetRequiredService
      // would throw an exception if the service is not registered
      _ = provider.GetRequiredService<IDataAnnotationValidator>();
    }

    [Fact]
    public void AddPaginationTokenFactory_SimpleCall_TokenFactoryIsAdded()
    {
      // Arrange
      var services = new ServiceCollection();

      // Act
      services.AddPaginationTokenFactory();
      var provider = services.BuildServiceProvider();

      // Assert
      // No need to call any Xunit.Assert because GetRequiredService
      // would throw an exception if the service is not registered
      _ = provider.GetRequiredService<PaginationTokenFactory>();
    }

    [Fact]
    public void TokenFactory_NullTokenValueAndTokenTypeNotSupported_ThrowsNotSupportedException()
    {
      // Arrange
      var services = new ServiceCollection();
      services.AddPaginationTokenFactory();
      var provider = services.BuildServiceProvider();
      var tokenFactory = provider.GetRequiredService<PaginationTokenFactory>();

      // Act & Assert
      var invalidTokenType = (PaginationTokenType) (-1);
      var tokenArgs = new DummyPaginationTokenArgs { Value = null };
      Assert.Throws<NotSupportedException>(() => tokenFactory(invalidTokenType, tokenArgs));
    }

    [Fact]
    public void TokenFactory_NonNullTokenValueAndTokenTypeNotSupported_ThrowsNotSupportedException()
    {
      // Arrange
      var services = new ServiceCollection();
      services.AddPaginationTokenFactory();
      var provider = services.BuildServiceProvider();
      var tokenFactory = provider.GetRequiredService<PaginationTokenFactory>();

      // Act & Assert
      var invalidTokenType = (PaginationTokenType)(-1);
      var tokenArgs = new DummyPaginationTokenArgs { Value = "dummy_token" };
      Assert.Throws<NotSupportedException>(() => tokenFactory(invalidTokenType, tokenArgs));
    }

    [Fact]
    public void TokenFactory_TokenTypeAndTokenArgsDoNotMatch_ThrowsArgumentException()
    {
      // Arrange
      var services = new ServiceCollection();
      services.AddPaginationTokenFactory();
      var provider = services.BuildServiceProvider();
      var tokenFactory = provider.GetRequiredService<PaginationTokenFactory>();

      // Act & Assert
      var tokenArgs = new DummyPaginationTokenArgs { Value = null };
      foreach (PaginationTokenType tokenType in Enum.GetValues(typeof(PaginationTokenType)))
      {
        Assert.Throws<ArgumentException>(() => tokenFactory(tokenType, tokenArgs));
      }
    }

  }
}
