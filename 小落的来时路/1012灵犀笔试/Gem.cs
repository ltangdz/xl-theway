namespace 灵犀笔试;

public class Gem
{
    public static void Main1()
    {
        var nm = Console.ReadLine().Split();
        int n = int.Parse(nm[0]);
        int m = int.Parse(nm[1]);
        
        List<int> abilities = Console.ReadLine().Split().Select(int.Parse).ToList();
        List<int> hardness = Console.ReadLine().Split().Select(int.Parse).ToList();

        int ans = 0;
        
        // 把能力和宝石硬度分别降序排序
        abilities.Sort((a, b) => b - a);        
        hardness.Sort((a, b) => b - a);

        foreach (int ability in abilities)
        {
            for (int i=0 ; i < hardness.Count ; i++)
            {
                int hard = hardness[i];
                if (ability >= hard)
                {
                    hardness.Remove(hard);
                    ans++;
                    break;
                }
            }
        }
        
        Console.WriteLine(ans);
    }
    
    public static void  Main()
    {
        Console.WriteLine((int)'A');
        Console.WriteLine((int)'0');
    
    }
}