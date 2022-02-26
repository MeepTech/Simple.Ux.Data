using System;

namespace Simple.Ux.Data {

  /// <summary>
  /// Denotes a numeric field that should appear as a range slider in the UI
  /// </summary>
  public class RangeSliderAttribute : Attribute {
    internal float _min;
    internal float _max;
    internal bool _isClampedToInt;

    public RangeSliderAttribute(float min, float max, bool ClampToIntegers = false) {
      _min = min;
      _max = max;
      _isClampedToInt = ClampToIntegers;
    }
  }
}