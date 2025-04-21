namespace Game.Utilities
{

    public class CircularQueue<T>
        where T : class
    {
        private readonly T[] _Items;
        private int _Front;
        private int _Back;
        private int _Count;
        public int Count { get { return _Count; } }

        public CircularQueue(int size)
        {
            _Items = new T[size];
            _Front = -1;
            _Back = -1;
            _Count = 0;
        }

        public bool Enqueue(T item)
        {
            // Queue is full, unable to add more items
            if (_Count == _Items.Length) { return false; }

            // If the queue is empty reset indices, otherwise slide Back by one spot
            if (_Front < 0) { _Front = _Back = 0; }
            else { _Back = ++_Back % _Items.Length; }

            // Add item at the back of the queue
            _Items[_Back] = item;
            _Count++;
            return true;
        }

        public T? Dequeue()
        {
            if (_Count == 0) { return null; }

            T result = _Items[_Front];
            if (_Front == _Back) { _Front = _Back = -1; }
            else { _Front = ++_Front % _Items.Length; }

            _Count--;
            return result;
        }

        public T? Peek()
        {
            if (_Count == 0) { return null; }
            return _Items[_Front];
        }
    }

}