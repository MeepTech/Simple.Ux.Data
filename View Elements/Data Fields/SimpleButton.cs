namespace Simple.Ux.Data {

  /// <summary>
  /// Represents a simple clickable button in a ui.
  /// You can add more onClick callbacks via the OnValueChangeListeners callbacks.
  /// </summary>
  public class SimpleButton : DataField<bool> {

    /// <summary>
    /// Make a clickable UI button that does something on click.
    /// You can add more onClick callbacks via the OnValueChangedListeners callbacks.
    /// </summary>
    public SimpleButton(
      string name,
      string tooltip = null,
      string dataKey = null,
      bool isReadOnly = false
    ) : base(
      DisplayType.Button,
      name,
      tooltip,
      null, 
      dataKey,
      isReadOnly
    ) {}

    /// <summary>
    /// Used to update the colletction
    /// You can add more onClick callbacks via the OnValueChangedListeners callbacks.
    /// </summary>
    public void Click() {
      TryToSetValue(Value, out _);
    }
  }
}