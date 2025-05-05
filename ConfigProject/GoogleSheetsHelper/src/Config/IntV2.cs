using System;

namespace CSFramework
{
    public class IntV2
    {
        public int V1;
        public int V2;

        public static bool TryParse(string s, out IntV2 result)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var datas = s.Split(',');
                result = new IntV2();
                if (!int.TryParse(datas[0], out result.V1))
                {
                    result = default(IntV2);
                    return false;
                }

                if (datas.Length >= 2)
                {
                    if (!int.TryParse(datas[1], out result.V2))
                    {
                        result = default(IntV2);
                        return false;
                    }
                }
                
                return true;
            }
            result = default(IntV2);
            return false;
        }
    }
}