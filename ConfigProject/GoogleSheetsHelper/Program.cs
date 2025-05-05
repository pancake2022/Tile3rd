using System;

namespace GoogleSheetsHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            // Helper.WriteLocalizeToRemote(); return;
            // MagicMapConfigConvert.Convert();

            if (args.Length > 0)
            {
                var a1 = args[0];
                if (a1 == "WriteToRemote")
                {
                    Helper.WriteToRemote("res/tile3rd");
                }
                else
                {
                    Helper.ReadFromRemote("res/tile3rd", "out/cs/Config", a1);
                }
            }
            else
            {
                Helper.ReadFromRemote("res/tile3rd", "out/cs/Config");
            }

            // Helper.ReadFromRemote("res/tile3rd", "out/cs/Config", "MapConfig");
        }
    }
}
