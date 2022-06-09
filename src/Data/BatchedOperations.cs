//using PW.FailFast;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;

//// See: http://www.ben-morris.com/optimising-bulk-inserts-with-entity-framework-6/

//namespace Data
//{
//  /// <summary>
//  /// Performs batched inserts and updates. Saves entities in batches of <see cref="BatchSize"/>. Automatically saves changes on <see cref="Dispose"/>.
//  /// </summary>
//  /// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
//  /// <typeparam name="TEntity">The type of the entities to be inserted or updated.</typeparam>
//  public class BulkOperations<TDbContext, TEntity> : IDisposable where TDbContext : DbContext, new() where TEntity : class
//  {
//    #region Constructors

//    /// <summary>
//    /// Creates a new <see cref="BulkOperations{TDbContext, TEntity}"/> instance using the default constructor for <typeparamref name="TDbContext"/>.
//    /// </summary>
//    public BulkOperations() : this(() => new TDbContext()) { }

//    /// <summary>
//    /// Creates a new instance of <see cref="BulkOperations{TDbContext, TEntity}"/>. 
//    /// Uses <paramref name="contextInitializer"/> as the initializer for <typeparamref name="TDbContext"/>.
//    /// </summary>
//    /// <param name="contextInitializer"><see cref="Func{TResult}"/> used to create new <see cref="TDbContext"/> instances.</param>
//    public BulkOperations(Func<TDbContext> contextInitializer)
//    {
//      ContextInitializer = contextInitializer;
//      InitializeContextObjects();
//    }
//    #endregion

//    #region Full-Property Backing Fields
//    private int _batchSize = 100;
//    private int _batchCount = 0;
//    #endregion

//    #region Private Properties

//    private Func<TDbContext> ContextInitializer { get; }
//    private TDbContext Context { get; set; }
//    private DbSet<TEntity> EntitySet { get; set; }

//    private int BatchCount
//    {
//      get => _batchCount;
//      set
//      {
//        if (value > BatchSize)
//        {
//          _batchCount = 1;
//          EntitySet = null;
//          SaveChanges();
//          Context.Dispose();
//          Context = null;
//          InitializeContextObjects();
//        }
//      }
//    }


//    #endregion

//    #region  Public Properties

//    public int TotalSaved { get; private set; }

//    /// <summary>
//    /// The number of entities to insert before saving the batch.
//    /// </summary>
//    /// <exception cref="ArgumentException">If value is less than 1.</exception>
//    public int BatchSize
//    {
//      get => _batchSize;
//      set => _batchSize = value > 0 ? value : throw new ArgumentException(nameof(BatchSize) + " must be greater than 0");
//    }

//    #endregion

//    #region Private Methods

//    private void InitializeContextObjects()
//    {
//      Context = ContextInitializer();
//      Context.Configuration.AutoDetectChangesEnabled = false;
//      EntitySet = Context.Set<TEntity>();
//    }

//    // Common functionality of both InsertRange and UpdateRange methods.
//    // Pass the required method as func.
//    private int EnumerableOperationCommon(IEnumerable<TEntity> entities, Func<TEntity, TEntity> func)
//    {
//      var count = 0;
//      foreach (var e in entities)
//      {
//        func(e);
//        count++;
//      }
//      //SaveChanges(); //[10/03/19] Not required. Saves are handled in batches, within func(), and on dispose. 
//      return count;
//    }

//    #endregion

//    #region Public Methods


//    #region Insert

//    /// <summary>
//    /// Adds <paramref name="entity"/> to the <see cref="DbContext"/>.
//    /// </summary>
//    /// <param name="entity"></param>
//    /// <returns></returns>
//    public TEntity Insert(TEntity entity)
//    {
//      BatchCount++;
//      return EntitySet.Add(entity);
//    }

//    /// <summary>
//    /// Adds <paramref name="entities"/> to the <see cref="DbContext"/>. Automatically saves changes.
//    /// </summary>
//    /// <param name="entities"></param>
//    /// <returns></returns>
//    public int Insert(IEnumerable<TEntity> entities)
//    {
//      return EnumerableOperationCommon(entities, Insert);
//    }

//    #endregion

//    #region Update

//    /// <summary>
//    /// Adds <paramref name="entity"/> to the <see cref="DbContext"/>.
//    /// </summary>
//    /// <param name="entity"></param>
//    /// <returns></returns>
//    public TEntity Update(TEntity entity)
//    {
//      BatchCount++;
//      Context.Entry(entity).State = EntityState.Modified;
//      return entity;

//      // entity = EntitySet.Attach(entity); // [10/03/19] Swiched to using Entry() method (above)
//      // [09/03/19] BUGFIX: Added this line - Entity was not previously getting saved.
//      // Context.Entry(entity).State = EntityState.Modified; 
//      // return entity;

//    }

//    /// <summary>
//    /// Adds <paramref name="entities"/> to the <see cref="DbContext"/>. Automatically saves changes.
//    /// </summary>
//    /// <param name="entities"></param>
//    /// <returns></returns>
//    public int Update(IEnumerable<TEntity> entities) => EnumerableOperationCommon(entities, Update);
//    #endregion

//    #region Delete

//    public TEntity Delete(TEntity entity)
//    {
//      BatchCount++;
//      Context.Entry(entity).State = EntityState.Deleted;
//      return entity;
//    }

//    public int Delete(IEnumerable<TEntity> entities) => EnumerableOperationCommon(entities, Delete);


//    #endregion


//    /// <summary>
//    /// Saves any outstanding changes to the database. Returns the number of entities saved on this save.
//    /// Use <see cref="TotalSaved"/> to get the total number of entities saved by this <see cref="BulkOperations{TDbContext, TEntity} "/> instance.
//    /// </summary>
//    /// <returns></returns>
//    public int SaveChanges()
//    {
//      var count = Context.SaveChanges();
//      TotalSaved += count;
//      BatchCount = 0;
//      return count;
//    }


//    #region Dispose
//    /// <summary>
//    /// Disposes the object and saves any outstanding entities.
//    /// </summary>
//    public void Dispose()
//    {
//      if (Context is TDbContext context)
//      {
//        context.SaveChanges();
//        context?.Dispose();
//      }

//    }
//    #endregion


//    #endregion

//  }
//}
