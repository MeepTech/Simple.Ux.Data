using Meep.Tech.Collections.Generic;
using System;
using System.Linq;

namespace Simple.Ux.Data {

  /// <summary>
  /// A slider for a value between the min and max.
  /// </summary>
  public class RangeSliderField : DataField<double> {

    /// <summary>
    /// The valid range for this slider
    /// </summary>
    public (double min, double max) ValidRange {
      get;
    }

    /// <summary>
    /// If this only allows whole numbers
    /// </summary>
    public bool IsClampedToWholeNumbers {
      get;
    }

    protected override DelegateCollection<Func<DataField, double, (bool success, string message)>> DefaultValidations 
      => base.DefaultValidations.Append("__defaultSliderWithinRangeValidator_", (f, v) => {
        bool r = v >= (ValidRange.min - 0.001f) && v <= (ValidRange.max + 0.001f);
        if(r) {
          return (true, null);
        } else {
          return (false, $"Value: {v}, is outside of valid range: {(ValidRange.min - 0.001f)} to {(ValidRange.max + 0.001f)}");
        }
      });

    public RangeSliderField(
      string name,
      double min,
      double max,
      bool clampedToWholeNumbers = false,
      string tooltip = null,
      float? value = null,
      string dataKey = null,
      bool isReadOnly = false
    ) : base(
      DisplayType.RangeSlider,
      name,
      tooltip,
      clampedToWholeNumbers ? Math.Floor(value ?? min) : value ?? min,
      dataKey,
      isReadOnly
    ) {
      IsClampedToWholeNumbers = clampedToWholeNumbers;
      ValidRange = clampedToWholeNumbers ? ((int)Math.Floor(min), (int)Math.Floor(max)) : (min, max);
    }

    ///<summary><inheritdoc/></summary>
    public override bool TryToSetValue(object value, out string message) {
      double number = Math.Round(
          double.TryParse(
            value?.ToString()
              ?? "",
            out double d
          )
            ? d
            : 0,
          2
        );
      value = number;
      return base.TryToSetValue(value, out message);
    }
  }
}
