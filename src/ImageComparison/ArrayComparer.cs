# nullable enable

using System;
using System.Collections.Generic;


// TODO: Investigate replacing this with build in sequential compare.

namespace XnaFan.ImageComparison
{
  /// <summary>
  /// Helper class for comparing arrays of equal length containing comparable items
  /// </summary>
  /// <typeparam name="T">The type of items to compare - must be IComparable</typeparam>
  internal class ArrayComparer<T> : IComparer<T[]> where T : IComparable
  {
    public int Compare(T[]? array1, T[]? array2)
    {
      if (array1 is null) return -1;
      if (array2 is null) return 1;

      if (array1.Length != array2.Length) throw new Exception("Array lengths must match.");

      for (int i = 0; i < array1.Length; i++)
      {
        var result = array1[i].CompareTo(array2[i]);
        if (result != 0) return result;
      }
      return 0;
    }

  }
}