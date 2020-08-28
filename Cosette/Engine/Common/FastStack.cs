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

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Push(T value)
        {
            _stack[_pointer++] = value;
        }

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public T Pop()
        {
            return _stack[--_pointer];
        }

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Clear()
        {
            _pointer = 0;
        }
    }
}
