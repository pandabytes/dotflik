using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.DependencyInjection;

using Dotflik.Application.Validation;
using Dotflik.Infrastructure.Validation.Data.UnitTests;

using Xunit;

namespace Dotflik.Infrastructure.Validation.UnitTests
{
  public class DataAnnotationValidatorTests
  {
    /// <summary>
    /// Random seed to generate fake data
    /// </summary>
    private const int RandomSeed = 8675309;

    private static readonly IList<Address> ValidAddresses = GetFakeAddresses(10, RandomSeed);

    public static readonly TheoryData<Address> ValidAddressesData = 
      new TheoryData<Address>().AddRange(ValidAddresses);

    public static readonly TheoryData<Person> InvalidPeopleData = new()
    {
      new Person
      (
        Name: string.Empty,
        NickName: string.Empty,
        Age: 3,
        Job: new Job(string.Empty),
        Addresses: ValidAddresses,
        Kids: new List<Person> { }
      ),
      new Person
      (
        Name: "Ben",
        NickName: string.Empty,
        Age: -1,
        Job: new Job(string.Empty),
        Addresses: ValidAddresses,
        Kids: new List<Person> { }
      ),
      new Person
      (
        Name: "Nick",
        NickName: string.Empty,
        Age: 3,
        Job: new Job(string.Empty),
        Addresses: new List<Address>(),
        Kids: new List<Person> { }
      )
    };

    /// <summary>
    /// Object under test
    /// </summary>
    private readonly IDataAnnotationValidator m_validator;

    /// <summary>
    /// Contain a record of valid person. What this means
    /// is this object will not have fail validation and tests
    /// can use this object to create invalid person object
    /// </summary>
    private readonly Person m_validPerson;

    /// <summary>
    /// Constructor
    /// </summary>
    public DataAnnotationValidatorTests()
    {
      var services = new ServiceCollection();
      services.AddDataAnnotationValidator();
      var provider = services.BuildServiceProvider();

      m_validator = provider.GetRequiredService<IDataAnnotationValidator>();

      m_validPerson = new Person
      (
        Name: "Valid",
        NickName: string.Empty,
        Age: 0,
        Job: new Job("No job"),
        Addresses: ValidAddresses,
        Kids: new List<Person> { }
      );
    }

    #region Validate

    [Theory]
    [MemberData(nameof(InvalidPeopleData))]
    public void Validate_FailValidation_ThrowsValidationException(Person person)
      => Assert.Throws<ValidationException>(() => m_validator.Validate(person));

    [Theory]
    [MemberData(nameof(ValidAddressesData))]
    public void Validate_PassValidation_NoExceptionThrown(Address address)
      => m_validator.Validate(address);

    [Theory]
    [MemberData(nameof(InvalidPeopleData))]
    public void TryValidate_FailValidation_ValidationResultsNotEmpty(Person person)
    {
      // Act
      var isValid = m_validator.TryValidate(person, out ICollection<ValidationResult> validationResults);

      // Assert
      Assert.False(isValid, "Validation should have failed");
      Assert.NotEmpty(validationResults);
    }

    [Theory]
    [MemberData(nameof(ValidAddressesData))]
    public void TryValidate_PassValidation_ValidationResultsEmpty(Address address)
    {
      // Act
      var isValid = m_validator.TryValidate(address, out ICollection<ValidationResult> validationResults);

      // Assert
      Assert.True(isValid, "Validation should have passed");
      Assert.Empty(validationResults);
    }

    #endregion

    #region Validate Recursively

    [Fact]
    public void ValidateRecursively_NestedFailValidation_ThrowsValidationException()
    {
      // Arrange
      var person = m_validPerson with { Job = new Job(string.Empty) };

      // Act & Assert
      Assert.Throws<ValidationException>(() => m_validator.ValidateRecursively(person));
    }

    [Fact]
    public void ValidateRecursively_NestedPassValidation_NoExceptionThrown()
      => m_validator.ValidateRecursively(m_validPerson);

    [Fact]
    public void ValidateRecursively_NestedCollectionFailValidation_ThrowsValidationException()
    {
      // Arrange
      var person = m_validPerson with { Addresses = new List<Address>() };

      var kid1 = person with { Name = "Nick" };
      var kid2 = person with { Name = "Lily" };
      var kid3 = person with { Name = "Hannah" };
      var parent = person with 
      { 
        Name = "Bob", 
        Addresses = ValidAddresses, 
        Kids = new List<Person> { kid1, kid2, kid3 } 
      };

      // Act & Assert
      Assert.Throws<ValidationException>(() => m_validator.ValidateRecursively(parent));
    }

    [Fact]
    public void ValidateRecursively_NestedCollectionPassValidation_NoExceptionThrown()
    {
      // Arrange
      var kid1 = m_validPerson with { Name = "Nick" };
      var kid2 = m_validPerson with { Name = "Lily" };
      var kid3 = m_validPerson with { Name = "Hannah" };
      var parent = m_validPerson with
      {
        Name = "Bob",
        Kids = new List<Person> { kid1, kid2, kid3 }
      };

      // Act
      m_validator.ValidateRecursively(parent);
    }

