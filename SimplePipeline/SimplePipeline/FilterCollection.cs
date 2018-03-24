﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace SimplePipeline
{
    /// <summary>
    ///     Represents a sequence of filters that the output of previous filter matches the input of the following filter.
    /// </summary>
    public sealed class FilterCollection : IReadOnlyCollection<FilterData>
    {
        //private readonly Queue<FilterData> innerCollection = new Queue<FilterData>();
        //private FilterData first;
        //private FilterData last;
        private readonly LinkedList<FilterData> innerCollection = new LinkedList<FilterData>();

        /// <summary>
        ///     Creates a new <see cref="FilterCollection" /> instance.
        /// </summary>
        public FilterCollection() { }

        /// <summary>
        ///     Create a new <see cref="FilterCollection" /> instance.
        /// </summary>
        /// <param name="filterDatas">The collection of data to add to this sequence.</param>
        /// <exception cref="ArgumentNullException"></exception>
        // ReSharper disable once UnusedMember.Global
        public FilterCollection(IEnumerable<FilterData> filterDatas)
        {
            if (filterDatas == null)
                throw new ArgumentNullException(nameof(filterDatas));
            try
            {
                Load(filterDatas);
            }
            catch (InvalidFilterException e)
            {
                throw new ArgumentException("", nameof(filterDatas), e);
            }
        }

        public FilterData FirstFilter
        {
            get
            {
                return innerCollection.First?.Value;
            }
        }

        public FilterData LastFilter
        {
            get
            {
                return innerCollection.Last?.Value;
            }
        }

        /// <summary>
        ///     Gets the input type of the first filter in this sequence.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public Type InputType
        {
            get
            {
                return FirstFilter?.InputType;
            }
        }

        /// <summary>
        ///     Get the output type of the last filter in this sequence.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public Type OutputType
        {
            get
            {
                return LastFilter?.OutputType;
            }
        }

        /// <inheritdoc />
        public IEnumerator<FilterData> GetEnumerator()
        {
            return innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public Int32 Count
        {
            get
            {
                return innerCollection.Count;
            }
        }

        private void Load(IEnumerable<FilterData> filterDatas)
        {
            foreach (FilterData filterData in filterDatas)
                Add(filterData);
        }

        /// <summary>
        ///     Adds a filter to the end of the sequence.
        /// </summary>
        /// <typeparam name="TInput">The type of the filter input.</typeparam>
        /// <typeparam name="TOutput">The type of the filter output.</typeparam>
        /// <param name="filter">The filter to add to the end of the sequence.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidFilterException"></exception>
        public void Add<TInput, TOutput>(IFilter<TInput, TOutput> filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            Add(FilterData.Create(filter));
        }

        /// <summary>
        ///     Adds a fucntion to the end of the sequence.
        /// </summary>
        /// <typeparam name="TInput">The type of the function input.</typeparam>
        /// <typeparam name="TOutput">The type of the function output.</typeparam>
        /// <param name="func">The function to add to the end of the sequence.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidFilterException"></exception>
        public void Add<TInput, TOutput>(Func<TInput, TOutput> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            Add(func.ToFilter());
        }

        /// <summary>
        ///     Adds a pipeline to the end of the sequence.
        /// </summary>
        /// <typeparam name="TInput">The type of the pipeline input.</typeparam>
        /// <typeparam name="TOutput">The type of the pipeline output.</typeparam>
        /// <param name="pipeline">The pipeline to add to the end of the sequence.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidFilterException"></exception>
        public void Add<TInput, TOutput>(IPipeline<TInput, TOutput> pipeline)
        {
            if (pipeline == null)
                throw new ArgumentNullException(nameof(pipeline));
            Add(pipeline.ToFilter());
        }

        /// <summary>
        ///     Adds a non-generic filter to the end of the sequence.
        /// </summary>
        /// <param name="filterData">The non-generic filter to add to the end of the sequence.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidFilterException"></exception>
        public void Add(FilterData filterData)
        {
            if (filterData == null)
                throw new ArgumentNullException(nameof(filterData));
            if (Count > 0 && !filterData.InputType.IsAssignableFrom(LastFilter.OutputType))
                throw new InvalidFilterException(filterData.InputType, LastFilter.OutputType);
            innerCollection.AddLast(filterData);
        }

        /// <summary>
        ///     Checks whether this sequence of filters can be used to create a pipeline of the provided input and output type.
        /// </summary>
        /// <param name="pipelineInputType">The type of the pipeline input to check if it is compatible with this sequence.</param>
        /// <param name="pipelineOutputType">The type of the pipeline output to check if it is compatible with this sequence.</param>
        /// <returns>A value indicating if this sequence is compatible.</returns>
        public Boolean CanCreatePipeline(Type pipelineInputType, Type pipelineOutputType)
        {
            return pipelineInputType != null && pipelineOutputType != null && (Count > 0 ? InputType.IsAssignableFrom(pipelineInputType) && pipelineOutputType.IsAssignableFrom(OutputType) : pipelineOutputType.IsAssignableFrom(pipelineInputType));
        }
    }
}