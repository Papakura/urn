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
    }
}
