using System;

namespace Сombine.Exceptions
{
    public class SlotConsistencyException : Exception
    {
        public SlotConsistencyException(string message) : base(message){}
    }
}