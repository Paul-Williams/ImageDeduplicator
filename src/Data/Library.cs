# nullable enable

using CommonServiceLocator;
using Prism.Events;

namespace Data
{
  public static class Library
  {
    // HACK
    private static readonly IEventAggregator _ea = ServiceLocator.Current.GetInstance<IEventAggregator>();


    public static T GetEvent<T>() where T : EventBase, new() => _ea.GetEvent<T>();
  }
}
