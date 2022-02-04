using System;

namespace Simple.Ux.Data {

  /// <summary>
  /// A Text specific Simple Ux Field.
  /// Used just to display text
  /// </summary>
  public class ReadOnlyTextField : TextField {

    public ReadOnlyTextField(
      object text,
      string title = null,
      string tooltip = null,
      string dataKey = null
    ) : base(
      title,
      " ",
      tooltip,
      text,
      dataKey
        ?? (!string.IsNullOrWhiteSpace(title)
          ? title
          : Guid.NewGuid().ToString()),
      true
    ) {
    }
  }
}
