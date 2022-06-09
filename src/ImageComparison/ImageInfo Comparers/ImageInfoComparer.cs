#nullable enable

using System.Collections.Generic;
using Data.Models;

namespace XnaFan.ImageComparison
{

  internal class ImageInfoComparer : IComparer<ImageEntity>
  {
    public static readonly ImageInfoComparer Instance = new();

    private static readonly ArrayComparer<byte> _arrayComparer = new();

    public int Compare(ImageEntity? x, ImageEntity? y) => _arrayComparer.Compare(x?.Bytes, y?.Bytes);
  }
}
