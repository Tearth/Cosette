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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(T value)
        {
            _stack[_pointer++] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Pop()
        {
            return _stack[--_pointer];
        }
    }
}
