//----------------------------------------------------------------------------------
// <copyright file="MapResolver.cs" company="Prakrishta Technologies">
//     Copyright (c) 2019 Prakrishta Technologies. All rights reserved.
// </copyright>
// <author>Arul Sengottaiyan</author>
// <date>6/27/2019</date>
// <summary>Class to resolve mapping between source and destination objects.</summary>
//-----------------------------------------------------------------------------------

namespace Prakrishta.DataMapper.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Prakrishta.DataMapper.Enums;    

    /// <summary>
    /// Class to resolve <see cref="SimpleMapper"/> between source and destination objects.
    /// </summary>
    internal static class MapResolver
    {
        #region |Private Fields|

        /// <summary>
        /// Cached maps between types.
        /// </summary>
        private static readonly Dictionary<string, SimpleMapper> maps = new Dictionary<string, SimpleMapper>();

        /// <summary>
        /// Defines the mutex
        /// </summary>
        private static readonly object mutex = new object();

        #endregion

        #region |Methods|

        /// <summary>
        /// Removes mapping between source and destination type.
        /// </summary>
        /// <typeparam name="TSource">The type of an source object.</typeparam>
        /// <typeparam name="TDestination">The type of an destination object.</typeparam>
        /// <returns><c>true</c> if mapping was removed; <c>false</c> otherwise.</returns>
        public static bool Remove<TSource, TDestination>()
        {
            Type sourceType = typeof(TSource);
            Type destinationType = typeof(TDestination);

            return Remove(sourceType, destinationType);
        }

        /// <summary>
        /// Removes mapping between source and destination type.
        /// </summary>
        /// <param name="sourceType">A source type.</param>
        /// <param name="destinationType">A destination type.</param>
        /// <returns><c>true</c> if mapping was removed; <c>false</c> otherwise.</returns>
        public static bool Remove(Type sourceType, Type destinationType)
        {
            if (sourceType == null)
            {
                throw new ArgumentNullException(nameof(sourceType));
            }

            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            string key = sourceType.FullName + ";" + destinationType.FullName;

            lock (mutex)
            {
                return maps.Remove(key);
            }
        }

        /// <summary>
        /// Resolves the mapping between <typeparamref name="TSource"/> and <typeparamref name="TDestination"/>. This method only resolves mappings between
        /// public get properties in source type and public set properties on destination type. Possible indexer properties
        /// are ignored. To add additional mappings after initial resolve use <see cref="Map.Add"/> method.
        /// </summary>
        /// <typeparam name="TSource">The type of an source object.</typeparam>
        /// <typeparam name="TDestination">The type of an destination object.</typeparam>
        /// <param name="options">The options<see cref="MapResolveOptions"/></param>
        /// <returns>A <see cref="Map"/> of resolved member mappings.</returns>
        public static SimpleMapper Resolve<TSource, TDestination>(MapResolveOptions options = MapResolveOptions.None) where TDestination : new()
        {
            Type sourceType = typeof(TSource);
            Type destinationType = typeof(TDestination);

            return Resolve(sourceType, destinationType, options);
        }

        /// <summary>
        /// Resolves the mapping between source and destination type. This method only resolves mappings between
        /// public get properties in source type and public set properties on destination type. Possible indexer properties
        /// are ignored. To add additional mappings after initial resolve use <see cref="Map.Add"/> method.
        /// </summary>
        /// <param name="sourceType">A source type.</param>
        /// <param name="destinationType">A destination type.</param>
        /// <param name="options">The options<see cref="MapResolveOptions"/></param>
        /// <returns>A <see cref="Map"/> of resolved member mappings.</returns>
        public static SimpleMapper Resolve(Type sourceType, Type destinationType, MapResolveOptions options = MapResolveOptions.None)
        {
            if (sourceType == null)
            {
                throw new ArgumentNullException(nameof(sourceType));
            }

            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            string key = sourceType.FullName + ";" + destinationType.FullName;

            if (!maps.TryGetValue(key, out SimpleMapper map))
            {
                lock (mutex)
                {
                    if (!maps.TryGetValue(key, out map))
                    {
                        map = new SimpleMapper(sourceType, destinationType);

                        //// get properties in source type
                        var getProperties = GetProperties(sourceType, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

                        //// set properties in destination type
                        var setProperties = GetProperties(destinationType, BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);

                        foreach (var getProperty in getProperties)
                        {
                            //// get set property matching by name
                            var setProperty = setProperties.Where(x => x.Name.Equals(getProperty.Name))
                                                           .FirstOrDefault();

                            //// add mapping only if both properties match and
                            //// set can be written to.
                            if (setProperty != null && setProperty.CanWrite)
                            {
                                //// check if provite setter can be used
                                bool usePrivateSetter = options.HasFlag(MapResolveOptions.UsePrivateSetter);

                                if (!usePrivateSetter && !setProperty.GetSetMethod(true).IsPublic)
                                {
                                    continue;
                                }

                                //// add mapping
                                map.Add(getProperty, setProperty);
                            }
                        }

                        maps.Add(key, map);
                    }
                }
            }

            return map;
        }

        /// <summary>
        /// Gets properties of the type.
        /// </summary>
        /// <param name="type">The type<see cref="Type"/></param>
        /// <param name="flags">The flags<see cref="BindingFlags"/></param>
        /// <returns>The <see cref="IEnumerable{PropertyInfo}"/></returns>
        private static IEnumerable<PropertyInfo> GetProperties(Type type, BindingFlags flags)
        {
            var properties = type.GetProperties(flags);

            foreach (var property in properties)
            {
                //// if property is not indexer, return it
                if (!IsIndexerProperty(property))
                {
                    yield return property;
                }
            }

            yield break;
        }

        /// <summary>
        /// Check if <see cref="PropertyInfo"/> represents indexer property.
        /// </summary>
        /// <param name="property">A <see cref="PropertyInfo"/> to check.</param>
        /// <returns><c>true</c> if <paramref name="property"/> is indexer; <c>false</c> otherwise.</returns>
        private static bool IsIndexerProperty(PropertyInfo property)
        {
            var parameters = property.GetIndexParameters();

            if (parameters != null && parameters.Length > 0)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
