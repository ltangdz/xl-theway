namespace 灵犀笔试;

public class ChestHunter
{
    public static void Main2()
    {
        var nm = Console.ReadLine().Split();
        int n = int.Parse(nm[0]);
        int m = int.Parse(nm[1]);
        
        // 提前判断
        if (n > m)
        {
            Console.WriteLine("No");
            return;
        }
        
        List<int> abilities = Console.ReadLine().Split().Select(int.Parse).ToList();
        List<int> chests = Console.ReadLine().Split().Select(int.Parse).ToList();

        // 贪心 从能力值最低的开始分配 每次让最低能力值的先选
        // 如果n > m 直接return false
        // 如果最后宝箱还有剩余 返回false
        
        bool ans = false;
        
        // 能力值 宝箱价值 分别升序排序
        abilities.Sort();
        chests.Sort();

        for (int i = 0; i < n - 1; i++)
        {
            int ability = abilities[i];
            for (int j = 0; j < chests.Count; j++)
            {
                int value = chests[j];
                if (ability >= value)
                {
                    chests.RemoveAt(j);
                    break;
                }
                
                // 该勇士没有打开箱子
                Console.WriteLine("No");
                return;
            }
        }
        
        ans = abilities[^1] >= chests[^1];
        Console.WriteLine(ans ? "Yes" : "No");
    }

}