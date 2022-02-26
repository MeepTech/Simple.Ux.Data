using Meep.Tech.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Simple.Ux.Data {

  /// <summary>
  /// The base class for a data field that's actually a group/collection of fields.
  /// </summary>
  public abstract class CollectionOfDataFields<TIndividualItemIndex, TOverallFieldValue> : DataField<TOverallFieldValue>, IIndexedItemsDataField<TIndividualItemIndex> {

    /// <summary>
    /// Attributes that are applied to each child field.
    /// </summary>
    protected virtual IEnumerable<Attribute> ChildFieldAttributes {
      get;
      private set;
    }

    /// <summary>
    /// Extra validation to use on an individual entry whenever one is changed.
    /// </summary>
    public DelegateCollection<Func<DataField, KeyValuePair<TIndividualItemIndex, object>, (bool success, string message)>> EntryValidations {
      get => DefaultEntryValidations;
      init => value?.ForEach(DefaultEntryValidations.Add);
    } 
    
    /// <summary>
    /// The default entry validations for this type.
    /// </summary>
    protected virtual DelegateCollection<Func<DataField, KeyValuePair<TIndividualItemIndex, object>, (bool success, string message)>> DefaultEntryValidations {
      get;
      private set;
    } = new();

    ///<summary><inheritdoc/></summary>
    public abstract object this[TIndividualItemIndex key] {
      get;
      protected set;
    }

    ///<summary><inheritdoc/></summary>
    protected CollectionOfDataFields(DisplayType type, string name, string tooltip = null, object value = null, string dataKey = null, bool isReadOnly = false, IEnumerable<Attribute> childFieldAttributes = null)
      : base(type, name, tooltip, value, dataKey, isReadOnly) {
      ChildFieldAttributes = childFieldAttributes;
    }

    ///<summary><inheritdoc/></summary>
    public virtual bool TryToUpdateValueAtIndex(TIndividualItemIndex key, object newValue, out string resultMessage) {
      if(!RunEntryValidationsOn(key, newValue, out resultMessage)) {
        return false;
      }

      object oldValue = this[key];
      try {
        this[key] = newValue;
        if(!Validate(Value, out resultMessage)) {
          this[key] = oldValue;
          return false;
        }

        return true;
      } catch(Exception e) {
        this[key] = oldValue;
        resultMessage = e.Message;
        return false;
      }
    }

    /// <summary>
    /// used to run validations on an entry added to a collection field.
    /// </summary>
    protected virtual bool RunEntryValidationsOn(TIndividualItemIndex key, object newValue, out string resultMessage) {
      resultMessage = "Validated Entry Successfully";

      if(EntryValidations.Any()) {
        //Default func
        foreach((bool success, string message) in EntryValidations.Select(validator
          => validator.Value(this, new KeyValuePair<TIndividualItemIndex, object>(key, newValue)))
        ) {
          if(!success) {
            resultMessage = string.IsNullOrWhiteSpace(message)
              ? "Value did not pass custom entry validation functions."
              : message;

            return false;
          } else
            resultMessage = message ?? resultMessage;
        }
      }

      return true;
    }

    ///<summary><inheritdoc/></summary>
    public override DataField Copy(View toNewView = null, bool withCurrentValuesAsNewDefaults = false) {
      var value = base.Copy(toNewView, withCurrentValuesAsNewDefaults);
      (value as CollectionOfDataFields<TIndividualItemIndex, TOverallFieldValue>).ChildFieldAttributes = ChildFieldAttributes;
      (value as CollectionOfDataFields<TIndividualItemIndex, TOverallFieldValue>).DefaultEntryValidations = new(EntryValidations);

      return value;
    }

    /// <summary>
    /// try to remove the last item from the collection
    /// </summary>
    public bool TryToRemoveLastEntry(out string resultMessage) {
      throw new NotImplementedException();
    }

    /// <summary>
    /// remove the item at the collection index
    /// </summary>
    public bool TryToRemoveEntryAt(string key, out string resultMessage) {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Try to add a new entry to the end of this list.
    /// </summary>
    public bool TryToAppendNewEntry(string key, object value, out string resultMessage) {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Try to add a new empty entry to the end of this list.
    /// </summary>
    public bool TryToAppendNewEmptyEntry(out string resultMessage) {
      throw new NotImplementedException();
    }

    ///<summary>
    /// Implimentation needed to complete to Copy function.
    ///</summary>
    protected abstract DataField _copyValueAndDefaults(View toNewView = null, bool withCurrentValuesAsNewDefaults = false);
  }
}