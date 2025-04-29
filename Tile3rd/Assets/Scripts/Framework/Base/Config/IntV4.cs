using System;

namespace CSFramework
{
    public class IntV4
    {
        public int V1;
        public int V2;
        public int V3;
        public int V4;

        public static bool TryParse(string s, out IntV4 result)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var datas = s.Split(',');
                result = new IntV4();
                if (!int.TryParse(datas[0], out result.V1))
                {
                    result = default(IntV4);
                    return false;
                }

                if (datas.Length >= 2)
                {
                    if (!int.TryParse(datas[1], out result.V2))
                    {
                        result = default(IntV4);
                        return false;
                    }
                }

                if (datas.Length >= 3)
                {
                    if (!int.TryParse(datas[2], out result.V3))
                    {
                        result = default(IntV4);
                        return false;
                    }
                }

                if (datas.Length >= 4)
                {
                    if (!int.TryParse(datas[3], out result.V4))
                    {
                        result = default(IntV4);
                        return false;
                    }
                }
                
                return true;
            }
            result = default(IntV4);
            return false;
        }
    }
}