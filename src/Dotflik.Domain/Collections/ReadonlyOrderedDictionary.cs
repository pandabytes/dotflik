using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Dotflik.Domain.Collections
{
  /// <summary>
  /// Represent a readonly ordered dictionary.
  /// </summary>
  /// <typeparam name="TKey">Key type.</typeparam>
  /// <typeparam name="TValue">Value type.</typeparam>
  public sealed class ReadonlyOrderedDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
  {
    private OrderedDictionary m_orderedDict;

    /// <summary>
    /// Construct an empty readonly ordered dictionary.
    /// </summary>
    public ReadonlyOrderedDictionary() => m_orderedDict = new();

    /// <summary>
    /// Populate with key-value pairs from <paramref name="orderedDict"/>.
    /// </summary>
    /// <remarks>
    /// Since <see cref="IOrderedDictionary"/> doesn't specify key and value type,
    /// this constructor will validate to see if the keys and values in
    /// <paramref name="orderedDict"/> match with <typeparamref name="TKey"/> and
    /// <typeparamref name="TValue"/>.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when key or value type doesn't match with <typeparamref name="TKey"/> or
    /// <typeparamref name="TValue"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when value is null.
    /// </exception>
    /// <param name="dict">Ordered dictionary.</param>
    public ReadonlyOrderedDictionary(IOrderedDictionary orderedDict)
    {
      m_orderedDict = new();

      var enumerator = orderedDict.GetEnumerator();
      while (enumerator.MoveNext())
      {
        TKey key;
        TValue value;

        try
        {
          key = (TKey)enumerator.Key;
        }
        catch (InvalidCastException ex)
        {
          throw new ArgumentException($"Key must be of type {typeof(TKey).FullName}.", ex);
        }

        if (enumerator.Value is null)
        {
          throw new ArgumentNullException("Value must not be null.");
        }

        try
        {
          value = (TValue)enumerator.Value;
        }
        catch (InvalidCastException ex)
        {
          throw new ArgumentException($"Value must be of type {typeof(TValue).FullName}.", ex);
        }

        m_orderedDict.Add(key, value);
      }
    }

    /// <inheritdoc/>
    public TValue this[TKey key]
    {
      get
      {
        var value = m_orderedDict[key];
        if (value is null)
        {
          throw new KeyNotFoundException($"Cannot find key \"{key}\".");
        }
        return (TValue)value;
      }
    }

    public TValue this[int index] => (TValue)m_orderedDict[index]!;

    /// <inheritdoc/>
    public IEnumerable<TKey> Keys
    {
      get
      {
        foreach (var key in m_orderedDict.Keys)
        {
          yield return (TKey)key;
        }
      }
    }

    /// <inheritdoc/>
    public IEnumerable<TValue> Values
    {
      get
      {
        foreach (var value in m_orderedDict.Values)
        {
          yield return (TValue)value;
        }
      }
    }

    /// <inheritdoc/>
    public int Count => m_orderedDict.Count;

    /// <inheritdoc/>
    public bool ContainsKey(TKey key) => m_orderedDict.Contains(key);

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      var enumerator = m_orderedDict.GetEnumerator();
      while (enumerator.MoveNext())
      {
        var key = (TKey)enumerator.Key;
        var value = (TValue)enumerator.Value!;
        yield return new KeyValuePair<TKey, TValue>(key, value);
      }
    }
    
    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
      if (!ContainsKey(key))
      {
        value = default(TValue);
        return false;
      }

      value = this[key];
      return true;
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => m_orderedDict.GetEnumerator();

  }
}
