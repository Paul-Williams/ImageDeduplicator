# nullable enable

using Prism.Events;

namespace PubSubEvents.DatabaseEvents
{
  public class ImageRenamedEvent : PubSubEvent<ImageDeduper.FileRenamePair> { }

}
