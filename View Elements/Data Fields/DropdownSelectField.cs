using Meep.Tech.Data;
using Meep.Tech.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using Simple.Ux.Utility;

namespace Simple.Ux.Data {

  /// <summary>
  /// A drop down that can select enum values
  /// </summary>
  public class DropdownSelectField<TEnum> : DropdownSelectField
    where TEnum : Enum {

    public DropdownSelectField(
      string name,
      int maxSelectableValues = 1,
      string tooltip = null,
      IEnumerable<string> alreadySelectedOptionKeys = null, 
      string dataKey = null, 
      bool isReadOnly = false
    ) : base(
      name,
      Enum.GetValues(typeof(TEnum)).Cast<object>().ToDictionary(e => e.ToString().ToDisplayCase()),
      maxSelectableValues,
      tooltip,
      alreadySelectedOptionKeys,
      dataKey,
      isReadOnly
    ) {}
  }

  /// <summary>
  /// A drop down that can select values
  /// </summary>
  public class DropdownSelectField : DataField<List<KeyValuePair<string, object>>> {

    /// <summary>
    /// The valid options, indexed by their display key.
    /// </summary>
    public IReadOnlyOrderedDictionary<string, object> Options
      => _options;

    /// <summary>
    /// The delegate collection key for the default dropdown options validation method
    /// </summary>
    public const string DefaultDropdownOptionsValidationDelegateKey 
      = "__defaultDropdownInOptionsValidation_";

    OrderedDictionary<string, object> _options;

    /// <summary>
    /// Actions to be executed when the amount of values this can hold changes.
    /// Takes the current field, and the old limit.
    /// </summary>
    public DelegateCollection<Action<DataField, int>> OnSelectableValueLimitChangeListeners {
      get;
      init;
    } = new();

    /// <summary>
    /// If the user can select more than one value
    /// </summary>
    public bool MultiSelectAllowed
      => MaxValuesAllowed > 1;

    /// <summary>
    /// The max values allowed to be selected at once.
    /// </summary>
    public int MaxValuesAllowed {
      get => _maxValuesAllowed;
      set {
        var old = _maxValuesAllowed;
        var oldAllowed = MultiSelectAllowed;
        _maxValuesAllowed = value;
        OnSelectableValueLimitChangeListeners.ForEach(listener => listener.Value(this, old));
      }
    } int _maxValuesAllowed = 1;

    ///<summary><inheritdoc/></summary>
    protected override DelegateCollection<Func<DataField, List<KeyValuePair<string, object>>, (bool success, string message)>> DefaultValidations
      => base.DefaultValidations.Append(
        DefaultDropdownOptionsValidationDelegateKey,
        (f, v) => MultiSelectAllowed 
          ? _multiSelectDefaultValidationLogic(f, v)
          : _singleSelectDefaultValidationLogic(f, v)
      );

    public DropdownSelectField(
      string name,
      IEnumerable<object> optionValues,
      IEnumerable<string> optionNames = null,
      int maxSelectableValues = 1,
      string tooltip = null,
      IEnumerable<string> alreadySelectedOptionKeys = null,
      string dataKey = null,
      bool isReadOnly = false
    ) : this(
        name,
        new Dictionary<string, object>((optionNames ?? optionValues.Select(
          value => value.GetType().IsEnum ? value.ToString().ToDisplayCase() : value.ToString())
        ).Zip(optionValues, (n, v) => new KeyValuePair<string, object>(n, v))),
        maxSelectableValues,
        tooltip,
        alreadySelectedOptionKeys,
        dataKey,
        isReadOnly
    ) {
    }

    public DropdownSelectField(
      string name,
      Dictionary<string, object> options,
      int maxSelectableValues = 1,
      string tooltip = null,
      IEnumerable<string> alreadySelectedOptionKeys = null,
      string dataKey = null,
      bool isReadOnly = false
    ) : base(
      DisplayType.Dropdown,
      name,
      tooltip,
      alreadySelectedOptionKeys?.Select(key => new KeyValuePair<string, object>(key, options[key])).ToList(),
      dataKey,
      isReadOnly
    ) {
      _options = new OrderedDictionary<string, object>(options);
      _maxValuesAllowed = Math.Max(0, maxSelectableValues);
    }

    ///<summary><inheritdoc/></summary>
    public override DataField Copy(View toNewView = null, bool withCurrentValuesAsNewDefaults = false) {
      DataField copy = base.Copy(toNewView, withCurrentValuesAsNewDefaults);
      copy.Value = Value?.ToList();
      copy.DefaultValue = withCurrentValuesAsNewDefaults ? Value?.ToList() : (DefaultValue as List<KeyValuePair<string, object>>);
      (copy as DropdownSelectField)._options = new(_options);

      return copy;
    }

    (bool, string) _singleSelectDefaultValidationLogic(DataField field, List<KeyValuePair<string, object>> values) {
      if(values.Count > 1) {
        return (false, $"Too Many Values. {values.Count} are selected, but Only 1 is allowed.");
      }

      return (field as DropdownSelectField)._options.TryGetValue(values.First().Key, out object expected) && values.First().Value == expected
        ? (true, "")
        : (false, $"Unrecognized Select Item: {values.First().Key}, with value: {values.First().Value ?? "null"}. \n Valid Items:\n{string.Join('\n', (field as DropdownSelectField)._options.Keys.Select(key => $"\t> {key}"))}");
    }

    (bool, string) _multiSelectDefaultValidationLogic(DataField f, List<KeyValuePair<string, object>> values) {
      if(values.Count > MaxValuesAllowed) {
        return (false, $"Too Many Values. {values.Count} are selected, but Only {MaxValuesAllowed} are allowed.");
      }

      return values.Select(value => (f as DropdownSelectField)._options.TryGetValue(value.Key, out object expected) && value.Value == expected
        ? ((bool success, string message)?)(true, "")
        : (false, $"Unrecognized Select Item: {value.Key}, with value: {value.Value ?? "null"}. \n Valid Items:\n{string.Join('\n', (f as DropdownSelectField)._options.Keys.Select(key => $"\t> {key}"))}"))
          .FirstOrDefault(result => !result.Value.success) ?? (true, "All Values Match Known Options!");
    }
  }
}
