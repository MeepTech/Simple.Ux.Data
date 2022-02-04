using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Simple.Ux.Data {

  /// <summary>
  /// A column in a simple Ux.
  /// A pannel can have up to 3 columns, but 3 is the reccomended.
  /// Columns can have labels at the top.
  /// Columns cannot contain other columns, but Columns can contain rows.
  /// </summary>
  public class Column : IUxViewElement, IEnumerable<IUxViewElement> {

    /// <summary>
    /// The view this field is in.
    /// </summary>
    public View View {
      get;
      internal set;
    }

    List<IUxViewElement> _elements;

    /// <summary>
    /// The label for this row.
    /// </summary>
    public Title Title {
      get;
    }

    internal Column(IEnumerable<IUxViewElement> elements, Title label) {
      _elements = elements.Select(element => {
        return element is Column
          ? throw new System.Exception($"Cannot place a Simple Ux Column inside another column.")
          : element is Pannel
            ? throw new System.Exception($"Cannot add a Pannel to a Simple Ux Column")
            : element;
      }).ToList();
      Title = label;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerator<IUxViewElement> GetEnumerator() {
      return ((IEnumerable<IUxViewElement>)_elements).GetEnumerator();
    }

    ///<summary><inheritdoc/></summary>
    IEnumerator IEnumerable.GetEnumerator() {
      return ((IEnumerable)_elements).GetEnumerator();
    }

    /// <summary>
    /// Copy this column and it's contents
    /// </summary>
    public Column Copy(View toNewView = null) 
      => new(_elements.Select(element => element.Copy(toNewView)), Title);

    ///<summary><inheritdoc/></summary>
    IUxViewElement IUxViewElement.Copy(View toNewView)
      => Copy(toNewView);
  }
}
