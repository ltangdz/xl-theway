using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Hash {
    public static void Main3() {
        var nm = Console.ReadLine().Split();
        int n = int.Parse(nm[0]);
        int m = int.Parse(nm[1]);

        int[][] grid = new int[n][];
        for (int i = 0; i < n; i++) grid[i] = new int[2];

        for (int i = 0; i < n; i++) {
            var temp = Console.ReadLine().Split();
            StringBuilder sb = new StringBuilder();
            foreach (var str in temp) {
                if (str.Trim() != "") {
                    sb.Append(str);
                    sb.Append(' ');
                }
            }
            string[] s = sb.ToString().Split();
            grid[i][0] = int.Parse(s[0]);   // 开始时间
            grid[i][1] = int.Parse(s[1]);   // 伤害
        }

        int ans = 0;

        // Dictionary 存放某个时间段的伤害
        Dictionary<int, int> dict = new Dictionary<int, int>();
        for (int i = 0; i < n; i++) {
            int time = grid[i][0];
            int dmg = grid[i][1];
            for (int k = time; k < m; k++) {
                dict[k] = dict.GetValueOrDefault(k, 0) + dmg;
                ans = Math.Max(ans, dict[k]);
            }
        }

        Console.WriteLine(ans);
    }
}