#nullable enable

using PW.FailFast;
using System;
using System.Threading;

namespace ImageDeduper
{
  //See: https://blog.stephencleary.com/2009/08/gotchas-from-synchronizationcontext.html

  /// <summary>
  /// Provides extension methods for <see cref="SynchronizationContext"/>.
  /// </summary>
  public static class SynchronizationContextExtensions
  {
    /// <summary>
    /// Synchronously invokes a delegate by passing it to a <see cref="SynchronizationContext"/>, waiting for it to complete.
    /// </summary>
    /// <param name="synchronizationContext">The <see cref="SynchronizationContext"/> to pass the delegate to. May not be null.</param>
    /// <param name="action">The delegate to invoke. May not be null.</param>
    /// <remarks>
    /// <para>This method is guaranteed to not be reentrant.</para>
    /// </remarks>
    public static void SendOrInvoke(this SynchronizationContext synchronizationContext, Action action)
    {
      Guard.NotNull(action, nameof(action));

      // Added by me:
      if (synchronizationContext is null) action.Invoke();
      else
      {
        // The semantics of SynchronizationContext.Send allow it to invoke the delegate directly, but we can't allow that.
        Action forwardDelegate = () => synchronizationContext.Send((state) => action(), null);
        IAsyncResult result = forwardDelegate.BeginInvoke(null, null);
        result.AsyncWaitHandle.WaitOne();
      }
    }

    /// <summary>
    /// Asynchronously invokes a delegate by passing it to a <see cref="SynchronizationContext"/>, returning immediately.
    /// </summary>
    /// <param name="synchronizationContext">The <see cref="SynchronizationContext"/> to pass the delegate to. May not be null.</param>
    /// <param name="action">The delegate to invoke. May not be null.</param>
    /// <remarks>
    /// <para>This method is guaranteed to not be reentrant.</para>
    /// </remarks>
    public static void PostOrInvoke(this SynchronizationContext synchronizationContext, Action action)
    {
      Guard.NotNull(action, nameof(action));

      // Added by me:
      if (synchronizationContext is null) action.Invoke();
      else
      {
        // The semantics of SynchronizationContext.Post allow it to invoke the delegate directly, but we can't allow that.
        ThreadPool.QueueUserWorkItem((state) => synchronizationContext.Post((state2) => action(), null));
      }
    }
  }

}
