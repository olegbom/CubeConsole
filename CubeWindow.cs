using System.Diagnostics;
using System.Numerics;
using Terminal.Gui;

namespace CubeConsole;

public class CubeWindow:Window
{

    public readonly int N = 8;

    private byte[,,] Cube;

    private readonly SliceView _frontSlice;

    private readonly SliceView _topSlice;

    private readonly SliceView _leftSlice;

    private readonly Label _possibilityLabel;
    

    private (int x, int y, int z) _selected;
    public (int x, int y, int z) Selected
    {
        get => _selected;
        set
        {
            if (_selected == value)
                return;
            _selected = value;
            OnSelectedChange();
        }
    }

    public void OnSelectedChange()
    {
        _frontSlice.SetSlice(Cube, _selected.z, SliceDirection.Front);
        _frontSlice.SetCross(_selected.x, _selected.y);
        _frontSlice.SetCursor(_selected.x, _selected.y);
        
        _topSlice.SetSlice(Cube, _selected.y, SliceDirection.Top);
        _topSlice.SetCross(_selected.x, N - _selected.z - 1);
        _topSlice.SetCursor(_selected.x, N - _selected.z - 1);
        
        _leftSlice.SetSlice(Cube, _selected.x, SliceDirection.Left);
        _leftSlice.SetCross(N - _selected.z - 1, _selected.y);
        _leftSlice.SetCursor(N - _selected.z - 1, _selected.y);
        
        _possibilityLabel.Text = LatinCube.ToPossibility(Cube[_selected.x, _selected.y, _selected.z]);
    }


