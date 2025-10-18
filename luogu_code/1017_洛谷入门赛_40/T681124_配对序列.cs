namespace _1017_洛谷入门赛_40;

using System;
using System.Linq;

public class T681124_配对序列
{
    public static void Main5()
    {
        int T = int.Parse(Console.ReadLine());
        {
            for (int k = 0; k < T; k++)
            {
                int n = Convert.ToInt32(Console.ReadLine());
                int[] seq = new int[n];
                bool flag = true;
                var line = Console.ReadLine().Split().Select(int.Parse).ToArray();
                for (int i = 1; i <= n; i++)
                {
                    seq[i - 1] = line[i - 1];
                    if (i > 0 && i % 2 == 1 && seq[i] == seq[i - 1])
                    {
                        Console.WriteLine("No");
                        flag = false;
                        break;
                    }

                    if (i > 0 && i % 2 == 0 && seq[i] != seq[i - 1])
                    {
                        Console.WriteLine("No");
                        flag = false;
                        break;
                    }
                }

                if(flag) Console.WriteLine("Yes");
            }
        }
    }
}