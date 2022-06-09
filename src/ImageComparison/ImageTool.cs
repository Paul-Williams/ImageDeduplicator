# nullable enable

using System.Collections.Generic;
using System.Linq;
using Data.Models;
using PW.Extensions;
using PW.FailFast;

namespace XnaFan.ImageComparison
{
  public static class ImageTool
  {

    //// This is test code to see if we can get more accurate when not sorting the list, which does not work properly.
    //// This is not practical. It will take hours, as is.
    //public static ImageEntity[][] CreateGroupsFromUnsortedList(List<ImageEntity> imageInfos, IImageMatchComparer comparer, System.Action doEvents)
    //{
    //  //var temp = imageInfos.Select(x => new { x.Id, x.Bytes, Skip = false });

    //  comparer = new ExactMatchComparer();

    //  var t = new List<ImageEntity>();

    //  for (int i = 0; i < imageInfos.Count; i++)
    //  {
    //    var imageI = imageInfos[i];
    //    for (int j = 0; j < imageInfos.Count; j++)
    //    {
    //      if (i == j) continue;
    //      var imageJ = imageInfos[j];

    //      if (comparer.Match(imageI, imageJ))
    //      {
    //        t.Add(imageI);
    //        t.Add(imageJ);            
    //      }
    //    }
    //    doEvents();
    //  }

    //  return new ImageEntity[][] { t.ToArray() };
    //}


    public static ImageEntity[][] CreateGroupsFromAlreadySortedList(List<ImageEntity> imageInfos, IImageMatchComparer comparer)
    {
      Guard.NotNull(imageInfos, nameof(imageInfos));
      Guard.NotNull(comparer, nameof(comparer));

      var imageInfoGroups = GroupDuplicates(imageInfos, comparer);
      var ImageInfoEntityGroups = new List<ImageEntity[]>();

      foreach (var list in imageInfoGroups)
        ImageInfoEntityGroups.Add(list.ToArray());

      return ImageInfoEntityGroups.ToArray();
    }

    private static List<List<ImageEntity>> GroupDuplicates(List<ImageEntity> sortedImageInfos, IImageMatchComparer comparer)
    {
      var groups = new List<List<ImageEntity>>();
      var currentGroup = new List<ImageEntity>();

      foreach (var ImageInfoEntity in sortedImageInfos)
      {
        if (currentGroup.IsNotEmpty() && !comparer.Match(currentGroup.First(), ImageInfoEntity))
        {
          if (currentGroup.HasMoreThanOneItem())
          {
            groups.Add(currentGroup);
            currentGroup = new List<ImageEntity>();
          }
          else currentGroup.Clear();
        }
        currentGroup.Add(ImageInfoEntity);
      }
      if (currentGroup.HasMoreThanOneItem()) { groups.Add(currentGroup); }
      return groups;
    }
  }
}