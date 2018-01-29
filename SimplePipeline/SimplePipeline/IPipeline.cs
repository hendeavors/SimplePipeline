﻿using System;
using System.Collections.Generic;

namespace SimplePipeline
{
    /// <summary>
    ///     Provides methods and properties to process a generic input in a collection of filters.
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    public interface IPipeline<in TInput, out TOutput> : IEnumerable<Object>
    {
        /// <summary>
        ///     Gets the exception of a processed input, if unsuccessful. If not, the default value is returned.
        /// </summary>
        TOutput Output { get; }

        /// <summary>
        ///     Gets the exception of a processed input, if unsuccessful. If not, the default value is returned.
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        ///     Gets the state of the pipeline.
        /// </summary>
        Boolean IsBeginState { get; }

        /// <summary>
        ///     Processes the input in a collection of filters and returns a boolean that determines if the processing was
        ///     successful.
        /// </summary>
        /// <param name="input">The input to process in a collection of filters.</param>
        /// <returns>True if the processing was successful. If not, false is returned.</returns>
        Boolean Execute(TInput input);

        /// <summary>
        ///     Resets the pipeline to a state that is similar to a newly instantiated pipeline.
        /// </summary>
        void Reset();
    }
}