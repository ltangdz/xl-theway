using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RPC {
    public static void Main() {
        int n = int.Parse(Console.ReadLine());
        int[] id = new int[n];
        string[] name = new string[n];
        string[] def = new string[n];

        for (int i = 0; i < n; i++) {
            var temp = Console.ReadLine().Split();
            StringBuilder sb = new StringBuilder();
            foreach (var str in temp) {
                if (str.Trim() != "") {
                    sb.Append(str);
                    sb.Append(' ');
                }
            }
            string[] s = sb.ToString().Trim().Split();
            id[i] = int.Parse(s[0]);
            name[i] = s[1];
            def[i] = s[2];
        }

        // rpc数据
        string input = Console.ReadLine();
        List<string> output = new List<string>();

        // 模拟
        int ptr = 0;
        while(ptr < input.Length){
            string rpcResolve = "";

            // 1.读rpc编号
            string no = "";
            int num;
            for(int i=0; i<2; i++){
                no += input[ptr++];
            }
            num = int.Parse(no);
            rpcResolve += name[num];
            rpcResolve += "(";

            // 2.读参数定义
            string defnition = def[num];

            // 3.获取字符串类型参数的个数n
            int cnt = 0;
            for(int i=0; i<defnition.Length; i++){
                if(defnition[i] == 's') cnt++;
            }

            // 4.读n个字符串参数的长度(1Byte * length)
            int[] lengths = new int[cnt];
            for(int i=0; i<cnt; i++){
                string t = "";
                t += input[ptr++];
                t += input[ptr++];
                lengths[i] = int.Parse(t);
            }

            // 5.读参数
            int p1 = 0;
            for(int i=0; i<defnition.Length; i++){
                // 整型
                if(defnition[i] == 'i'){
                    string k = "";
                    for(int j=0; j<4; j++){
                        k += input[ptr++];
                        k += input[ptr++];
                    }
                    rpcResolve += int.Parse(k).ToString();
                }
                // 字符串型
                else{
                    rpcResolve += "\"";
                    for(int j=0; j<lengths[p1++]; j++){
                        rpcResolve += input[ptr++];
                        rpcResolve += input[ptr++];
                    }
                    rpcResolve += "\"";
                }
                if(i != defnition.Length - 1){
                    rpcResolve += ",";
                }
            }

            // 6.解析后把结果加入output
            output.Add(rpcResolve);
        }

        foreach(string str in output){
            Console.WriteLine(str);
        }
    }
}