using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
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
        public string Value { get; private set; }

        [JsonConstructor]
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
            if(request.Context.ContainsKey(Key))
            {
                return (IsInRange(request.Context[Key], Value));
            }
            return false;
        }

        /// <summary>
        /// Checks if an ip address is part of a subnet 
        /// </summary>
        /// <param name="ipAddress">The ip that should be checked</param>
        /// <param name="CIDRmask">The CIDR mask that should be checked against</param>
        /// <returns>true if the mask is applicable / false if not</returns>
        private bool IsInRange(string ipAddress, string CIDRmask)
        {
            string[] parts = CIDRmask.Split('/');

            int IP_addr = BitConverter.ToInt32(IPAddress.Parse(parts[0]).GetAddressBytes(), 0);
            int CIDR_addr = BitConverter.ToInt32(IPAddress.Parse(ipAddress).GetAddressBytes(), 0);
            int CIDR_mask = IPAddress.HostToNetworkOrder(-1 << (32 - int.Parse(parts[1])));

            return ((IP_addr & CIDR_mask) == (CIDR_addr & CIDR_mask));
        }
    }
}
