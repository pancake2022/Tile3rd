using System.Collections.Generic;

namespace CSFramework
{
    public class TimelineItem
    {
        public float Time { get; set; }
        public int Type { get; set; }
        public List<int> Foods { get; set; }

        public static bool TryParse(string s, out TimelineItem item)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var datas = s.Split('#');
                item = new TimelineItem();
                if (float.TryParse(datas[0], out var time))
                {
                    item.Time = time;
                }
                else
                {
                    item = default(TimelineItem);
                    return false;
                }

                if (datas.Length >= 2)
                {
                    if (int.TryParse(datas[1], out var type))
                    {
                        item.Type = type;
                    }
                    else
                    {
                        item = default(TimelineItem);
                        return false;
                    }
                }

                if (datas.Length >= 3)
                {
                    var food_datas = datas[2].Split(',');
                    item.Foods = new List<int>();
                    foreach (var food_data in food_datas)
                    {
                        if (int.TryParse(food_data, out var food_id))
                        {
                            item.Foods.Add(food_id);
                        }
                        else
                        {
                            item = default(TimelineItem);
                            return false;
                        }
                    }
                }
                
                return true;
            }
            item = default(TimelineItem);
            return false;
        }

        public static string ToStr (TimelineItem item)
        {
            return $"{item.Time}#{item.Type}#{string.Join(',', item.Foods)}";
        }
    }
}