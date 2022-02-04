using System;

namespace Simple.Ux.Data {

  /// <summary>
  /// Denotes how the field should be validated.
  /// You can name a method that takes one argument and returns a bool,
  /// or for numbers you can provide (int min, int max) as a tuple.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
  public class ValidationAttribute : Attribute {
    internal object _validation;

    public ValidationAttribute(object Validation) {
      _validation = Validation;
    }
  }
}