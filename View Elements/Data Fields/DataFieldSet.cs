using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Simple.Ux.Data {

  /// <summary>
  /// Represents a key value set in a ui
  /// </summary>
  public class DataFieldSet : CollectionOfDataFields<int, ArrayList> {

    /// <summary>
    /// The type of data this set holds.
    /// </summary>
    public System.Type DataType {
      get;
    }

    ///<summary><inheritdoc/></summary>
    public override object this[int key] { 
      get => Value[key]; 
      protected set => Value[key] = value; 
    }

    /// <summary>
    /// Make a key value set to display in a ux.
    /// </summary>
    /// <param name="dataType">The type of data the list will accept</param>
    /// <param name="childFieldAttributes">Add attributes to each generated child input</param>
    /// <param name="rowValues">The default/current list values</param>
    public DataFieldSet(
      string name,
      System.Type dataType,
      IEnumerable<Attribute> childFieldAttributes = null,
      string tooltip = null,
      ArrayList rowValues = null,
      string dataKey = null,
      bool isReadOnly = false
    ) : base(
      DataFieldSet.DisplayType.FieldList,
      name,
      tooltip,
      rowValues, 
      dataKey,
      isReadOnly
    ) {
      DataType = dataType;
    }

    ///<summary><inheritdoc/></summary>
    protected override DataField _copyValueAndDefaults(View toNewView = null, bool withCurrentValuesAsNewDefaults = false) {
      var value = base.Copy(toNewView, withCurrentValuesAsNewDefaults);
      value.Value = new ArrayList((Value as ArrayList).Cast<object>().ToList());
      value.DefaultValue = withCurrentValuesAsNewDefaults
        ? new ArrayList(Value as ArrayList)
        : new ArrayList(DefaultValue as ArrayList);

      return value;
    }
  }
}