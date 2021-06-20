using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Dotflik.Application.Validation;

namespace Dotflik.Infrastructure.Validation
{
  /// <summary>
  /// Implementation of a data annotation validator
  /// </summary>
  internal class DataAnnotationValidator : IDataAnnotationValidator
  {
    /// <inheritdoc/>
    void IDataAnnotationValidator.Validate<T>(T obj)
    {
      var validator = ((IDataAnnotationValidator)this);
      var isValid = validator.TryValidate(obj, out ICollection<ValidationResult> validationResults);

      if (!isValid)
      {
        var errorMessages = validationResults.Select(vr => vr.ErrorMessage);
        var exMessage = $"Validation failed for object of type {obj.GetType().FullName}. " +
                        string.Join(" ", errorMessages);
        throw new ValidationException(exMessage);
      }
    }

    /// <inheritdoc/>
    bool IDataAnnotationValidator.TryValidate<T>(T obj, out ICollection<ValidationResult> validationResults)
    {
      validationResults = new List<ValidationResult>();
      var validationContext = new ValidationContext(obj, null, null);
      return Validator.TryValidateObject(obj, validationContext, validationResults, true);
    }

    /// <inheritdoc/>
    void IDataAnnotationValidator.ValidateRecursively<T>(T obj)
      => ValidateRecursively(obj, new HashSet<object>());

    /// <inheritdoc/>
    bool IDataAnnotationValidator.TryValidateRecursively<T>(T obj, out IDictionary<object, ICollection<ValidationResult>> validationResults)
    {
      validationResults = new Dictionary<object, ICollection<ValidationResult>>();
      TryValidateRecursively(obj, new HashSet<object>(), validationResults);

      return validationResults.Count == 0;
    }

    /// <summary>
    /// Validate recursively by using a set to store already validated
    /// objects so that infinite recursion won't occur
    /// </summary>
    /// <exception cref="ValidationException">
    /// Thrown when the first data annotation validation fails
    /// </exception>
    /// <typeparam name="T">
    /// A class that has <see cref="ValidationAttribute"/> marked on its properties
    /// </typeparam>
    /// <param name="obj">Object to be validated against</param>
    /// <param name="validatedObjects">Set containing already validated objects</param>
    private void ValidateRecursively<T>(T obj, HashSet<object> validatedObjects) where T : class
    {
      // Prevent infinite recursion if there is a cycle in the object
      if (validatedObjects.Contains(obj))
      {
        return;
      }

      var validator = ((IDataAnnotationValidator)this);
      validator.Validate(obj);

      validatedObjects.Add(obj);

      var properties = obj.GetType().GetProperties();
      foreach (var property in properties)
      {
        var attributes = property.CustomAttributes;
        var hasValAttrb = attributes.Any(a => typeof(ValidationAttribute).IsAssignableFrom(a.AttributeType));

        // Only validate properties that
        // have data annotation attribute marked
        // and are not primitive types
        if (hasValAttrb && !IsPrimitive(property.PropertyType))
        {
          var value = property.GetValue(obj);
          if (value is not null)
          {
            if (IsEnumerable(property.PropertyType))
            {
              foreach (var item in (IEnumerable)value)
              {
                ValidateRecursively(item, validatedObjects);
              }
            }
            else
            {
              ValidateRecursively(value, validatedObjects);
            }
            validatedObjects.Add(value);
          }
        }
      }
    }

    /// <summary>
    /// Validate recursively by using a set to store already validated
    /// objects so that infinite recursion won't occur
    /// </summary>
    /// <typeparam name="T">
    /// A class that has <see cref="ValidationAttribute"/> marked on its properties
    /// </typeparam>
    /// <param name="obj">Object to be validated against</param>
    /// <param name="validatedObjects">Set containing already validated objects</param>
    /// <param name="validationResults">Store all the failed validations where the key is 
    /// the object that has failed validations and the value is the associated
    /// collection of failed validations</param>
    private void TryValidateRecursively<T>(T obj, 
                                           HashSet<object> validatedObjects,
                                           IDictionary<object, ICollection<ValidationResult>> totalValidationResults) 
      where T : class
    {
      // Prevent infinite recursion if there is a cycle in the object
      if (validatedObjects.Contains(obj))
      {
        return;
      }

      var validator = ((IDataAnnotationValidator)this);
      var isValid = validator.TryValidate(obj, out ICollection<ValidationResult> validationResults);
      
      // Store the object that fails the validation
      if (!isValid)
      {
        totalValidationResults.Add(obj, validationResults);
      }

      validatedObjects.Add(obj);

      var properties = obj.GetType().GetProperties();
      foreach (var property in properties)
      {
        var attributes = property.CustomAttributes;
        var hasValAttrb = attributes.Any(a => typeof(ValidationAttribute).IsAssignableFrom(a.AttributeType));

        // Only validate properties that
        // have data annotation attribute marked
        // and are not primitive types
        if (hasValAttrb && !IsPrimitive(property.PropertyType))
        {
          var value = property.GetValue(obj);
          if (value is not null)
          {
            if (IsEnumerable(property.PropertyType))
            {
              foreach (var item in (IEnumerable)value)
              {
                TryValidateRecursively(item, validatedObjects, totalValidationResults);
              }
            }
            else
            {
              TryValidateRecursively(value, validatedObjects, totalValidationResults);
            }
            validatedObjects.Add(value);
          }
        }

      }
    }

    /// <summary>
    /// Check if given type is a primitive type
    /// </summary>
    /// <remarks>
    /// This method considers <see cref="string"/> to be primitive as well
    /// </remarks>
    /// <param name="type">Type</param>
    /// <returns>True if type is primitive. False otherwise</returns>
    private static bool IsPrimitive(Type type)
      => type.IsPrimitive || type == typeof(string);

    /// <summary>
    /// Check if given type is enumerable, aka can
    /// we iterate the object with the given type
    /// </summary>
    /// <remarks>
    /// This method checks if type implements <see cref="IEnumerable{T}"/>
    /// </remarks>
    /// <param name="type">Type</param>
    /// <returns>True if type is enumerable. False otherwise</returns>
    private static bool IsEnumerable(Type type)
    {
      var interfaces = type.GetInterfaces();
      return interfaces.Any(interf => interf == typeof(IEnumerable));
    }

  }

}
