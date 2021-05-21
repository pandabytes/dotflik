using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Dotflik.Protobuf.Movie;

namespace Dotflik.Web.Server.Services
{
  public class MovieService : Protobuf.Movie.MovieService.MovieServiceBase
  {
    public override async Task GetMovies(Empty request, IServerStreamWriter<Movie> responseStream, ServerCallContext context)
    {
      for (int i = 0; i < 5; i++)
      {
        await Task.Delay(1000);
        var response = new Movie { Id = (i+1).ToString(), Year = 2021 + i + 1};
        await responseStream.WriteAsync(response);
      }
    }


  }
}
