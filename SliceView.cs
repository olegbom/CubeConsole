using Terminal.Gui;
using Terminal.Gui.Graphs;

namespace CubeConsole;

public enum SliceDirection
{
    Front, 
    Top,
    Left,
}

public class SliceView: View
{
    public readonly int N;

    private readonly LineCanvas _canvas;
    private readonly Label[,] _labels;

    public SliceView(int n, string name)
    {
        Width = 2 + n * 2 - 1;
        Height = 2 + n + n/2 - 1;

        

       
        N = n;
        _labels = new Label[n, n];

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                _labels[i,j] = new Label()
                {
                    X = 1 + i * 2,
                    Y = 1 + j  + j/2,
                    Text = "a",
                };
                Add(_labels[i,j]);
            }
        }

        _canvas = new LineCanvas();
        for (int i = 0; i <= N; i += 2)
        {
            _canvas.AddLine(new Point(0, i + i/2), N * 2, Orientation.Horizontal, BorderStyle.Single);
            _canvas.AddLine(new Point(i * 2, 0), N + N/2, Orientation.Vertical, BorderStyle.Single);
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

    public override void Redraw(Rect bounds)
    {
        base.Redraw(bounds);
    
        foreach (var p in _canvas.GenerateImage(Bounds))
        {
            AddRune(p.Key.X, p.Key.Y, p.Value);
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
        _labels[row, column].ColorScheme = Colors.Error;
    }

}