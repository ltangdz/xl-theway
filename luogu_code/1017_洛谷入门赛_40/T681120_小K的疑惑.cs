namespace _1017_洛谷入门赛_40;

using System;
public class T681120_小K的疑惑
{
    public static void Main1()
    {
        var ab = Console.ReadLine().Split();
        int a = int.Parse(ab[0]);
        int b = int.Parse(ab[1]);
        Console.WriteLine((a - 1) * (b - 1) - 1);
    }
}