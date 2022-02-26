using Meep.Tech.Collections.Generic;
using Simple.Ux.Utility;
using System;
using System.Collections.Generic;

namespace Simple.Ux.Data {

  public abstract partial class DataField {

    /// <summary>
    /// How a data field should be displayed.
    /// Also contains the basic display types.
    /// </summary>
    public class DisplayType : Meep.Tech.Data.Enumeration<DisplayType> {
      internal readonly DefaultFieldConstructor _defaultFieldConstructor;
      internal static readonly Dictionary<System.Type, DisplayType> _byDefaultFieldTypes
        = new();

      /// <summary>
      /// A text field
      /// </summary>
      public static DisplayType Text {
        get;
      } = new DisplayType(nameof(Text), (
        string title,
        string tooltip,
        object value,
        bool isReadOnly,
        string dataKey,
        DelegateCollection < Func<DataField, View, bool> > enabledIf,
        DelegateCollection < Func<DataField, View, bool> > hiddenIf,
        DelegateCollection < Func<DataField, object, (bool success, string message)> > validations,
        DelegateCollection < Action<DataField, object> > onValueChanged,
        Dictionary < Type, Attribute > attributes
      ) => {
        if(isReadOnly) {
          return new ReadOnlyTextField(
            title: title,
            tooltip: tooltip,
            text: value,
            dataKey: dataKey
          ) {
            HideIfCheckers = hiddenIf
          };
        } else
          return new TextField(
            name: title,
            tooltip: tooltip,
            value: value,
            dataKey: dataKey
          ) {
            EnabledIfCheckers = enabledIf,
            HideIfCheckers = hiddenIf,
            Validations = validations?.ReDelegate(func => func.CastMiddleType<object, string>()),
            OnValueChangedListeners = onValueChanged?.ReDelegate(func => func.CastEndType<object, string>())
          };
      });

      /// <summary>
      /// A checkbox/toggle field for a boolean value
      /// </summary>
      public static DisplayType Toggle {
        get;
      } = new DisplayType(nameof(Toggle), (
         string title,
         string tooltip,
         object value,
         bool isReadOnly,
         string dataKey,
         DelegateCollection<Func<DataField, View, bool>> enabledIf,
         DelegateCollection<Func<DataField, View, bool>> hiddenIf,
         DelegateCollection<Func<DataField, object, (bool success, string message)>> validations,
         DelegateCollection<Action<DataField, object>> onValueChanged,
         Dictionary<Type, Attribute> attributes
       ) => {
         bool boolValue = value is bool asBool
            ? asBool
            : float.TryParse(value.ToString(), out float parsedAsFloat) && parsedAsFloat > 0;
         return new ToggleField(
           name: title,
           tooltip: tooltip,
           value: boolValue,
           dataKey: dataKey
         ) {
           EnabledIfCheckers = enabledIf,
           HideIfCheckers = hiddenIf,
           Validations = validations?.ReDelegate(func => func.CastMiddleType<object, bool>()),
           OnValueChangedListeners = onValueChanged?.ReDelegate(func => func.CastEndType<object, bool>())
         };
       });

      /// <summary>
      /// A slideable numeric input that's clamped by a max and min
      /// </summary>
      public static DisplayType RangeSlider {
        get;
      } = new DisplayType(nameof(RangeSlider), (
         string title,
         string tooltip,
         object value,
         bool isReadOnly,
         string dataKey,
         DelegateCollection<Func<DataField, View, bool>> enabledIf,
         DelegateCollection<Func<DataField, View, bool>> hiddenIf,
         DelegateCollection<Func<DataField, object, (bool success, string message)>> validations,
         DelegateCollection<Action<DataField, object>> onValueChanged,
         Dictionary<Type, Attribute> attributes
       ) => {
         RangeSliderAttribute rangeSliderAttribute
            = attributes.TryGetValue(typeof(RangeSliderAttribute), out var foundrsa)
              ? foundrsa as RangeSliderAttribute
              : null;

         bool clamped = rangeSliderAttribute?._isClampedToInt ?? false;
         (float min, float max)? minAndMax = rangeSliderAttribute is not null
            ? (rangeSliderAttribute._min, rangeSliderAttribute._max)
            : null;

         float? floatValue = value is double asFloat
            ? (float)asFloat
            : float.TryParse(value.ToString(), out float parsedFloat)
             ? parsedFloat
             : null;

         return new RangeSliderField(
           name: title,
           min: minAndMax?.min ?? 0,
           max: minAndMax?.max ?? 1,
           clampedToWholeNumbers: clamped,
           tooltip: tooltip,
           value: floatValue,
           dataKey: dataKey
         ) {
           EnabledIfCheckers = enabledIf,
           HideIfCheckers = hiddenIf,
           Validations = validations?.ReDelegate(func => func.CastMiddleType<object, double>()),
           OnValueChangedListeners = onValueChanged?.ReDelegate(func => func.CastEndType<object, double>())
         };
       });

