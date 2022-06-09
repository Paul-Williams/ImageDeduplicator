//# nullable enable

//using System;
//using System.IO;
//using System.Runtime.Caching;
//using System.Threading;
//using PW.IO;

///*  Notes:
// *  This class is currently using MemoryCache class. The issue with this is that the MemoryCache expiry timer only runs every 20 seconds.
// *  As such, it gives wildly varying expiry times, for items in that cache.
// *  Reading around I found an alternative to MemoryCache, 'Microsoft.Extensions.Caching.Memory' (https://www.nuget.org/packages/Microsoft.Extensions.Caching.Memory)
// *  In the links below, the 1st gives the suggestion for the alternate class. 
// *  The 2nd link explains and gives example usage, to get around and the fact that the new class does not use a timer for eviction.
// * https://stackoverflow.com/questions/12630168/memorycache-absoluteexpiration-acting-strange
// * https://stackoverflow.com/questions/42535408/net-core-memorycache-postevictioncallback-not-working-properly/47949111#47949111
// * 
// * An alternate approach suggested in the following link hacks the internals of the existing MemoryCache class to change the timer frequency (_tsPerBucket).
// */


//namespace ImageDeduper
//{
//  internal class FileEventDelayingFilter
//  {
//    private SynchronizationContext SynchronizationContext { get; }

//    public event FileSystemEventHandler? NewEvent;

//    /// <summary>
//    /// The minimum time the event will be delayed. NB: This is queued events are checked every 20sec via a timer. So this is very rough.
//    /// see: http://benhall.io/category/c-net/dotnetmemorycache/
//    /// </summary>
//    public TimeSpan Delay { get; set; } = TimeSpan.FromSeconds(5);

//    public int MaxRetries { get; set; } = 3;

//    public FileEventDelayingFilter(SynchronizationContext synchronizationContext)
//    {
//      SynchronizationContext = synchronizationContext;
//      //TraceHelper.WriteLineThreadId("WatcherEventDelayingFilter.ctor(SynchronizationContext)");
//    }


//    private CacheItemPolicy CreateCacheItemPolicy() =>
//      new CacheItemPolicy
//      {
//        RemovedCallback = OnRemovedFromCache,
//        AbsoluteExpiration = DateTimeOffset.Now.Add(Delay)
//      };


//    // Handle cache item expiring 
//    private void OnRemovedFromCache(CacheEntryRemovedArguments args)
//    {
//      TraceHelper.WriteLineThreadId();
//      //// Checking if expired, for a bit of future-proofing (NOT MY CODE)
//      //if (args.RemovedReason != CacheEntryRemovedReason.Expired) return;

//      var retryable = (Retryable<FileSystemEventArgs>)args.CacheItem.Value;

//      //26/04/19: Change: Attempt to fix bug #30 'FileNotFoundException'
//      if (new FileInfo(retryable.Value.FullPath).IsReadLocked())
//      {
//        if (retryable.Retries++ <= MaxRetries) MemoryCache.Default.Add(retryable.Value.Name, retryable, CreateCacheItemPolicy());
//        // ISSUE 34: Handle case where we are out of retries -- Currently the object will be silently lost.
//      }
//      else
//      {
//        // If we got here then the event can now be handled as it would normally have been in the watcher event handler.
//        // Just gonna spoof a normal FileSystemEventHandler event
//        SyncPost(retryable.Value);
//      }

//    }

//    /// <summary>
//    /// Posts the <see cref="FileSystemEventArgs"/> back to the main thread.
//    /// </summary>
//    private void SyncPost(FileSystemEventArgs ea)
//    {
//      if (NewEvent is FileSystemEventHandler local) SynchronizationContext.PostOrInvoke(() => local.Invoke(this, ea));
//    }


//    /// <summary>
//    /// Add file event to cache (won't add if already there so assured of only one occurrence) 
//    /// </summary>
//    /// <param name="e"></param>
//    public void Add(FileSystemEventArgs e)
//    {
//      MemoryCache.Default.AddOrGetExisting(e.Name, new Retryable<FileSystemEventArgs>(e), CreateCacheItemPolicy());
//    }

//  }
//}
