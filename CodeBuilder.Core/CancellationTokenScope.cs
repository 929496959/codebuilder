// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common;
using System.Threading;

namespace CodeBuilder.Core
{
    public sealed class CancellationTokenScope : Scope<CancellationTokenScope>
    {
        private CancellationToken token;

        public CancellationTokenScope(CancellationToken token)
        {
            this.token = token;
        }

        /// <summary>
        /// 获取当前线程内的请求是否已取消。
        /// </summary>
        public bool IsCancellationRequested
        {
            get
            {
                return token.IsCancellationRequested;
            }
        }
    }
}
