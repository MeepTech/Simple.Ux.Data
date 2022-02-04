using Meep.Tech.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Simple.Ux.Data {

  /// <summary>
  /// Represents a key value set in a ui
  /// </summary>
  public class DataFieldKeyValueSet : CollectionOfDataFields<string, OrderedDictionary<string, object>>, IReadOnlyDictionary<string, object> {

    ///<summary><inheritdoc/></summary>
    public ICollection<string> Keys 
      => ((IDictionary<string, object>)Value).Keys;
    IEnumerable<string> IReadOnlyDictionary<string, object>.Keys 
      => Keys;

    ///<summary><inheritdoc/></summary>
    public ICollection<object> Values 
      => ((IDictionary<string, object>)Value).Values;
    IEnumerable<object> IReadOnlyDictionary<string, object>.Values 
      => Values;

    ///<summary><inheritdoc/></summary>
    public int Count
      => ((ICollection<KeyValuePair<string, object>>)Value).Count;

    /// <summary>
    /// Make a key value set to display in a ux.
    /// </summary>
    /// <param name="childFieldAttributes">Add attributes to each generated child input</param>
    public DataFieldKeyValueSet(
      string name,
      string tooltip = null,
      Dictionary<string, object> rows = null,
      IEnumerable<Attribute> childFieldAttributes = null,
      string dataKey = null,
      bool isReadOnly = false
    ) : base(
      DisplayType.KeyValueFieldList,
      name,
      tooltip,
      rows, 
      dataKey,
      isReadOnly,
      childFieldAttributes
    ) { }

    ///<summary><inheritdoc/></summary>
    public override object this[string key] {
      get => Value[key];
      protected set => Value[key] = value;
    }

    ///<summary><inheritdoc/></summary>
    protected override DataField _copyValueAndDefaults(View toNewView = null, bool withCurrentValuesAsNewDefaults = false) {
      var value = base.Copy(toNewView, withCurrentValuesAsNewDefaults);
      value.Value = new Dictionary<string, object>(Value);
      value.DefaultValue = withCurrentValuesAsNewDefaults ? new Dictionary<string, object>(Value) : new Dictionary<string, object>((IDictionary<string, object>)DefaultValue);

      return value;
    }

    #region IReadonly Dictionary

    ///<summary><inheritdoc/></summary>
    public bool ContainsKey(string key) {
      return ((IDictionary<string, object>)Value).ContainsKey(key);
    }

    ///<summary><inheritdoc/></summary>
    public bool TryGetValue(string key, out object value) {
      return ((IDictionary<string, object>)Value).TryGetValue(key, out value);
    }

    ///<summary><inheritdoc/></summary>
    public bool Contains(KeyValuePair<string, object> item) {
      return ((ICollection<KeyValuePair<string, object>>)Value).Contains(item);
    }

    ///<summary><inheritdoc/></summary>
    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) {
      ((ICollection<KeyValuePair<string, object>>)Value).CopyTo(array, arrayIndex);
    }
    
    ///<summary><inheritdoc/></summary>
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
      return ((IEnumerable<KeyValuePair<string, object>>)Value).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return ((IEnumerable)Value).GetEnumerator();
    }

    #endregion
  }
}