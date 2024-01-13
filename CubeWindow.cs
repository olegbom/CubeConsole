using System.Drawing;
using System.Dynamic;
using Terminal.Gui;
using Terminal.Gui.Trees;


namespace CubeConsole;
public class CubeWindow:Window
{

    public int N = 4;

    private byte[,,] Cube;

    private readonly FrontSliceView _frontSlice;

    private readonly FrontSliceView _topSlice;

    private readonly FrontSliceView _leftSlice;

    

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
        _frontSlice.Panel.Border.Title = $"{_selected.z + 1} Front";
        _frontSlice.SetCross(_selected.x, _selected.y);
        _topSlice.SetSlice(Cube, _selected.y, SliceDirection.Top);
        _topSlice.Panel.Border.Title = $"{_selected.y + 1} Top";
        _topSlice.SetCross(_selected.x, N - _selected.z - 1);
        _leftSlice.SetSlice(Cube, _selected.x, SliceDirection.Left);
        _leftSlice.Panel.Border.Title = $"{_selected.x + 1} Left";
        _leftSlice.SetCross(N - _selected.z - 1, _selected.y);
        _frontSlice.SetCursor(_selected.x, _selected.y);
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
                    Cube[i, j, k] = (byte)(Random.Shared.Next(N) + 1);
                }
            }
        }


        _frontSlice = new FrontSliceView(N, "Front");
        _frontSlice.Panel.X = 2;
        _frontSlice.Panel.Y = 1;
        _frontSlice.SetSlice(Cube, 0, SliceDirection.Front);
        Add(_frontSlice.Panel);

        _topSlice = new FrontSliceView(N, "Top");
        _topSlice.Panel.X = 2;
        _topSlice.Panel.Y = Pos.Bottom(_frontSlice.Panel);
        _topSlice.SetSlice(Cube, 0, SliceDirection.Top);
        Add(_topSlice.Panel);

        _leftSlice = new FrontSliceView(N, "Left");
        _leftSlice.Panel.X = Pos.Right(_topSlice.Panel) + 1;
        _leftSlice.Panel.Y = 1;
        _leftSlice.SetSlice(Cube, 0, SliceDirection.Left);
        Add(_leftSlice.Panel);
        OnSelectedChange();

        KeyPress += args =>
        {
            switch (args.KeyEvent.Key)
            {
                case Key.CursorLeft:
                    if (Selected.x > 0)
                    {
                        Selected = _selected with {x = _selected.x - 1};
                    }
                    args.Handled = true;
                    break;
                case Key.CursorRight:
                    if (Selected.x < N - 1)
                    {
                        Selected = _selected with { x = _selected.x + 1 };
                    }
                    args.Handled = true;
                    break;
                case Key.CursorUp:
                    if (Selected.y > 0)
                    {
                        Selected = _selected with { y = _selected.y - 1 };
                    }
                    args.Handled = true;
                    break;
                case Key.CursorDown:
                    if (Selected.y < N - 1)
                    {
                        Selected = _selected with { y = _selected.y + 1 };
                    }
                    args.Handled = true;
                    break;
                case Key.PageDown:
                    if (Selected.z > 0)
                    {
                        Selected = _selected with { z = _selected.z - 1 };
                    }
                    args.Handled = true;
                    break;
                case Key.PageUp:
                    if (Selected.z < N - 1)
                    {
                        Selected = _selected with { z = _selected.z + 1 };
                    }
                    args.Handled = true;
                    break;
                case Key.D1:
                    SetValueToSelectedCell(1);
                    break;
                case Key.D2:
                    SetValueToSelectedCell(2);
                    break;
                case Key.D3:
                    SetValueToSelectedCell(3);
                    break;
                case Key.D4:
                    SetValueToSelectedCell(4);
                    break;
            }
        };
    }

    public void SetValueToSelectedCell(byte value)
    {
        Cube[Selected.x, Selected.y, Selected.z] = value;
        OnSelectedChange();
    }
}