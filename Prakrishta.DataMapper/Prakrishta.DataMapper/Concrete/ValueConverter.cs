//----------------------------------------------------------------------------------
// <copyright file="ValueConverter.cs" company="Prakrishta Technologies">
//     Copyright (c) 2019 Prakrishta Technologies. All rights reserved.
// </copyright>
// <author>Arul Sengottaiyan</author>
// <date>6/28/2019</date>
// <summary>Class that performs value conversion between source and destination type.</summary>
//-----------------------------------------------------------------------------------

namespace Prakrishta.DataMapper.Concrete
{
    using Prakrishta.DataMapper.Enums;
    using Prakrishta.DataMapper.Exceptions;
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using static Prakrishta.DataMapper.Enablers.ReflectionEnabler;

    /// <summary>
    /// Class that performs value conversion between source and destination type.
    /// </summary>
    internal static class ValueConverter
    {
        #region |Methods|

        /// <summary>
        /// Convert provided source object to destination object based
        /// of an provided <see cref="MappedItem"/>.
        /// </summary>
        /// <param name="mapItem">A <see cref="MappedItem"/> that defines the mapping.</param>
        /// <param name="source">A source member value.</param>
        /// <returns>A destination member value or <c>null</c>.</returns>
        public static object Convert(MappedItem mapItem, object source)
        {
            if (mapItem == null)
            {
                throw new ArgumentNullException(nameof(mapItem));
            }

            Type sourceType = mapItem.Source.Type;
            Type destinationType = mapItem.Destination.Type;

            return Convert(source, sourceType, destinationType);
        }

        /// <summary>
        /// Convert provided source object to destination object.
        /// </summary>
        /// <param name="source">The source object instance.</param>
        /// <param name="sourceType">The source object type.</param>
        /// <param name="destinationType">The destination object type.</param>
        /// <returns>A destination object type value.</returns>
        public static object Convert(object source, Type sourceType, Type destinationType)
        {
            object destination = null;

            try
            {
                if (source != null)
                {
                    TypeConversion conversion = ConversionResolver.Resolve(source, sourceType, destinationType);

                    switch (conversion)
                    {
                        case TypeConversion.Direct:
                            destination = source;
                            break;
                        case TypeConversion.Enum:
                            destination = FromEnum(sourceType, destinationType, source);
                            break;
                        case TypeConversion.Convertable:
                            destination = FromConvertable(sourceType, destinationType, source);
                            break;
                        case TypeConversion.Converter:
                            destination = FromConverter(sourceType, destinationType, source);
                            break;
                        case TypeConversion.String:
                            destination = FromString(sourceType, destinationType, source);
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new ValueConverterException(exception.Message, exception);
            }

            return destination;
        }

        /// <summary>
        /// Convert source type value to destination type value using <see cref="IConvertable"/> interface.
        /// </summary>
        /// <param name="sourceType">A type of an source member.</param>
        /// <param name="destinationType">A type of an destination member.</param>
        /// <param name="source">A source instance.</param>
        /// <returns>A value converted with <see cref="IConvertable"/>.</returns>
        private static object FromConvertable(Type sourceType, Type destinationType, object source)
        {
            object destination = System.Convert.ChangeType(source, destinationType);
            return destination;
        }

        /// <summary>
        /// Convert source type value to destination type value using <see cref="TypeConverter"/>
        /// applied to destination type.
        /// </summary>
        /// <param name="sourceType">A type of an source member.</param>
        /// <param name="destinationType">A type of an destination member.</param>
        /// <param name="source">A source instance.</param>
        /// <returns>A value converted with type converter or <c>null</c>.</returns>
        private static object FromConverter(Type sourceType, Type destinationType, object source)
        {
            TypeConverter converter = TypeConverterCache.GetConverter(destinationType);

            object destination = null;

            if (converter != null)
            {
                if (converter.CanConvertFrom(sourceType))
                {
                    destination = converter.ConvertFrom(source);
                }
                else if (converter.CanConvertFrom(typeof(string)))
                {
                    string str = source.ToString();

                    destination = converter.ConvertFromString(str);
                }
            }

            return destination;
        }

        /// <summary>
        /// Convert source type value to destination value using <see cref="Enum.Parse"/>
        /// or <see cref="Enum.ToObject"/> depending on the <paramref name="sourceType"/>.
        /// </summary>
        /// <param name="sourceType">A type of an source member.</param>
        /// <param name="destinationType">A type of an destination member.</param>
        /// <param name="source">A source instance.</param>
        /// <returns>A enum value converted with <see cref="Enum.Parse"/> or <see cref="Enum.ToObject"/>.</returns>
        private static object FromEnum(Type sourceType, Type destinationType, object source)
        {
            if (sourceType.Equals(typeof(string)))
            {
                return Enum.Parse(destinationType, source.ToString(), true);
            }

            return Enum.ToObject(destinationType, source);
        }

        /// <summary>
        /// Convert source type value to destination type value using parsing methods if destination
        /// type has one.
        /// </summary>
        /// <param name="sourceType">A type of an source member.</param>
        /// <param name="destinationType">A type of an destination member.</param>
        /// <param name="source">A source instance.</param>
        /// <returns>A value converted from parse method or <c>null</c>.</returns>
        private static object FromString(Type sourceType, Type destinationType, object source)
        {
            object destination = null;

            string str = source.ToString();

            MethodInfo parsingMethod = GetTryParseMethod(destinationType);

            if (parsingMethod != null)
            {
                destination = parsingMethod.Invoke(null, new object[] { str });
            }
            else if ((parsingMethod = GetParseMethod(destinationType)) != null)
            {
                object[] parameters = new object[] { str, null };

                bool result = (bool)parsingMethod.Invoke(null, parameters);

                if (result)
                {
                    destination = parameters[1];
                }
            }

            return destination;
        }

        #endregion
    }
}
