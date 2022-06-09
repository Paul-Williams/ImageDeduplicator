# nullable enable

using CSharpFunctionalExtensions;
using PW.IO.FileSystemObjects;
using System.Collections.Generic;

namespace PubSubEvents.DatabaseEvents
{
  public class DirectoryRenamedEventArgs : ValueObject
  {
    public DirectoryRenamedEventArgs(DirectoryPath oldPath, DirectoryPath newPath, int imagesUpdated)
    {
      OldPath = oldPath;
      NewPath = newPath;
      ImagesUpdated = imagesUpdated;
    }

    public DirectoryPath OldPath { get; }
    public DirectoryPath NewPath { get; }
    public int ImagesUpdated { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return ImagesUpdated;
      yield return OldPath;
      yield return NewPath;
    }
  }


  //public class ImageAddedEventEventArgs
  //{
  //  ImageAddedEventEventArgs(int entityId, string imagePath)
  //  {
  //    EntityId = entityId;
  //    ImagePath = imagePath;
  //  }

  //  public int EntityId { get; }
  //  public string ImagePath { get; }
  //}



}
