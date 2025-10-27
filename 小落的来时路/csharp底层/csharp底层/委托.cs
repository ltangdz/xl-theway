namespace csharp底层;

class DelegateTest
{
    // 1.定义委托类型，指向一个参数为string，无返回值的方法
    private delegate void MyDelegate(string message);
    
    // 定义一个委托，指向一个参数为2个int，返回值为int的方法
    private delegate int MathOperationDelegate(int a, int b);
    
    // 疑问：以下两种有什么区别
    public static void Test3()
    {
        MathOperationDelegate del1 = (a, b) => a + b;
        MathOperationDelegate del2 = (a, b) => a * b;
        MathOperationDelegate multiDel = del1 + del2 + del1;
        
        // int MyAdd(int a, int b) => a + b;
        
        Console.WriteLine(del1(1, 2));
        Console.WriteLine(del2(1, 2));
        Console.WriteLine(multiDel(1, 2));
        // Console.WriteLine(MyAdd(1, 2));
    }
    
    public static void Test1()
    {
        // 2.创建委托实例并赋值
        MyDelegate del1 = new MyDelegate(MyMethod);
        MathOperationDelegate del2 = new MathOperationDelegate(Add);
        MathOperationDelegate del3 = (a, b) => a * b;
        
        // 3.调用委托
        Console.WriteLine(typeof(MyDelegate));
        Console.WriteLine(typeof(MathOperationDelegate));
        del1("Hello World!");
        Console.WriteLine(del2(114514, 1919810));
        Console.WriteLine(del3(114, 514));
    }

    private static int Add(int a, int b)
    {
        return a + b;
    }

    // 4.方法匹配委托签名
    private static void MyMethod(string message)
    {
        Console.WriteLine(message);
    }
    
    // 5.多播委托
    public static void Test2()
    {
        MyDelegate del1 = new MyDelegate(Method1);
        MyDelegate del2 = new MyDelegate(Method2);
        MyDelegate multiDel = del1 + del2 + del2;
        multiDel("Hello from 多播委托 ");
        // multiDel -= del1;
        // multiDel -= del2;
        // multiDel -= del2;
        multiDel = null;
        multiDel?.Invoke("Hello from 多播委托 ");
        
        // 6.匿名方法
        MyDelegate del3 = delegate(string msg)
        {
            Console.WriteLine(msg);
        };
        del3("Hello from 匿名方法");
        
        // 7.Lambda表达式
        // MyDelegate del4 = (msg) =>
        // {
        //     Console.WriteLine(msg);
        // };        
        MyDelegate del4 = Console.WriteLine;
        del4("Hello from Lambda表达式");
    }

    private static void Method2(string message)
    {
        Console.WriteLine(message + nameof(Method2));
    }

    private static void Method1(string message)
    {
        Console.WriteLine(message + nameof(Method1));
    }

    // private delegate void EventDelegate(string msg);
    private event MyDelegate MyEvent;
    
    private int cnt = 0;

    private void TriggerEvent()
    {
        MyEvent?.Invoke("Event triggered! For the " + cnt++ + " time"); 
    }
    
    public static void Test4() 
    {
        
        DelegateTest test = new DelegateTest();
        test.MyEvent += Console.WriteLine;
        test.MyEvent += Console.WriteLine;
        test.MyEvent += Console.WriteLine;
        test.TriggerEvent();
        test.MyEvent -= Console.WriteLine;
        test.TriggerEvent();
    }

}