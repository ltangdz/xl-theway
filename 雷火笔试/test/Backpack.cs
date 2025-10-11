using System;
using System.Collections.Generic;
using System.Linq;
public class Backpack {
    public static void Main2() {
        int m = int.Parse(Console.ReadLine());
        List<int> ans = new List<int>();

        string[] line = Console.ReadLine().Split();
        int HP = int.Parse(line[0]);
        int lowHP = int.Parse(line[1]);
        int upHP = int.Parse(line[2]);
        int n = int.Parse(line[3]);

        int[] dmg = new int[n];
        string[] s = Console.ReadLine().Split();
        // for(int i=0; i<n; i++) grid[i] = int.Parse(s[i]);
        int x = dmg.Max();
        // 完全背包 我忘了怎么写了
    }
}