using System;

namespace RosReestrImp.Data
{
    /// <summary>Ошибка загрузки данных</summary>
    public class DataLoadException : Exception
    {
        /// <summary>Ошибка загрузки данных</summary>
        public DataLoadException() { }

        /// <summary>Ошибка загрузки данных</summary>
        /// <param name="message">сообщение</param>
        public DataLoadException(string message) : base(message) { }

        /// <summary>Ошибка загрузки данных</summary>
        /// <param name="message">сообщение</param>
        /// <param name="inner">внешняя ошибка</param>
        public DataLoadException(string message, Exception inner) : base(message, inner) { }
    }
}