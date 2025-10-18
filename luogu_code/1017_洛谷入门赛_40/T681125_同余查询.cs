namespace _1017_洛谷入门赛_40;


using System;
using System.Collections.Generic;
using System.Linq;
public class T681125_同余查询
{
    public static void Main6()
    {
        var nq = Console.ReadLine().Split().Select(int.Parse).ToArray();
        var n = nq[0];
        var q = nq[1];
        
        var line = Console.ReadLine().Split().Select(int.Parse).ToArray();
        for (int i = 0; i < q; i++)
        {
            HashSet<int> set = new HashSet<int>();
            int x = Convert.ToInt32(Console.ReadLine());
            for (int j = 0; j < n; j++)
            {
                set.Add(line[j] % x);
            }
            Console.WriteLine(set.Count);
        }
    }
}