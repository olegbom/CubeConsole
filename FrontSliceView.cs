using System.Drawing;
using Terminal.Gui;

namespace CubeConsole;

public enum SliceDirection
{
    Front, 
    Top,
    Left,
}

public class FrontSliceView
{
    public readonly int N;

    private readonly Label[,] _labels;
    public readonly FrameView Panel;

    public FrontSliceView(int n, string name)
    {
        Panel = new FrameView()
        {
            Width = 2 + n*3 - 1,
            Height = 2 + n*2,
            
            Border = new Border()
            {
                BorderStyle = BorderStyle.Single,
                DrawMarginFrame = false,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(0),
                Effect3D = false,
                Title = name,
            },
        };

       
        N = n;
        _labels = new Label[n, n];

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                _labels[i,j] = new Label()
                {
                    X = i * 3,
                    Y = j * 2,
                    Text = "a",
                };
                Panel.Add(_labels[i,j]);
            }
        }
    }

    public void SetSlice(byte[,,] cube, int slice, SliceDirection dir)
    {
        if (cube.GetLength(0) != N ||
            cube.GetLength(1) != N ||
            cube.GetLength(2) != N)
        {
            throw new ArgumentOutOfRangeException("bad cube!");
        }

        switch (dir)
        {
            case SliceDirection.Front:
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        _labels[i, j].Text = $"{cube[i, j, slice]}";
                    }
                }
                break;
            case SliceDirection.Top:
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        _labels[i, j].Text = $"{cube[i, slice, N - j - 1]}";
                    }
                }
                break;
            case SliceDirection.Left:
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        _labels[i, j].Text = $"{cube[slice, j, N - i - 1]}";
                    }
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
    }

    public void SetCross(int row, int column)
    {
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                _labels[i, j].ColorScheme = Colors.Base;
            }
        }

        for (int i = 0; i < N; i++)
        {
            _labels[row, i].ColorScheme = Colors.TopLevel;
            _labels[i, column].ColorScheme = Colors.TopLevel;
        }
    }

    public void SetCursor(int row, int column)
    {
        _labels[row, column].ColorScheme = Colors.Error; }

}