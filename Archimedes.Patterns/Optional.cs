using System;


namespace Archimedes.Patterns
{
    /// <summary>
    /// Provides static builder methods for optional wrappers
    /// </summary>
    public static class Optional
    {
        #region Static Builder

        /// <summary>
        /// Builds an optional with the given value, which must not be null
        /// </summary>
        /// <typeparam name="TN"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if the value was null</exception>
        public static Optional<TN> Of<TN>(TN value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return new Optional<TN>(value);
        }

        /// <summary>
        /// Builds an optional which has the given value if not null, or is empty otherwise.
        /// </summary>
        /// <typeparam name="TN"></typeparam>
        /// <param name="nullable"></param>
        /// <returns></returns>
        public static Optional<TN> OfNullable<TN>(TN nullable)
        {
            return nullable != null ? Of(nullable) : new Optional<TN>();
        }

        /// <summary>
        /// Returns a Optional which does not contain a value.
        /// </summary>
        /// <returns></returns>
        public static Optional<TN> Empty<TN>()
        {
            return new Optional<TN>();
        }

        #endregion
    }


    /// <summary>
    /// A immutable (thus thread save) container-object which may or may not contain a non-null value. 
    /// 
    /// If a value is present (not null), IsPresent will return true and Value will return the value. 
    /// Invoking  the Value property causes an exception.
    /// 
    /// Use the static builder methods to create instances of this class.
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Optional<T>
    {
        #region Fields

        private readonly T _value;
        private readonly bool _hasValue;

        #endregion

        #region Internal Constructors

        /// <summary>
        /// Creates a new Optional - use the static builder functions
        /// </summary>
        /// <param name="value"></param>
        internal Optional(T value)
        {
            _value = value;
            _hasValue = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the value if available, otherwise throws <see cref="InvalidOperationException "/>
        /// </summary>
        /// <returns></returns>
        public T Value
        {
            get
            {
                if (!IsPresent) throw new InvalidOperationException("You invoked Value on an Optional<T> which values is NOT present. Use IsPresent to check or any of the 'OrElse' methods!");
                return _value;
            }
        }


        /// <summary>
        /// Checks if the value is present, i.e. the value is not null.
        /// Note that this does not guarantee that the value is NOT null!
        /// </summary>
        public bool IsPresent
        {
            get
            {
                return _hasValue && _value != null;
            }
        }


        #endregion

        #region Functional Programming

        /// <summary>
        /// Returns the value if available, or else the provided value as fall-back.
        /// </summary>
        /// <param name="elseValue"></param>
        /// <returns></returns>
        public T OrElse(T elseValue)
        {
            if (IsPresent) return Value;
            return elseValue;
        }

        /// <summary>
        /// Returns the value if available, or else the provided value as fall-back.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elseProvider"></param>
        /// <returns></returns>
        public T OrElse(Func<T> elseProvider)
        {
            return elseProvider();
        }

        /// <summary>
        /// Returns the value if available, or else the default value of the type -
        /// Which is null for all classes, and empty/zero for all structures.
        /// </summary>
        /// <returns></returns>
        public T OrDefault()
        {
            if (IsPresent) return Value;
            return default(T);
        }

        /// <summary>
        /// Maps this value if present, using the given mapper function.
        /// Otherwise, an empty optional is returned.
        /// </summary>
        /// <typeparam name="TOut">The output type of the maping function</typeparam>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public Optional<TOut> Map<TOut>(Func<T, TOut> mapper)
        {
            if (IsPresent)
            {
                return Optional.OfNullable(mapper(Value));
            }

            return Optional.Empty<TOut>();
        }

        /// <summary>
        /// If a value is present, apply the provided Optional-bearing mapping function to it, return that result, otherwise return an empty Optional.
        /// This method is similar to Map(Function), but the provided mapper is one whose result is already an Optional.
        /// </summary>
        /// <typeparam name="TOut">The output type of the maping function</typeparam>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public Optional<TOut> FlatMap<TOut>(Func<T, Optional<TOut>> mapper)
        {
            if (IsPresent)
            {
                return mapper(Value);
            }
            return Optional.Empty<TOut>();
        }

        /// <summary>
        /// If a value is present, and the value matches the given predicate, return an Optional describing the value, otherwise return an empty Optional.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Optional<T> Filter(Predicate<T> predicate)
        {
            if (IsPresent)
            {
                if (predicate(Value))
                {
                    return this;
                }
            }
            return Optional.Empty<T>();
        }

        /// <summary>
        /// If a value is present, invoke the specified action with the value, otherwise do nothing.
        /// </summary>
        /// <param name="consumer"></param>
        public void IfPresent(Action<T> consumer)
        {
            if (IsPresent)
            {
                consumer(Value);
            }
        }

        #endregion

        public override string ToString()
        {
            return IsPresent ? Value + "" : "{Empty-Optional}";
        }
    }
}
