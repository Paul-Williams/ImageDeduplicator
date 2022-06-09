# nullable enable

using CSharpFunctionalExtensions;
using PW.IO.FileSystemObjects;
using System.Collections.Generic;

namespace PubSubEvents.DatabaseEvents
{
  /// <summary>
  /// Payload of the ImageAddedEvent
  /// </summary>
  public class ImageAddedEventArgs : ValueObject
  {
    public ImageAddedEventArgs(int EntityId, FilePath FilePath)
    {
      this.EntityId = EntityId;
      this.FilePath = FilePath;
    }

    /// <summary>
    /// Id of the entity in the database.
    /// </summary>
    public int EntityId { get; }

    /// <summary>
    /// Path to the file on disk.
    /// </summary>
    public FilePath FilePath { get; }


    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return EntityId;
      yield return FilePath;
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
