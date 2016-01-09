using System;

namespace NTephra2.Core
{
    public static class Guard
    {
        public static T NotNull<T>(T e, string name)
        {
            if (e == null)
            {
                throw new ArgumentNullException(name);
            }
            return e;
        }
    }
}
