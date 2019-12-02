using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Urn
{
    /// <summary>
    ///     Represents the NSS of a URN
    /// </summary>
    public class NamespaceSpecificString : IEquatable<NamespaceSpecificString>
    {
        private const string Unreserved = "[a-z0-9-._~]";
        private const string PercentEscaped = "%[a-f0-9]{2}";
        private const string Delimiters = "[!$&'()*+,;=]";
        private const string Valid = Unreserved + "|" + PercentEscaped + "|" + Delimiters + "|:|@";
        private readonly string _escaped;
        private readonly Regex _pattern = new Regex($"^({Valid})({Valid}|/|\\?)*$", RegexOptions.IgnoreCase);

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="nss">The text of the NSS</param>
        /// <param name="format">Indicates how text is escape</param>
        /// <exception cref="FormatException"></exception>
        public NamespaceSpecificString(string nss, UriFormat format)
        {
            if (string.IsNullOrEmpty(nss))
            {
                throw new UriFormatException(nameof(nss));
            }

            if (format == UriFormat.UriEscaped)
            {
                if (!_pattern.IsMatch(nss))
                {
                    throw new UriFormatException(nss, new UriFormatException());
                }

                _escaped = LowerCaseOctetPairs(nss);
                Unescaped = Uri.UnescapeDataString(_escaped);
            }
            else
            {
                _escaped = Encode(nss);
                Unescaped = nss;
            }
        }

        /// <summary>
        ///     The raw NSS, unescaped
        /// </summary>
        public string Unescaped { get; }

        /// <inheritdoc />
        public bool Equals(NamespaceSpecificString other)
        {
            return _escaped.Equals(other._escaped, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((NamespaceSpecificString) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _escaped.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _escaped;
        }

        private bool IsReserved(char c)
        {
            return c > 0x80 || c >= 0x01 && c <= 0x20 || c >= 0x7F && c <= 0xFF || c == '%' || c == '/' || c == '?' ||
                   c == '#' || c == '<' || c == '"' || c == '&' || c == '\\' || c == '>' || c == '[' || c == ']' ||
                   c == '^' || c == '`' || c == '{' || c == '|' || c == '}' || c == '~';
        }

        private void AppendEncoded(StringBuilder sb, char c)
        {
            foreach (var b in Encoding.UTF8.GetBytes(c.ToString()))
            {
                sb.Append('%');
                sb.Append(b.ToString("x2"));
            }
        }

        private static string LowerCaseOctetPairs(string s)
        {
            var sb = new StringBuilder(s.Length);

            using (var sr = new StringReader(s))
            {
                int i;
                while ((i = sr.Read()) != -1)
                {
                    var c = (char) i;
                    if (c == '%')
                    {
                        sb.Append('%')
                            .Append(char.ToLowerInvariant((char) sr.Read()))
                            .Append(char.ToLowerInvariant((char) sr.Read()));
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }

            return sb.ToString();
        }

        private string Encode(string s)
        {
            var sb = new StringBuilder();
            foreach (var c in s)
            {
                if (c == 0)
                {
                    throw new FormatException("Illegal character `0` found");
                }

                if (IsReserved(c))
                {
                    AppendEncoded(sb, c);
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}