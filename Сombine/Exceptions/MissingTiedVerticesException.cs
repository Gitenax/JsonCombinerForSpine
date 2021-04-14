using System;

namespace Сombine.Exceptions
{
    public class MissingTiedVerticesException : Exception
    {
        public MissingTiedVerticesException(string message) : base(message) {}
    }
}