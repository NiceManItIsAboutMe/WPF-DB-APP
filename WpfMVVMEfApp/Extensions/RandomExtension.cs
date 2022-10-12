using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    internal static class RandomExtension
    {
        public static T NextItem<T>(this Random random, params T[] items)=> items[random.Next(items.Length)];
    }
}
