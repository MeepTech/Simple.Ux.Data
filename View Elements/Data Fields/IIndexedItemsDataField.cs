namespace Simple.Ux.Data {

  /// <summary>
  /// A Data field that is an indexed collection of other fields.
  /// </summary>
  public interface IIndexedItemsDataField<TIndex> : IIndexedItemsDataField {

    /// <summary>
    /// Called after an individual field has validated itself.
    /// Used to update the internal Value at the key with the updated child field value.
    /// This should call RunValidationsOn.
    /// TODO: break this up into a few smaller functions like RunEntryValidationsOn()
    /// </summary>
    public bool TryToUpdateValueAtIndex(TIndex key, object newValue, out string resultMessage);
    bool IIndexedItemsDataField.TryToUpdateValueAtIndex(object key, object newValue, out string resultMessage)
      => TryToUpdateValueAtIndex((TIndex)key, newValue, out resultMessage);
  }

  /// <summary>
  /// A Data field that is an indexed collection of other fields.
  /// </summary>
  public interface IIndexedItemsDataField : IUxViewElement {

    /// <summary>
    /// Called after an individual field has validated itself.
    /// Used to update the internal Value at the key with the updated child field value.
    /// This should call RunValidationsOn.
    /// </summary>
    bool TryToUpdateValueAtIndex(object key, object newValue, out string resultMessage);
  }
}