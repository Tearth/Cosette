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
            return _stack[_pointer--];
        }
    }
}
