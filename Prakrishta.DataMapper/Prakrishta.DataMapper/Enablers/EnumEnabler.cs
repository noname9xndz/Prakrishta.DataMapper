//----------------------------------------------------------------------------------
// <copyright file="EnumEnabler.cs" company="Prakrishta Technologies">
//     Copyright (c) 2019 Prakrishta Technologies. All rights reserved.
// </copyright>
// <author>Arul Sengottaiyan</author>
// <date>6/28/2019</date>
// <summary>Enum Enabler class</summary>
//-----------------------------------------------------------------------------------

namespace Prakrishta.DataMapper.Enablers
{
    using System;

    /// <summary>
    /// Defines the <see cref="EnumEnabler" /> helper class
    /// </summary>
    internal static class EnumEnabler
    {
        #region |Methods|

        /// <summary>
        /// The IsEnumType
        /// </summary>
        /// <param name="type">The type<see cref="Type"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public static bool IsEnumType(Type type)
        {
            return type.Equals(typeof(SByte)) ||
                   type.Equals(typeof(Int16)) ||
                   type.Equals(typeof(Int32)) ||
                   type.Equals(typeof(Int64)) ||
                   type.Equals(typeof(Byte)) ||
                   type.Equals(typeof(UInt16)) ||
                   type.Equals(typeof(UInt32)) ||
                   type.Equals(typeof(UInt64));
        }

        #endregion
    }
}
