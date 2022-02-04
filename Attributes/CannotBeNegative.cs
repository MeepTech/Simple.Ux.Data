namespace Simple.Ux.Data {

  /// <summary>
  /// Denotes a field that should not be negative.
  /// Overriden by range slider attribute.
  /// </summary>
  public class CannotBeNegative : RestrictToRangeAttribute {
    public CannotBeNegative()
      : base(0, float.MaxValue) { }
  }
}