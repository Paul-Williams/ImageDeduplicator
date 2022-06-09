# nullable enable

using PW.FailFast;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;

/* See: 
 * https://romiller.com/2014/04/08/ef6-1-mapping-between-types-tables/
 * https://romiller.com/2014/10/07/ef6-1-getting-key-properties-for-an-entity/
 * https://romiller.com/2015/08/05/ef6-1-get-mapping-between-properties-and-columns/
 */
namespace Data
{
  public static class DbContextMetaDataExtensions
  {

    /// <summary>
    /// Lookup cache used by <see cref="EntitySetMapping(DbContext, Type)"/>.
    /// The key is the |-separated composite of the DbContext type and the entity type.
    /// </summary>
    private static Dictionary<string, EntitySetMapping> EntitySetMappingCache { get; } = new Dictionary<string, EntitySetMapping>(StringComparer.Ordinal);

    private static string GenerateCacheKey(DbContext context, Type entity) => context.GetType().FullName + "|" + entity.FullName;


    /// <summary>
    /// Generic version of <see cref="EntityTableName(DbContext, Type)"/>.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string EntityTableName<TEntity>(this DbContext context) where TEntity : class =>
      context.EntityTableName(typeof(TEntity));

    /// <summary>
    /// Returns the table name for a given type.
    /// </summary>
    public static string EntityTableName(this DbContext context, Type type)
    {
      Guard.NotNull(context, nameof(context));
      Guard.NotNull(type, nameof(type));

      //var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

      //// Get the part of the model that contains info about the actual CLR types
      //var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

      //// Get the entity type from the model that maps to the CLR type
      //var entityType = metadata
      //        .GetItems<EntityType>(DataSpace.OSpace)
      //        .Single(e => objectItemCollection.GetClrType(e) == type);

      //// Get the entity set that uses this entity type
      //var entitySet = metadata
      //    .GetItems<EntityContainer>(DataSpace.CSpace)
      //    .Single()
      //    .EntitySets
      //    .Single(s => s.ElementType.Name == entityType.Name);

      //// Find the mapping between conceptual and storage model for this entity set
      //var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
      //        .Single()
      //        .EntitySetMappings
      //        .Single(s => s.EntitySet == entitySet);


      var mapping = context.EntitySetMapping(type);

      // Find the storage entity set (table) that the entity is mapped
      var table = mapping
          .EntityTypeMappings.Single()
          .Fragments.Single()
          .StoreEntitySet;

      // Return the table name from the storage entity set
      return (string)table.MetadataProperties["Table"].Value ?? table.Name;
    }

    public static IEnumerable<string> EntityTableNames<TEntity>(this DbContext context) where TEntity : class =>
      context.EntityTableNames(typeof(TEntity));

    /// <summary>
    /// Returns the table names for an type whose entity is split across multiple tables.
    /// </summary>
    public static IEnumerable<string> EntityTableNames(this DbContext context, Type type)
    {
      Guard.NotNull(context, nameof(context));
      Guard.NotNull(type, nameof(type));

      //var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

      //// Get the part of the model that contains info about the actual CLR types
      //var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

      //// Get the entity type from the model that maps to the CLR type
      //var entityType = metadata
      //        .GetItems<EntityType>(DataSpace.OSpace)
      //        .Single(e => objectItemCollection.GetClrType(e) == type);

      //// Get the entity set that uses this entity type
      //var entitySet = metadata
      //    .GetItems<EntityContainer>(DataSpace.CSpace)
      //    .Single()
      //    .EntitySets
      //    .Single(s => s.ElementType.Name == entityType.Name);

      //// Find the mapping between conceptual and storage model for this entity set
      //var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
      //        .Single()
      //        .EntitySetMappings
      //        .Single(s => s.EntitySet == entitySet);

      var mapping = context.EntitySetMapping(type);

      // Find the storage entity sets (tables) that the entity is mapped
      var tables = mapping
          .EntityTypeMappings.Single()
          .Fragments;

      // Return the table name from the storage entity set
      return tables.Select(f => (string)f.StoreEntitySet.MetadataProperties["Table"].Value ?? f.StoreEntitySet.Name);
    }

