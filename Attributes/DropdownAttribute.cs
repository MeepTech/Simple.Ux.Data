using System;
using System.Linq;
using System.Collections.Generic;
using Simple.Ux.Utility;

namespace Simple.Ux.Data {
  /// <summary>
  /// Used to indicate a field where you can select one of a set of options.
  /// </summary>
  public class DropdownAttribute : Attribute {
    internal int _selectLimit;
    internal Dictionary<string, object> _options;

    /// <summary>
    /// Make a new selectable
    /// </summary>
    /// <param name="SelectableValuesLimit">The max values this select can hold as selected values. >1 makes it a multiselect. 0 makes it disabled.</param>
    /// <param name="OptionValues">The option values. If no names are provided, these are turned into strings and those are used as the field keys</param>
    /// <param name="OptionNames">The option names. Must either have none, or the same number as the values</param>
    public DropdownAttribute(int SelectableValuesLimit = 1, object[] OptionValues = null, string[] OptionNames = null) {
      _options = OptionValues is not null 
        ? new Dictionary<string, object>((OptionNames ?? OptionValues.Select(
          value => value.GetType().IsEnum ? value.ToString().ToDisplayCase() : value.ToString()))
          .Zip(OptionValues, (n, v) => new KeyValuePair<string, object>(n, v)))
        : null;
      _selectLimit = SelectableValuesLimit;
    }
  }
}