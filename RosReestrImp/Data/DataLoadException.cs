using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RosReestrImp.Data
{
    /// <summary>
    /// Ошика загрузки данных
    /// </summary>
    public class DataLoadException : Exception
    {
        /// <summary>
        /// Ошика загрузки данных
        /// </summary>
        public DataLoadException() { }

        /// <summary>
        /// Ошика загрузки данных
        /// </summary>
        /// <param name="message"> сообщение </param>
        public DataLoadException(string message) : base(message) { }

        /// <summary>
        /// Ошика загрузки данных
        /// </summary>
        /// <param name="message"> сообщение </param>
        /// <param name="inner"> внешняяя ошибка </param>
        public DataLoadException(string message, Exception inner) : base(message, inner) { }
    }
}
