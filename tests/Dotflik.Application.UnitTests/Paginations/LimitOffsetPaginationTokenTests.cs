using System;
using Dotflik.Application.Paginations.Args;
using Dotflik.Infrastructure;
using Dotflik.Domain.Exceptions;

using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Dotflik.Application.Paginations.UnitTests
{
  public class LimitOffsetPaginationTokenTests
  {
    public class PageTokenTestData 
    {
      public string PageToken { get; set; } = string.Empty;

      public int ExpectLimit { get; set; }

      public int ExpectOffset { get; set; }
    }

    public static TheoryData<PageTokenTestData> ValidPageTokenData = 
      new TheoryData<PageTokenTestData>()
      {
        new PageTokenTestData { PageToken = string.Empty, ExpectLimit = 0, ExpectOffset = 0},
        new PageTokenTestData { PageToken = "   ", ExpectLimit = 0, ExpectOffset = 0},
        new PageTokenTestData { PageToken = "limit=1&offset=0", ExpectLimit = 1, ExpectOffset = 0},
        new PageTokenTestData { PageToken = "limit=0&offset=1", ExpectLimit = 0, ExpectOffset = 1},
        new PageTokenTestData { PageToken = "limit=111&offset=123", ExpectLimit = 111, ExpectOffset = 123}
      };

    public static TheoryData<PageTokenTestData> ValidNonEmptyPageTokenData =
      new TheoryData<PageTokenTestData>()
      {
        new PageTokenTestData { PageToken = "limit=1&offset=0", ExpectLimit = 1, ExpectOffset = 0},
        new PageTokenTestData { PageToken = "limit=0&offset=1", ExpectLimit = 0, ExpectOffset = 1},
        new PageTokenTestData { PageToken = "limit=111&offset=123", ExpectLimit = 111, ExpectOffset = 123}
      };

    private readonly PaginationTokenFactory m_tokenFactory;

    public LimitOffsetPaginationTokenTests()
    {
      var services = new ServiceCollection();
      services.AddPaginationTokenFactory();

      var provider = services.BuildServiceProvider();
      m_tokenFactory = provider.GetRequiredService<PaginationTokenFactory>();
    }

    [Theory]
    [InlineData("x")]
    [InlineData("limit")]
    [InlineData("offset")]
    [InlineData("limit=10&offset=x")]
    [InlineData("limit=x&offset=10")]
    [InlineData("limit=x&offset=x")]
    [InlineData("limit=x&offset=1x")]
    [InlineData("limit=1x&offset=x")]
    [InlineData("limit=1x&offset=1x")]
    public void Constructor_InvalidPageTokenFormat_ThrowsPageTokenFormatException(string pageToken)
      => Assert.Throws<PageTokenFormatException>(() => {
        m_tokenFactory(PaginationTokenType.LimitOffset, pageToken);
      });

    [Theory]
    [MemberData(nameof(ValidPageTokenData))]
    public void Constructor_ValidPageToken_CorrectLimitParsed(PageTokenTestData testData)
    {
      // Act
      var limitOffsetPageToken = (LimitOffsetPaginationToken) m_tokenFactory(PaginationTokenType.LimitOffset, testData.PageToken);

      // Assert
      Assert.Equal(testData.ExpectLimit, limitOffsetPageToken.Limit);
    }

    [Theory]
    [MemberData(nameof(ValidPageTokenData))]
    public void Constructor_ValidPageToken_CorrectOffsetParsed(PageTokenTestData testData)
    {
      // Act
      var limitOffsetPageToken = (LimitOffsetPaginationToken)m_tokenFactory(PaginationTokenType.LimitOffset, testData.PageToken);

      // Assert
      Assert.Equal(testData.ExpectOffset, limitOffsetPageToken.Offset);
    }

    [Fact]
    public void Constructor_EmptyPageToken_LimitAndOffsetAreZeroes()
    {
      // Act
      var limitOffsetPageToken = (LimitOffsetPaginationToken)m_tokenFactory(PaginationTokenType.LimitOffset, string.Empty);

      // Assert
      Assert.Equal(0, limitOffsetPageToken.Limit);
      Assert.Equal(0, limitOffsetPageToken.Offset);
    }

    [Theory]
    [MemberData(nameof(ValidNonEmptyPageTokenData))]
    public void Token_ValidPageToken_TokenIsNotEmpty(PageTokenTestData testData)
    {
      // Act
      var limitOffsetPageToken = (LimitOffsetPaginationToken)m_tokenFactory(PaginationTokenType.LimitOffset, testData.PageToken);

      // Assert
      Assert.False(string.IsNullOrWhiteSpace(limitOffsetPageToken.Token), $"Token is emtpy.");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    [InlineData(-100)]
    [InlineData(int.MinValue)]
    public void LimitProperty_LimitLessThanZero_ThrowsArgumentException(int limit)
    {
      Assert.Throws<ArgumentException>(() => {
        var tokenArgs = new LimitOffsetPaginationTokenArgs { Limit = limit, Offset = 0 };
        m_tokenFactory(PaginationTokenType.LimitOffset, tokenArgs);
      });
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    [InlineData(-100)]
    [InlineData(int.MinValue)]
    public void OffsetProperty_OffsetLessThanZero_ThrowsArgumentException(int offset)
    {
      Assert.Throws<ArgumentException>(() => {
        var tokenArgs = new LimitOffsetPaginationTokenArgs { Limit = 0, Offset = offset };
        m_tokenFactory(PaginationTokenType.LimitOffset, tokenArgs);
      });
    }
  }
}