    /// <summary>
    /// Common functionality of the GetTableName methods
    /// </summary>
    /// <param name="context"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private static EntitySetMapping EntitySetMapping(this DbContext context, Type type)
    {
      // First attempt to retrieve the type's mapping from the cache
      var cacheKey = GenerateCacheKey(context, type);
      if (EntitySetMappingCache.TryGetValue(cacheKey, out var cachedMapping)) return cachedMapping;

      var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

      // Get the part of the model that contains info about the actual CLR types
      var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

      // Get the entity type from the model that maps to the CLR type
      var entityType = metadata
              .GetItems<EntityType>(DataSpace.OSpace)
              .SingleOrDefault(e => objectItemCollection.GetClrType(e) == type);

      if (entityType is null) throw new InvalidOperationException($"The DbContext '{context.GetType().FullName}' does not contain a mapping for entity type: '{type.FullName}'.");

      // Get the entity set that uses this entity type
      var entitySet = metadata
          .GetItems<EntityContainer>(DataSpace.CSpace)
          .Single()
          .EntitySets
          .Single(s => s.ElementType.Name == entityType.Name);

      // Find the mapping between conceptual and storage model for this entity set
      var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
        .Single()
        .EntitySetMappings
        .Single(s => s.EntitySet == entitySet);

      // Add the mapping to the cache
      EntitySetMappingCache.Add(cacheKey, mapping);

      return mapping;
    }

    /// <summary>
    /// Generic version of <see cref="EntityKeyNames(DbContext, Type)"/>.
    /// </summary>
    public static string[] EntityKeyNames<TEntity>(this DbContext context) where TEntity : class =>
      context.EntityKeyNames(typeof(TEntity));


    /// <summary>
    /// Returns the names of key properties of a given entity type.
    /// </summary>
    public static string[] EntityKeyNames(this DbContext context, Type entityType)
    {
      Guard.NotNull(context, nameof(context));
      Guard.NotNull(entityType, nameof(entityType));

      var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

      // Get the mapping between CLR types and metadata OSpace
      var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

      // Get metadata for given CLR type
      var entityMetadata = metadata
              .GetItems<EntityType>(DataSpace.OSpace)
              .Single(e => objectItemCollection.GetClrType(e) == entityType);

      return entityMetadata.KeyProperties.Select(p => p.Name).ToArray();
    }

    public static string GetColumnName(this DbContext context, Type type, string propertyName)
    {
      Guard.NotNull(context, nameof(context));
      Guard.NotNull(type, nameof(type));
      Guard.NotNull(propertyName, nameof(propertyName));

      //var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

      //// Get the part of the model that contains info about the actual CLR types
      //var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

      //// Get the entity type from the model that maps to the CLR type
      //var entityType = metadata
      //        .GetItems & lt; EntityType & gt; (DataSpace.OSpace)
      //              .Single(e = &gt; objectItemCollection.GetClrType(e) == type);

      //// Get the entity set that uses this entity type
      //var entitySet = metadata
      //    .GetItems & lt; EntityContainer & gt; (DataSpace.CSpace)
      //          .Single()
      //          .EntitySets
      //          .Single(s = &gt; s.ElementType.Name == entityType.Name);

      //// Find the mapping between conceptual and storage model for this entity set
      //var mapping = metadata.GetItems & lt; EntityContainerMapping & gt; (DataSpace.CSSpace)
      //              .Single()
      //              .EntitySetMappings
      //              .Single(s = &gt; s.EntitySet == entitySet);

      var mapping = context.EntitySetMapping(type);


      // Find the storage entity set (table) that the entity is mapped
      var tableEntitySet = mapping
          .EntityTypeMappings.Single()
          .Fragments.Single()
          .StoreEntitySet;

      // Return the table name from the storage entity set
      var tableName = tableEntitySet.MetadataProperties["Table"].Value ?? tableEntitySet.Name;

      // Find the storage property (column) that the property is mapped
      var columnName = mapping
          .EntityTypeMappings.Single()
          .Fragments.Single()
          .PropertyMappings
          .OfType<ScalarPropertyMapping>()
                .Single(m => m.Property.Name == propertyName)
                .Column
                .Name;

      return tableName + "." + columnName;
    }

  }
}

