using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Simple.Ux.Data {

  /// <summary>
  /// A Row of UX items.
  /// Can't contain columns or other rows.
  /// Can have a label.
  /// </summary>
  public class Row : IUxViewElement, IEnumerable<DataField> {

    /// <summary>
    /// The view this field is in.
    /// </summary>
    public View View {
      get;
      internal set;
    }

    /// <summary>
    /// The label for this row.
    /// </summary>
    public Title Label { 
      get;
    }

    /// <summary>
    /// Info tooltip for the row label
    /// </summary>
    public virtual string LabelTooltip {
      get;
    } = null;

    List<DataField> _elements;

    internal Row(IEnumerable<DataField> elements, Title label) {
      _elements = elements.Select(e =>
        e is not DataFieldKeyValueSet 
          ? e 
          : throw new System.ArgumentException($"Cannot add a key value list to a simple Ux Row.")
      ).ToList();
      Label = label;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerator<DataField> GetEnumerator() {
      return ((IEnumerable<DataField>)_elements).GetEnumerator();
    }

    ///<summary><inheritdoc/></summary>
    IEnumerator IEnumerable.GetEnumerator() {
      return ((IEnumerable)_elements).GetEnumerator();
    }

    /// <summary>
    /// Copy this row and it's contents
    /// </summary>
    public Row Copy(View toNewView = null)
      => new(_elements.Select(element => element.Copy(toNewView)), Label);

    ///<summary><inheritdoc/></summary>
    IUxViewElement IUxViewElement.Copy(View toNewView)
      => Copy(toNewView);
  }
}
