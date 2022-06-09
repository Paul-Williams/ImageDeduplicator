# nullable enable

using System;

namespace ImageDeduper
{
  /// <summary>
  /// Simple wrapper to add retry-count to any type <typeparamref name="T"/>
  /// </summary>
  public class Retryable<T> //where T : class
  {
    /// <summary>
    /// Object being wrapped by this <see cref="Retryable{T}"/>
    /// </summary>
    public T Value { get; set; }

    /// <summary>
    /// Used or remaining retires (implementer decides exact usage)
    /// </summary>
    public int Retries { get; set; }


    /// <summary>
    /// Creates an new instance wrapping value <typeparamref name="T"/>
    /// </summary>
    public Retryable(T item) => Value = item ?? throw new ArgumentNullException(nameof(item));

    /// <summary>
    /// Explicitly converts <see cref="Retryable{T}"/> back to <typeparamref name="T"/>. Alternative to calling <see cref="Retryable{T}.Value"/>.
    /// </summary>
    public static explicit operator T(Retryable<T> r) =>  r != null ? r.Value : throw new ArgumentNullException(nameof(r));

    public T ToT(Retryable<T> r) => r.Value;

  }
}
