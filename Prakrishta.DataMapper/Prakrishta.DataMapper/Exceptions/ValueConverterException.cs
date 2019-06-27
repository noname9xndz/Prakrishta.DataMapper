//----------------------------------------------------------------------------------
// <copyright file="ValueConverterException.cs" company="Prakrishta Technologies">
//     Copyright (c) 2019 Prakrishta Technologies. All rights reserved.
// </copyright>
// <author>Arul Sengottaiyan</author>
// <date>6/27/2019</date>
// <summary>Value Converter Exception class</summary>
//-----------------------------------------------------------------------------------

namespace Prakrishta.DataMapper.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines the <see cref="ValueConverterException" /> class
    /// </summary>
    internal class ValueConverterException : Exception
    {
        #region |Constructors|

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConverterException"/> class.
        /// </summary>
        public ValueConverterException()
            : this("Value conversion error.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConverterException"/> class.
        /// </summary>
        /// <param name="message">The error message <see cref="string"/></param>
        public ValueConverterException(string message)
            : this(message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConverterException"/> class.
        /// </summary>
        /// <param name="message">The error message <see cref="string"/></param>
        /// <param name="innerException">The innerException <see cref="Exception"/></param>
        public ValueConverterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConverterException"/> class.
        /// </summary>
        /// <param name="info">The serialization info<see cref="SerializationInfo"/></param>
        /// <param name="context">The context<see cref="StreamingContext"/></param>
        protected ValueConverterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
