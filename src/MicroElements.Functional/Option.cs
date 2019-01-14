﻿// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace MicroElements.Functional
{
    /// <summary>
    /// Discriminated union type. Can be in one of two states: Some(a) or None.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    public struct Option<T> :
        IEnumerable<T>,
        IOptional,
        IEquatable<Option<T>>
    {
        /// <summary>
        /// None.
        /// </summary>
        public static readonly Option<T> None = new Option<T>(false, default(T));

        /// <summary>
        /// Is the option in a Some state.
        /// </summary>
        private readonly bool isSome;

        /// <summary>
        /// Option value.
        /// </summary>
        private readonly T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Option{T}"/> struct.
        /// </summary>
        /// <param name="isSome">Is the option in a Some state.</param>
        /// <param name="value">Option value.</param>
        private Option(bool isSome, T value)
        {
            this.isSome = isSome;
            this.value = value;
        }

        /// <summary>
        /// Creates option from IEnumarable.
        /// </summary>
        /// <param name="option">IEnumerable as value source.</param>
        public Option(IEnumerable<T> option)
        {
            var first = option.Take(1).ToArray();
            if (first.Length == 0)
            {
                isSome = false;
                value = default(T);
            }
            else
            {
                isSome = true;
                value = first[0];
            }
        }

        /// <summary>
        /// Is the option in a Some state.
        /// </summary>
        [Pure]
        public bool IsSome => isSome;

        /// <summary>
        /// Is the option in a None state.
        /// </summary>
        [Pure]
        public bool IsNone => !IsSome;

        /// <summary>
        /// Returns option underlying type.
        /// </summary>
        /// <returns>Option underlying type.</returns>
        [Pure]
        public Type GetUnderlyingType() => typeof(T);

        /// <summary>
        /// Gets the Option Value.
        /// </summary>
        internal T Value
        {
            get
            {
                if (IsSome)
                    return value;
                throw new ValueIsNoneException();
            }
        }

        /// <summary>
        /// Convert the Option to an enumerable of zero or one items.
        /// </summary>
        /// <returns>An enumerable of zero or one items.</returns>
        public IEnumerable<T> AsEnumerable()
        {
            if (IsSome)
                yield return Value;
        }

        /// <summary>
        /// Gets enumerator for option.
        /// </summary>
        /// <returns>Enumerator for option.</returns>
        public IEnumerator<T> GetEnumerator()
            => AsEnumerable().GetEnumerator();

        /// <summary>
        /// Gets enumerator for option.
        /// </summary>
        /// <returns>Enumerator for option.</returns>
        IEnumerator IEnumerable.GetEnumerator()
            => AsEnumerable().GetEnumerator();

        /// <inheritdoc />
        public bool Equals(Option<T> other)
        {
            if (IsNone && other.IsNone)
                return true;

            if (IsSome && other.IsSome)
                return EqualityComparer<T>.Default.Equals(Value, other.Value);

            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode() =>
            IsSome
                ? Value.GetHashCode()
                : 0;

        /// <summary>
        /// Equality operator.
        /// </summary>
        public static bool operator ==(Option<T> @this, Option<T> other) => @this.Equals(other);

        /// <summary>
        /// Non-equality operator.
        /// </summary>
        public static bool operator !=(Option<T> @this, Option<T> other) => !(@this == other);

        /// <inheritdoc />
        public override string ToString() => IsSome ? $"Some({Value})" : "None";

        /// <summary>
        /// Implicit conversion of None to Option{T}.
        /// </summary>
        /// <param name="none">None.</param>
        public static implicit operator Option<T>(OptionNone none) => MOption<T>.Inst.None;

        /// <summary>
        /// Implicit conversion of Some to Option.
        /// </summary>
        /// <param name="some">Some.</param>
        public static implicit operator Option<T>(Some<T> some) => new Option<T>(true, some.Value);

        /// <summary>
        /// Implicit conversion of value to Option.
        /// </summary>
        /// <param name="value">Value.</param>
        public static implicit operator Option<T>(T value) => value.IsNull() ? None : Prelude.Some(value);

        /// <summary>
        /// Explicit conversion to underlying type.
        /// </summary>
        /// <param name="option">Optional.</param>
        [Pure]
        public static explicit operator T(Option<T> option) =>
            option.IsSome
                ? option.Value
                : throw new InvalidCastException("Option is not in a Some state");

        /// <summary>
        /// Match the two states of the Option and return a non-null Result.
        /// </summary>
        /// <typeparam name="Result">Result type.</typeparam>
        /// <param name="Some">Some match operation.</param>
        /// <param name="None">None match operation.</param>
        /// <returns>non null Result.</returns>
        public Result Match<Result>(Func<T, Result> Some, Func<Result> None)
            => IsSome ? Some(Value) : None();

        /// <summary>
        /// Match the two states of the Option.
        /// </summary>
        /// <param name="Some">Some match operation.</param>
        /// <param name="None">None match operation.</param>
        /// <returns>Unit.</returns>
        public Unit Match(Action<T> Some, Action None) =>
            MOption<T>.Inst.Match(this, Some, None);
    }

    /// <summary>
    /// Some wrapper for value. Can not be null.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    public struct Some<T>
    {
        internal T Value { get; }

        internal Some(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Cannot wrap a null value in a 'Some'; use 'None' instead");
            Value = value;
        }
    }

    public struct MOption<A>
    {
        public static readonly MOption<A> Inst = default(MOption<A>);

        [Pure]
        public Option<A> None => Option<A>.None;

        [Pure]
        public B Match<B>(Option<A> opt, Func<A, B> Some, Func<B> None)
        {
            if (Some == null)
                throw new ArgumentNullException(nameof(Some));
            if (None == null)
                throw new ArgumentNullException(nameof(None));
            return opt.IsSome
                ? Check.NullReturn(Some(opt.Value))
                : Check.NullReturn(None());
        }

        [Pure]
        public B Match<B>(Option<A> opt, Func<A, B> Some, B None)
        {
            if (Some == null)
                throw new ArgumentNullException(nameof(Some));
            return opt.IsSome
                ? Check.NullReturn(Some(opt.Value))
                : Check.NullReturn(None);
        }

        public Unit Match(Option<A> opt, Action<A> Some, Action None)
        {
            if (Some == null) throw new ArgumentNullException(nameof(Some));
            if (None == null) throw new ArgumentNullException(nameof(None));
            if (opt.IsSome) Some(opt.Value); else None();
            return Unit.Default;
        }
    }

    internal static class Check
    {
        internal static T NullReturn<T>(T value) =>
            value.IsNull()
                ? throw new ResultIsNullException()
                : value;
    }

    /// <summary>
    /// Result is null.
    /// </summary>
    [Serializable]
    public class ResultIsNullException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ResultIsNullException()
            : base("Result is null.")
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ResultIsNullException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ResultIsNullException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Value is none.
    /// </summary>
    [Serializable]
    public class ValueIsNoneException : Exception
    {
        public static readonly ValueIsNoneException Default = new ValueIsNoneException();

        /// <summary>
        /// Ctor
        /// </summary>
        public ValueIsNoneException()
            : base("Value is none.")
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ValueIsNoneException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ValueIsNoneException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
