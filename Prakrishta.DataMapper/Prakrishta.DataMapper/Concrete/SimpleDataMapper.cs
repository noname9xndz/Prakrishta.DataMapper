//----------------------------------------------------------------------------------
// <copyright file="SimpleDataMapper.cs" company="Prakrishta Technologies">
//     Copyright (c) 2019 Prakrishta Technologies. All rights reserved.
// </copyright>
// <author>Arul Sengottaiyan</author>
// <date>6/28/2019</date>
// <summary>Simple Mapper Factory class</summary>
//-----------------------------------------------------------------------------------

namespace Prakrishta.DataMapper.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Prakrishta.DataMapper.Abstracts;
    using Prakrishta.DataMapper.Enums;    

    /// <summary>
    /// Class that perform actual mapping.
    /// </summary>
    public static class SimpleDataMapper
    {
        #region |Methods|

        /// <summary>
        /// Maps enumerable of <typeparamref name="TSource"/> to enumerable of <typeparamref name="TDestination"/>.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="enumerable">The source enumerable.</param>
        /// <returns>A enumerable of <typeparamref name="TDestination"/>.</returns>
        public static IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> enumerable) where TDestination : new()
        {
            foreach (TSource source in enumerable)
            {
                if (source == null)
                {
                    continue;
                }

                TDestination destination = Map<TSource, TDestination>(source);

                yield return destination;
            }

            yield break;
        }

        /// <summary>
        /// Maps source object to destination object.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="destinationType">The type of an destination object.</param>
        /// <returns>A destination object instance or null or default.</returns>
        public static object Map(object source, Type destinationType)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            Type sourceType = source.GetType();

            //// create destination instance
            object destination = Activator.CreateInstance(destinationType);

            if (destination == null)
                throw new InvalidOperationException($"Could not create instance of {destinationType.FullName}.");

            //// perform mapping
            Map(source, destination, sourceType, destinationType);

            return destination;
        }

        /// <summary>
        /// Maps source object to destination object.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source">The source object.</param>
        /// <returns>A destination object instance or null or default.</returns>
        public static TDestination Map<TSource, TDestination>(TSource source) where TDestination : new()
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            TDestination destination = new TDestination();

            Type sourceType = typeof(TSource);
            Type destinationType = typeof(TDestination);

            Map(source, destination, sourceType, destinationType);

            return destination;
        }

        /// <summary>
        /// Removes map between source and destination type.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <returns>true if map was removed; false otherwise.</returns>
        public static bool RemoveMap<TSource, TDestination>() where TDestination : new()
        {
            return MapResolver.Remove<TSource, TDestination>();
        }

        /// <summary>
        /// Removes map between source and destination type.
        /// </summary>
        /// <param name="sourceType">The sourceType<see cref="Type"/></param>
        /// <param name="destinationType">The destinationType<see cref="Type"/></param>
        /// <returns>true if map was removed; false otherwise.</returns>
        public static bool RemoveMap(Type sourceType, Type destinationType)
        {
            return MapResolver.Remove(sourceType, destinationType);
        }

        /// <summary>
        /// Resolves <see cref="ISimpleMapper"/> between source and destination type.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="options">The options<see cref="MapResolveOptions"/></param>
        /// <returns>A resolved map.</returns>
        public static ISimpleMapper Resolve<TSource, TDestination>(MapResolveOptions options = MapResolveOptions.None) where TDestination : new()
        {
            return MapResolver.Resolve<TSource, TDestination>(options);
        }

        /// <summary>
        /// Resolves <see cref="ISimpleMapper"/> between source and destination type.
        /// </summary>
        /// <param name="sourceType">The sourceType<see cref="Type"/></param>
        /// <param name="destinationType">The destinationType<see cref="Type"/></param>
        /// <param name="options">The options<see cref="MapResolveOptions"/></param>
        /// <returns>A resolved map.</returns>
        public static ISimpleMapper Resolve(Type sourceType, Type destinationType, MapResolveOptions options = MapResolveOptions.None)
        {
            return MapResolver.Resolve(sourceType, destinationType, options);
        }

        /// <summary>
        /// Gets source object member value.
        /// </summary>
        /// <param name="mapping">The mapping<see cref="MappedItem.Item"/></param>
        /// <param name="source">The source<see cref="object"/></param>
        /// <returns>A value returned by member.</returns>
        private static object GetSourceValue(MappedItem.Item mapping, object source)
        {
            object value;

            //// check whether to get property value or invoke method
            if (mapping.Member == MemberType.Property)
            {
                value = ((PropertyInfo)mapping.MemberInfo).GetValue(source, null);
            }
            else
            {
                //// expected get method has no parameters
                value = ((MethodInfo)mapping.MemberInfo).Invoke(source, null);
            }

            return value;
        }

        /// <summary>
        /// Maps source object to destination object.
        /// </summary>
        /// <param name="source">The source<see cref="object"/></param>
        /// <param name="destination">The destination<see cref="object"/></param>
        /// <param name="sourceType">The sourceType<see cref="Type"/></param>
        /// <param name="destinationType">The destinationType<see cref="Type"/></param>
        private static void Map(object source, object destination, Type sourceType, Type destinationType)
        {
            //// get new or previously resolved map
            SimpleMapper map = MapResolver.Resolve(sourceType, destinationType);

            //// map source to destination
            Map(map, source, destination, sourceType, destinationType);
        }

        /// <summary>
        /// Maps source object to destination object using provided map.
        /// </summary>
        /// <param name="map">The map<see cref="SimpleMapper"/></param>
        /// <param name="source">The source<see cref="object"/></param>
        /// <param name="destination">The destination<see cref="object"/></param>
        /// <param name="sourceType">The sourceType<see cref="Type"/></param>
        /// <param name="destinationType">The destinationType<see cref="Type"/></param>
        private static void Map(SimpleMapper map, object source, object destination, Type sourceType, Type destinationType)
        {
            //// map each resolved mapping
            foreach (MappedItem item in map.Items)
            {
                //// if mapping should be ignored,
                //// continue to next mapping
                if (item.IsIgnored)
                {
                    continue;
                }

                object sourceValue = GetSourceValue(item.Source, source);
                object destinationValue = null;

                //// if source value is null
                if (sourceValue == null)
                {
                    //// and destination value is struct
                    if (item.Destination.Type.IsSubclassOf(typeof(System.ValueType)))
                    {
                        //// leave value to zero
                        continue;
                    }
                }
                else
                {
                    //// check if type is complex
                    if (item.IsComplex)
                    {
                        //// get resolved sub map between complex types
                        SimpleMapper subMap = map.GetSubMap(item);

                        //// create instance of the destination type, expects that type has default constructor
                        destinationValue = Activator.CreateInstance(item.Destination.Type);

                        //// map source complex type to destination complex type by making recursive call
                        Map(subMap, sourceValue, destinationValue, sourceValue.GetType(), destinationValue.GetType());
                    }
                    else
                    {
                        //// convert source value
                        destinationValue = ValueConverter.Convert(item, sourceValue);
                    }
                }

                //// set value to the instance
                SetDestinationValue(item.Destination, destination, destinationValue);
            }
        }

        /// <summary>
        /// Sets destination object member value.
        /// </summary>
        /// <param name="mapping">The mapping<see cref="MappedItem.Item"/></param>
        /// <param name="destination">The destination<see cref="object"/></param>
        /// <param name="value">The value<see cref="object"/></param>
        private static void SetDestinationValue(MappedItem.Item mapping, object destination, object value)
        {
            //// check whether to set property value or invoke method
            if (mapping.Member == MemberType.Property)
            {
                ((PropertyInfo)mapping.MemberInfo).SetValue(destination, value, null);
            }
            else
            {
                //// expected set method has 1 parameter
                ((MethodInfo)mapping.MemberInfo).Invoke(destination, new object[] { value });
            }
        }

        #endregion
    }
}
