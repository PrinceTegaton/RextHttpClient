using System;

namespace Rext
{
    /// <summary>
    /// Resiliency policy
    /// </summary>
    public class ResiliencyPolicy
    {
        /// <summary>
        /// Status code to enforce policy on
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Multiple status codes to enforce policy on
        /// </summary>
        public int[] StatusCodes { get; set; } = Array.Empty<int>();

        /// <summary>
        /// Number of times to retry request
        /// </summary>
        public int Retry { get; set; }

        /// <summary>
        /// Duration to wait between retries
        /// </summary>
        public TimeSpan? Interval { get; set; } = TimeSpan.FromSeconds(3);
    }
}
