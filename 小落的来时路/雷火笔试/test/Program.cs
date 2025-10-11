using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DFS {
    public static void Main1() {
        var nm = Console.ReadLine().Split();
        int n = int.Parse(nm[0]);
        int m = int.Parse(nm[1]);

        var xy = Console.ReadLine().Split();
        int s_x = int.Parse(xy[0]) - 1;
        int s_y = int.Parse(xy[1]) - 1;

        var zw = Console.ReadLine().Split();
        int e_x = int.Parse(zw[0]) - 1;
        int e_y = int.Parse(zw[1]) - 1;

        int[][] grid = new int[n][];
        for (int i = 0; i < n; i++) grid[i] = new int[m];

        for (int i = 0; i < n; i++) {
            // trim: 去除首尾空格
            // 如何去除中间的多个空格
            var temp = Console.ReadLine().Split();
            StringBuilder sb = new StringBuilder();
            foreach (var str in temp)
            {
                if (str.Trim() != "")
                {
                    sb.Append(str);
                    sb.Append(' ');
                }
            }
            string[] s = sb.ToString().Split();

            for (int j = 0; j < m; j++) {
                grid[i][j] = int.Parse(s[j]);
            }
        }

        // 最开始 先等到水位涨过起点和终点
        int ans = Math.Max(grid[s_x][s_y], grid[e_x][e_y]);
        // bfs 路上如果遍历到水位低于ans 则不可达(visit=true)
        bool[][] vis = new bool[n][];
        for (int i = 0; i < n; i++) {
            vis[i] = new bool[m];
            for (int j = 0; j < m; j++) {
                vis[i][j] = grid[i][j] < ans;
            }
        }

        Queue<(int x, int y)> q = new Queue<(int, int)>();
        q.Enqueue((s_x, s_y));
        vis[s_x][s_y] = true;

        while (vis[e_x][e_y] == false) {
            var(currX, currY) = q.Dequeue();
            for (int dx = -1; dx <= 1; dx++) {
                for (int dy = -1; dy <= 1; dy++) {
                    int x = currX + dx, y = currY + dy;
                    if (x < 0 || x >= n || y < 0 || y >= m || vis[x][y]) continue;
                    if(x == e_x && y == e_y) break;  // 到终点了
                    q.Enqueue((x, y));
                    vis[x][y] = true;
                }
            }

            // 重置vis
            for (int i = 0; i < n; i++) {
                for (int j = 0; j < m; j++) {
                    vis[i][j] = grid[i][j] < ans;
                }
            }

            // 更新ans
            ans++;
        }


        Console.WriteLine(ans);
    }
}