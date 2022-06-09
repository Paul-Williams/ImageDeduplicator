# nullable enable

using CSharpFunctionalExtensions;
using Data.Models;
using System.Collections.Generic;

namespace PubSubEvents.DatabaseEvents
{
  public class DuplicateImageAddedEventArgs : ValueObject
  {
    public DuplicateImageAddedEventArgs(ImageEntity existing, ImageEntity duplicate)
    {
      Existing = existing;
      Duplicate = duplicate;
    }

    public ImageEntity Existing { get; }
    public ImageEntity Duplicate { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return Existing.Id;
      yield return Duplicate.Id;
    }
  }


}
