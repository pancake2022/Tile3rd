using System;
using FixMath.NET;

namespace CSFramework
{
    public static class GrowableValueExtension
    {
        public static Fix64 Value (this GrowableValue growable_value, int level, int battle_level = 1)
        {
            if (growable_value == null)
                return Fix64.Zero;
                
            var f1 = Fix64.One;
            var f2 = Fix64.One;
            if (growable_value.GF != null)
            {
                for (var i = 1; i < level; ++i)
                    f1 *= growable_value.GF;
            }
            if (growable_value.GF2 != null)
            {
                for (var i = 1; i < battle_level; ++i)
                    f2 *= growable_value.GF2;
            }
            return growable_value.BV * f1 * f2;
        }
    }

    public class GrowableValue
    {
        public Fix64 BV; // BaseValue
        public Fix64 GF; // GrownFactor
        public Fix64 GF2; // GrownFactor2

        public static bool TryParse(string s, out GrowableValue result)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var datas = s.Split('^');
                if (datas.Length >= 3)
                {
                    var parem_list = new Object[] { datas[0], null };
                    if (float.TryParse(datas[0], out var bv) && 
                        float.TryParse(datas[1], out var gf) && 
                        float.TryParse(datas[2], out var gf2))
                    {
                        result = new GrowableValue
                        {
                            BV = (Fix64)bv,
                            GF = (Fix64)gf,
                            GF2 = (Fix64)gf2,
                        };
                        return true;
                    }
                }
                else if (datas.Length == 2)
                {
                    var parem_list = new Object[] { datas[0], null };
                    if (float.TryParse(datas[0], out var bv) && 
                        float.TryParse(datas[1], out var gf))
                    {
                        result = new GrowableValue
                        {
                            BV = (Fix64)bv,
                            GF = (Fix64)gf,
                        };
                        return true;
                    }
                }
                else if (datas.Length == 1)
                {
                    var parem_list = new Object[] { datas[0], null };
                    if (float.TryParse(datas[0], out var bv))
                    {
                        result = new GrowableValue
                        {
                            BV = (Fix64)bv,
                        };
                        return true;
                    }
                }
            }
            result = default(GrowableValue);
            return false;
        }
    }
}