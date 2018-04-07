﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimplePipeline
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IPipeline{TInput,TOutput}" /> interface.
    /// </summary>
    public static class Pipeline
    {
        /// <summary>
        ///     Converts a pipeline to a filter.
        /// </summary>
        /// <typeparam name="TInput">The type of the pipeline input.</typeparam>
        /// <typeparam name="TOutput">The type of the pipeline output.</typeparam>
        /// <param name="pipeline">The pipeline to convert to a filter.</param>
        /// <returns>A newly constructed filter that is based on the provided pipeline.</returns>
        public static IFilter<TInput, TOutput> ToFilter<TInput, TOutput>(this IPipeline<TInput, TOutput> pipeline)
        {
            if (pipeline == null)
                throw new ArgumentNullException(nameof(pipeline));
            return new PipelineFilter<TInput, TOutput>(pipeline);
        }

        private class PipelineFilter<TInput, TOutput> : IFilter<TInput, TOutput>
        {
            private readonly IPipeline<TInput, TOutput> pipeline;

            public PipelineFilter(IPipeline<TInput, TOutput> pipeline)
            {
                this.pipeline = pipeline;
            }

            public TOutput Execute(TInput input)
            {
                try
                {
                    return pipeline.Execute(input) ? pipeline.Output : throw pipeline.Exception;
                }
                finally
                {
                    pipeline.Reset();
                }

            }
        }
    }

    /// <summary>
    ///     Represents a concrete implementation of the <see cref="IPipeline{TInput,TOutput}" /> interface.
    /// </summary>
    /// <typeparam name="TInput">The type of the pipeline input.</typeparam>
    /// <typeparam name="TOutput">The type of the pipeline output.</typeparam>
    public class Pipeline<TInput, TOutput> : IPipeline<TInput, TOutput>
    {
        private readonly IEnumerable<FilterData> filters;
        private Tuple<Exception> exceptionResult;
        private Tuple<TOutput> outputResult;

        /// <summary>
        ///     Creates a new <see cref="Pipeline{TInput,TOutput}" /> instance.
        /// </summary>
        /// <param name="sequence">The filter sequence to populate this pipeline with.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidFilterCollectionException"></exception>
        public Pipeline(FilterSequence sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));
            if (!sequence.CanCreatePipeline(typeof(TInput), typeof(TOutput)))
                throw new InvalidFilterCollectionException();
            IEnumerable<FilterData> copyFilterDatas = sequence.ToList();
            filters = copyFilterDatas;
        }

        /// <summary>
        ///     Creates a new <see cref="Pipeline{TInput,TOutput}" /> instance.
        /// </summary>
        /// <param name="filters">The filter collection to populate this pipeline with.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidFilterCollectionException"></exception>
        public Pipeline(IEnumerable<FilterData> filters) : this(new FilterSequence(filters)) { }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public TOutput Output
        {
            get
            {
                if (outputResult != null)
                    return outputResult.Item1;
                throw new InvalidOperationException();
            }
        }

        /// <inheritdoc />
        public Exception Exception
        {
            get
            {
                if (exceptionResult != null)
                    return exceptionResult.Item1;
                throw new InvalidOperationException();
            }
        }

        /// <inheritdoc />
        public Boolean IsBeginState
        {
            get
            {
                return outputResult == null && exceptionResult == null;
            }
        }

        /// <inheritdoc />
        public Boolean Execute(TInput input)
        {
            Reset();
            try
            {
                outputResult = Tuple.Create((TOutput)this.Aggregate<FilterData, Object>(input, (value, filter) => filter.Execute(value)));
                return true;
            }
            catch (Exception e)
            {
                exceptionResult = Tuple.Create(e);
                return false;
            }
        }

        /// <inheritdoc />
        public void Reset()
        {
            if (IsBeginState)
                return;
            outputResult = null;
            exceptionResult = null;
        }

        /// <inheritdoc />
        public IEnumerator<FilterData> GetEnumerator()
        {
            return filters.GetEnumerator();
        }
    }
}