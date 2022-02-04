namespace Simple.Ux.Data {
  /// <summary>
  /// A title that takes up it's own row, or can be added to a row or column to prefix it.
  /// </summary>
  public class Title : IUxViewElement {

    /// <summary>
    /// The view this title is in.
    /// </summary>
    public View View {
      get;
      internal set;
    }

    /// <summary>
    /// Title Size
    /// </summary>
    public enum FontSize {
      Small,
      Medium,
      Large
    }

    /// <summary>
    /// The tile text
    /// </summary>
    public string Text;

    /// <summary>
    /// The tile tooltip
    /// </summary>
    public string Tooltip;

    /// <summary>
    /// The title size
    /// </summary>
    public FontSize Size {
      get;
      private set;
    }

    /// <summary>
    /// Make a title for a UX.
    /// </summary>
    public Title(string label, string labelTooltip = null) {
      Size = FontSize.Medium;
      Tooltip = labelTooltip;
      Text = label;
    }

    ///<summary><inheritdoc/></summary>
    public Title Copy(View toNewView = null)
      => new(Text, Tooltip) {
        View = toNewView,
        Size = Size
      };

    IUxViewElement IUxViewElement.Copy(View toNewView)
      => Copy();
  }
}
