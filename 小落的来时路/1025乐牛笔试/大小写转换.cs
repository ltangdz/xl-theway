namespace _1025乐牛笔试;

public class 大小写转换
{
    public static void Mai2n()
    {
        string s;
        while ((s = Console.ReadLine()) != null)
        {
            if (s[0] >= 'a' && s[0] <= 'z')
            {
                Console.WriteLine((char)(s[0] - 32));
            }else if (s[0] >= 'A' && s[0] <= 'Z')
            {
                Console.WriteLine((char)(s[0] + 32));
            }
        }
    }
}