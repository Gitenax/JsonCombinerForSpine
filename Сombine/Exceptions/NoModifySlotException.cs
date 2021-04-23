using System;

namespace Сombine.Exceptions
{
    public class NoModifySlotException : Exception
    {
        public NoModifySlotException():
            base(message:"Отсутствуют или не указаны слоты для модификации") {}
        
        public NoModifySlotException(string message) : base(message) {}
    }
}