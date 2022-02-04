namespace Simple.Ux.Data {
  /// <summary>
  /// Denotes a numeric field restricted to a minimum value.
  /// Overriden by range slider attribute.
  /// </summary>
  public class MinimumValueAttribute : RestrictToRangeAttribute {

    public MinimumValueAttribute(float minValue)
      : base(minValue, float.MaxValue) { }
  }
}