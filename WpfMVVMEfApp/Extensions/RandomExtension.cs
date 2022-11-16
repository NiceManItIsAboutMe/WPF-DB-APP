namespace System
{
    internal static class RandomExtension
    {
        public static T NextItem<T>(this Random random, params T[] items) => items[random.Next(items.Length)];
    }
}
