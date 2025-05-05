using System;

namespace CSFramework
{
    public class StringIntV2
    {
        public string V1;
        public int V2;

        public static bool TryParse(string s, out StringIntV2 result)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var datas = s.Split(',');
                result = new StringIntV2();
                result.V1 = datas[0];
                
                if (datas.Length >= 2)
                {
                    if (!int.TryParse(datas[1], out result.V2))
                    {
                        result = default(StringIntV2);
                        return false;
                    }
                }
                
                return true;
            }
            result = default(StringIntV2);
            return false;
        }
    }
}