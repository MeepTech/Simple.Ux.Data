namespace Simple.Ux.Data {

  public partial class Pannel {

    /// <summary>
    /// A tab used to switch between pannels of a simple ux.
    /// </summary>
    public struct Tab : IUxViewElement {

      /// <summary>
      /// The view this is attached to
      /// </summary>
      public View View {
        get;
        internal set;
      }

      /// <summary>
      /// The pannel for this tab:
      /// </summary>
      public Pannel Pannel {
        get;
        internal set;
      }

      /// <summary>
      /// Tab optional key, name is used by default
      /// </summary>
      public readonly string Key;

      /// <summary>
      /// Tab display name
      /// </summary>
      public readonly string Name;

      /// <summary>
      /// Optional tab tooltip
      /// </summary>
      public readonly string Tooltip;

      /// <summary>
      /// If you want an icon, an can be placed in your mod package containing this Ux Pannel, and the url after mods/$PackageName$/ should go here.
      /// </summary>
      public readonly string ImageLocationWithinModPackageFolder;

      /// <summary>
      /// Make a new tab for a pannel
      /// </summary>
      public Tab(string name, string key = null, string tooltip = null, string imageLocationWithinModPackageFolder = null) {
        Name = name;
        Key = key ?? name;
        Tooltip = tooltip;
        ImageLocationWithinModPackageFolder = imageLocationWithinModPackageFolder;
        View = null;
        Pannel = null;
      }

      /// <summary>
      /// Copy this tab.
      /// </summary>
      public Tab Copy(View toNewView = null)
        => new(Name, Key, Tooltip, ImageLocationWithinModPackageFolder) {
          View = toNewView
        };

      ///<summary><inheritdoc/></summary>
      public override int GetHashCode() {
        return Key.GetHashCode();
      }

      IUxViewElement IUxViewElement.Copy(View toNewView)
        => Copy(toNewView);
    }
  }
}