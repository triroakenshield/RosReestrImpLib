using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RosReestrImp.Rule
{
    /// <summary>
    /// Ошибка загрузки схемы
    /// </summary>
    public class RuleLoadException : Exception
    {
        /// <summary>
        /// Ошибка загрузки схемы
        /// </summary>
        public RuleLoadException() { }

        /// <summary>
        /// Ошибка загрузки схемы
        /// </summary>
        /// <param name="message"> сообщение </param>
        public RuleLoadException(string message) : base(message) { }

        /// <summary>
        /// Ошибка загрузки схемы
        /// </summary>
        /// <param name="message"> сообщение </param>
        /// <param name="inner"> внешняяя ошибка </param>
        public RuleLoadException(string message, Exception inner) : base(message, inner) { }
    }
}
