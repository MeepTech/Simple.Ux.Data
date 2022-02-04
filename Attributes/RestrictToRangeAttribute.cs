namespace Simple.Ux.Data {
  /// <summary>
  /// Denotes a numeric field restricted to a range of values.
  /// Overriden by range slider attribute.
  /// </summary>
  public class RestrictToRangeAttribute : ValidationAttribute {
    internal new(float, float) _validation
      => ((float, float))base._validation;

    public RestrictToRangeAttribute(float minInclusive, float maxInclusive)
      : base((minInclusive, maxInclusive)) { }
  }
}