using System.Runtime.CompilerServices;

namespace Cosette.Engine.Common
{
    public class FastStack<T>
    {
        private T[] _stack;
        private int _pointer;

        public FastStack(int capacity)
        {
            _stack = new T[capacity];
        }

        public void Push(T value)
        {
            _stack[_pointer++] = value;
        }

        public T Pop()
        {
            return _stack[--_pointer];
        }

        public T Peek(int index)
        {
            return _stack[_pointer - index - 1];
        }

        public int Count()
        {
            return _pointer;
        }

        public void Clear()
        {
            _pointer = 0;
        }
    }
}
