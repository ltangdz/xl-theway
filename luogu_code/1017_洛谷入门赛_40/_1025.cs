using System;
using System.Linq;
using System.Text;

namespace _1017_洛谷入门赛_40;

public class _1025 {
    public string LexSmallest(string s) {
        string ans = new string(s);
        int n = s.Length;
        for(int i=0; i<n; i++){
            StringBuilder sb = new StringBuilder(s);
            
            char[] c1 = s.Substring(0, i).ToCharArray();
            char[] c2 = s.Substring(i, n - i).ToCharArray();
            
            Array.Reverse(c1);
            sb.Append(c1);
            sb.Append(c2);

            string temp1 = sb.ToString();
            if(ans.CompareTo(temp1) > 0) ans = temp1;
        }        
        
        for(int i=0; i<n; i++){
            StringBuilder sb = new StringBuilder(s);
            
            char[] c1 = s.Substring(n - i, i).ToCharArray();
            char[] c2 = s.Substring(0, n - i).ToCharArray();
            
            Array.Reverse(c1);
            sb.Append(c2);
            sb.Append(c1);
            
            string temp2 = sb.ToString();
            if(ans.CompareTo(temp2) > 0) ans = temp2;
        }
        
        return ans;
    }
}