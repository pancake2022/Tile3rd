using System;

namespace CSFramework
{
    public class IntV6
    {
        public int V1;
        public int V2;
        public int V3;
        public int V4;
        public int V5;
        public int V6;

        public static bool TryParse(string s, out IntV6 result)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var datas = s.Split(',');
                result = new IntV6();
                if (!int.TryParse(datas[0], out result.V1))
                {
                    result = default(IntV6);
                    return false;
                }

                if (datas.Length >= 2)
                {
                    if (!int.TryParse(datas[1], out result.V2))
                    {
                        result = default(IntV6);
                        return false;
                    }
                }

                if (datas.Length >= 3)
                {
                    if (!int.TryParse(datas[2], out result.V3))
                    {
                        result = default(IntV6);
                        return false;
                    }
                }

                if (datas.Length >= 4)
                {
                    if (!int.TryParse(datas[3], out result.V4))
                    {
                        result = default(IntV6);
                        return false;
                    }
                }

                if (datas.Length >= 5)
                {
                    if (!int.TryParse(datas[4], out result.V5))
                    {
                        result = default(IntV6);
                        return false;
                    }
                }

                if (datas.Length >= 6)
                {
                    if (!int.TryParse(datas[5], out result.V6))
                    {
                        result = default(IntV6);
                        return false;
                    }
                }
                
                return true;
            }
            result = default(IntV6);
            return false;
        }
    }
}