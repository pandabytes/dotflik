using System;
using System.Threading.Tasks;

using Google.Protobuf.WellKnownTypes;

using Dotflik.Application.Repositories;
using Dotflik.Protobuf;
using Grpc.Core;

namespace Dotflik.WebApp.Server.Services
{
  /// <summary>
  /// gRPC implementation of genre service
  /// </summary>
  public class GenreService : Protobuf.GenreService.GenreServiceBase
  {
    private IGenreRepository m_genreRepository;

    public GenreService(IGenreRepository genreRepository)
      => m_genreRepository = genreRepository;

    /// <inheritdoc/>
    public async override Task<GetGenreNamesOnlyResponse> GetGenreNamesOnly(Empty request, ServerCallContext context)
    {
      var genreNames = await m_genreRepository.GetGenreNamesAsync();

      var response = new GetGenreNamesOnlyResponse();
      response.GenreNames.AddRange(genreNames);

      return response;
    }

  }
}
