using System;
using System.Collections.Generic;
using System.Text;

namespace System.Urn
{
    internal class UrnWriter : Urn
    {
        public UrnWriter(string urn, UriFormat format = UriFormat.UriEscaped) : base(urn, format)
        {
        }

        public UrnWriter(string nid, string nss, UriFormat format = UriFormat.UriEscaped) : base(nid, nss, format)
        {
        }

        public UrnWriter(string nid, string nss, string rqf, UriFormat format = UriFormat.UriEscaped) : base(nid, nss, rqf, format)
        {
        }

        public new NamespaceIdentifier Nid
        {
            get => base.Nid;
            set => base.Nid = value;
        }

        public new NamespaceSpecificString Nss
        {
            get => base.Nss;
            set => base.Nss = value;
        }

        public new string Fragment
        {
            get => base.Fragment;
            set => base.Fragment = value;
        }

        /// <summary>
        /// Add a query parameter
        /// </summary>
        /// <exception cref="ArgumentException">If key already exists</exception>
        public void AddQuery(KeyValuePair<string, string> kvp)
        {
            if (Query.ContainsKey(kvp.Key))
            {
                throw new ArgumentException($"{kvp.Key} already exists");
            }
            (Query as Dictionary<string,string>).Add(kvp.Key,kvp.Value);
            
        }

        /// <summary>
        /// Remove a query parameter
        /// </summary>
        public void RemoveQuery(string key)
        {
            if (Query.ContainsKey(key))
            {
                (Query as Dictionary<string, string>).Remove(key);
            }
            
        }

        /// <summary>
        /// Add a resolution parameter
        /// </summary>
        /// <exception cref="ArgumentException">If key already exists</exception>
        public void AddResolution(KeyValuePair<string, string> kvp)
        {
            if (Query.ContainsKey(kvp.Key))
            {
                throw new ArgumentException($"{kvp.Key} already exists");
            }
            (Query as IDictionary<string,string>).Add(kvp);
            
        }

        /// <summary>
        /// Remove a query parameter
        /// </summary>
        public void RemoveResolution(string key)
        {
            if (Resolution.ContainsKey(key))
            {
                (Resolution as IDictionary<string,string>).Remove(key);
            }
            
        }
    }
}
