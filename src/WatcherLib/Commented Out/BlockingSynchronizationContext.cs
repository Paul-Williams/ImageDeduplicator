// Moved to PW.Common

//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Threading;

//namespace ImageDeduper
//{
//  /// <summary> 
//  /// A SynchronizationContext which blocks and waits for completion.
//  /// Use: Thread A starts thread B and is blocked until thread B is complete or disposed.
//  /// </summary>
//  public sealed class BlockingSynchronizationContext : SynchronizationContext, IDisposable
//  {
//    /// <summary>The queue of work items.</summary>
//    private BlockingCollection<KeyValuePair<SendOrPostCallback, object>> Queue { get; } =
//        new BlockingCollection<KeyValuePair<SendOrPostCallback, object>>();

//    /// <summary>Dispatches an asynchronous message to the synchronization context.</summary>
//    /// <param name="d">The System.Threading.SendOrPostCallback delegate to call.</param>
//    /// <param name="state">The object passed to the delegate.</param>
//    public override void Post(SendOrPostCallback d, object state)
//    {
//      if (d == null) throw new ArgumentNullException(nameof(d));
//      Queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
//    }

//    /// <summary>Not supported.</summary>
//    public override void Send(SendOrPostCallback d, object state)
//    {
//      throw new NotSupportedException("Synchronously sending is not supported.");
//    }

//    /// <summary>Runs an loop to process all queued work items. Will block until cancelled via the <see cref="CancellationToken"/>.</summary>
//    public void WaitForCompletion(CancellationToken cancellationToken)
//    {
//      try
//      {
//      foreach (var workItem in Queue.GetConsumingEnumerable(cancellationToken))
//        workItem.Key(workItem.Value);
//      }
//      catch (OperationCanceledException)      {      }

//    }

//    /// <summary>Notifies the context that no more work will arrive.</summary>
//    public void Complete() { Queue.CompleteAdding();}

//    public void Dispose() => Queue.Dispose();
//  }


//}
