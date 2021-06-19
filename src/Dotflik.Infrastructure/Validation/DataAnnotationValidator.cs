using System;
using System.Collections.Generic;
using System.Linq;
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
    bool IDataAnnotationValidator.TryValidate<T>(T obj, out ICollection<ValidationResult> validationResults)
    {
      validationResults = new List<ValidationResult>();
      var validationContext = new ValidationContext(obj, null, null);
      return Validator.TryValidateObject(obj, validationContext, validationResults, true);
    }

    /// <inheritdoc/>
    bool IDataAnnotationValidator.TryValidateRecursively<T>(T obj, out IDictionary<object, ICollection<ValidationResult>> validationResults)
    {
      validationResults = new Dictionary<object, ICollection<ValidationResult>>();
      TryValidateRecursively(obj, new HashSet<object>(), validationResults);

      return validationResults.Count == 0;
    }

    /// <inheritdoc/>
    void IDataAnnotationValidator.Validate<T>(T obj)
    {
      var validator = ((IDataAnnotationValidator)this);
      var isValid = validator.TryValidate(obj, out ICollection<ValidationResult> validationResults);

      if (!isValid)
      {
        var errorMessages = validationResults.Select(vr => vr.ErrorMessage);
        var exMessage = $"Validation failed for object type {obj.GetType().FullName};" + 
                        string.Join("; ", errorMessages);
        throw new ValidationException(exMessage);
      }
    }

    /// <inheritdoc/>
    void IDataAnnotationValidator.ValidateRecursively<T>(T obj)
    {
      ValidateRecursively(obj, new HashSet<object>());
    }

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

      // Recursively validate non-null properties
      var properties = obj.GetType().GetProperties();
      foreach (var property in properties)
      {
        if (property.PropertyType != typeof(string) && !property.PropertyType.IsPrimitive)
        {
          var value = property.GetValue(obj);
          if (value is not null)
          {
            ValidateRecursively(value, validatedObjects);
            validatedObjects.Add(value);
          }
        }
      }
    }

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
      if (!isValid)
      {
        totalValidationResults.Add(obj, validationResults);
      }

      validatedObjects.Add(obj);

      // Recursively validate non-null properties
      var properties = obj.GetType().GetProperties();
      foreach (var property in properties)
      {
        if (property.PropertyType != typeof(string) && !property.PropertyType.IsPrimitive)
        {
          var value = property.GetValue(obj);
          if (value is not null)
          {
            TryValidateRecursively(value, validatedObjects, totalValidationResults);
            validatedObjects.Add(value);
          }
        }
      }
    }

  }

}
