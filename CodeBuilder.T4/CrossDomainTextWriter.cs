// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace CodeBuilder.T4
{
    [Serializable]
    public sealed class CrossDomainTextWriter : MarshalByRefObject
    {
        private CrossDomainTextWriter remoteTracer;

        public CrossDomainTextWriter()
        {
        }

        public CrossDomainTextWriter(AppDomain domain, TextWriter writer)
        {
            this.remoteTracer = domain.CreateInstanceFrom(Assembly.GetExecutingAssembly().Location, typeof(CrossDomainTextWriter).FullName).Unwrap() as CrossDomainTextWriter;
            if (remoteTracer != null)
            {
                remoteTracer.StartListening(this, writer);
            }
        }

        public void StartListening(CrossDomainTextWriter farTracer, TextWriter writer)
        {
            this.remoteTracer = farTracer;
            Console.SetOut(writer);
        }
    }
}
