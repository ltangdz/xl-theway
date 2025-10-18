namespace newker_templates;

using System;
using System.Collections.Generic;
using System.Linq;

public static class BISHI3_模板_队列操作
{
    public static void Main3(string[] args)
    {
        Queue<int> q = new Queue<int>();
        
        int n = Convert.ToInt32(Console.ReadLine());
        for (int i = 0; i < n; i++)
        {
            string[] input = Console.ReadLine().Split();
            switch (input[0])
            {
                case "1":
                    q.Enqueue(Convert.ToInt32(input[1]));
                    break;
                
                case "2":
                    if(q.Count > 0) q.Dequeue();
                    else Console.WriteLine("ERR_CANNOT_POP");
                    break;
                
                case "3":
                    if (q.Count > 0) Console.WriteLine(q.Peek());
                    else Console.WriteLine("ERR_CANNOT_QUERY");
                    break;
                
                case "4":
                    Console.WriteLine(q.Count);
                    break;
                
                default:
                    break;
            }
        }
    }
}