    public CubeWindow()
    {
        Title = "CubeConsole (Ctrl+Q to quit)";

        Cube = new byte[N, N, N];

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                for (int k = 0; k < N; k++)
                {
                    Cube[i, j, k] = 0xFF;
                }
            }
        }

        CubeStack cubeStack = new CubeStack();


        _frontSlice = new SliceView(N, "Front");
        _frontSlice.X = 2;
        _frontSlice.Y = 1;
        _frontSlice.SetSlice(Cube, 0, SliceDirection.Front);
        Add(_frontSlice);

        _topSlice = new SliceView(N, "Top");
        _topSlice.X = 2;
        _topSlice.Y = Pos.Bottom(_frontSlice);
        _topSlice.SetSlice(Cube, 0, SliceDirection.Top);
        Add(_topSlice);

        _leftSlice = new SliceView(N, "Left");
        _leftSlice.X = Pos.Right(_topSlice) + 1;
        _leftSlice.Y = 1;
        _leftSlice.SetSlice(Cube, 0, SliceDirection.Left);
        Add(_leftSlice);

        _possibilityLabel = new Label()
        {
            X = Pos.Right(_topSlice) + 2,
            Y = Pos.Bottom(_frontSlice) + 1,
            Width = 8,
            Border = new Border()
            {
                Title = "valid",
                BorderStyle = BorderStyle.Single,
            },
        };
        Add(_possibilityLabel);

        TryRecursiveSolver(cubeStack, 4, 0, 0);
        
        OnSelectedChange();

        KeyPress += args =>
        {
            switch (args.KeyEvent.Key)
            {
                case Key.CursorLeft:
                    if (Selected.x > 0) Selected = _selected with { x = _selected.x - 1 };
                    args.Handled = true;
                    break;
                case Key.CursorRight:
                    if (Selected.x < N - 1) Selected = _selected with { x = _selected.x + 1 };
                    args.Handled = true;
                    break;
                case Key.CursorUp:
                    if (Selected.y > 0) Selected = _selected with { y = _selected.y - 1 };
                    args.Handled = true;
                    break;
                case Key.CursorDown:
                    if (Selected.y < N - 1) Selected = _selected with { y = _selected.y + 1 };
                    args.Handled = true;
                    break;
                case Key.PageDown:
                    if (Selected.z > 0) Selected = _selected with { z = _selected.z - 1 };
                    args.Handled = true;
                    break;
                case Key.PageUp:
                    if (Selected.z < N - 1) Selected = _selected with { z = _selected.z + 1 };
                    args.Handled = true;
                    break;
                default:
                    if (args.KeyEvent.KeyValue is >= (int)Key.D1 and <= (int)Key.D8)
                    {
                        SetValueToSelectedCell((byte)(1 << (args.KeyEvent.KeyValue - (int)Key.D1)));
                        args.Handled = true;
                    }
                    break;
            }
        };
    }

    private int count = 0;

    private bool TryRecursiveSolver(CubeStack stack, int x, int y, int z)
    {
        
        bool NextValue()
        {
            z++;
            if (z == 8)
            {
                z = 0;
                y++;
                if (y == 8)
                {
                    y = 0;
                    x++;
                    if (x == 8)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        byte value = Cube[x, y, z];
        
        while (BitOperations.PopCount(value) <= 1)
        {
            if (!NextValue())
                return false;
            value = Cube[x, y, z];
        }
        
        Span<int> indices = stackalloc int[8];
        int index = 0;
        for (int j = 0; j < 8; j++)
        {
            if ((value & (1 << j)) > 0)
            {
                indices[index] = j;
                index++;
            }
        }

        bool isGood = false;
        Random.Shared.Shuffle(indices.Slice(0,index));
        for (int k = 0; k < index; k++)
        {
            stack.Push(Cube);
            if (TrySetValueToCell((byte)(1 << indices[k]), x, y, z))
            {
                if (!TryRecursiveSolver(stack, x, y, z)) 
                    return false;
            }
            stack.Pop(Cube);
        }

        if (!isGood)
        {
            count++;
        }
    
        return true;
    }

    private bool IsCubeHaveZero()
    {
        for (int i = 0; i < 8; i++)
        for (int k = 0; k < 8; k++)
        for (int j = 0; j < 8; j++)
        {
            if (Cube[i, j, k] == 0) 
                return true;
        }
        return false;
    }

    public void SetValueToSelectedCell(byte value)
    {
        var (x, y, z) = (Selected.x, Selected.y, Selected.z);
        TrySetValueToCell(value, x, y, z);
        OnSelectedChange();
    }

    private bool TrySetValueToCell(byte value, int x, int y, int z)
    {
        Cube[x, y, z] = value;
        for (int i = 0; i < N; i++)
        {
            if (i != x)
            {

                if (!TryRemoveProbability(value, i, y, z)) return false;
            }

            if (i != y)
            {
                if (!TryRemoveProbability(value, x, i, z)) return false;
            }

            if (i != z)
            {
                if (!TryRemoveProbability(value, x, y, i)) return false;
            }
        }

        for (int i = x / 2 * 2; i < (x + 2) / 2 * 2; i++)
        for (int j = y / 2 * 2; j < (y + 2) / 2 * 2; j++)
        for (int k = z / 2 * 2; k < (z + 2) / 2 * 2; k++)
        {
            if (i != x || j != y || k != z)
            {
                if (!TryRemoveProbability(value, i, j, k)) return false;
            }
        }

        Span<int> counts = stackalloc int[N];

        bool CountsCheck(Span<int> counts)
        {
            for (int i = 0; i < N; i++)
                if (counts[i] < 1)
                    return false;

            return true;
        }

        for (int i = 0; i < N; i++) ProbCount(i, y, z, counts);
        if (!CountsCheck(counts)) return false;


        for (int i = 0; i < N; i++)
        {
            if (counts[i] == 1)
            {
                for (int j = 0; j < N; j++)
                {
                    if (BitOperations.PopCount(Cube[j, y, z]) > 1 && (Cube[j, y, z] & (1 << i)) != 0)
                    {
                        if (!TrySetValueToCell((byte)(1 << i), j, y, z)) return false;
                    }
                }
            }
        }

        counts.Clear();

        for (int i = 0; i < N; i++) ProbCount(x, i, z, counts);
        if (!CountsCheck(counts)) return false;

        for (int i = 0; i < N; i++)
        {
            if (counts[i] == 1)
            {
                for (int j = 0; j < N; j++)
                {
                    if (BitOperations.PopCount(Cube[x, j, z]) > 1 && (Cube[x, j, z] & (1 << i)) != 0)
                    {
                        if (!TrySetValueToCell((byte)(1 << i), x, j, z)) return false;
                    }
                }
            }
        }

        counts.Clear();

        for (int i = 0; i < N; i++) ProbCount(x, y, i, counts);
        if (!CountsCheck(counts)) return false;

        for (int i = 0; i < N; i++)
        {
            if (counts[i] == 1)
            {
                for (int j = 0; j < N; j++)
                {
                    if (BitOperations.PopCount(Cube[x, y, j]) > 1 && (Cube[x, y, j] & (1 << i)) != 0)
                    {
                        if (!TrySetValueToCell((byte)(1 << i), x, y, j)) return false;
                    }
                }
            }
        }

        counts.Clear();

        for (int i = x / 2 * 2; i < (x + 2) / 2 * 2; i++)
        for (int j = y / 2 * 2; j < (y + 2) / 2 * 2; j++)
        for (int k = z / 2 * 2; k < (z + 2) / 2 * 2; k++)
        {
            ProbCount(i, j, k, counts);
        }

        if (!CountsCheck(counts)) return false;

        for (int ind = 0; ind < N; ind++)
        {
            if (counts[ind] == 1)
            {
                for (int i = x / 2 * 2; i < (x + 2) / 2 * 2; i++)
                for (int j = y / 2 * 2; j < (y + 2) / 2 * 2; j++)
                for (int k = z / 2 * 2; k < (z + 2) / 2 * 2; k++)
                {
                    if (BitOperations.PopCount(Cube[i, j, k]) > 1 && (Cube[i, j, k] & (1 << ind)) != 0)
                    {
                        if (!TrySetValueToCell((byte)(1 << ind), i, j, k)) return false;
                    }
                }
            }
        }

        return true;
    }

    private void ProbCount(int x0, int y0, int z0, Span<int> countsSpan)
    {
        byte cell = Cube[x0, y0, z0];
        int cnt = BitOperations.PopCount(cell);
        for (int j = 0; j < N; j++)
        {
            if ((cell & (1 << j)) != 0)
            {
                countsSpan[j]++;
            }
        }
    }

    private bool TryRemoveProbability(byte value, int x, int y, int z)
    {
        byte cellValue = Cube[x, y, z];
        int cnt = BitOperations.PopCount(cellValue);
        cellValue &= (byte)~value;
        int newCnt = BitOperations.PopCount(cellValue);
        Cube[x, y, z] = cellValue;

        if (cellValue != 0)
        {
            if (cnt == 2 && newCnt == 1)
            {
                if (!TrySetValueToCell(cellValue, x, y, z)) return false;
            }

            return true;
        }

        return false;
    }
}