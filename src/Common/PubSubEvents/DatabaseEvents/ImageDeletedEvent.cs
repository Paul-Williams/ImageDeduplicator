# nullable enable

using Prism.Events;
using PW.IO.FileSystemObjects;

namespace PubSubEvents.DatabaseEvents
{
  public class ImageDeletedEvent : PubSubEvent<FilePath> { }


}