    [Fact]
    public void ValidateRecursively_HasCyclePassValidation_ThrowsValidationException()
    {
      // Arrange
      var p1 = m_validPerson with { Name = "Nick", Age = 1, Kids = new List<Person>() };
      var p2 = m_validPerson with { Name = "Ben", Age = 5, Kids = new List<Person>() };

      // Create a cycle here
      p1.Kids.Add(p2);
      p2.Kids.Add(p1);

      // Act
      m_validator.ValidateRecursively(p1);
      m_validator.ValidateRecursively(p2);
    }

    [Fact]
    public void TryValidateRecursively_NestedFailValidation_ValidationResultsNotEmpty()
    {
      // Arrange
      var person = m_validPerson with { Job = new Job(string.Empty) };
      IDictionary<object, ICollection<ValidationResult>> vr;

      // Act
      var isValid = m_validator.TryValidateRecursively(person, out vr);

      // Assert
      Assert.False(isValid, "Validation should have failed");
      Assert.NotEmpty(vr);
    }

    [Fact]
    public void TryValidateRecursively_NestedPassValidation_ValidationResultsEmpty()
    {
      // Arrange
      IDictionary<object, ICollection<ValidationResult>> vr;

      // Act
      var isValid = m_validator.TryValidateRecursively(m_validPerson, out vr);

      // Assert
      Assert.True(isValid, "Validation should have passed");
      Assert.Empty(vr);
    }

    [Fact]
    public void TryValidateRecursively_NestedCollectionFailValidation_ValidationResultsNotEmpty()
    {
      // Arrange
      var person = m_validPerson with { Addresses = new List<Address>() };

      var kid1 = person with { Name = "Nick" };
      var kid2 = person with { Name = "Lily" };
      var kid3 = person with { Name = "Hannah" };
      var parent = person with
      {
        Name = "Bob",
        Addresses = ValidAddresses,
        Kids = new List<Person> { kid1, kid2, kid3 }
      };

      IDictionary<object, ICollection<ValidationResult>> vr;

      // Act
      var isValid = m_validator.TryValidateRecursively(parent, out vr);

      // Assert
      Assert.False(isValid, "Validation should have failed");
      Assert.NotEmpty(vr);
    }

    [Fact]
    public void TryValidateRecursively_NestedCollectionPassValidation_ValidationResultsNotEmpty()
    {
      // Arrange
      var person = m_validPerson with { Addresses = new List<Address>() };

      var kid1 = m_validPerson with { Name = "Nick" };
      var kid2 = m_validPerson with { Name = "Lily" };
      var kid3 = m_validPerson with { Name = "Hannah" };
      var parent = m_validPerson with
      {
        Name = "Bob",
        Addresses = ValidAddresses,
        Kids = new List<Person> { kid1, kid2, kid3 }
      };

      IDictionary<object, ICollection<ValidationResult>> vr;

      // Act
      var isValid = m_validator.TryValidateRecursively(parent, out vr);

      // Assert
      Assert.True(isValid, "Validation should have passed");
      Assert.Empty(vr);
    }

    [Fact]
    public void TryValidateRecursively_HasCyclePassValidation_ThrowsValidationException()
    {
      // Arrange
      var p1 = m_validPerson with { Name = "Nick", Age = 1, Kids = new List<Person>() };
      var p2 = m_validPerson with { Name = "Ben", Age = 5, Kids = new List<Person>() };

      // Create a cycle here
      p1.Kids.Add(p2);
      p2.Kids.Add(p1);

      IDictionary<object, ICollection<ValidationResult>> vr1;
      IDictionary<object, ICollection<ValidationResult>> vr2;

      // Act
      var isValid1 = m_validator.TryValidateRecursively(p1, out vr1);
      var isValid2 = m_validator.TryValidateRecursively(p2, out vr2);

      // Assert
      Assert.True(isValid1, "Validation should have passed");
      Assert.Empty(vr1);

      Assert.True(isValid2, "Validation should have passed");
      Assert.Empty(vr2);
    }

    #endregion

    /// <summary>
    /// Generate <paramref name="size"/> fake addresses
    /// </summary>
    /// <param name="size">Number of fake address objects</param>
    /// <param name="seed">Random seed</param>
    /// <returns><paramref name="size"/> number of address objects</returns>
    private static IList<Address> GetFakeAddresses(int size, int seed)
    {
      var fakeAddresses = new Bogus.Faker<Address>()
                            .StrictMode(false)
                            .CustomInstantiator(f =>
                            {
                              var addr = f.Address;
                              return new Address(addr.BuildingNumber(), addr.StreetName(),
                                                 addr.State(), addr.City(), addr.ZipCode());
                            });
      return fakeAddresses.UseSeed(seed).Generate(size);
    }

  }

  /// <summary>
  /// Theory data extensions
  /// </summary>
  internal static class TheoryDataExtensions
  {
    /// <summary>
    /// Add an enumerable of <typeparamref name="T"/> objects to
    /// the theory data
    /// </summary>
    /// <typeparam name="T">Type stored in the theory data</typeparam>
    /// <param name="theoryData">Theory data</param>
    /// <param name="enumerable">Enumerable object to be added to the theory data</param>
    /// <returns>Theory data</returns>
    public static TheoryData<T> AddRange<T>(this TheoryData<T> theoryData, IEnumerable<T> enumerable)
    {
      foreach (var item in enumerable)
      {
        theoryData.Add(item);
      }
      return theoryData;
    }

  }

}
