using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace System.Urn
{
    /// <summary>
    ///     Represents a Uniform Resource Name (URN)
    /// </summary>
    /// <remarks>
    ///     Every implementation has has a Namespace Identifier and Namespace Specific String component
    ///     https://tools.ietf.org/html/rfc8141
    ///     http://www.iana.org/assignments/urn-namespaces/urn-namespaces.xhtml
    /// </remarks>
    public class Urn : IEquatable<Urn>, ISerializable
    {
        private const string Seperator = ":";

        /// <summary>
        ///     The URN prefix
        /// </summary>
        public static string Scheme = "urn";

        private readonly UriFormat _format = UriFormat.Unescaped;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="urn">The textual URN</param>
        /// <param name="format">Indicate if already escaped</param>
        public Urn(string urn, UriFormat format = UriFormat.UriEscaped)
        {
            var uri = urn ?? throw new ArgumentNullException(nameof(urn));
            var parts = uri.Split(':');
            if (parts.Length < 3 || !Scheme.Equals(parts[0]))
            {
                throw new UriFormatException($"'{urn}' is not a valid URN");
            }

            Nid = new NamespaceIdentifier(parts[1]);
            var schemeSpecificPart = uri.Substring(uri.IndexOf(parts[1]) + parts[1].Length + 1);
            var nss = SubstringUntil(schemeSpecificPart, "?+", "?=", "#");
            Nss = new NamespaceSpecificString(nss, format);
            Rqf = schemeSpecificPart.Substring(nss.Length);
            Resolution = GetResolutions(Rqf);
            Query = GetQueries(Rqf);
            Fragment = SubstringFrom(Rqf, "#");
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="nid">The NID</param>
        /// <param name="nss">The NSS</param>
        /// <param name="format">Indicate if NSS already escaped</param>
        public Urn(string nid, string nss, UriFormat format = UriFormat.UriEscaped) : this(
            $"{Scheme}{Seperator}{nid}{Seperator}{nss}", format)
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="nid">The NID</param>
        /// <param name="nss">The NSS</param>
        /// <param name="rqf">The RQF parameter</param>
        /// <param name="format">Indicate if NSS already escaped</param>
        public Urn(string nid, string nss, string rqf, UriFormat format = UriFormat.UriEscaped) : this(
            $"{Scheme}{Seperator}{nid}{Seperator}{nss}{rqf}", format)
        {
        }

        /// <summary>
        ///     Indicates that the URN string was escaped before the instance was created
        /// </summary>
        public bool Escaped => _format == UriFormat.UriEscaped;

        /// <summary>
        ///     The NID
        /// </summary>
        public NamespaceIdentifier Nid { get; protected set; }

        /// <summary>
        ///     The NSS
        /// </summary>
        public virtual NamespaceSpecificString Nss { get; protected set; }

        /// <summary>
        ///     The text for RQF
        /// </summary>
        public string Rqf { get; } = string.Empty;

        /// <summary>
        ///     The fragment parameter if any
        /// </summary>
        public string Fragment { get; protected set; } = string.Empty;

        /// <summary>
        ///     The resolution parameters
        /// </summary>
        public IReadOnlyDictionary<string, string> Resolution { get; protected set; } = new Dictionary<string, string>();

        /// <summary>
        ///     The query parameters
        /// </summary>
        public IReadOnlyDictionary<string, string> Query { get; protected set; } = new Dictionary<string, string>();

        /// <summary>
        ///     Gets an array containing the path segments that make up the specified URN
        /// </summary>
        public string[] Segments => new[]
            {Scheme, Nid.ToString(), Nss.ToString(), ToResolutionString(Resolution), ToQueryString(Query), Fragment};

        /// <summary>
        ///     Gets the original URN string that was passed to the Urn constructor
        /// </summary>
        public string OriginalString => $"{Scheme}{Seperator}{Nid}{Seperator}{Nss}{Rqf}";

        /// <inheritdoc />
        public bool Equals(Urn other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Nid.Equals(other.Nid) && Nss.Equals(other.Nss);
        }

        /// <inheritdoc />
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the specified components of the current instance using the specified escaping for special characters
        /// </summary>
        public string GetComponents(UrnComponents flags, UriFormat format)
        {
            var builder = new StringBuilder();
            if ((flags & UrnComponents.Scheme) != 0)
            {
                builder.Append(Scheme);
            }

            if ((flags & UrnComponents.Nid) != 0)
            {
                builder.Append(Nid);
            }

            if ((flags & UrnComponents.Nss) != 0)
            {
                builder.Append(Nss);
            }

            if ((flags & UrnComponents.Resolution) != 0)
            {
                builder.Append(ToResolutionString(Resolution));
            }

            if ((flags & UrnComponents.Query) != 0)
            {
                builder.Append(ToQueryString(Resolution));
            }

            if ((flags & UrnComponents.Fragment) != 0)
            {
                builder.Append(Fragment);
            }

            return builder.ToString();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Scheme}{Seperator}{Nid}{Seperator}{Nss}{ToString(Resolution, Query, Fragment)}";
        }

        /// <summary>
        ///     Creates a new Urn using the specified String instance
        /// </summary>
        public static bool TryCreate(string urnString, UriFormat format, out Urn urn)
        {
            urn = null;
            try
            {
                urn = new Urn(urnString, format);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Indicates whether the string is well-formed by attempting to construct a URN with the string and ensures that the string does not require further escaping.
        /// </summary>
        public static bool IsWellFormedUrnString(string urnString, UriFormat format)
        {
            try
            {
                Urn urn = new Urn(urnString,format);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string SubstringUntil(string s, params string[] delimiters)
        {
            var endIndex = s.Length; // Default to whole string if no occurrence of end delimiter is found.
            foreach (var endDelimiter in delimiters)
            {
                var currentEnd = s.IndexOf(endDelimiter);
                if (currentEnd >= 0 && currentEnd < endIndex)
                {
                    endIndex = currentEnd;
                }
            }

            return s.Substring(0, endIndex);
        }

        private string SubstringFrom(string str, string startDelimiter)
        {
            var startIndex = str.IndexOf(startDelimiter);
            if (startIndex < 0)
            {
                return ""; // Default to empty string if no occurrence of start delimiter is found.
            }

            return str.Substring(startIndex + startDelimiter.Length);
        }

        private IReadOnlyDictionary<string, string> Parse(string component)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(component))
            {
                return parameters;
            }

            foreach (var rComponent in component.Split('&'))
            {
                var kv = rComponent.Split('=');
                var key = kv[0];
                var val = kv.Length == 2 ? kv[1] : "";
                parameters.Add(key, val);
            }

            return parameters;
        }

        private IReadOnlyDictionary<string, string> GetResolutions(string rqfComponents)
        {
            var rComponent = SubstringUntil(SubstringFrom(rqfComponents, "?+"), "?=", "#");
            return Parse(rComponent);
        }

        private IReadOnlyDictionary<string, string> GetQueries(string rqfComponents)
        {
            var rComponent = SubstringUntil(SubstringFrom(rqfComponents, "?="), "#");
            ;
            return Parse(rComponent);
        }

        private string ToResolutionString(IReadOnlyDictionary<string, string> resolutions)
        {
            var sb = new StringBuilder();
            if (resolutions.Count != 0)
            {
                bool first = true;
                sb.Append("?+");
                foreach (var kv in resolutions)
                {
                    if (!first)
                    {
                        sb.Append("&");
                    }

                    sb.Append(kv.Key);
                    if (!string.IsNullOrWhiteSpace(kv.Value))
                        sb.Append('=').Append(kv.Value);
                    first = false;
                }
            }

            return sb.ToString();
        }

        private string ToQueryString(IReadOnlyDictionary<string, string> query)
        {
            var sb = new StringBuilder();
            if (query.Count != 0)
            {
                bool first = true;
                sb.Append("?=");
                foreach (var kv in query)
                {
                    if (!first)
                    {
                        sb.Append("&");
                    }

                    sb.Append(kv.Key);
                    if (!string.IsNullOrWhiteSpace(kv.Value))
                        sb.Append('=').Append(kv.Value);
                    first = false;
                }
            }

            return sb.ToString();
        }

        private string ToString(IReadOnlyDictionary<string, string> resolutions, IReadOnlyDictionary<string, string> queries,
            string fragment)
        {
            var sb = new StringBuilder();
            sb.Append(ToResolutionString(resolutions))
              .Append(ToQueryString(queries));
            if (!string.IsNullOrWhiteSpace(fragment))
            {
                sb.Append('#').Append(fragment);
            }
            return sb.ToString();
        }
    }
}