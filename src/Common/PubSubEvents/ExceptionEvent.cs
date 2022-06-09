# nullable enable

using Prism.Events;

namespace PubSubEvents
{

  public class ExceptionEvent : PubSubEvent<System.Exception> { }

}
