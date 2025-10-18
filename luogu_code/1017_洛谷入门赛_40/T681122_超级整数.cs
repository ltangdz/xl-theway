namespace _1017_洛谷入门赛_40;

using System;

public class T681122_超级整数
{
    public static void Main3()
    {
        int n = int.Parse(Console.ReadLine());
        while (n > 10)
        {
            if (n % 10 != 0)
            {
                Console.WriteLine("No");
                return;
            }
            n /= 10;
        }
        Console.WriteLine("Yes");
    }
}