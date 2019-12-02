# urn
Library for representing, parsing and encoding URNs as specified in [RFC 8141].
This library provides classes for representing, parsing and constructing an Uniform Resource Name (URN) 
and its parts Namespace Identifier (NID), Namespace Specific String (NSS) and optional Resolution, Query and Fragment components (RQF)
While URNs look very similiar to URIs and are somewhat related, different equality rules apply for URNs in regard to URN character encoding rules.


## Usage
```C#
var text = "urn:isbn:123?+resolution?=query=x";
var urn = new Urn(text);
```

This code is largely a port of the Java library [urnlib]


[RFC 8141]: https://tools.ietf.org/html/rfc8141
[urnlib]: https://github.com/slub/urnlib/blob/master/README.md
