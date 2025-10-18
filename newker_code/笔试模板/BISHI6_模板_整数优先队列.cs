namespace newker_templates;
using System;
using System.Collections.Generic;
using System.Linq;

public class BISHI6_模板_整数优先队列
{
    public static void Main6()
    {
        // PriorityQueue<int, int> pq = new PriorityQueue<int, int>();
        SortedSet<(int val, int idx)> ss = new SortedSet<(int val, int idx)>();
        
        int n = int.Parse(Console.ReadLine());
        for (int i = 0; i < n; i++)
        {
            string[] input = Console.ReadLine().Split();
            switch (input[0])
            {
                case "1":
                    // pq.Enqueue(int.Parse(input[1]), int.Parse(input[1]));
                    ss.Add((int.Parse(input[1]), i));
                    break;
                
                case "2":
                    // Console.WriteLine(pq.Peek());
                    Console.WriteLine(ss.Min.val);
                    break;
                
                case "3":
                    // Console.WriteLine(pq.Dequeue());
                    ss.Remove(ss.Min);
                    break;
                
                default:
                    break;
            }
        }
    }
}