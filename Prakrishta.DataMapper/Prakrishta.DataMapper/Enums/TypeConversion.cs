//----------------------------------------------------------------------------------
// <copyright file="TypeConversion.cs" company="Prakrishta Technologies">
//     Copyright (c) 2019 Prakrishta Technologies. All rights reserved.
// </copyright>
// <author>Arul Sengottaiyan</author>
// <date>6/27/2019</date>
// <summary>TypeConversion Enum</summary>
//-----------------------------------------------------------------------------------

namespace Prakrishta.DataMapper.Enums
{
    /// <summary>
    /// Supported conversions between types.
    /// </summary>
    public enum TypeConversion : int
    {
        /// <summary>
        /// Conversion not determined.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Direct assignment.
        /// </summary>
        Direct = 1,
        /// <summary>
        /// Enum conversion.
        /// </summary>
        Enum = 2,
        /// <summary>
        /// Conversion using <see cref="IConvertable"/> interface.
        /// </summary>
        Convertable = 3,
        /// <summary>
        /// Conversion using <see cref="System.ComponentModel.TypeConverter"/>
        /// </summary>
        Converter = 4,
        /// <summary>
        /// String conversion aka parsing.
        /// </summary>
        String = 5,
        /// <summary>
        /// No conversion between types.
        /// </summary>
        None = 6
    }
}
