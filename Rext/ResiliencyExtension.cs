using System.Linq;

namespace Rext
{
    /// <summary>
    /// Resiliency extension
    /// </summary>
    public static class ResiliencyExtension
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
    }
}