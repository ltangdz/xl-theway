using System.Text;

namespace _1025乐牛笔试;

public class 字符串压缩
{
    public string compressString (string param) {
        if(param == "") return "";
        StringBuilder sb = new StringBuilder();

        char c = param[0];
        int cnt = 1;
        for (int i = 1; i < param.Length; i++) {
            if (c != param[i]) {
                sb.Append(c);
                if (cnt > 1) sb.Append(cnt.ToString());
                c = param[i];
                cnt = 1;
            } else {
                cnt++;
            }
        }

        sb.Append(c);
        if (cnt > 1) sb.Append(cnt.ToString());
        return sb.ToString();
    }
}