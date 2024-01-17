using System.Drawing;
using System.Dynamic;
using Terminal.Gui;
using Terminal.Gui.Trees;


namespace CubeConsole;
public class CubeWindow:Window
{

    public int N = 8;

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
        //_frontSlice.Border.Title = $"{_selected.z + 1} Front";
        _frontSlice.SetCross(_selected.x, _selected.y);
        _frontSlice.SetCursor(_selected.x, _selected.y);

        _topSlice.SetSlice(Cube, _selected.y, SliceDirection.Top);
        //_topSlice.Border.Title = $"{_selected.y + 1} Top";
        _topSlice.SetCross(_selected.x, N - _selected.z - 1);
        _topSlice.SetCursor(_selected.x, N - _selected.z - 1);
        _leftSlice.SetSlice(Cube, _selected.x, SliceDirection.Left);
        // _leftSlice.Border.Title = $"{_selected.x + 1} Left";
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

    public void SetValueToSelectedCell(byte value)
    {
        Cube[Selected.x, Selected.y, Selected.z] = value;
        for (int i = 0; i < N; i++)
        {
            if (i != Selected.x)
            {
                Cube[i, Selected.y, Selected.z] &= (byte)~value;
            }

            if (i != Selected.z)
            {
                Cube[Selected.x, Selected.y, i] &= (byte)~value;
            }

            if (i != Selected.y)
            {
                Cube[Selected.x, i, Selected.z] &= (byte)~value;
            }
        }

        

        OnSelectedChange();
    }
}