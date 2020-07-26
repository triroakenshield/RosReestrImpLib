using System;

namespace RosReestrImp.Data
{
    /// <summary>Ошика загрузки данных</summary>
    public class DataLoadException : Exception
    {
        /// <summary>Ошика загрузки данных</summary>
        public DataLoadException() { }

        /// <summary>Ошика загрузки данных</summary>
        /// <param name="message"> сообщение </param>
        public DataLoadException(string message) : base(message) { }

        /// <summary>Ошика загрузки данных</summary>
        /// <param name="message"> сообщение</param>
        /// <param name="inner"> внешняя ошибка</param>
        public DataLoadException(string message, Exception inner) : base(message, inner) { }
    }
}