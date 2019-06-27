//----------------------------------------------------------------------------------
// <copyright file="ConversionResolver.cs" company="Prakrishta Technologies">
//     Copyright (c) 2019 Prakrishta Technologies. All rights reserved.
// </copyright>
// <author>Arul Sengottaiyan</author>
// <date>6/28/2019</date>
// <summary>Class to determine type conversion.</summary>
//-----------------------------------------------------------------------------------

namespace Prakrishta.DataMapper.Concrete
{    
    using System;
    using Prakrishta.DataMapper.Enums;
    using static Prakrishta.DataMapper.Enablers.EnumEnabler;
    using static Prakrishta.DataMapper.Enablers.ReflectionEnabler;

    /// <summary>
    /// Class to determine type conversion.
    /// </summary>
    internal static class ConversionResolver
    {
        #region |Methods|

        /// <summary>
        /// Resolves type conversion between map item source and destination type.
        /// </summary>
        /// <param name="item">The item<see cref="MappedItem"/></param>
        /// <param name="sourceValue">The sourceValue<see cref="object"/></param>
        /// <returns>A possible conversion to try from source to destination type.</returns>
        public static TypeConversion Resolve(MappedItem item, object sourceValue)
        {
            Type sourceType = item.Source.Type;
            Type destinationType = item.Destination.Type;

            return Resolve(sourceValue, sourceType, destinationType);
        }

        /// <summary>
        /// Resolves type conversion between source and destination type.
        /// </summary>
        /// <param name="sourceValue">The sourceValue<see cref="object"/></param>
        /// <param name="sourceType">The sourceType<see cref="Type"/></param>
        /// <param name="destinationType">The destinationType<see cref="Type"/></param>
        /// <returns>A possible conversion to try from source to destination type.</returns>
        public static TypeConversion Resolve(object sourceValue, Type sourceType, Type destinationType)
        {
            //// if types match or destination is assignable from source, direct assign is possible
            if (sourceType.Equals(destinationType) || destinationType.IsAssignableFrom(sourceType))
            {
                return TypeConversion.Direct;
            }

            //// if destination type is enum and source type is enum underlying type or string,
            //// enum conversion is possible
            if (destinationType.IsEnum && (IsEnumType(sourceType) || sourceType.Equals(typeof(string))))
            {
                return TypeConversion.Enum;
            }

            //// if destination type has TypeConverter, then use that
            if (TypeConverterCache.HasConverter(destinationType))
            {
                return TypeConversion.Converter;

            }

            //// if source is IConvertible, use System.Convert.ChangeType
            if (CanChangeType(sourceValue, destinationType))
            {
                return TypeConversion.Convertable;
            }

            //// check if detination type has parsing methods
            if (SupportsParsing(destinationType))
            {
                //// try string parsing conversion
                return TypeConversion.String;
            }

            return TypeConversion.None;
        }

        /// <summary>
        /// Checks if using <see cref="IConvertible"/> interface might be possible.
        /// </summary>
        /// <param name="value">The value<see cref="object"/></param>
        /// <param name="conversionType">The conversionType<see cref="Type"/></param>
        /// <returns><c>true</c> if using <see cref="IConvertible"/> might be possible; <c>false</c> otherwise.</returns>
        private static bool CanChangeType(object value, Type conversionType)
        {
            if (conversionType == null)
            {
                return false;
            }

            if (value == null)
            {
                return false;
            }

            IConvertible convertible = value as IConvertible;

            if (convertible == null)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
