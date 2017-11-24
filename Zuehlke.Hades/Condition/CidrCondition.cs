using System;
using System.Net;
using Newtonsoft.Json;
using Zuehlke.Hades.Interfaces;

namespace Zuehlke.Hades.Condition
{
    /// <summary>
    /// A condition that checks if an ip address is part of a subnet (in Classless Inter-Domain Routing notation)
    /// </summary>
    public class CidrCondition : ICondition
    {
        /// <summary>
        /// The key of the condition is used to get a value from the <see cref="AccessRequest.Context"/>
        /// </summary>
        public string Key => "cidr_ip";
        
        /// <summary>
        /// The subnet in CIDR notation that should be checked against
        /// </summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public string Value { get; private set; }

        [JsonConstructor]
        // ReSharper disable once UnusedMember.Local
        private CidrCondition() { }

        /// <summary>
        /// Initializes a new instance of an <see cref="CidrCondition"/>
        /// </summary>
        /// <param name="value">The subnet in CIDR notation that should be checked against</param>
        public CidrCondition(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Checks if the ip address of the request is part of a subnet 
        /// </summary>
        /// <param name="request">The <see cref="AccessRequest"/> inquiry</param>
        /// <returns>true if the ip is part of the subnet / false if not</returns>
        public bool FulfillsCondition(AccessRequest request)
        {
            return request.Context.ContainsKey(Key) && IsInRange(request.Context[Key], Value);
        }

        /// <summary>
        /// Checks if an ip address is part of a subnet 
        /// </summary>
        /// <param name="ipAddress">The ip that should be checked</param>
        /// <param name="CIDRmask">The CIDR mask that should be checked against</param>
        /// <returns>true if the mask is applicable / false if not</returns>
        private static bool IsInRange(string ipAddress, string CIDRmask)
        {
            var parts = CIDRmask.Split('/');

            var ipAddr = BitConverter.ToInt32(IPAddress.Parse(parts[0]).GetAddressBytes(), 0);
            var cidrAddr = BitConverter.ToInt32(IPAddress.Parse(ipAddress).GetAddressBytes(), 0);
            var cidrMask = IPAddress.HostToNetworkOrder(-1 << (32 - int.Parse(parts[1])));

            return (ipAddr & cidrMask) == (cidrAddr & cidrMask);
        }
    }
}
