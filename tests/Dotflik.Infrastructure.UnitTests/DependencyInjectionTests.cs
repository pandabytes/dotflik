using System;
using Microsoft.Extensions.DependencyInjection;

using Dotflik.Application.Pagination;
using Dotflik.Application.Repositories;
using Dotflik.Application.Repositories.Settings;

using Xunit;
using Moq;

namespace Dotflik.Infrastructure.UnitTests
{
  public class DependencyInjectionTests
  {
    [Fact]
    public void AddMovieRepository_SupportDatabase_MovieRepositoryIsAdded()
    {
      // Mock a dummy database settings
      var dbSettingsMock = new Mock<IDatabaseSettings>();
      dbSettingsMock.SetupGet(m => m.ConnectionString).Returns("dummy");

      var services = new ServiceCollection();
      services.AddMovieRepository(Database.PostgresSQL);
      services.AddSingleton(dbSettingsMock.Object);
      var provider = services.BuildServiceProvider();

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

  }
}
