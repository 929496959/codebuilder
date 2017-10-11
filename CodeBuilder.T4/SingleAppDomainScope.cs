// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common;
using System;

namespace CodeBuilder.T4
{
    public class SingleAppDomainScope : Scope<SingleAppDomainScope>
    {
        private AppDomain domain;

        public AppDomain AppDomain
        {
            get
            {
                return domain ?? (domain = AppDomain.CreateDomain("Generation App Domain"));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (domain != null)
            {
                AppDomain.Unload(domain);
            }

            base.Dispose(disposing);
        }
    }
}
