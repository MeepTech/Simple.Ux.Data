using Meep.Tech.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Simple.Ux.Data {

  /// <summary>
  /// A data field for input or display in a simple ux pannel/view
  /// </summary>
  public abstract partial class DataField : IUxViewElement {

    /// <summary>
    /// The name of the field.
    /// Used as a default data key
    /// </summary>
    public virtual string Name {
      get;
    }

    /// <summary>
    /// Data key for the field.
    /// Used to access it from the editor component display data.
    /// </summary>
    public virtual string DataKey {
      get;
    }

    /// <summary>
    /// Info tooltip for the field
    /// </summary>
    public virtual string Tooltip {
      get;
    } = null;

    /// <summary>
    /// The current value of the field.
    /// </summary>
    public object Value {
      get;
      internal set;
    }

    /// <summary>
    /// The default initial value.
    /// </summary>
    public object DefaultValue {
      get;
      internal set;
    }

    /// <summary>
    /// If this field is readonly
    /// </summary>
    public virtual bool IsReadOnly {
      get;
      internal set;
    } = false;

    /// <summary>
    /// The type of display this field should use.
    /// </summary>
    public DisplayType Type {
      get;
    }

    /// <summary>
    /// The view this field is in.
    /// </summary>
    public View View {
      get;
      internal set;
    }

    /// <summary>
    /// Functions that check if the current field should be enabled.
    /// Called when another field in the same view is updated, or Update is called on the view.
    /// </summary>
    public DelegateCollection<Func<DataField, View, bool>> EnabledIfCheckers {
      get => DefaultEnabledIfCheckers;
      init => value?.ForEach(DefaultEnabledIfCheckers.Add);
    }
    /// <summary>
    /// Default enable if checkers added to the field.
    /// </summary>
    protected virtual DelegateCollection<Func<DataField, View, bool>> DefaultEnabledIfCheckers {
      get;
      private set;
    } = new();

    /// <summary>
    /// Functions that check if the current field should be hidden.
    /// Called when another field in the same view is updated, or Update is called on the view.
    /// </summary>
    public DelegateCollection<Func<DataField, View, bool>> HideIfCheckers {
      get => DefaultHideIfCheckers;
      init => value?.ForEach(DefaultHideIfCheckers.Add);
    }
    /// <summary>
    /// Default hide if checkers added to the field.
    /// </summary>
    protected virtual DelegateCollection<Func<DataField, View, bool>> DefaultHideIfCheckers {
      get;
      private set;
    } = new();

    /// <summary>
    /// If this field should be enabled for editing.
    /// </summary>
    public bool IsEnabled {
      get {
        if(_overrideIsEnabledCheckers) {
          return _isEnabledOverride.Value;
        } else if(EnabledIfCheckers is not null && EnabledIfCheckers.Any(checker => !checker.Value(this, View))) {
          return false;
        }

        return _isEnabledOverride ?? true;
      }
    }

    /// <summary>
    /// If this field should be hidden to the current view
    /// </summary>
    public bool IsHidden {
      get {
        if(_overrideIsHiddenCheckers) {
          return _isHiddenOverride.Value;
        } else if(HideIfCheckers is not null && HideIfCheckers.Any(checker => checker.Value(this, View))) {
          return true;
        }

        return _isHiddenOverride ?? false;
      }
    }

    /// <summary>
    /// If this field should be tracked by it's view
    /// </summary>
    public bool ShouldBeTrackedByView
      => (!IsReadOnly || !string.IsNullOrWhiteSpace(DataKey));

    internal DataField _controllerField;
    internal bool? _isEnabledOverride = null;
    internal bool? _isHiddenOverride = null;
    internal bool _overrideIsEnabledCheckers = false;
    internal bool _overrideIsHiddenCheckers = false;

    /// <summary>
    /// Make a new data field for a Simple Ux.
    /// </summary>
    /// <param name="type">the DisplayType to use for this field</param>
    /// <param name="name">the field name. should be unique unless you change the data key</param>
    /// <param name="tooltip">a breif description of the field, will appear on mouse hover in the ui</param>
    /// <param name="value">default/current value of the field</param>
    /// <param name="dataKey">Used to get the value of this field from the view</param>
    /// <param name="isReadOnly">Some read only fields may be formatted differently (like Text). try passing '() => false' to enable if you want a blured out input field instead.</param>
    protected DataField(
      DisplayType type,
      string name,
      string tooltip = null,
      object value = null,
      string dataKey = null,
      bool isReadOnly = false
    ) {
      Type = type;
      Name = name;
      Tooltip = tooltip;
      DefaultValue = Value = value;
      IsReadOnly = isReadOnly;
      DataKey = string.IsNullOrWhiteSpace(dataKey)
        ? name
        : dataKey;

      if(!isReadOnly && DataKey is null) {
        throw new ArgumentException($"Non-read-only fields require a data key. Provide a title, name, or datakey to the field constructor or Make function");
      }
    }

    /// <summary>
    /// Try to update the field value to a new one.
    /// Checks validations and returns an error message if there is one.
    /// </summary>
    public virtual bool TryToSetValue(object value, out string resultMessage) {
      var oldValue = Value;
      resultMessage = "Success!";

      /// for controller fields, that need to be validated by their parent.
      if(_controllerField is not null) {
        (object key, object value)? pair = null;
        if(value is KeyValuePair<string, object> stringKeyedPair) {
          pair = (stringKeyedPair.Key, stringKeyedPair.Value);
        } else if(value is KeyValuePair<int, object> intKeyedPair) {
          pair = (intKeyedPair.Key, intKeyedPair.Value);
        }
        if(pair.HasValue) {
          oldValue = _controllerField.Value;
          if(!((_controllerField as IIndexedItemsDataField)?.TryToUpdateValueAtIndex(pair.Value.key, pair.Value.value, out resultMessage) ?? true)) {
            return false;
          }
        }
      } else if(!Validate(value, out resultMessage)) {
        return false;
      }

      Value = value;
      _runOnValueChangedCallbacks(this, oldValue);

      return true;
    }

    /// <summary>
    /// Used to run validations on the given value.
    /// </summary>
    protected abstract bool Validate(object value, out string resultMessage);
    internal abstract void _runOnValueChangedCallbacks(DataField updatedField, object oldValue);

    /// <summary>
    /// Memberwise clone to copy
    /// </summary>
    /// <returns></returns>
    public virtual DataField Copy(View toNewView = null, bool withCurrentValuesAsNewDefaults = false) {
      var newField = MemberwiseClone() as DataField;
      newField.View = toNewView;
      newField.DefaultValue = withCurrentValuesAsNewDefaults ? Value : DefaultValue;
      newField.DefaultHideIfCheckers = new(DefaultHideIfCheckers);
      newField.DefaultEnabledIfCheckers = new(DefaultEnabledIfCheckers);

      return newField;
    }

    /// <summary>
    /// Manually sets the field to show as enabled.
    /// </summary>
    public void Enable(bool overrideCheckers = false) {
      _overrideIsEnabledCheckers = overrideCheckers;
      _isEnabledOverride = true;
      _runOnValueChangedCallbacks(this, Value);
    }

    /// <summary>
    /// Manually sets the field to show as disabled.
    /// </summary>
    public void Disable(bool overrideCheckers = false) {
      _overrideIsEnabledCheckers = overrideCheckers;
      _isEnabledOverride = false;
      _runOnValueChangedCallbacks(this, Value);
    }

    /// <summary>
    /// Used to toggle this field enabled or disabled manually.
    /// Can be used with or without the current checker delegates in EnabledIfCheckers.
    /// </summary>
    public void SetEnabled(bool setIsEnabledToTrue = true, bool overrideCheckers = false) {
      if(setIsEnabledToTrue)
        Enable(overrideCheckers);
      else
        Disable(overrideCheckers);
    }

    /// <summary>
    /// Reset the is-enabled state to use just the EnabledIfCheckers, and no manually set value
    /// </summary>
    public void ResetIsEnabled() {
      _overrideIsEnabledCheckers = false;
      _isEnabledOverride = null;
      _runOnValueChangedCallbacks(this, Value);
    }

    /// <summary>
    /// Manually set this field to unhidden.
    /// </summary>
    public void Hide(bool overrideCheckers = false) {
      _overrideIsHiddenCheckers = overrideCheckers;
      _runOnValueChangedCallbacks(this, Value);
    }

    /// <summary>
    /// Manually set this field to hidden.
    /// </summary>
    public void UnHide(bool overrideCheckers = false) {
      _overrideIsHiddenCheckers = overrideCheckers;
      _runOnValueChangedCallbacks(this, Value);
    }

    /// <summary>
    /// Used to toggle this field hidden or visible manually.
    /// Can be used with or without the current checker delegates in HiddenIfCheckers.
    /// </summary>
    public void SetHidden(bool setIsHiddenToTrue = true, bool overrideCheckers = false) {
      if(setIsHiddenToTrue)
        Enable(overrideCheckers);
      else
        Disable(overrideCheckers);
    }

    /// <summary>
    /// Reset the is-hidden state to use just the HiddenIfCheckers, and no manually set value
    /// </summary>
    public void ResetIsHidden() {
      _overrideIsHiddenCheckers = false;
      _isHiddenOverride = null;
      _runOnValueChangedCallbacks(this, Value);
    }

    /// <summary>
    /// Reset the value of this field to it's default
    /// </summary>
    public void ResetValueToDefault()
      => Value = DefaultValue;

    ///<summary><inheritdoc/></summary>
    IUxViewElement IUxViewElement.Copy(View toNewView)
      => Copy(toNewView);

    /// <summary>
    /// Make a new field that fits your needs.
    /// Some field types require attribute data.
    /// </summary>
    public static DataField Make(
      DisplayType type,
      string title = null,
      string tooltip = null,
      object value = null,
      bool isReadOnly = false,
      Func<DataField, View, bool> enabledIf = null,
      string dataKey = null,
      Dictionary<Type, Attribute> attributes = null,
      params Func<DataField, object, (bool success, string message)>[] validations
    ) => Make(type, title, tooltip, value, isReadOnly, enabledIf, dataKey, attributes, validations);

    /// <summary>
    /// Make a new field that fits your needs.
    /// Some field types require attribute data.
    /// </summary>
    public static DataField MakeDefault(
      DisplayType type,
      string title = null,
      string tooltip = null,
      object value = null,
      bool isReadOnly = false,
      string dataKey = null,
      DelegateCollection<Func<DataField, View, bool>> enabledIf = null,
      DelegateCollection<Func<DataField, View, bool>> hiddenIf = null,
      DelegateCollection<Func<DataField, object, (bool success, string message)>> validations = null,
      DelegateCollection<Action<DataField, object>> onValueChanged = null,
      Dictionary<Type, Attribute> attributes = null
    ) => type._defaultFieldConstructor(title, tooltip, value, isReadOnly, dataKey, enabledIf, hiddenIf, validations, onValueChanged, attributes);

    /// <summary>
    /// Generic function for adding a value change listener without knowing the type.
    /// </summary>
    public abstract void AddValueChangeListener(string listenerKey, Action<DataField, object> onValueChanged);
  }

  /// <summary>
  /// A data field for input or display in a simple ux pannel/view
  /// </summary>
  public abstract class DataField<TValue> : DataField {

    /// <summary>
    /// Actions to be executed on change.
    /// Takes the current field, and the old value.
    /// </summary>
    public DelegateCollection<Action<DataField, TValue>> OnValueChangedListeners {
      get => DefaultOnValueChangedListeners;
      init => value?.ForEach(DefaultOnValueChangedListeners.Add);
    }
    /// <summary>
    /// Default fields added to the on changed listeners on init.
    /// </summary>
    protected virtual DelegateCollection<Action<DataField, TValue>> DefaultOnValueChangedListeners {
      get;
      private set;
    } = new();

    /// <summary>
    /// Functions that take the current field, and updated object data, and validate it.
    /// Called whenever the value is changed. If the validation fails, the data view's value won't change from it's previous one.
    /// TODO: if a field is invalid, a red X should appear to clear/reset it with a tooltip explaining why it's invalid.
    /// </summary>
    public virtual DelegateCollection<Func<DataField, TValue, (bool success, string message)>> Validations {
      get => DefaultValidations;
      init => value?.ForEach(DefaultValidations.Add);
    }
    /// <summary>
    /// Default validations added to the field.
    /// </summary>
    protected virtual DelegateCollection<Func<DataField, TValue, (bool success, string message)>> DefaultValidations {
      get;
      private set;
    } = new();

    /// <summary>
    /// The value(s) selected.
    /// </summary>
    public new TValue Value {
      get => (TValue)base.Value;
      protected set => base.Value = value;
    }

    /// <summary>
    /// For making new datafield types
    /// </summary>
    protected DataField(DisplayType type, string name, string tooltip = null, object value = null, string dataKey = null, bool isReadOnly = false)
      : base(type, name, tooltip, value, dataKey, isReadOnly) { }

    ///<summary><inheritdoc/></summary>
    public override DataField Copy(View toNewView = null, bool withCurrentValuesAsNewDefaults = false) {
      var newField = base.Copy(toNewView, withCurrentValuesAsNewDefaults) as DataField<TValue>;
      newField.DefaultValidations = new(Validations);
      newField.DefaultOnValueChangedListeners = new(DefaultOnValueChangedListeners);

      return newField;
    }

    ///<summary><inheritdoc/></summary>
    public override void AddValueChangeListener(string listenerKey, Action<DataField, object> onValueChanged)
      => OnValueChangedListeners.Add(listenerKey, (f, o) => onValueChanged(f, o));

    ///<summary><inheritdoc/></summary>
    protected override bool Validate(object value, out string resultMessage) {
      resultMessage = "Value Is Valid! :D";
      TValue convertedValue;
      try {
        convertedValue = (TValue)value;
      } catch (Exception e) {
        throw new InvalidCastException($"Could not cast value's type for Field: {Name}!\n\tFrom:{value.GetType()}\n\tTo:{typeof(TValue)}\n\tValue To String:{value?.ToString() ?? "NULL"}", e.InnerException ?? e);
      }

      if(Validations.Any()) {
        foreach((bool success, string message) in Validations?.Select(validator => { return validator.Value(this, convertedValue); })) {
          if(!success) {
            resultMessage = string.IsNullOrWhiteSpace(message)
              ? "Value did not pass custom validation functions."
              : message;

            return false;
          } else
            resultMessage = message ?? resultMessage;
        }
      }

      return true;
    }

    internal override void _runOnValueChangedCallbacks(DataField updatedField, object oldValue)
      => OnValueChangedListeners?.ForEach(listener => listener.Value(this, (TValue)oldValue));
  }
}
