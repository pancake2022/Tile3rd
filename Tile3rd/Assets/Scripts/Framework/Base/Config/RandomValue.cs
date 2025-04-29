using System;
using FixMath.NET;

namespace CSFramework
{
    public static class RandomValueExtension
    {
        public static Fix64 RandomValue (this RandomValue growable_value, Random random = null)
        {
            if (growable_value == null)
                return Fix64.Zero;

            if (random == null)
                random = new Random();

            return (Fix64)random.Next((int)growable_value.Min, (int)growable_value.Max);
        }
    }

    public class RandomValue
    {
        public Fix64 Min; // MinValue
        public Fix64 Max; // MaxValue

        public static bool TryParse(string s, out RandomValue result)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var datas = s.Split(',');
                if (datas.Length >= 2)
                {
                    var parem_list = new Object[] { datas[0], null };
                    if (float.TryParse(datas[0], out var min) && 
                        float.TryParse(datas[1], out var max))
                    {
                        result = new RandomValue
                        {
                            Min = (Fix64)min,
                            Max = (Fix64)max,
                        };
                        return true;
                    }
                }
            }
            result = default(RandomValue);
            return false;
        }
    }
}