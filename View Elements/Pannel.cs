using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Simple.Ux.Data {

  /// <summary>
  /// Display data for a component.
  /// </summary>
  public partial class Pannel : IUxViewElement, IEnumerable<Column> {

    /// <summary>
    /// The tab data for this pannel
    /// </summary>
    public Pannel.Tab Key {
      get;
      internal set;
    }

    /// <summary>
    /// The view this field is in.
    /// </summary>
    public View View {
      get;
      internal set;
    }

    /// <summary>
    /// The fiels in this model, by key.
    /// </summary>
    public IReadOnlyList<Column> Elements {
      get;
      private set;
    }

    internal Pannel(IList<Column> orderedFields, Tab tab) {
      Elements = orderedFields?.ToList();
      Key = tab;
      tab.Pannel = this;
    }

    /// <summary>
    /// Copy this pannels UI scheme.
    /// </summary>
    public Pannel Copy(View toNewView = null) {
      Dictionary<string, DataField> copiedFields = new();
      Tab tab = Key;
      Pannel pannel = new(null, tab) {
        Elements = Elements.Select(element => element.Copy(toNewView)).ToList(),
        View = toNewView,
      };
      pannel.Key = tab;
      tab.Pannel = pannel;

      return pannel;
    }

    ///<summary><inheritdoc/></summary>
    IUxViewElement IUxViewElement.Copy(View toNewView)
      => Copy(toNewView);

    ///<summary><inheritdoc/></summary>
    public IEnumerator<Column> GetEnumerator()
      => Elements.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();
  }
}