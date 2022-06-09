# nullable enable

using Prism.Events;

namespace PubSubEvents
{
  public class FilesDeletedEvent : PubSubEvent<PW.IO.FileSystemObjects.FilePath[]> { }

}
