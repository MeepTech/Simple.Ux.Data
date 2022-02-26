namespace Simple.Ux.Data {

  /// <summary>
  /// Used to space contents with an empty space, as if a title or minimum height field was present.
  /// </summary>
  public struct EmptySpacer : IUxViewElement {

    /// <summary>
    /// The view this field is in.
    /// </summary>
    public View View {
      get;
      internal set;
    }

    public IUxViewElement Copy(View toNewView = null)
      => new EmptySpacer {
        View = toNewView
      };
  }
}