using System;
using System.Collections.Generic;

namespace CodeBuilder.T4
{
    [Serializable]
    public class GuidDispatcher : MarshalByRefObject
    {
        private Dictionary<string, Guid> dic = new Dictionary<string, Guid>();

        public Guid this[string key]
        {
            get
            {
                Guid guid;
                if (!dic.TryGetValue(key, out guid))
                {
                    guid = Guid.NewGuid();
                    dic.Add(key, guid);
                }

                return guid;
            }
        }
    }
}
