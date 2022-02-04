namespace Simple.Ux.Data {
  /// <summary>
  /// Denotes a numeric field restricted to a minimum value.
  /// Overriden by range slider attribute.
  /// </summary>
  public class MaximumValueAttribute : RestrictToRangeAttribute {

    public MaximumValueAttribute(float maxValue)
      : base(maxValue, float.MaxValue) { }
  }
}