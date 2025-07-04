﻿#region License, Terms and Author(s)
//
// LINQBridge
// Copyright (c) 2007 Atif Aziz, Joseph Albahari. All rights reserved.
//
//  Author(s):
//
//      Atif Aziz, http://www.raboof.com
//
// This library is free software; you can redistribute it and/or modify it 
// under the terms of the New BSD License, a copy of which should have 
// been delivered along with this distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT 
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY 
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
#endregion

// $Id: Enumerable.cs c08984d432b1 2012/04/17 16:05:19 azizatif $

namespace System.Linq
{
    #region Imports

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using LinqBridge;

    #endregion

    /// <summary>
    /// Provides a set of static (Shared in Visual Basic) methods for 
    /// querying objects that implement <see cref="IEnumerable{T}" />.
    /// </summary>

    static partial class Enumerable
    {
        /// <summary>
        /// Returns the input typed as <see cref="IEnumerable{T}"/>.
        /// </summary>

        public static IEnumerable<TSource> AsEnumerable<TSource>(this IEnumerable<TSource> source)
        {
            return source;
        }

        /// <summary>
        /// Returns an empty <see cref="IEnumerable{T}"/> that has the 
        /// specified type argument.
        /// </summary>

        public static IEnumerable<TResult> Empty<TResult>()
        {
            return Sequence<TResult>.Empty;
        }

        /// <summary>
        /// Converts the elements of an <see cref="IEnumerable"/> to the 
        /// specified type.
        /// </summary>

        public static IEnumerable<TResult> Cast<TResult>(
            this IEnumerable source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return CastYield<TResult>(source);
        }

        private static IEnumerable<TResult> CastYield<TResult>(
            IEnumerable source)
        {
            foreach (var item in source)
                yield return (TResult)item;
        }

        /// <summary>
        /// Filters the elements of an <see cref="IEnumerable"/> based on a specified type.
        /// </summary>

        public static IEnumerable<TResult> OfType<TResult>(
            this IEnumerable source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return OfTypeYield<TResult>(source);
        }

        private static IEnumerable<TResult> OfTypeYield<TResult>(
            IEnumerable source)
        {
            foreach (var item in source)
                if (item is TResult)
                    yield return (TResult)item;
        }

        /// <summary>
        /// Generates a sequence of integral numbers within a specified range.
        /// </summary>
        /// <param name="start">The value of the first integer in the sequence.</param>
        /// <param name="count">The number of sequential integers to generate.</param>

        public static IEnumerable<int> Range(int start, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", count, null);

            var end = (long)start + count;
            if (end - 1 >= int.MaxValue)
                throw new ArgumentOutOfRangeException("count", count, null);

            return RangeYield(start, end);
        }

        private static IEnumerable<int> RangeYield(int start, long end)
        {
            for (var i = start; i < end; i++)
                yield return i;
        }

        /// <summary>
        /// Generates a sequence that contains one repeated value.
        /// </summary>

        public static IEnumerable<TResult> Repeat<TResult>(TResult element, int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException("count", count, null);

            return RepeatYield(element, count);
        }

        private static IEnumerable<TResult> RepeatYield<TResult>(TResult element, int count)
        {
            for (var i = 0; i < count; i++)
                yield return element;
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>

        public static IEnumerable<TSource> Where<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");

            return source.Where((item, i) => predicate(item));
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate. 
        /// Each element's index is used in the logic of the predicate function.
        /// </summary>

        public static IEnumerable<TSource> Where<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, int, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            return WhereYield(source, predicate);
        }

        private static IEnumerable<TSource> WhereYield<TSource>(
            IEnumerable<TSource> source,
            Func<TSource, int, bool> predicate)
        {
            var i = 0;
            foreach (var item in source)
                if (predicate(item, i++))
                    yield return item;
        }

        /// <summary>
        /// Projects each element of a sequence into a new form.
        /// </summary>

        public static IEnumerable<TResult> Select<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");

            return source.Select((item, i) => selector(item));
        }

        /// <summary>
        /// Projects each element of a sequence into a new form by 
        /// incorporating the element's index.
        /// </summary>

