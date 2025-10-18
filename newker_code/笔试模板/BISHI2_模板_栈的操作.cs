namespace newker_templates;
using System;
using System.Collections.Generic;
using System.Linq;

public static class BISHI2_模板_栈的操作
{
    public static void Main2(string[] args)
    {
        Stack<int> st = new Stack<int>();
        
        int n = Convert.ToInt32(Console.ReadLine());
        for (int i = 0; i < n; i++)
        {
            string[] input = Console.ReadLine().Split();
            switch (input[0])
            {
                case "push":
                    st.Push(Convert.ToInt32(input[1]));
                    break;
                case "pop":
                    if(st.Count > 0) st.Pop();
                    else Console.WriteLine("Empty");
                    break;
                case "query":
                    if(st.Count > 0) Console.WriteLine(st.Peek());
                    else Console.WriteLine("Empty");
                    break;
                case "size":
                    Console.WriteLine(st.Count);
                    break;
                default:
                    break;
            }
        }
    }
}