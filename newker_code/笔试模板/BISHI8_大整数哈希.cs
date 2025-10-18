namespace newker_templates;

using System;
using System.Collections.Generic;
using System.Linq;

public class BISHI8_大整数哈希 {
    private static ulong MOD = (ulong)Math.Pow(2, 64);

    public static void Main8() {
        int n = Convert.ToInt32(Console.ReadLine());
        ulong ans = 0;
        Dictionary<ulong, ulong> dict = new Dictionary<ulong, ulong>();

        for (int i = 1; i <= n; i++) {
            var line = Console.ReadLine().Split().Select(ulong.Parse).ToArray();
            ulong a = line[0];
            var tmp = dict.ContainsKey(a) ? dict[a] : 0;
            ans += (ulong)i * tmp;
            ulong b = line[1];
            dict[a] = b;
        }

        Console.WriteLine(ans % MOD);
    }
}