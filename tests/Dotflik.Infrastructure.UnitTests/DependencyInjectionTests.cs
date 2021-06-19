using System;
using Microsoft.Extensions.DependencyInjection;

using Dotflik.Application.Repositories;
using Dotflik.Application.Repositories.Settings;
using Dotflik.Application.Validation;

using Xunit;
using Moq;

namespace Dotflik.Infrastructure.UnitTests
{
  public class DependencyInjectionTests
  {
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

  }
}
