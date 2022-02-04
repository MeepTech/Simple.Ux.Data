using System;

namespace Simple.Ux.Data {
  /// <summary>
  /// Denotes a method that should be auto rendered as a button.
  /// A valid method takes either no parameters, or a SimpleButton(the one clicked on) and a Pannel as parameters.
  /// TODO: impliment
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
  public class SimpleButtonAttribute : Attribute {
    internal string _buttonTitle;
    internal string _buttonKey;

    /// <summary>
    /// Make a simple button out of a method, with an overrideable display name and key.
    /// </summary>
    public SimpleButtonAttribute(string Name = null, string Key = null) {
      _buttonTitle = Name;
      _buttonKey = Key;
    }
  }
}