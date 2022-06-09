# nullable enable

using Data.Models;

namespace Data
{
  public partial class DatabaseRefresh
  {
    // DTO structs -- Back-peddled from using ValueTuples for simplicity

    private struct EntityOp
    {
      public ImageEntity? Entity;
      public DbOperation Operation;
      public EntityOp(ImageEntity? entity, DbOperation operation) { Entity = entity; Operation = operation; }
    }

  }

}
