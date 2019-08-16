namespace LiveReloading.Middleware.Shared
{
    using System;
    using System.Linq;

    public static class FunctionalExtensions
    {
        public static void Using<T>(this T value, Action<T> action) where T : IDisposable
        {
            using (value)
                action(value);
        }


        public static byte[] Combined(params byte[][] args)
        {
            byte[] result = new byte[args.Sum(v => v.Length)];
            int offset = 0;
            foreach (var arg in args)
            {
                System.Buffer.BlockCopy(arg, 0, result, offset, arg.Length);
                offset += arg.Length;
            }
            return result;
        }

    }
}
