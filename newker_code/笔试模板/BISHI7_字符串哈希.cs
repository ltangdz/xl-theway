namespace newker_templates;


using System;
using System.Collections.Generic;

public class BISHI7_字符串哈希
{
    public static void Main7()
    {
        Dictionary<string, int> dict = new Dictionary<string, int>();
        
        int n = Convert.ToInt32(Console.ReadLine());
        for (int i = 0; i < n; i++)
        {
            string s = Console.ReadLine();
            dict[s] = dict.GetValueOrDefault(s, 0) + 1;
        }
        
        Console.WriteLine(dict.Count);
    }
}