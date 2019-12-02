using System;
using System.Collections.Generic;
using System.Text;

namespace System.Urn
{
    /// <summary>
    /// Specifies the parts of a Urn.
    /// This enumeration has a FlagsAttribute attribute that allows a bitwise combination of its member values
    /// </summary>
    [Flags]
    public enum UrnComponents
    {
        /// <summary>
        /// The NID
        /// </summary>
        Nid,
        /// <summary>
        /// The NSS
        /// </summary>
        Nss,
        /// <summary>
        /// The Query parameters
        /// </summary>
        Query,
        /// <summary>
        /// The resolution parameters
        /// </summary>
        Resolution,
        /// <summary>
        /// The Fragment
        /// </summary>
        Fragment,
        /// <summary>
        /// The Scheme
        /// </summary>
        Scheme,
    }
}
