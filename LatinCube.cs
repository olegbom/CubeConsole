using System.Numerics;
using System.Text;

namespace CubeConsole;

public static class LatinCube
{
    public static string ToIndicator(byte v)
    {
        int count = BitOperations.PopCount(v);
        return count switch
        {
            > 1 => "*",
            1 => v switch
            {
                0x01 => "1",
                0x02 => "2",
                0x04 => "3",
                0x08 => "4",
                0x10 => "5",
                0x20 => "6",
                0x40 => "7",
                0x80 => "8",
                _ => "x"
            },
            _ => "x"
        };
    }

    public static string ToPossibility(byte v)
    {
        StringBuilder sb = new(9);
        for (int i = 0; i < 8; i++)
        {
            if ((v & (1 << i)) == 0)
            {
                sb.Append(' ');
            }
            else
            {
                sb.Append(i + 1);
            }
        }

        return sb.ToString();
    }
}