using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dotflik.Application.Validation
{
  /// <summary>
  /// Interface for validating object that is marked
  /// with data annotations
  /// </summary>
  public interface IDataAnnotationValidator
  {
    /// <summary>
    /// Validate the data annotations on the <paramref name="obj"/>. This only
    /// validates the top level properties of <paramref name="obj"/>
    /// </summary>
    /// <exception cref="ValidationException">
    /// Thrown when the first data annotation validation fails
    /// </exception>
    /// <typeparam name="T">
    /// A class that has <see cref="ValidationAttribute"/> marked on its properties
    /// </typeparam>
    /// <param name="obj">Object to be validated against</param>
    void Validate<T>(T obj) where T : class;

    /// <summary>
    /// Validate the data annotations on the <paramref name="obj"/>. This only
    /// validates the top level properties of <paramref name="obj"/>. The return
    /// value indicates whether the validations pass
    /// </summary>
    /// <typeparam name="T">
    /// A class that has <see cref="ValidationAttribute"/> marked on its properties
    /// </typeparam>
    /// <param name="obj">Object to be validated against</param>
    /// <param name="validationResults">Store all the failed validations</param>
    /// <returns>True if validations pass. False otherwise</returns>
    bool TryValidate<T>(T obj, out ICollection<ValidationResult> validationResults)
      where T : class;

    /// <summary>
    /// Validate the data annotations on the <paramref name="obj"/>. This recursively
    /// traverses all <paramref name="obj"/>'s properties and validate each property.
    /// </summary>
    /// <remarks>
    /// This method can handle the case when there 
    /// are cycles in the structure of <paramref name="obj"/>
    /// </remarks>
    /// <exception cref="ValidationException">
    /// Thrown when the first data annotation validation fails
    /// </exception>
    /// <typeparam name="T">
    /// A class that has <see cref="ValidationAttribute"/> marked on its properties
    /// </typeparam>
    /// <param name="obj">Object to be validated against</param>
    void ValidateRecursively<T>(T obj) where T : class;

    /// <summary>
    /// Validate the data annotations on the <paramref name="obj"/>. This recursively
    /// traverses all <paramref name="obj"/>'s properties and validate each property.
    /// The return value indicates whether the validations pass
    /// </summary>
    /// <remarks>
    /// This method can handle the case when there 
    /// are cycles in the structure of <paramref name="obj"/>
    /// </remarks>
    /// <typeparam name="T">
    /// A class that has <see cref="ValidationAttribute"/> marked on its properties
    /// </typeparam>
    /// <param name="obj">Object to be validated against</param>
    /// <param name="validationResults">Store all the failed validations where the key is 
    /// the object that has failed validations and the value is the associated
    /// collection of failed validations</param>
    /// <returns>True if validations pass. False otherwise</returns>
    bool TryValidateRecursively<T>(T obj, out IDictionary<object, ICollection<ValidationResult>> validationResults) 
      where T : class;
  }
}
