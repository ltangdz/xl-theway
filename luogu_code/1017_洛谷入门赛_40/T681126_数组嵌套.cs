namespace _1017_洛谷入门赛_40;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class T681126_数组嵌套
{
    public static void Main7()
    {
        int n = Convert.ToInt32(Console.ReadLine());
        int[] a = Console.ReadLine().Split().Select(int.Parse).ToArray();
        int[] b = Console.ReadLine().Split().Select(int.Parse).ToArray();
        var line = Console.ReadLine();
        
        Stack<char> st = new Stack<char>();

        for (int i = 0; i < line.Length; i++)
        {
            if(line[i] != '[' && line[i] != ']') st.Push(line[i]);
        }

        int ans = 0;
        while (st.Count > 0)
        {
            int idx = st.Pop() - '0' - 1;
            if (st.Count > 0)
            {
                if (st.Pop() == 'a') ans = a[idx];
                else ans = b[idx];
                st.Push((char)(ans + '0'));
            }
            else
            {
                Console.WriteLine(ans);
                return;
            }
        }
    }
}