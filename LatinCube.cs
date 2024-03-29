﻿using System.Numerics;
using System.Text;

namespace CubeConsole;

public static class LatinCube
{

    public static void SwapSlices(byte[,,] cube, int dimension, int a, int b)
    {
        switch (dimension)
        {
            case 0: 
                SwapSlicesX(cube, a, b);
                break;
            case 1:
                SwapSlicesY(cube, a, b);
                break;
            case 2:
                SwapSlicesZ(cube, a, b);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dimension));
        }
    }

    public static void SwapSlicesX(byte[,,] cube, int a, int b)
    {
        for (int i = 0; i < cube.GetLength(1); i++)
        for (int j = 0; j < cube.GetLength(2); j++)
        {
            (cube[a, i, j], cube[b, i, j]) = (cube[b, i, j], cube[a, i, j]);
        }
    }

    public static void SwapSlicesY(byte[,,] cube, int a, int b)
    {
        for (int i = 0; i < cube.GetLength(0); i++)
        for (int j = 0; j < cube.GetLength(2); j++)
        {
            (cube[i, a, j], cube[i, b, j]) = (cube[i, b, j], cube[i, a, j]);
        }
    }

    public static void SwapSlicesZ(byte[,,] cube, int a, int b)
    {
        for (int i = 0; i < cube.GetLength(0); i++)
        for (int j = 0; j < cube.GetLength(1); j++)
        {
            (cube[i, j, a], cube[i, j, b]) = (cube[i, j, b], cube[i, j, a]);
        }
    }




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