      /// <summary>
      /// A dropdown with pre-set options
      /// </summary>
      public static DisplayType Dropdown {
        get;
      } = new DisplayType(nameof(Dropdown), (
         string title,
         string tooltip,
         object value,
         bool isReadOnly,
         string dataKey,
         DelegateCollection<Func<DataField, View, bool>> enabledIf,
         DelegateCollection<Func<DataField, View, bool>> hiddenIf,
         DelegateCollection<Func<DataField, object, (bool success, string message)>> validations,
         DelegateCollection<Action<DataField, object>> onValueChanged,
         Dictionary<Type, Attribute> attributes
       ) => {
         DropdownAttribute selectableData = attributes.TryGetValue(typeof(DropdownAttribute), out var found)
            ? found as DropdownAttribute
            : null;

         Dictionary<string, object> options = selectableData?._options;
         return new DropdownSelectField(
           name: title,
           options: options ?? throw new ArgumentNullException(nameof(options)),
           tooltip: tooltip,
           maxSelectableValues: selectableData?._selectLimit ?? 1,
           alreadySelectedOptionKeys: value as string[],
           dataKey: dataKey,
           isReadOnly: isReadOnly
         ) {
           EnabledIfCheckers = enabledIf,
           HideIfCheckers = hiddenIf,
           Validations = validations?.ReDelegate(func => func.CastMiddleType<object, List<KeyValuePair<string, object>>>()),
           OnValueChangedListeners = onValueChanged?.ReDelegate(func => func.CastEndType<object, List<KeyValuePair<string, object>>>())
         };
       });

      /// <summary>
      /// An expandable collection of other fields by numeric index
      /// </summary>
      public static DisplayType FieldList {
        get;
      } = new DisplayType(nameof(FieldList), (
         string title,
         string tooltip,
         object value,
         bool isReadOnly,
         string dataKey,
         DelegateCollection<Func<DataField, View, bool>> enabledIf,
         DelegateCollection<Func<DataField, View, bool>> hiddenIf,
         DelegateCollection<Func<DataField, object, (bool success, string message)>> validations,
         DelegateCollection<Action<DataField, object>> onValueChanged,
         Dictionary<Type, Attribute> attributes
       ) => {
         throw new NotImplementedException();
       });

      /// <summary>
      /// An expandable collection of other fields by string index
      /// </summary>
      public static DisplayType KeyValueFieldList {
        get;
      } = new DisplayType(nameof(KeyValueFieldList), (
         string title,
         string tooltip,
         object value,
         bool isReadOnly,
         string dataKey,
         DelegateCollection<Func<DataField, View, bool>> enabledIf,
         DelegateCollection<Func<DataField, View, bool>> hiddenIf,
         DelegateCollection<Func<DataField, object, (bool success, string message)>> validations,
         DelegateCollection<Action<DataField, object>> onValueChanged,
         Dictionary<Type, Attribute> attributes
       ) => {
         return new DataFieldKeyValueSet(
            name: title,
            rows: value as Dictionary<string, object>,
            tooltip: tooltip,
            dataKey: dataKey,
            childFieldAttributes: attributes.Values,
            isReadOnly: isReadOnly
          ) {
           EnabledIfCheckers = enabledIf,
           HideIfCheckers = hiddenIf,
           EntryValidations = validations?.ReDelegate(func => func.CastMiddleType<object, KeyValuePair<string, object>>()),
           OnValueChangedListeners = onValueChanged?.ReDelegate(func => func.CastEndType<object, OrderedDictionary<string, object>>())
         };
       });

      /// <summary>
      /// A simple button that can be clicked to do things.
      /// </summary>
      public static DisplayType Button {
        get;
      } = new DisplayType(nameof(Button), (
         string title,
         string tooltip,
         object value,
         bool isReadOnly,
         string dataKey,
         DelegateCollection<Func<DataField, View, bool>> enabledIf,
         DelegateCollection<Func<DataField, View, bool>> hiddenIf,
         DelegateCollection<Func<DataField, object, (bool success, string message)>> validations,
         DelegateCollection<Action<DataField, object>> onValueChanged,
         Dictionary<Type, Attribute> attributes
       ) => {
         throw new NotImplementedException();
       });

      /// <summary>
      /// Make a new display type
      /// </summary>
      public DisplayType(string nameId, DefaultFieldConstructor defaultFieldConstructor, IEnumerable<System.Type> defaultFieldTypes = null)
        : base(nameId) {
        _defaultFieldConstructor = defaultFieldConstructor;
        if(defaultFieldTypes is not null) {
          defaultFieldTypes?.ForEach(defaultFieldType => {
            try {
              _byDefaultFieldTypes.Add(defaultFieldType, this);
            } catch (ArgumentException e) {
              throw new ArgumentException($"The Default Field Type: {defaultFieldType} is already taken by SimpleUx Field: {_byDefaultFieldTypes[defaultFieldType]}", e);
            }
          });
        }
      }

      /// <summary>
      /// Used to construct a default field.
      /// Usually via attribute/reflection construction.
      /// </summary>
      public delegate DataField DefaultFieldConstructor(
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
      );
    }
  }
}
