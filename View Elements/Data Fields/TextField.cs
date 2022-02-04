namespace Simple.Ux.Data {

  /// <summary>
  /// A Text specific Simple Ux Field.
  /// Can be used for input, or Read Only for just display text.
  /// </summary>
  public class TextField : DataField<string> {

    /// <summary>
    /// Placeholder text for the input
    /// </summary>
    public string PlaceholderText {
      get;
    }

    public TextField(
      string name,
      string placeholderText = null,
      string tooltip = null,
      object value = null,
      string dataKey = null,
      bool isReadOnly = false
    ) : base(
      DisplayType.Text,
      name,
      tooltip,
      value ?? "",
      dataKey,
      isReadOnly
    ) {
      PlaceholderText = placeholderText;
    }
  }
}
