#nullable enable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ImageDeduper
{
  internal static class BlockingCollectionExtensions
  {
    public static List<T> TakeAll<T>(this BlockingCollection<T> collection)
    {
      var r = new List<T>();
      while (collection.TryTake(out var item)) r.Add(item);
      return r;
    }


  }
}
