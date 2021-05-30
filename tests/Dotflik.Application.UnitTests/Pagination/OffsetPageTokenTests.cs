using System;
using Dotflik.Application.Pagination;
using Dotflik.Domain.Exceptions;
using Xunit;

namespace Dotflik.Application.Pagination.UnitTests
{
  public class OffsetPageTokenTests
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
    public void Constructor_InvalidPageTokenFormat_ThrowsBadPageTokenFormatException(string pageToken)
    {
      // Assert
      Assert.Throws<BadPageTokenFormatException>(() => new OffsetPageToken(pageToken));
    }

    [Theory]
    [MemberData(nameof(ValidPageTokenData))]
    public void Constructor_ValidPageToken_CorrectLimitParsed(PageTokenTestData testData)
    {
      // Act
      var offsetPageToken = new OffsetPageToken(testData.PageToken);

      // Assert
      Assert.Equal(testData.ExpectLimit, offsetPageToken.Limit);
    }

    [Theory]
    [MemberData(nameof(ValidPageTokenData))]
    public void Constructor_ValidPageToken_CorrectOffsetParsed(PageTokenTestData testData)
    {
      // Act
      var offsetPageToken = new OffsetPageToken(testData.PageToken);

      // Assert
      Assert.Equal(testData.ExpectOffset, offsetPageToken.Offset);
    }

    [Theory]
    [MemberData(nameof(ValidPageTokenData))]
    public void ToToken_ValidPageToken_CorrectTokenFormatConstructed(PageTokenTestData testData)
    {
      // Act
      var offsetPageToken = new OffsetPageToken(testData.PageToken);

      // Assert
      var expectFormat = string.Format(OffsetPageToken.PageTokenFormat, testData.ExpectLimit, testData.ExpectOffset);
      Assert.Equal(expectFormat, offsetPageToken.ToToken());
    }

  }
}

