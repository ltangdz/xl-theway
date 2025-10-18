namespace _1017_洛谷入门赛_40;

using System;
using System.Collections.Generic;
using System.Linq;

public class T681123_筛选查询
{
    public static void Main4()
    {
        int n = Convert.ToInt32(Console.ReadLine());
        Dictionary<int, int> dict = new Dictionary<int, int>();
        List<int> list = new List<int>();
        var line = Console.ReadLine().Split().Select(int.Parse).ToArray();
        for (int i = 0; i < n; i++)
        {
            int v = line[i];
            dict[v] = dict.GetValueOrDefault(v, 0) + 1;
            list.Add(v);
        }
        
        var kx = Console.ReadLine().Split().Select(int.Parse).ToArray();
        int k = kx[0];
        int x = kx[1];
        if (!dict.ContainsKey(x) || dict[x] < k)
        {
            Console.WriteLine("Error");
            return;
        }
        
        int cnt = 1;
        for (int i = 0; i < n; i++)
        {
            if (list[i] == x)
            {
                if(cnt < k) cnt++;
                else
                {
                    Console.WriteLine(i + 1);
                    return;
                }
            }
        }
    }
}