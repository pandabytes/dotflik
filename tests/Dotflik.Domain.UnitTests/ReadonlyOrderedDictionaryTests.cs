using System;
using System.Linq;
using System.Collections.Specialized;

using Xunit;
using Dotflik.Domain.Collections;

namespace Dotflik.Domain.UnitTests
{
  public class ReadonlyOrderedDictionaryTests
  {
    [Fact]
    public void Constructor_ArgumentIsIOrderedDictionary_ReadonlyOrderedDictionaryPopulated()
    {
      // Arrange
      var orderedDict = new OrderedDictionary
        { { "a", "b" }, { "c", "d" }, { "x", "y" } };

      // Act
      var readonlyOrderedDict = new ReadonlyOrderedDictionary<string, string>(orderedDict);

      // Assert
      Assert.Equal(orderedDict.Count, readonlyOrderedDict.Count);

      var enumerator = orderedDict.GetEnumerator();
      while (enumerator.MoveNext())
      {
        var key = (string)enumerator.Key;
        Assert.True(readonlyOrderedDict.ContainsKey(key),
          $"Readonly ordered dictionary doesn't contain key {key}");
        Assert.Equal(enumerator.Value, readonlyOrderedDict[key]);
      }
    }

    [Fact]
    public void Constructor_IOrderedDictionaryHasIncorrectKeyType_ThrowsArgumentException()
    {
      // Arrange
      var orderedDict = new OrderedDictionary
        { { "a", "b" }, { 0, "d" }, { "x", "y" } };

      // Act & Assert
      Assert.Throws<ArgumentException>(() => new ReadonlyOrderedDictionary<string, string>(orderedDict));
    }

    [Fact]
    public void Constructor_IOrderedDictionaryHasNullValue_ThrowsArgumentNullException()
    {
      // Arrange
      var orderedDict = new OrderedDictionary
        { { "a", "b" }, { "c", "d" }, { "x", null } };

      // Act & Assert
      Assert.Throws<ArgumentNullException>(() => new ReadonlyOrderedDictionary<string, string>(orderedDict));
    }

    [Fact]
    public void Constructor_IOrderedDictionaryHasIncorrectValueType_ThrowsArgumentNullException()
    {
      // Arrange
      var orderedDict = new OrderedDictionary
        { { "a", 0 }, { "c", "d" }, { "x", "y" } };

      // Act & Assert
      Assert.Throws<ArgumentException>(() => new ReadonlyOrderedDictionary<string, string>(orderedDict));
    }

    [Fact]
    public void TryGetValue_KeyNotExist_ReturnsFalseAndValueSetToDefault()
    {
      // Arrange
      var orderedDict = new OrderedDictionary 
        { { "a", "b" }, { "c", "d" }, { "x", "y" } };
      var readonlyOrderedDict = new ReadonlyOrderedDictionary<string, string>(orderedDict);

      // Act
      var success = readonlyOrderedDict.TryGetValue(string.Empty, out string? value);

      // Assert
      Assert.False(success);
      Assert.Null(value);
    }

    [Fact]
    public void TryGetValue_KeyExist_ReturnsTrueAndValueIsReturned()
    {
      // Arrange
      var key = "a";
      var value = "b";
      var orderedDict = new OrderedDictionary
        { { key, value }, { "c", "d" }, { "x", "y" } };
      var readonlyOrderedDict = new ReadonlyOrderedDictionary<string, string>(orderedDict);

      // Act
      var success = readonlyOrderedDict.TryGetValue(key, out string? actualValue);

      // Assert
      Assert.True(success);
      Assert.Equal(value, actualValue);
    }

    [Fact]
    public void Keys_KeysMatch()
    {
      // Arrange
      var orderedDict = new OrderedDictionary
        { { "a", "b" }, { "c", "d" }, { "x", "y" } };

      // Act
      var readonlyOrderedDict = new ReadonlyOrderedDictionary<string, string>(orderedDict);

      // Assert
      Assert.Equal(orderedDict.Keys.Count, readonlyOrderedDict.Keys.Count());

      var orderedDictKeysEnum = orderedDict.Keys.GetEnumerator();
      var readonlyOrderedDictKeysEnum = readonlyOrderedDict.Keys.GetEnumerator();

      while (orderedDictKeysEnum.MoveNext() && readonlyOrderedDictKeysEnum.MoveNext())
      {
        Assert.Equal(orderedDictKeysEnum.Current, readonlyOrderedDictKeysEnum.Current);
      }
    }

    [Fact]
    public void Values_ValuesMatch()
    {
      // Arrange
      var orderedDict = new OrderedDictionary
        { { "a", "b" }, { "c", "d" }, { "x", "y" } };

      // Act
      var readonlyOrderedDict = new ReadonlyOrderedDictionary<string, string>(orderedDict);

      // Assert
      Assert.Equal(orderedDict.Values.Count, readonlyOrderedDict.Values.Count());

      var orderedDictValuesEnum = orderedDict.Values.GetEnumerator();
      var readonlyOrderedDictValuesEnum = readonlyOrderedDict.Values.GetEnumerator();

      while (orderedDictValuesEnum.MoveNext() && readonlyOrderedDictValuesEnum.MoveNext())
      {
        Assert.Equal(orderedDictValuesEnum.Current, readonlyOrderedDictValuesEnum.Current);
      }
    }

  }
}
