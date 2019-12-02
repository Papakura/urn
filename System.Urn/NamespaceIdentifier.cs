﻿using System.Text.RegularExpressions;

namespace System.Urn
{
    /// <summary>
    ///     Represents the NID of URN
    /// </summary>
    public class NamespaceIdentifier : IEquatable<NamespaceIdentifier>
    {
        private readonly Regex _allowed = new Regex("^[0-9a-z][0-9a-z-]{0,30}[0-9a-z]$", RegexOptions.IgnoreCase);
        private readonly string _nid;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="nid">The NID text</param>
        public NamespaceIdentifier(string nid)
        {
            if (string.IsNullOrEmpty(nid))
            {
                throw new UriFormatException("NID cannot be null or empty");
            }

            if (Urn.Scheme.Equals(nid))
            {
                throw new UriFormatException($"'{nid}' is reserved");
            }

            if (nid.Length > 32)
            {
                throw new UriFormatException($"'{nid}' cannot be more than 31 characters");
            }

            var reason = ValidateNamespaceIdentifier(nid);
            if (!string.IsNullOrEmpty(reason))
            {
                throw new UriFormatException(reason);
            }

            _nid = nid;
        }

        /// <inheritdoc />
        public bool Equals(NamespaceIdentifier other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return _nid.Equals(other._nid, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _nid;
        }

        private string ValidateNamespaceIdentifier(string nid)
        {
            if (nid.Length < 2)
            {
                return $"Namespace Identifier '{nid}' is too short.";
            }

            if (!_allowed.IsMatch(nid))
            {
                return $"Invalid characters in Namespace Identifier '{nid}'";
            }

            return null;
        }
    }
}