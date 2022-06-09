# nullable enable

using System.Collections.Generic;

namespace XnaFan.ImageComparison
{
  internal static class ListExtensions
  {
    public static bool IsNotEmpty<T>(this List<T> list) => list.Count > 0;

    public static bool HasMoreThanOneItem<T>(this List<T> list) => list.Count > 1;
  }
}
