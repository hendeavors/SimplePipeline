﻿using System;

namespace SimplePipeline
{
    /// <summary>
    /// Contains information about a filter.
    /// </summary>
    public sealed class FilterData : IEquatable<FilterData>
    {
        private FilterData(Object filter, Type inputType, Type outputType)
        {
            Filter = filter;
            InputType = inputType;
            OutputType = outputType;
        }

        /// <summary>
        /// Returns the filter that this data is based on.
        /// </summary>
        public Object Filter { get; }

        /// <summary>
        /// Returns the type of the input of the filter that this data is based on.
        /// </summary>
        public Type InputType { get; }

        /// <summary>
        /// Returns the type of the output of the filter that this data is based on.
        /// </summary>
        public Type OutputType { get; }

        public Boolean Equals(FilterData other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(Filter, other.Filter) && InputType == other.InputType && OutputType == other.OutputType;
        }

        /// <summary>
        /// Creates information from the provided filter.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="filter">The filter to create the information from.</param>
        /// <returns>The information about the provided filter.</returns>
        public static FilterData Create<TInput, TOutput>(IFilter<TInput, TOutput> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            return new FilterData(filter, typeof(TInput), typeof(TOutput));
        }

        public override Boolean Equals(Object obj)
        {
            return obj is FilterData data && Equals(data);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 hashCode = Filter != null ? Filter.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (InputType != null ? InputType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (OutputType != null ? OutputType.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}