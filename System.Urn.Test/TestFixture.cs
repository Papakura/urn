using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace System.Urn.Test
{
    [TestFixture]
    public class TestFixture
    {
        private static IEnumerable<string> Valid
        {
            get
            {
                yield return "urn:isbn:0451450523";
                yield return "urn:isan:0000-0000-2CEA-0000-1-0000-0000-Y";
                yield return "urn:uuid:6e8bc430-9c3a-11d9-9669-0800200c9a66";
            }
        }

        private static IEnumerable<string> InvalidNid
        {
            get
            {
                yield return "urn:urn:0451450523";
                yield return "urn:i:0000-0000-2CEA-0000-1-0000-0000-Y";
                yield return "urn:012345678901234567890123456789012356:0000-0000-2CEA-0000-1-0000-0000-Y";
                yield return "urn:$%£%£%£%:0000-0000-2CEA-0000-1-0000-0000-Y";
            }
        }

        private static IEnumerable<string> InvalidNss
        {
            get
            {
                yield return "urn:isbn:";
                yield return "urn:isbn:#";
            }
        }

        private static IEnumerable<string> EncodedUrn
        {
            get { yield return "urn:isbn:123%26"; }
        }

        private static IEnumerable<string> Fragment
        {
            get { yield return "urn:isbn:123#fragment"; }
        }

        private static IEnumerable<string> Query
        {
            get { yield return "urn:isbn:123?+resolution=x?=query=x"; }
        }

        [Test]
        public void TestEscaped([ValueSource(nameof(EncodedUrn))] string text)
        {
            Urn urn = null;
            Assert.DoesNotThrow(() => { urn = new Urn(text); });
            urn.Nss.Unescaped.Should().Be("123&");
            urn.Nss.ToString().Should().Be("123%26");
        }

        [Test]
        public void TestFragment([ValueSource(nameof(Fragment))] string text)
        {
            Urn urn = null;
            Assert.DoesNotThrow(() => { urn = new Urn(text); });
            urn.Fragment.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void TestInvalidNid([ValueSource(nameof(InvalidNid))] string text)
        {
            Assert.Throws<UriFormatException>(() =>
            {
                var urn = new Urn(text);
            });
        }

        [Test]
        public void TestInvalidNss([ValueSource(nameof(InvalidNss))] string text)
        {
            Assert.Throws<UriFormatException>(() =>
            {
                var urn = new Urn(text);
            });
        }

        [Test]
        public void TestQuery([ValueSource(nameof(Query))] string text)
        {
            Urn urn = null;
            Assert.DoesNotThrow(() => { urn = new Urn(text); });
            urn.Resolution.Count.Should().NotBe(0);
            urn.Query.Count.Should().NotBe(0);
        }

        [Test]
        public void TestUnescaped([ValueSource(nameof(EncodedUrn))] string text)
        {
            Urn urn = null;
            Assert.DoesNotThrow(() => { urn = new Urn(text, UriFormat.Unescaped); });
            urn.Nss.Unescaped.Should().Be("123%26");
            urn.Nss.ToString().Should().Be("123%2526");
        }

        [Test]
        public void TestValid([ValueSource(nameof(Valid))] string text)
        {
            Assert.DoesNotThrow(() =>
            {
                var urn = new Urn(text);
            });
        }

        [Test]
        public void TestUrnBuilder([ValueSource(nameof(Query))]string text)
        {
            UrnBuilder builder = new UrnBuilder(text,UriFormat.UriEscaped);
            builder.Fragment = "Fragment";
            var result = builder.Urn.ToString();
            result.Should().Contain("#Fragment");
            builder.AddQuery(new KeyValuePair<string, string>("key", "value"));
            builder.Urn.ToString().Should().Contain("?=query=x&key=value");
        }
    }
}