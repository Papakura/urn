using System;
using System.Collections.Generic;
using System.Text;

namespace System.Urn
{
    /// <summary>
    /// Provides a custom constructor for uniform resource names (URNs) and modifies URNs for the Urn class
    /// </summary>
    /// <remarks>
    /// The UrnBuilder class provides a convenient way to modify the contents of a Urn instance without creating a new Urn instance for each modification.
    ///The UrnBuilder properties provide read/write access to the read-only Urn properties so that they can be modified.
    /// </remarks>
    public class UrnBuilder
    {
        private readonly UrnWriter _urn;
        /// <summary>
        /// Initializes a new instance of the UriBuilder class with the specified URN.
        /// </summary>
        public UrnBuilder(string urn, UriFormat format)
        {
            _urn = new UrnWriter(urn, format);
        }
        /// <summary>
        /// Gets the Urn instance constructed by the specified UrnBuilder instance.
        /// </summary>
        public Urn Urn => _urn;

        /// <summary>
        /// Get/Set the NID
        /// </summary>
        public string Nid 
        {
            
            set => _urn.Nid = new NamespaceIdentifier(value);
            get => _urn.Nid.ToString();
        }

        /// <summary>
        /// Get/Set the NSS
        /// </summary>
        public string Nss
        {

            set => _urn.Nss = new NamespaceSpecificString(value,UriFormat.UriEscaped);
            get => _urn.Nss.ToString();
        }

        /// <summary>
        /// Get/Set the fragment
        /// </summary>
        public string Fragment
        {
            set => _urn.Fragment = value;
            get => _urn.Fragment;

        }

        /// <summary>
        /// Add a query parameter
        /// </summary>
        /// <exception cref="ArgumentException">If key already exists</exception>
        public UrnBuilder AddQuery(KeyValuePair<string, string> kvp)
        {
            _urn.AddQuery(kvp);
            return this;
        }

        /// <summary>
        /// Remove a query parameter
        /// </summary>
        public UrnBuilder RemoveQuery(string key)
        {
            _urn.RemoveQuery(key);
            return this;
        }

        /// <summary>
        /// Add a resolution parameter
        /// </summary>
        /// <exception cref="ArgumentException">If key already exists</exception>
        public UrnBuilder AddResolution(KeyValuePair<string, string> kvp)
        {
            _urn.AddResolution(kvp);
            return this;
        }

        /// <summary>
        /// Remove a query parameter
        /// </summary>
        public UrnBuilder RemoveResolution(string key)
        {
            _urn.RemoveResolution(key);
            return this;
        }


    }
}
