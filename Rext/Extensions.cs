using System;
using System.Linq;

namespace Rext
{
    /// <summary>
    /// General extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Add resiliency policies at runtime
        /// </summary>
        /// <param name="client"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static IRextHttpClient AddResiliencyPolicy(this IRextHttpClient client, ResiliencyPolicy policy)
        {
            if (client.ResiliencyPolicies != null && client.ResiliencyPolicies.Any(a => a.StatusCode == policy.StatusCode))
            {
                var p = client.ResiliencyPolicies.FirstOrDefault(a => a.StatusCode == policy.StatusCode);
                client.ResiliencyPolicies.Remove(p);
            }

            client.ResiliencyPolicies.Add(policy);

            return client;
        }

        /// <summary>
        /// Set custom timeout value for request. This overrides the global configuration.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="timeoutInSeconds"></param>
        /// <returns></returns>
        public static IRextHttpClient SetTimeout(this IRextHttpClient client, int timeoutInSeconds)
        {
            client.Timeout = TimeSpan.FromSeconds(timeoutInSeconds);
            return client;
        }

        /// <summary>
        /// Set custom timeout value for request. This overrides the global configuration.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static IRextHttpClient SetTimeout(this IRextHttpClient client, TimeSpan timeout)
        {
            client.Timeout = timeout;
            return client;
        }
    }
}
