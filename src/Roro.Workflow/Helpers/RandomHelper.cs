using System;

namespace Roro.Workflow
{
    internal class RandomHelper
    {
        private static Random _random = new Random();

        public static int Next(int minValue, int maxValue) => _random.Next(minValue, maxValue);
    }
}
