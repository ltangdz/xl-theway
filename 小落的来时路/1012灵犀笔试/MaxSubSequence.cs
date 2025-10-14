using System.Text.RegularExpressions;

namespace 灵犀笔试;

public class MaxSubSequence
{
    public static void Main3()
    {
        var nk = Console.ReadLine().Split();
        int n = int.Parse(nk[0]);
        int k = int.Parse(nk[1]);
        
        // 找到子串 满足
        // 1. 所有元素和为偶数
        // 2. 任意两个相邻元素绝对值 <= k
        // 变长滑动窗口 + 前缀和
        int[] seq = Regex.Replace(Console.ReadLine(), @"\s{2,}", " ").Split().Select(int.Parse).ToArray();

        int maxLen = 0, len = 0, left = 0;
        
        int[] pre1 = new int[n+1];
        int[] pre2 = new int[n+1];
        int[] diff = new int[n];
        diff[0] = 0;

        for (int i = 1; i < n; i++)
        {
            diff[i] = Math.Abs(seq[i] - seq[i-1]) <= k ? 0 : 1;
        }
        
        for (int i = 1; i <= n; i++)
        {
            pre1[i] = pre1[i-1] + seq[i-1];
            pre2[i] = pre2[i-1] + diff[i-1];
        }
        
        // 窗口长度 从n开始
        int size = n;
        for (int i = size; i > 0; i--)
        {
            for (int right = size - 1; right < n; right++)
            {
                left = right - i + 1;
                if (pre2[right+1] - pre2[left] > 0 || (pre1[right+1] - pre1[left])% 2 != 0) continue;
                
                // result
                maxLen = i;
                Console.WriteLine(maxLen);
                return;
            }
            
        }
        Console.WriteLine(maxLen);
    }

}