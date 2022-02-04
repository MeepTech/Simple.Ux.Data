using System;

namespace Simple.Ux.Data {

  /// <summary>
  /// Adds a tooltip to the field.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
  public class TooltipAttribute : Attribute {
    internal string _text;

    public TooltipAttribute(string text) {
      _text = text;
    }
  }
}