using System;

namespace CodeBuilder.Core
{
    public class PropertyUnity
    {
        private static Action<object> action;

        public static void Register(Action<object> action)
        {
            PropertyUnity.action = action;
        }

        public static void SetObject(object obj)
        {
            if (action != null)
            {
                action(obj);
            }
        }
    }
}
