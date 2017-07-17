using System;

namespace DragDrop2
{
    public class DragDropBehaviorException : Exception
    {
        public DragDropBehaviorException() { }
        public DragDropBehaviorException(string message) : base(message) { }
    }
}
