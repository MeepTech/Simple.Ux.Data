using Simple.Ux.Data;
using System;
using System.Text.RegularExpressions;

namespace Simple.Ux.Utility {

  public static class Extensions {

  internal static Func<DataField, TTo, (bool, string)> CastMiddleType<TFrom, TTo>(this Func<DataField, TFrom, (bool, string)> from)
    => (f, v) => from(f, v is TFrom fromType ? fromType : throw new Exception($"Cannot cast from {typeof(TTo).FullName}[TTo] to {typeof(TFrom).FullName}[TFrom]."));

  internal static Action<DataField, TTo> CastEndType<TFrom, TTo>(this Action<DataField, TFrom> from)
    => (f, v) => from(f, v is TFrom fromType ? fromType : throw new Exception($"Cannot cast from {typeof(TTo).FullName}[TTo] to {typeof(TFrom).FullName}[TFrom]."));

    /// <summary>
    /// Make a string from "CamelCase" to "Display Case"
    /// </summary>
    public static string ToDisplayCase(this string @string)
      => Regex.Replace(@string, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1").Trim('_').Replace("_", " ");
  }
}
