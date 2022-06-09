#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace PW.ImageDeduplicator.Common
{
  public static class MiscExtensions
  {
    public static T Last<T>(this T[] array) =>
      array != null ? array[^1] : throw new ArgumentNullException(nameof(array));

    public static T First<T>(this T[] array) =>
      array != null ? array[0] : throw new ArgumentNullException(nameof(array));





  }
}