        public static IEnumerable<TResult> Select<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, int, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            return SelectYield(source, selector);
        }

        private static IEnumerable<TResult> SelectYield<TSource, TResult>(
            IEnumerable<TSource> source,
            Func<TSource, int, TResult> selector)
        {
            var i = 0;
            foreach (var item in source)
                yield return selector(item, i++);
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{T}" /> 
        /// and flattens the resulting sequences into one sequence.
        /// </summary>

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TResult>> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");

            return source.SelectMany((item, i) => selector(item));
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{T}" />, 
        /// and flattens the resulting sequences into one sequence. The 
        /// index of each source element is used in the projected form of 
        /// that element.
        /// </summary>

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, int, IEnumerable<TResult>> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");

            return source.SelectMany(selector, (item, subitem) => subitem);
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{T}" />, 
        /// flattens the resulting sequences into one sequence, and invokes 
        /// a result selector function on each element therein.
        /// </summary>

        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            if (collectionSelector == null) throw new ArgumentNullException("collectionSelector");

            return source.SelectMany((item, i) => collectionSelector(item), resultSelector);
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{T}" />, 
        /// flattens the resulting sequences into one sequence, and invokes 
        /// a result selector function on each element therein. The index of 
        /// each source element is used in the intermediate projected form 
        /// of that element.
        /// </summary>

        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, int, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (collectionSelector == null) throw new ArgumentNullException("collectionSelector");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");

            return SelectManyYield(source, collectionSelector, resultSelector);
        }

        private static IEnumerable<TResult> SelectManyYield<TSource, TCollection, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, int, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            var i = 0;
            foreach (var item in source)
                foreach (var subitem in collectionSelector(item, i++))
                    yield return resultSelector(item, subitem);
        }

        /// <summary>
        /// Returns elements from a sequence as long as a specified condition is true.
        /// </summary>

        public static IEnumerable<TSource> TakeWhile<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");

            return source.TakeWhile((item, i) => predicate(item));
        }

        /// <summary>
        /// Returns elements from a sequence as long as a specified condition is true.
        /// The element's index is used in the logic of the predicate function.
        /// </summary>

        public static IEnumerable<TSource> TakeWhile<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, int, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            return TakeWhileYield(source, predicate);
        }

        private static IEnumerable<TSource> TakeWhileYield<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, int, bool> predicate)
        {
            var i = 0;
            foreach (var item in source)
                if (predicate(item, i++))
                    yield return item;
                else
                    break;
        }

        /// <summary>
        /// Returns a specified number of contiguous elements from the start 
        /// of a sequence.
        /// </summary>

        public static IEnumerable<TSource> Take<TSource>(
            this IEnumerable<TSource> source,
            int count)
        {
            return source.TakeWhile((item, i) => i < count);
        }

        private static class Futures<T>
        {
            public static readonly Func<T> Default = () => default(T);
            public static readonly Func<T> Undefined = () => { throw new InvalidOperationException(); };
        }

        /// <summary>
        /// Base implementation of First operator.
        /// </summary>

        private static TSource FirstImpl<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource> empty)
        {
            if (source == null) throw new ArgumentNullException("source");
            Debug.Assert(empty != null);

            var list = source as IList<TSource>;    // optimized case for lists
            if (list != null)
                return list.Count > 0 ? list[0] : empty();

            using (var e = source.GetEnumerator())  // fallback for enumeration
                return e.MoveNext() ? e.Current : empty();
        }

        /// <summary>
        /// Returns the first element of a sequence.
        /// </summary>

        public static TSource First<TSource>(
            this IEnumerable<TSource> source)
        {
            return source.FirstImpl(Futures<TSource>.Undefined);
        }

        /// <summary>
        /// Returns the first element in a sequence that satisfies a specified condition.
        /// </summary>

        public static TSource First<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return First(source.Where(predicate));
        }

        /// <summary>
        /// Returns the first element of a sequence, or a default value if 
        /// the sequence contains no elements.
        /// </summary>

        public static TSource FirstOrDefault<TSource>(
            this IEnumerable<TSource> source)
        {
            return source.FirstImpl(Futures<TSource>.Default);
        }

        /// <summary>
        /// Returns the first element of the sequence that satisfies a 
        /// condition or a default value if no such element is found.
        /// </summary>

        public static TSource FirstOrDefault<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return FirstOrDefault(source.Where(predicate));
        }

        /// <summary>
        /// Base implementation of Last operator.
        /// </summary>

        private static TSource LastImpl<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource> empty)
        {
            if (source == null) throw new ArgumentNullException("source");

            var list = source as IList<TSource>;    // optimized case for lists
            if (list != null)
                return list.Count > 0 ? list[list.Count - 1] : empty();

            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                    return empty();

                var last = e.Current;
                while (e.MoveNext())
                    last = e.Current;

                return last;
            }
        }

        /// <summary>
        /// Returns the last element of a sequence.
        /// </summary>
        public static TSource Last<TSource>(
            this IEnumerable<TSource> source)
        {
            return source.LastImpl(Futures<TSource>.Undefined);
        }

        /// <summary>
        /// Returns the last element of a sequence that satisfies a 
        /// specified condition.
        /// </summary>

        public static TSource Last<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return Last(source.Where(predicate));
        }

        /// <summary>
        /// Returns the last element of a sequence, or a default value if 
        /// the sequence contains no elements.
        /// </summary>

        public static TSource LastOrDefault<TSource>(
            this IEnumerable<TSource> source)
        {
            return source.LastImpl(Futures<TSource>.Default);
        }

        /// <summary>
        /// Returns the last element of a sequence that satisfies a 
        /// condition or a default value if no such element is found.
        /// </summary>

        public static TSource LastOrDefault<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return LastOrDefault(source.Where(predicate));
        }

        /// <summary>
        /// Base implementation of Single operator.
        /// </summary>

        private static TSource SingleImpl<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource> empty)
        {
            if (source == null) throw new ArgumentNullException("source");

            using (var e = source.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    var single = e.Current;
                    if (!e.MoveNext())
                        return single;

                    throw new InvalidOperationException();
                }

                return empty();
            }
        }

        /// <summary>
        /// Returns the only element of a sequence, and throws an exception 
        /// if there is not exactly one element in the sequence.
        /// </summary>

        public static TSource Single<TSource>(
            this IEnumerable<TSource> source)
        {
            return source.SingleImpl(Futures<TSource>.Undefined);
        }

        /// <summary>
        /// Returns the only element of a sequence that satisfies a 
        /// specified condition, and throws an exception if more than one 
        /// such element exists.
        /// </summary>

        public static TSource Single<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return Single(source.Where(predicate));
        }

        /// <summary>
        /// Returns the only element of a sequence, or a default value if 
        /// the sequence is empty; this method throws an exception if there 
        /// is more than one element in the sequence.
        /// </summary>

        public static TSource SingleOrDefault<TSource>(
            this IEnumerable<TSource> source)
        {
            return source.SingleImpl(Futures<TSource>.Default);
        }

        /// <summary>
        /// Returns the only element of a sequence that satisfies a 
        /// specified condition or a default value if no such element 
        /// exists; this method throws an exception if more than one element 
        /// satisfies the condition.
        /// </summary>

        public static TSource SingleOrDefault<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return SingleOrDefault(source.Where(predicate));
        }

        /// <summary>
        /// Returns the element at a specified index in a sequence.
        /// </summary>

        public static TSource ElementAt<TSource>(
            this IEnumerable<TSource> source,
            int index)
        {
            if (source == null) throw new ArgumentNullException("source");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index", index, null);

            var list = source as IList<TSource>;
            if (list != null)
                return list[index];

            try
            {
                return source.SkipWhile((item, i) => i < index).First();
            }
            catch (InvalidOperationException) // if thrown by First
            {
                throw new ArgumentOutOfRangeException("index", index, null);
            }
        }

        /// <summary>
        /// Returns the element at a specified index in a sequence or a 
        /// default value if the index is out of range.
        /// </summary>

        public static TSource ElementAtOrDefault<TSource>(
            this IEnumerable<TSource> source,
            int index)
        {
            if (source == null) throw new ArgumentNullException("source");

            if (index < 0)
                return default(TSource);

            var list = source as IList<TSource>;
            if (list != null)
                return index < list.Count ? list[index] : default(TSource);

            return source.SkipWhile((item, i) => i < index).FirstOrDefault();
        }

        /// <summary>
        /// Inverts the order of the elements in a sequence.
        /// </summary>

        public static IEnumerable<TSource> Reverse<TSource>(
            this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return ReverseYield(source);
        }

        private static IEnumerable<TSource> ReverseYield<TSource>(IEnumerable<TSource> source)
        {
            var stack = new Stack<TSource>();
            foreach (var item in source)
                stack.Push(item);

            foreach (var item in stack)
                yield return item;
        }

        /// <summary>
        /// Bypasses elements in a sequence as long as a specified condition 
        /// is true and then returns the remaining elements.
        /// </summary>

        public static IEnumerable<TSource> SkipWhile<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");

            return source.SkipWhile((item, i) => predicate(item));
        }

        /// <summary>
        /// Bypasses elements in a sequence as long as a specified condition 
        /// is true and then returns the remaining elements. The element's 
        /// index is used in the logic of the predicate function.
        /// </summary>

        public static IEnumerable<TSource> SkipWhile<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, int, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            return SkipWhileYield(source, predicate);
        }

        private static IEnumerable<TSource> SkipWhileYield<TSource>(
            IEnumerable<TSource> source,
            Func<TSource, int, bool> predicate)
        {
            using (var e = source.GetEnumerator())
            {
                for (var i = 0; ; i++)
                {
                    if (!e.MoveNext())
                        yield break;

                    if (!predicate(e.Current, i))
                        break;
                }

                do { yield return e.Current; } while (e.MoveNext());
            }
        }

        /// <summary>
        /// Bypasses a specified number of elements in a sequence and then 
        /// returns the remaining elements.
        /// </summary>

        public static IEnumerable<TSource> Skip<TSource>(
            this IEnumerable<TSource> source,
            int count)
        {
            return source.SkipWhile((item, i) => i < count);
        }

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>

        public static int Count<TSource>(
            this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            var collection = source as ICollection;
            return collection != null
                 ? collection.Count
                 : source.Aggregate(0, (count, item) => checked(count + 1));
        }

        /// <summary>
        /// Returns a number that represents how many elements in the 
        /// specified sequence satisfy a condition.
        /// </summary>

        public static int Count<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return Count(source.Where(predicate));
        }

        /// <summary>
        /// Returns an <see cref="Int64"/> that represents the total number 
        /// of elements in a sequence.
        /// </summary>

        public static long LongCount<TSource>(
            this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            var array = source as Array;
            return array != null
                 ? array.LongLength
                 : source.Aggregate(0L, (count, item) => count + 1);
        }

        /// <summary>
        /// Returns an <see cref="Int64"/> that represents how many elements 
        /// in a sequence satisfy a condition.
        /// </summary>

        public static long LongCount<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return LongCount(source.Where(predicate));
        }

        /// <summary>
        /// Concatenates two sequences.
        /// </summary>

        public static IEnumerable<TSource> Concat<TSource>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            return ConcatYield(first, second);
        }

        private static IEnumerable<TSource> ConcatYield<TSource>(
            IEnumerable<TSource> first,
            IEnumerable<TSource> second)
        {
            foreach (var item in first)
                yield return item;

            foreach (var item in second)
                yield return item;
        }

        /// <summary>
        /// Creates a <see cref="List{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>

        public static List<TSource> ToList<TSource>(
            this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return new List<TSource>(source);
        }

        /// <summary>
        /// Creates an array from an <see cref="IEnumerable{T}"/>.
        /// </summary>

        public static TSource[] ToArray<TSource>(
            this IEnumerable<TSource> source)
        {
            return source.ToList().ToArray();
        }

        /// <summary>
        /// Returns distinct elements from a sequence by using the default 
        /// equality comparer to compare values.
        /// </summary>

        public static IEnumerable<TSource> Distinct<TSource>(
            this IEnumerable<TSource> source)
        {
            return Distinct(source, /* comparer */ null);
        }

        /// <summary>
        /// Returns distinct elements from a sequence by using a specified 
        /// <see cref="IEqualityComparer{T}"/> to compare values.
        /// </summary>

        public static IEnumerable<TSource> Distinct<TSource>(
            this IEnumerable<TSource> source,
            IEqualityComparer<TSource> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");

            return DistinctYield(source, comparer);
        }

        private static IEnumerable<TSource> DistinctYield<TSource>(
            IEnumerable<TSource> source,
            IEqualityComparer<TSource> comparer)
        {
            var set = new Dictionary<TSource, object>(comparer);
            var gotNull = false;

            foreach (var item in source)
            {
                if (item == null)
                {
                    if (gotNull)
                        continue;
                    gotNull = true;
                }
                else
                {
                    if (set.ContainsKey(item))
                        continue;
                    set.Add(item, null);
                }

                yield return item;
            }
        }

        /// <summary>
        /// Creates a <see cref="Lookup{TKey,TElement}" /> from an 
        /// <see cref="IEnumerable{T}" /> according to a specified key 
        /// selector function.
        /// </summary>

        public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return ToLookup(source, keySelector, e => e, /* comparer */ null);
        }

        /// <summary>
        /// Creates a <see cref="Lookup{TKey,TElement}" /> from an 
        /// <see cref="IEnumerable{T}" /> according to a specified key 
        /// selector function and a key comparer.
        /// </summary>

        public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer)
        {
            return ToLookup(source, keySelector, e => e, comparer);
        }

        /// <summary>
        /// Creates a <see cref="Lookup{TKey,TElement}" /> from an 
        /// <see cref="IEnumerable{T}" /> according to specified key 
        /// and element selector functions.
        /// </summary>

        public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
        {
            return ToLookup(source, keySelector, elementSelector, /* comparer */ null);
        }

        /// <summary>
        /// Creates a <see cref="Lookup{TKey,TElement}" /> from an 
        /// <see cref="IEnumerable{T}" /> according to a specified key 
        /// selector function, a comparer and an element selector function.
        /// </summary>

        public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            if (elementSelector == null) throw new ArgumentNullException("elementSelector");

            var lookup = new Lookup<TKey, TElement>(comparer);

            foreach (var item in source)
            {
                var key = keySelector(item);

                var grouping = (Grouping<TKey, TElement>)lookup.Find(key);
                if (grouping == null)
                {
                    grouping = new Grouping<TKey, TElement>(key);
                    lookup.Add(grouping);
                }

                grouping.Add(elementSelector(item));
            }

            return lookup;
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key 
        /// selector function.
        /// </summary>

        public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return GroupBy(source, keySelector, /* comparer */ null);
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key 
        /// selector function and compares the keys by using a specified 
        /// comparer.
        /// </summary>

        public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer)
        {
            return GroupBy(source, keySelector, e => e, comparer);
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key 
        /// selector function and projects the elements for each group by 
        /// using a specified function.
        /// </summary>

        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
        {
            return GroupBy(source, keySelector, elementSelector, /* comparer */ null);
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key 
        /// selector function and creates a result value from each group and 
        /// its key.
        /// </summary>

        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            if (elementSelector == null) throw new ArgumentNullException("elementSelector");

            return ToLookup(source, keySelector, elementSelector, comparer);
        }

        /// <summary>
        /// Groups the elements of a sequence according to a key selector 
        /// function. The keys are compared by using a comparer and each 
        /// group's elements are projected by using a specified function.
        /// </summary>

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
        {
            return GroupBy(source, keySelector, resultSelector, /* comparer */ null);
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key 
        /// selector function and creates a result value from each group and 
        /// its key. The elements of each group are projected by using a 
        /// specified function.
        /// </summary>

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TKey, IEnumerable<TSource>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");

            return ToLookup(source, keySelector, comparer).Select(g => resultSelector(g.Key, g));
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key 
        /// selector function and creates a result value from each group and 
        /// its key. The keys are compared by using a specified comparer.
        /// </summary>

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
        {
            return GroupBy(source, keySelector, elementSelector, resultSelector, /* comparer */ null);
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key 
        /// selector function and creates a result value from each group and 
        /// its key. Key values are compared by using a specified comparer, 
        /// and the elements of each group are projected by using a 
        /// specified function.
        /// </summary>

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            Func<TKey, IEnumerable<TElement>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            if (elementSelector == null) throw new ArgumentNullException("elementSelector");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");

            return ToLookup(source, keySelector, elementSelector, comparer)
                   .Select(g => resultSelector(g.Key, g));
        }

        /// <summary>
        /// Applies an accumulator function over a sequence.
        /// </summary>

        public static TSource Aggregate<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, TSource, TSource> func)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (func == null) throw new ArgumentNullException("func");

            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                    throw new InvalidOperationException();

                return e.Renumerable().Skip(1).Aggregate(e.Current, func);
            }
        }

        /// <summary>
        /// Applies an accumulator function over a sequence. The specified 
        /// seed value is used as the initial accumulator value.
        /// </summary>

        public static TAccumulate Aggregate<TSource, TAccumulate>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func)
        {
            return Aggregate(source, seed, func, r => r);
        }

        /// <summary>
        /// Applies an accumulator function over a sequence. The specified 
        /// seed value is used as the initial accumulator value, and the 
        /// specified function is used to select the result value.
        /// </summary>

        public static TResult Aggregate<TSource, TAccumulate, TResult>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func,
            Func<TAccumulate, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (func == null) throw new ArgumentNullException("func");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");

            var result = seed;

            foreach (var item in source)
                result = func(result, item);

            return resultSelector(result);
        }

        /// <summary>
        /// Produces the set union of two sequences by using the default 
        /// equality comparer.
        /// </summary>

        public static IEnumerable<TSource> Union<TSource>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second)
        {
            return Union(first, second, /* comparer */ null);
        }

        /// <summary>
        /// Produces the set union of two sequences by using a specified 
        /// <see cref="IEqualityComparer{T}" />.
        /// </summary>

        public static IEnumerable<TSource> Union<TSource>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer)
        {
            return first.Concat(second).Distinct(comparer);
        }

        /// <summary>
        /// Returns the elements of the specified sequence or the type 
        /// parameter's default value in a singleton collection if the 
        /// sequence is empty.
        /// </summary>

        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(
            this IEnumerable<TSource> source)
        {
            return source.DefaultIfEmpty(default(TSource));
        }

        /// <summary>
        /// Returns the elements of the specified sequence or the specified 
        /// value in a singleton collection if the sequence is empty.
        /// </summary>

        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(
            this IEnumerable<TSource> source,
            TSource defaultValue)
        {
            if (source == null) throw new ArgumentNullException("source");

            return DefaultIfEmptyYield(source, defaultValue);
        }

        private static IEnumerable<TSource> DefaultIfEmptyYield<TSource>(
            IEnumerable<TSource> source,
            TSource defaultValue)
        {
            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                    yield return defaultValue;
                else
                    do { yield return e.Current; } while (e.MoveNext());
            }
        }

        /// <summary>
        /// Determines whether all elements of a sequence satisfy a condition.
        /// </summary>

        public static bool All<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            foreach (var item in source)
                if (!predicate(item))
                    return false;

            return true;
        }

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>

        public static bool Any<TSource>(
            this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            using (var e = source.GetEnumerator())
                return e.MoveNext();
        }

        /// <summary>
        /// Determines whether any element of a sequence satisfies a 
        /// condition.
        /// </summary>

        public static bool Any<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return source.Where(predicate).Any();
        }

        /// <summary>
        /// Determines whether a sequence contains a specified element by 
        /// using the default equality comparer.
        /// </summary>

        public static bool Contains<TSource>(
            this IEnumerable<TSource> source,
            TSource value)
        {
            return source.Contains(value, /* comparer */ null);
        }

        /// <summary>
        /// Determines whether a sequence contains a specified element by 
        /// using a specified <see cref="IEqualityComparer{T}" />.
        /// </summary>

        public static bool Contains<TSource>(
            this IEnumerable<TSource> source,
            TSource value,
            IEqualityComparer<TSource> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");

            if (comparer == null)
            {
                var collection = source as ICollection<TSource>;
                if (collection != null)
                    return collection.Contains(value);
            }

            comparer = comparer ?? EqualityComparer<TSource>.Default;
            return source.Any(item => comparer.Equals(item, value));
        }

        /// <summary>
        /// Determines whether two sequences are equal by comparing the 
        /// elements by using the default equality comparer for their type.
        /// </summary>

        public static bool SequenceEqual<TSource>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second)
        {
            return first.SequenceEqual(second, /* comparer */ null);
        }

        /// <summary>
        /// Determines whether two sequences are equal by comparing their 
        /// elements by using a specified <see cref="IEqualityComparer{T}" />.
        /// </summary>

        public static bool SequenceEqual<TSource>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer)
        {
            if (first == null) throw new ArgumentNullException("frist");
            if (second == null) throw new ArgumentNullException("second");

            comparer = comparer ?? EqualityComparer<TSource>.Default;

            using (IEnumerator<TSource> lhs = first.GetEnumerator(),
                                        rhs = second.GetEnumerator())
            {
                do
                {
                    if (!lhs.MoveNext())
                        return !rhs.MoveNext();

                    if (!rhs.MoveNext())
                        return false;
                }
                while (comparer.Equals(lhs.Current, rhs.Current));
            }

            return false;
        }

        /// <summary>
        /// Base implementation for Min/Max operator.
        /// </summary>

        private static TSource MinMaxImpl<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, TSource, bool> lesser)
        {
            if (source == null) throw new ArgumentNullException("source");
            Debug.Assert(lesser != null);

            if (typeof(TSource).IsClass) // ReSharper disable CompareNonConstrainedGenericWithNull                
                source = source.Where(e => e != null).DefaultIfEmpty(); // ReSharper restore CompareNonConstrainedGenericWithNull

            return source.Aggregate((a, item) => lesser(a, item) ? a : item);
        }

        /// <summary>
        /// Base implementation for Min/Max operator for nullable types.
        /// </summary>

        private static TSource? MinMaxImpl<TSource>(
            this IEnumerable<TSource?> source,
            TSource? seed, Func<TSource?, TSource?, bool> lesser) where TSource : struct
        {
            if (source == null) throw new ArgumentNullException("source");
            Debug.Assert(lesser != null);

            return source.Aggregate(seed, (a, item) => lesser(a, item) ? a : item);
            //  == MinMaxImpl(Repeat<TSource?>(null, 1).Concat(source), lesser);
        }

        /// <summary>
        /// Returns the minimum value in a generic sequence.
        /// </summary>

        public static TSource Min<TSource>(
            this IEnumerable<TSource> source)
        {
            var comparer = Comparer<TSource>.Default;
            return source.MinMaxImpl((x, y) => comparer.Compare(x, y) < 0);
        }

        /// <summary>
        /// Invokes a transform function on each element of a generic 
        /// sequence and returns the minimum resulting value.
        /// </summary>

        public static TResult Min<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TResult> selector)
        {
            return source.Select(selector).Min();
        }

        /// <summary>
        /// Returns the maximum value in a generic sequence.
        /// </summary>

        public static TSource Max<TSource>(
            this IEnumerable<TSource> source)
        {
            var comparer = Comparer<TSource>.Default;
            return source.MinMaxImpl((x, y) => comparer.Compare(x, y) > 0);
        }

        /// <summary>
        /// Invokes a transform function on each element of a generic 
        /// sequence and returns the maximum resulting value.
        /// </summary>

        public static TResult Max<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TResult> selector)
        {
            return source.Select(selector).Max();
        }

        /// <summary>
        /// Makes an enumerator seen as enumerable once more.
        /// </summary>
        /// <remarks>
        /// The supplied enumerator must have been started. The first element
        /// returned is the element the enumerator was on when passed in.
        /// DO NOT use this method if the caller must be a generator. It is
        /// mostly safe among aggregate operations.
        /// </remarks>

        private static IEnumerable<T> Renumerable<T>(this IEnumerator<T> e)
        {
            Debug.Assert(e != null);

            do { yield return e.Current; } while (e.MoveNext());
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a key.
        /// </summary>

        public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.OrderBy(keySelector, /* comparer */ null);
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending order by using a 
        /// specified comparer.
        /// </summary>

        public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");

            return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer, /* descending */ false);
        }

        /// <summary>
        /// Sorts the elements of a sequence in descending order according to a key.
        /// </summary>

        public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.OrderByDescending(keySelector, /* comparer */ null);
        }

        /// <summary>
        ///  Sorts the elements of a sequence in descending order by using a 
        /// specified comparer. 
        /// </summary>

        public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source == null) throw new ArgumentNullException("keySelector");

            return new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer, /* descending */ true);
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in 
        /// ascending order according to a key.
        /// </summary>

        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(
            this IOrderedEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.ThenBy(keySelector, /* comparer */ null);
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in 
        /// ascending order by using a specified comparer.
        /// </summary>

        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(
            this IOrderedEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");

            return source.CreateOrderedEnumerable(keySelector, comparer, /* descending */ false);
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in 
        /// descending order, according to a key.
        /// </summary>

        public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(
            this IOrderedEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.ThenByDescending(keySelector, /* comparer */ null);
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in 
        /// descending order by using a specified comparer.
        /// </summary>

        public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(
            this IOrderedEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");

            return source.CreateOrderedEnumerable(keySelector, comparer, /* descending */ true);
        }

        /// <summary>
        /// Base implementation for Intersect and Except operators.
        /// </summary>

        private static IEnumerable<TSource> IntersectExceptImpl<TSource>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer,
            bool flag)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            var keys = new List<Key<TSource>>();
            var flags = new Dictionary<Key<TSource>, bool>(new KeyComparer<TSource>(comparer));

            foreach (var item in from item in first
                                 select new Key<TSource>(item) into item
                                 where !flags.ContainsKey(item)
                                 select item)
            {
                flags.Add(item, !flag);
                keys.Add(item);
            }

            foreach (var item in from item in second
                                 select new Key<TSource>(item) into item
                                 where flags.ContainsKey(item)
                                 select item)
            {
                flags[item] = flag;
            }

            //
            // As per docs, "the marked elements are yielded in the order in 
            // which they were collected.
            //

            return from item in keys where flags[item] select item.Value;
        }

        /// <summary>
        /// Produces the set intersection of two sequences by using the 
        /// default equality comparer to compare values.
        /// </summary>

        public static IEnumerable<TSource> Intersect<TSource>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second)
        {
            return first.Intersect(second, /* comparer */ null);
        }

        /// <summary>
        /// Produces the set intersection of two sequences by using the 
        /// specified <see cref="IEqualityComparer{T}" /> to compare values.
        /// </summary>

        public static IEnumerable<TSource> Intersect<TSource>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer)
        {
            return IntersectExceptImpl(first, second, comparer, /* flag */ true);
        }

        /// <summary>
        /// Produces the set difference of two sequences by using the 
        /// default equality comparer to compare values.
        /// </summary>

        public static IEnumerable<TSource> Except<TSource>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second)
        {
            return first.Except(second, /* comparer */ null);
        }

        /// <summary>
        /// Produces the set difference of two sequences by using the 
        /// specified <see cref="IEqualityComparer{T}" /> to compare values.
        /// </summary>

        public static IEnumerable<TSource> Except<TSource>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer)
        {
            return IntersectExceptImpl(first, second, comparer, /* flag */ false);
        }

        /// <summary>
        /// Creates a <see cref="Dictionary{TKey,TValue}" /> from an 
        /// <see cref="IEnumerable{T}" /> according to a specified key 
        /// selector function.
        /// </summary>

        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.ToDictionary(keySelector, /* comparer */ null);
        }

        /// <summary>
        /// Creates a <see cref="Dictionary{TKey,TValue}" /> from an 
        /// <see cref="IEnumerable{T}" /> according to a specified key 
        /// selector function and key comparer.
        /// </summary>

        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer)
        {
            return source.ToDictionary(keySelector, e => e, comparer);
        }

        /// <summary>
        /// Creates a <see cref="Dictionary{TKey,TValue}" /> from an 
        /// <see cref="IEnumerable{T}" /> according to specified key 
        /// selector and element selector functions.
        /// </summary>

        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
        {
            return source.ToDictionary(keySelector, elementSelector, /* comparer */ null);
        }

        /// <summary>
        /// Creates a <see cref="Dictionary{TKey,TValue}" /> from an 
        /// <see cref="IEnumerable{T}" /> according to a specified key 
        /// selector function, a comparer, and an element selector function.
        /// </summary>

        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            if (elementSelector == null) throw new ArgumentNullException("elementSelector");

            var dict = new Dictionary<TKey, TElement>(comparer);

            foreach (var item in source)
            {
                //
                // ToDictionary is meant to throw ArgumentNullException if
                // keySelector produces a key that is null and 
                // Argument exception if keySelector produces duplicate keys 
                // for two elements. Incidentally, the doucmentation for
                // IDictionary<TKey, TValue>.Add says that the Add method
                // throws the same exceptions under the same circumstances
                // so we don't need to do any additional checking or work
                // here and let the Add implementation do all the heavy
                // lifting.
                //

                dict.Add(keySelector(item), elementSelector(item));
            }

            return dict;
        }

        /// <summary>
        /// Correlates the elements of two sequences based on matching keys. 
        /// The default equality comparer is used to compare keys.
        /// </summary>

        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector)
        {
            return outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector, /* comparer */ null);
        }

        /// <summary>
        /// Correlates the elements of two sequences based on matching keys. 
        /// The default equality comparer is used to compare keys. A 
        /// specified <see cref="IEqualityComparer{T}" /> is used to compare keys.
        /// </summary>

        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (outer == null) throw new ArgumentNullException("outer");
            if (inner == null) throw new ArgumentNullException("inner");
            if (outerKeySelector == null) throw new ArgumentNullException("outerKeySelector");
            if (innerKeySelector == null) throw new ArgumentNullException("innerKeySelector");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");

            var lookup = inner.ToLookup(innerKeySelector, comparer);

            return
                from o in outer
                from i in lookup[outerKeySelector(o)]
                select resultSelector(o, i);
        }

        /// <summary>
        /// Correlates the elements of two sequences based on equality of 
        /// keys and groups the results. The default equality comparer is 
        /// used to compare keys.
        /// </summary>

        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
        {
            return outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, /* comparer */ null);
        }

        /// <summary>
        /// Correlates the elements of two sequences based on equality of 
        /// keys and groups the results. The default equality comparer is 
        /// used to compare keys. A specified <see cref="IEqualityComparer{T}" /> 
        /// is used to compare keys.
        /// </summary>

        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, IEnumerable<TInner>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (outer == null) throw new ArgumentNullException("outer");
            if (inner == null) throw new ArgumentNullException("inner");
            if (outerKeySelector == null) throw new ArgumentNullException("outerKeySelector");
            if (innerKeySelector == null) throw new ArgumentNullException("innerKeySelector");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");

            var lookup = inner.ToLookup(innerKeySelector, comparer);
            return outer.Select(o => resultSelector(o, lookup[outerKeySelector(o)]));
        }

        private static class Sequence<T>
        {
            public static readonly IEnumerable<T> Empty = new T[0];
        }

        private sealed class Grouping<K, V> : List<V>, IGrouping<K, V>
        {
            internal Grouping(K key)
            {
                Key = key;
            }

            public K Key { get; private set; }
        }
    }
}

