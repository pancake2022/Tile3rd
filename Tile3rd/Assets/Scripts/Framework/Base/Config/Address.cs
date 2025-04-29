using System;

namespace CSFramework
{
    public class Address
    {
        public string IP;
        public int Port;

        public static bool TryParse(string s, out Address result)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var datas = s.Split(':');
                result = new Address
                {
                    IP = datas[0]
                };

                if (datas.Length >= 2)
                {
                    if (!int.TryParse(datas[1], out result.Port))
                    {
                        result = default(Address);
                        return false;
                    }
                }
                
                return true;
            }
            result = default(Address);
            return false;
        }
    }
}