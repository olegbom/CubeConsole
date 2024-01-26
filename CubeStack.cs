namespace CubeConsole;

public class CubeStack
{

    private int _size = 0;
    private List<byte[,,]> _stack = new ();

    public void Push(byte[,,] cube)
    {
        _size++;
        if (_size > _stack.Count)
        {
            byte[,,] b = (byte[,,]) cube.Clone();
            _stack.Add(b);
        }
        else
        {
            Array.Copy(cube, _stack[_size-1], cube.Length);
        }
    }

    public void Pop(byte[,,] cube)
    {
        if (_size > 0)
        {
            Array.Copy(_stack[_size - 1], cube, cube.Length);
            _size--;
        }
        else
        {
            throw new Exception("Pop without push!");
        }
    }
}