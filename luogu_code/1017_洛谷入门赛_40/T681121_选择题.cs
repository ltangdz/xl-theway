namespace _1017_洛谷入门赛_40;


using System;
using System.Collections.Generic;
using System.Linq;

public class T681121_选择题
{
    public static void Main2()
    {
        Dictionary<int, char> dict = new Dictionary<int, char>();
        var line = Console.ReadLine().Split();
        for (int i = 0; i < 3; i++)
        {
            if (!dict.ContainsKey(int.Parse(line[i])))
            {
                dict.Add(int.Parse(line[i]), (char)('A' + i));
            }
            else
            {
                Console.WriteLine("Report");
                return;
            }
        }

        if (dict.ContainsKey(int.Parse(line[3])))
        {
            Console.WriteLine(dict[int.Parse(line[3])]);
        }
        else
        {
            Console.WriteLine(dict.OrderBy(kv => kv.Key).Select(kv => kv.Value).ToList()[1]);
        }
    }
}