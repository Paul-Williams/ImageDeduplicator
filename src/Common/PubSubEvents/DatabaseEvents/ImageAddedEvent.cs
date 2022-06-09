# nullable enable

using Prism.Events;

namespace PubSubEvents.DatabaseEvents
{
  /// <summary>
  /// Raised when an image is added to the database.
  /// </summary>
  public class ImageAddedEvent : PubSubEvent<ImageAddedEventArgs> { }


}
