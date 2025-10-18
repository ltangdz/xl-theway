using System;
using System.Linq;

namespace _1017_洛谷入门赛_40;

public class T681127_下落模拟
{
    public static void Main()
    {
        var nm = Console.ReadLine().Split().Select(int.Parse).ToArray();
        var n = nm[0];
        var m = nm[1];
        char[][] grid = new char[n][];
        for (int i = 0; i < n; i++)
        {
            grid[i] = new char[m];
        }
        
        for (int i = 0; i < n; i++)
        {
            var line = Console.ReadLine();
            for (int j = 0; j < m; j++)
            {
                grid[i][j] = line[j];
                if (i >= 1)
                {
                    if (grid[i - 1][j] != '.' && grid[i - 1][j] != '_')
                    {
                        if (grid[i][j] != '_')
                        {
                            grid[i][j] = grid[i - 1][j];
                            grid[i - 1][j] = '.';
                        }
                    }
                }
            }
        }

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                Console.Write(grid[i][j]);
            }

            if (i != n - 1) Console.WriteLine();
        }
    }
}