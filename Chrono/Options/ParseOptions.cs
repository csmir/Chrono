namespace Chrono
{
    public enum ParseOptions
    {
        /// <summary>
        ///     Throws in execution if no matches were found on the passed string.
        /// </summary>
        ThrowIfNothingMatched,

        /// <summary>
        ///     Whether to decrement the final result from the passed action. If unset, this will automatically default to incrementation.
        /// </summary>
        DecrementResult,

        /// <summary>
        ///     Nothing.
        /// </summary>
        None
    }
}
