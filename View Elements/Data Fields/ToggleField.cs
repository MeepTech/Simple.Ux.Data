namespace Simple.Ux.Data {

  /// <summary>
  /// A boolean toggle specific Simple Ux Field.
  /// </summary>
  public class ToggleField : DataField<bool> {

    public ToggleField(
      string name,
      string tooltip = null, 
      bool value = false,
      string dataKey = null,
      bool isReadOnly = false
    ) : base(
      DisplayType.Toggle, 
      name, 
      tooltip,
      value,
      dataKey,
      isReadOnly
    ) {}
  }
}
