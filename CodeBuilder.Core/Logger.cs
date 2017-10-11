// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Logging;

namespace CodeBuilder.Core
{
    public class Logger
    {
        private static ILogger log;

        public static void SetLogger(ILogger logger)
        {
            log = logger;
        }

        public static void Write(string message)
        {
            if (log != null)
            {
                log.Info(message);
            }
        }
    }
}
