//# nullable enable

//using Data;
//using Data.Models;
//using PW.Data.EntityFramework;
//using System;
//using System.Data.Entity;


//// This seems a bit of a mess !!


//namespace ImageDeduper
//{

//  /// <summary>
//  /// DataContext for reads, BatchedOperations for writes.
//  /// NB: Do not make changes to DataContext. Only changes through BatchedOperations will get saved.
//  /// </summary>
//  internal class DataContexts : IDisposable
//  {
//    //private readonly static StatusInfoEvent info = CommonServiceLocator.ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<StatusInfoEvent>(); //HACK: Get it working

//    public DataContext Context { get; }

//    public BatchedOperations<DataContext, ImageEntity> Batch { get; }

//    public DbSet<ImageEntity> SavedImages => Context.Images;

//    public DataContexts()
//    {
//      Context = new DataContext();
//      Batch = new BatchedOperations<DataContext, ImageEntity>(() => new DataContext());
//    }


//    /// <summary>
//    /// Only to be used by Dispose method.
//    /// </summary>
//    private bool IsDisposed { get; set; } = false;


//    public void Dispose()
//    {
//      if (IsDisposed) return;
//      IsDisposed = true;

//      Context.Dispose();
//      Batch.SaveChanges();
//      Batch.Dispose();

//    }
//  }
//}
