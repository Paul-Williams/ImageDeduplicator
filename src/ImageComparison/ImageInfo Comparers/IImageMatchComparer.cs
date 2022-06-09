# nullable enable

using Data.Models;

namespace XnaFan.ImageComparison
{
  public interface IImageMatchComparer
  {
    bool Match(ImageEntity ii1, ImageEntity ii2);
  }
}