// $Id: Enumerable.g.tt 71137f497bf2 2012/04/16 20:01:27 azizatif $

namespace System.Linq
{
    #region Imports

    using System;
    using System.Collections.Generic;

    #endregion

    // This partial implementation was template-generated:
    // Mon, 16 Apr 2012 20:05:53 GMT

    partial class Enumerable
    {
        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Int32" /> values.
        /// </summary>

        public static int Sum(
            this IEnumerable<int> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            int sum = 0;
            foreach (var num in source)
                sum = checked(sum + num);

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Int32" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static int Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, int> selector)
        {
            return source.Select(selector).Sum();
        }

        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Int32" /> values.
        /// </summary>

        public static double Average(
            this IEnumerable<int> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            long sum = 0;
            long count = 0;

            foreach (var num in source)
                checked
                {
                    sum += (int)num;
                    count++;
                }

            if (count == 0)
                throw new InvalidOperationException();

            return (double)sum / count;
        }

        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Int32" /> values 
        /// that are obtained by invoking a transform function on each 
        /// element of the input sequence.
        /// </summary>

        public static double Average<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, int> selector)
        {
            return source.Select(selector).Average();
        }


        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Int32" /> values.
        /// </summary>

        public static int? Sum(
            this IEnumerable<int?> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            int sum = 0;
            foreach (var num in source)
                sum = checked(sum + (num ?? 0));

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Int32" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static int? Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, int?> selector)
        {
            return source.Select(selector).Sum();
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Int32" /> values.
        /// </summary>

        public static double? Average(
            this IEnumerable<int?> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            long sum = 0;
            long count = 0;

            foreach (var num in source.Where(n => n != null))
                checked
                {
                    sum += (int)num;
                    count++;
                }

            if (count == 0)
                return null;

            return (double?)sum / count;
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Int32" /> values 
        /// that are obtained by invoking a transform function on each 
        /// element of the input sequence.
        /// </summary>

        public static double? Average<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, int?> selector)
        {
            return source.Select(selector).Average();
        }

        /// <summary>
        /// Returns the minimum value in a sequence of nullable 
        /// <see cref="System.Int32" /> values.
        /// </summary>

        public static int? Min(
            this IEnumerable<int?> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return MinMaxImpl(source.Where(x => x != null), null, (min, x) => min < x);
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and 
        /// returns the minimum nullable <see cref="System.Int32" /> value.
        /// </summary>

        public static int? Min<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, int?> selector)
        {
            return source.Select(selector).Min();
        }

        /// <summary>
        /// Returns the maximum value in a sequence of nullable 
        /// <see cref="System.Int32" /> values.
        /// </summary>

        public static int? Max(
            this IEnumerable<int?> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return MinMaxImpl(source.Where(x => x != null),
                null, (max, x) => x == null || (max != null && x.Value < max.Value));
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and 
        /// returns the maximum nullable <see cref="System.Int32" /> value.
        /// </summary>

        public static int? Max<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, int?> selector)
        {
            return source.Select(selector).Max();
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Int64" /> values.
        /// </summary>

        public static long Sum(
            this IEnumerable<long> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            long sum = 0;
            foreach (var num in source)
                sum = checked(sum + num);

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Int64" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static long Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, long> selector)
        {
            return source.Select(selector).Sum();
        }

        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Int64" /> values.
        /// </summary>

        public static double Average(
            this IEnumerable<long> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            long sum = 0;
            long count = 0;

            foreach (var num in source)
                checked
                {
                    sum += (long)num;
                    count++;
                }

            if (count == 0)
                throw new InvalidOperationException();

            return (double)sum / count;
        }

        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Int64" /> values 
        /// that are obtained by invoking a transform function on each 
        /// element of the input sequence.
        /// </summary>

        public static double Average<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, long> selector)
        {
            return source.Select(selector).Average();
        }


        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Int64" /> values.
        /// </summary>

        public static long? Sum(
            this IEnumerable<long?> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            long sum = 0;
            foreach (var num in source)
                sum = checked(sum + (num ?? 0));

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Int64" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static long? Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, long?> selector)
        {
            return source.Select(selector).Sum();
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Int64" /> values.
        /// </summary>

        public static double? Average(
            this IEnumerable<long?> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            long sum = 0;
            long count = 0;

            foreach (var num in source.Where(n => n != null))
                checked
                {
                    sum += (long)num;
                    count++;
                }

            if (count == 0)
                return null;

            return (double?)sum / count;
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Int64" /> values 
        /// that are obtained by invoking a transform function on each 
        /// element of the input sequence.
        /// </summary>

        public static double? Average<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, long?> selector)
        {
            return source.Select(selector).Average();
        }

        /// <summary>
        /// Returns the minimum value in a sequence of nullable 
        /// <see cref="System.Int64" /> values.
        /// </summary>

        public static long? Min(
            this IEnumerable<long?> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return MinMaxImpl(source.Where(x => x != null), null, (min, x) => min < x);
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and 
        /// returns the minimum nullable <see cref="System.Int64" /> value.
        /// </summary>

        public static long? Min<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, long?> selector)
        {
            return source.Select(selector).Min();
        }

        /// <summary>
        /// Returns the maximum value in a sequence of nullable 
        /// <see cref="System.Int64" /> values.
        /// </summary>

        public static long? Max(
            this IEnumerable<long?> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return MinMaxImpl(source.Where(x => x != null),
                null, (max, x) => x == null || (max != null && x.Value < max.Value));
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and 
        /// returns the maximum nullable <see cref="System.Int64" /> value.
        /// </summary>

        public static long? Max<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, long?> selector)
        {
            return source.Select(selector).Max();
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Single" /> values.
        /// </summary>

        public static float Sum(
            this IEnumerable<float> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            float sum = 0;
            foreach (var num in source)
                sum = checked(sum + num);

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Single" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static float Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, float> selector)
        {
            return source.Select(selector).Sum();
        }

        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Single" /> values.
        /// </summary>

        public static float Average(
            this IEnumerable<float> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            float sum = 0;
            long count = 0;

            foreach (var num in source)
                checked
                {
                    sum += (float)num;
                    count++;
                }

            if (count == 0)
                throw new InvalidOperationException();

            return (float)sum / count;
        }

        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Single" /> values 
        /// that are obtained by invoking a transform function on each 
        /// element of the input sequence.
        /// </summary>

        public static float Average<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, float> selector)
        {
            return source.Select(selector).Average();
        }


        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Single" /> values.
        /// </summary>

        public static float? Sum(
            this IEnumerable<float?> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            float sum = 0;
            foreach (var num in source)
                sum = checked(sum + (num ?? 0));

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Single" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static float? Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, float?> selector)
        {
            return source.Select(selector).Sum();
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Single" /> values.
        /// </summary>

        public static float? Average(
            this IEnumerable<float?> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            float sum = 0;
            long count = 0;

            foreach (var num in source.Where(n => n != null))
                checked
                {
                    sum += (float)num;
                    count++;
                }

            if (count == 0)
                return null;

            return (float?)sum / count;
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Single" /> values 
        /// that are obtained by invoking a transform function on each 
        /// element of the input sequence.
        /// </summary>

        public static float? Average<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, float?> selector)
        {
            return source.Select(selector).Average();
        }

        /// <summary>
        /// Returns the minimum value in a sequence of nullable 
        /// <see cref="System.Single" /> values.
        /// </summary>

        public static float? Min(
            this IEnumerable<float?> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return MinMaxImpl(source.Where(x => x != null), null, (min, x) => min < x);
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and 
        /// returns the minimum nullable <see cref="System.Single" /> value.
        /// </summary>

        public static float? Min<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, float?> selector)
        {
            return source.Select(selector).Min();
        }

        /// <summary>
        /// Returns the maximum value in a sequence of nullable 
        /// <see cref="System.Single" /> values.
        /// </summary>

        public static float? Max(
            this IEnumerable<float?> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return MinMaxImpl(source.Where(x => x != null),
                null, (max, x) => x == null || (max != null && x.Value < max.Value));
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and 
        /// returns the maximum nullable <see cref="System.Single" /> value.
        /// </summary>

        public static float? Max<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, float?> selector)
        {
            return source.Select(selector).Max();
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Double" /> values.
        /// </summary>

        public static double Sum(
            this IEnumerable<double> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            double sum = 0;
            foreach (var num in source)
                sum = checked(sum + num);

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Double" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static double Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, double> selector)
        {
            return source.Select(selector).Sum();
        }

        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Double" /> values.
        /// </summary>

        public static double Average(
            this IEnumerable<double> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            double sum = 0;
            long count = 0;

            foreach (var num in source)
                checked
                {
                    sum += (double)num;
                    count++;
                }

            if (count == 0)
                throw new InvalidOperationException();

            return (double)sum / count;
        }

        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Double" /> values 
        /// that are obtained by invoking a transform function on each 
        /// element of the input sequence.
        /// </summary>

        public static double Average<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, double> selector)
        {
            return source.Select(selector).Average();
        }


        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Double" /> values.
        /// </summary>

        public static double? Sum(
            this IEnumerable<double?> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            double sum = 0;
            foreach (var num in source)
                sum = checked(sum + (num ?? 0));

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Double" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static double? Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, double?> selector)
        {
            return source.Select(selector).Sum();
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Double" /> values.
        /// </summary>

        public static double? Average(
            this IEnumerable<double?> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            double sum = 0;
            long count = 0;

            foreach (var num in source.Where(n => n != null))
                checked
                {
                    sum += (double)num;
                    count++;
                }

            if (count == 0)
                return null;

            return (double?)sum / count;
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Double" /> values 
        /// that are obtained by invoking a transform function on each 
        /// element of the input sequence.
        /// </summary>

        public static double? Average<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, double?> selector)
        {
            return source.Select(selector).Average();
        }

        /// <summary>
        /// Returns the minimum value in a sequence of nullable 
        /// <see cref="System.Double" /> values.
        /// </summary>

        public static double? Min(
            this IEnumerable<double?> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return MinMaxImpl(source.Where(x => x != null), null, (min, x) => min < x);
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and 
        /// returns the minimum nullable <see cref="System.Double" /> value.
        /// </summary>

        public static double? Min<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, double?> selector)
        {
            return source.Select(selector).Min();
        }

        /// <summary>
        /// Returns the maximum value in a sequence of nullable 
        /// <see cref="System.Double" /> values.
        /// </summary>

        public static double? Max(
            this IEnumerable<double?> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return MinMaxImpl(source.Where(x => x != null),
                null, (max, x) => x == null || (max != null && x.Value < max.Value));
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and 
        /// returns the maximum nullable <see cref="System.Double" /> value.
        /// </summary>

        public static double? Max<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, double?> selector)
        {
            return source.Select(selector).Max();
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Decimal" /> values.
        /// </summary>

        public static decimal Sum(
            this IEnumerable<decimal> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            decimal sum = 0;
            foreach (var num in source)
                sum = checked(sum + num);

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable <see cref="System.Decimal" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static decimal Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, decimal> selector)
        {
            return source.Select(selector).Sum();
        }

        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Decimal" /> values.
        /// </summary>

        public static decimal Average(
            this IEnumerable<decimal> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            decimal sum = 0;
            long count = 0;

            foreach (var num in source)
                checked
                {
                    sum += (decimal)num;
                    count++;
                }

            if (count == 0)
                throw new InvalidOperationException();

            return (decimal)sum / count;
        }

        /// <summary>
        /// Computes the average of a sequence of nullable <see cref="System.Decimal" /> values 
        /// that are obtained by invoking a transform function on each 
        /// element of the input sequence.
        /// </summary>

        public static decimal Average<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, decimal> selector)
        {
            return source.Select(selector).Average();
        }


        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Decimal" /> values.
        /// </summary>

        public static decimal? Sum(
            this IEnumerable<decimal?> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            decimal sum = 0;
            foreach (var num in source)
                sum = checked(sum + (num ?? 0));

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of <see cref="System.Decimal" /> 
        /// values that are obtained by invoking a transform function on 
        /// each element of the input sequence.
        /// </summary>

        public static decimal? Sum<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, decimal?> selector)
        {
            return source.Select(selector).Sum();
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Decimal" /> values.
        /// </summary>

        public static decimal? Average(
            this IEnumerable<decimal?> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            decimal sum = 0;
            long count = 0;

            foreach (var num in source.Where(n => n != null))
                checked
                {
                    sum += (decimal)num;
                    count++;
                }

            if (count == 0)
                return null;

            return (decimal?)sum / count;
        }

        /// <summary>
        /// Computes the average of a sequence of <see cref="System.Decimal" /> values 
        /// that are obtained by invoking a transform function on each 
        /// element of the input sequence.
        /// </summary>

        public static decimal? Average<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, decimal?> selector)
        {
            return source.Select(selector).Average();
        }

        /// <summary>
        /// Returns the minimum value in a sequence of nullable 
        /// <see cref="System.Decimal" /> values.
        /// </summary>

        public static decimal? Min(
            this IEnumerable<decimal?> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return MinMaxImpl(source.Where(x => x != null), null, (min, x) => min < x);
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and 
        /// returns the minimum nullable <see cref="System.Decimal" /> value.
        /// </summary>

        public static decimal? Min<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, decimal?> selector)
        {
            return source.Select(selector).Min();
        }

        /// <summary>
        /// Returns the maximum value in a sequence of nullable 
        /// <see cref="System.Decimal" /> values.
        /// </summary>

        public static decimal? Max(
            this IEnumerable<decimal?> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return MinMaxImpl(source.Where(x => x != null),
                null, (max, x) => x == null || (max != null && x.Value < max.Value));
        }

        /// <summary>
        /// Invokes a transform function on each element of a sequence and 
        /// returns the maximum nullable <see cref="System.Decimal" /> value.
        /// </summary>

        public static decimal? Max<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, decimal?> selector)
        {
            return source.Select(selector).Max();
        }
    }
}

// $Id: ExtensionAttribute.cs 898b3d493ed6 2012/04/17 20:09:57 azizatif $

namespace System.Runtime.CompilerServices
{
    /// <remarks>
    /// This attribute allows us to define extension methods without 
    /// requiring .NET Framework 3.5. For more information, see the section,
    /// <a href="http://msdn.microsoft.com/en-us/magazine/cc163317.aspx#S7">Extension Methods in .NET Framework 2.0 Apps</a>,
    /// of <a href="http://msdn.microsoft.com/en-us/magazine/cc163317.aspx">Basic Instincts: Extension Methods</a>
    /// column in <a href="http://msdn.microsoft.com/msdnmag/">MSDN Magazine</a>, 
    /// issue <a href="http://msdn.microsoft.com/en-us/magazine/cc135410.aspx">Nov 2007</a>.
    /// </remarks>

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
    sealed partial class ExtensionAttribute : Attribute { }
}

// $Id: Func.cs 71137f497bf2 2012/04/16 20:01:27 azizatif $

namespace System
{
#if LINQBRIDGE_LIB
    public delegate TResult Func<out TResult>();
    public delegate TResult Func<in T, out TResult>(T a);
    public delegate TResult Func<in T1, in T2, out TResult>(T1 arg1, T2 arg2);
    public delegate TResult Func<in T1, in T2, in T3, out TResult>(T1 arg1, T2 arg2, T3 arg3);
    public delegate TResult Func<in T1, in T2, in T3, in T4, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
#else
    delegate TResult Func<TResult>();
    delegate TResult Func<T, TResult>(T a);
    delegate TResult Func<T1, T2, TResult>(T1 arg1, T2 arg2);
    delegate TResult Func<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3);
    delegate TResult Func<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
#endif
}

// $Id: IGrouping.cs 71137f497bf2 2012/04/16 20:01:27 azizatif $

namespace System.Linq
{
    #region Imports

    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// Represents a collection of objects that have a common key.
    /// </summary>

    partial interface IGrouping<out TKey, TElement> : IEnumerable<TElement>
    {
        /// <summary>
        /// Gets the key of the <see cref="IGrouping{TKey,TElement}" />.
        /// </summary>

        TKey Key { get; }
    }
}

// $Id: ILookup.cs 71137f497bf2 2012/04/16 20:01:27 azizatif $

namespace System.Linq
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines an indexer, size property, and Boolean search method for 
    /// data structures that map keys to <see cref="IEnumerable{T}"/> 
    /// sequences of values.
    /// </summary>

    partial interface ILookup<TKey, TElement> : IEnumerable<IGrouping<TKey, TElement>>
    {
        bool Contains(TKey key);
        int Count { get; }
        IEnumerable<TElement> this[TKey key] { get; }
    }
}

// $Id: Internal.cs 1567e00f1a20 2012/04/17 16:09:51 azizatif $

namespace LinqBridge
{
    #region Imports

    using System;
    using System.Collections.Generic;

    #endregion

    /// <remarks>
    /// This type is not intended to be used directly from user code.
    /// It may be removed or changed in a future version without notice.
    /// </remarks>

    sealed class DelegatingComparer<T> : IComparer<T>
    {
        private readonly Func<T, T, int> _comparer;

        public DelegatingComparer(Func<T, T, int> comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            _comparer = comparer;
        }

        public int Compare(T x, T y) { return _comparer(x, y); }
    }

    /// <remarks>
    /// This type is not intended to be used directly from user code.
    /// It may be removed or changed in a future version without notice.
    /// </remarks>

    struct Key<T>
    {
        public Key(T value) : this() { Value = value; }
        public T Value { get; private set; }
    }

    /// <remarks>
    /// This type is not intended to be used directly from user code.
    /// It may be removed or changed in a future version without notice.
    /// </remarks>

    sealed class KeyComparer<T> : IEqualityComparer<Key<T>>
    {
        private readonly IEqualityComparer<T> _innerComparer;

        public KeyComparer(IEqualityComparer<T> innerComparer)
        {
            _innerComparer = innerComparer ?? EqualityComparer<T>.Default;
        }

        public bool Equals(Key<T> x, Key<T> y)
        {
            return _innerComparer.Equals(x.Value, y.Value);
        }

        public int GetHashCode(Key<T> obj)
        {
            return obj.Value == null ? 0 : _innerComparer.GetHashCode(obj.Value);
        }
    }
}

// $Id: IOrderedEnumerable.cs 71137f497bf2 2012/04/16 20:01:27 azizatif $

namespace System.Linq
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a sorted sequence.
    /// </summary>

    partial interface IOrderedEnumerable<TElement> : IEnumerable<TElement>
    {
        /// <summary>
        /// Performs a subsequent ordering on the elements of an 
        /// <see cref="IOrderedEnumerable{T}"/> according to a key.
        /// </summary>

        IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(
            Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending);
    }
}

// $Id: Lookup.cs c08984d432b1 2012/04/17 16:05:19 azizatif $

namespace System.Linq
{
    #region Imports

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using IEnumerable = System.Collections.IEnumerable;
    using LinqBridge;

    #endregion

    /// <summary>
    /// Represents a collection of keys each mapped to one or more values.
    /// </summary>

    internal sealed class Lookup<TKey, TElement> : ILookup<TKey, TElement>
    {
        private readonly Dictionary<Key<TKey>, IGrouping<TKey, TElement>> _map;
        private readonly List<Key<TKey>> _orderedKeys; // remember order of insertion

        internal Lookup(IEqualityComparer<TKey> comparer)
        {
            _map = new Dictionary<Key<TKey>, IGrouping<TKey, TElement>>(new KeyComparer<TKey>(comparer));
            _orderedKeys = new List<Key<TKey>>();
        }

        internal void Add(IGrouping<TKey, TElement> item)
        {
            var key = new Key<TKey>(item.Key);
            _map.Add(key, item);
            _orderedKeys.Add(key);
        }

        internal IEnumerable<TElement> Find(TKey key)
        {
            IGrouping<TKey, TElement> grouping;
            return _map.TryGetValue(new Key<TKey>(key), out grouping) ? grouping : null;
        }

        /// <summary>
        /// Gets the number of key/value collection pairs in the <see cref="Lookup{TKey,TElement}" />.
        /// </summary>

        public int Count
        {
            get { return _map.Count; }
        }

        /// <summary>
        /// Gets the collection of values indexed by the specified key.
        /// </summary>

        public IEnumerable<TElement> this[TKey key]
        {
            get
            {
                IGrouping<TKey, TElement> result;
                return _map.TryGetValue(new Key<TKey>(key), out result) ? result : Enumerable.Empty<TElement>();
            }
        }

        /// <summary>
        /// Determines whether a specified key is in the <see cref="Lookup{TKey,TElement}" />.
        /// </summary>

        public bool Contains(TKey key)
        {
            return _map.ContainsKey(new Key<TKey>(key));
        }

        /// <summary>
        /// Applies a transform function to each key and its associated 
        /// values and returns the results.
        /// </summary>

        public IEnumerable<TResult> ApplyResultSelector<TResult>(
            Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
        {
            if (resultSelector == null)
                throw new ArgumentNullException("resultSelector");

            foreach (var pair in _map)
                yield return resultSelector(pair.Key.Value, pair.Value);
        }

        /// <summary>
        /// Returns a generic enumerator that iterates through the <see cref="Lookup{TKey,TElement}" />.
        /// </summary>

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            foreach (var key in _orderedKeys)
                yield return _map[key];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

// $Id: OrderedEnumerable.cs 71137f497bf2 2012/04/16 20:01:27 azizatif $

namespace LinqBridge
{
    #region Imports

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    #endregion

    internal sealed class OrderedEnumerable<T, K> : IOrderedEnumerable<T>
    {
        private readonly IEnumerable<T> _source;
        private readonly Func<T[], IComparer<int>, IComparer<int>> _comparerComposer;

        public OrderedEnumerable(IEnumerable<T> source,
            Func<T, K> keySelector, IComparer<K> comparer, bool descending) :
            this(source, (_, next) => next, keySelector, comparer, descending) { }

        private OrderedEnumerable(IEnumerable<T> source,
            Func<T[], IComparer<int>, IComparer<int>> parent,
            Func<T, K> keySelector, IComparer<K> comparer, bool descending)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            Debug.Assert(parent != null);

            _source = source;

            comparer = comparer ?? Comparer<K>.Default;
            var direction = descending ? -1 : 1;

            _comparerComposer = (items, next) =>
            {
                Debug.Assert(items != null);
                Debug.Assert(next != null);

                var keys = new K[items.Length];
                for (var i = 0; i < items.Length; i++)
                    keys[i] = keySelector(items[i]);

                return parent(items, new DelegatingComparer<int>((i, j) =>
                {
                    var result = direction * comparer.Compare(keys[i], keys[j]);
                    return result != 0 ? result : next.Compare(i, j);
                }));
            };
        }

        public IOrderedEnumerable<T> CreateOrderedEnumerable<KK>(
            Func<T, KK> keySelector, IComparer<KK> comparer, bool descending)
        {
            return new OrderedEnumerable<T, KK>(_source, _comparerComposer, keySelector, comparer, descending);
        }

        public IEnumerator<T> GetEnumerator()
        {
            //
            // Sort using Array.Sort but docs say that it performs an 
            // unstable sort. LINQ, on the other hand, says OrderBy performs 
            // a stable sort. Use the item position then as a tie 
            // breaker when all keys compare equal, thus making the sort 
            // stable.
            //

            var items = _source.ToArray();
            var positionComparer = new DelegatingComparer<int>((i, j) => i.CompareTo(j));
            var comparer = _comparerComposer(items, positionComparer);
            var keys = new int[items.Length];
            for (var i = 0; i < keys.Length; i++)
                keys[i] = i;
            Array.Sort(keys, items, comparer);
            return ((IEnumerable<T>)items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

// $Id: Action.cs 71137f497bf2 2012/04/16 20:01:27 azizatif $

namespace System
{
#if LINQBRIDGE_LIB
    public delegate void Action();
    public delegate void Action<in T1, in T2>(T1 arg1, T2 arg2);
    public delegate void Action<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
    public delegate void Action<in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
#else
    delegate void Action();
    delegate void Action<T1, T2>(T1 arg1, T2 arg2);
    delegate void Action<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
    delegate void Action<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
#endif
}

