//----------------------------------------------------------------------------------
// <copyright file="SimpleMapper.cs" company="Prakrishta Technologies">
//     Copyright (c) 2019 Prakrishta Technologies. All rights reserved.
// </copyright>
// <author>Arul Sengottaiyan</author>
// <date>6/28/2019</date>
// <summary>Class to store and modify mappings between source and destination type</summary>
//-----------------------------------------------------------------------------------

namespace Prakrishta.DataMapper.Concrete
{    
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Prakrishta.DataMapper.Abstracts;

    /// <summary>
    /// Defines the <see cref="SimpleMapper" /> class
    /// </summary>
    internal sealed class SimpleMapper : ISimpleMapper
    {
        #region |Private Fields|

        /// <summary>
        /// Defines the destinationType
        /// </summary>
        private readonly Type destinationType;

        /// <summary>
        /// Defines the items
        /// </summary>
        private List<MappedItem> items;

        /// <summary>
        /// Defines the sourceType
        /// </summary>
        private readonly Type sourceType;

        /// <summary>
        /// Defines the subMaps
        /// </summary>
        private Dictionary<string, SimpleMapper> subMaps;

        #endregion

        #region |Constructors|

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleMapper"/> class.
        /// </summary>
        /// <param name="sourceType">The sourceType<see cref="Type"/></param>
        /// <param name="destinationType">The destinationType<see cref="Type"/></param>
        internal SimpleMapper(Type sourceType, Type destinationType)
        {
            this.sourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            this.destinationType = destinationType ?? throw new ArgumentNullException(nameof(destinationType));
            this.items = new List<MappedItem>();
            this.subMaps = new Dictionary<string, SimpleMapper>();
        }

        #endregion

        #region |Properties|

        /// <summary>
        /// Gets the Items
        /// Gets <see cref="IEnumerable{Mapping}"/> to enumerate
        /// all the mappings in current setup.
        /// </summary>
        public IEnumerable<MappedItem> Items
        {
            get { return this.items; }
        }

        #endregion

        #region |Methods|

        /// <summary>
        /// Create simple mapping between members in source and destination type.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="sourceMember"></param>
        /// <param name="destinationMember"></param>
        /// <returns></returns>
        public ISimpleMapper Add<TSource, TDestination>(Expression<Func<TSource, object>> sourceMember, Expression<Func<TDestination, object>> destinationMember) where TDestination : new()
        {
            string sourceMemberName = ExtractMemberName(sourceMember);
            string destinationMemberName = ExtractMemberName(destinationMember);

            return Add<TSource, TDestination>(sourceMemberName, destinationMemberName);
        }

        /// <summary>
        /// Create simple mapping between members in source and destination type.
        /// </summary>
        /// <typeparam name="TSource">The type of an source object.</typeparam>
        /// <typeparam name="TDestination">The type of an destination object.</typeparam>
        /// <param name="sourceMemberName">The name of the source member.</param>
        /// <param name="destinationMemberName">The name of the destination member.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        public ISimpleMapper Add<TSource, TDestination>(string sourceMemberName, string destinationMemberName) where TDestination : new()
        {
            Type sourceType = typeof(TSource);
            Type destinationType = typeof(TDestination);

            return Add(sourceMemberName, sourceType, destinationMemberName, destinationType);
        }

        /// <summary>
        /// Create simple mapping between members in source and destination type.
        /// </summary>
        /// <param name="sourceMemberName">The name of the source member.</param>
        /// <param name="sourceType">The type of an source object.</param>
        /// <param name="destinationMemberName">The name of the destination member.</param>
        /// <param name="destinationType">The type of an destination object.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        public ISimpleMapper Add(string sourceMemberName, Type sourceType, string destinationMemberName, Type destinationType)
        {
            return AddItem(sourceMemberName, sourceType, destinationMemberName, destinationType, false);
        }

        /// <summary>
        /// Create simple mapping between members in source and destination type.
        /// </summary>
        /// <typeparam name="TSourceMember"></typeparam>
        /// <typeparam name="TDestinationMember"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public ISimpleMapper Add<TSourceMember, TDestinationMember>(TSourceMember source, TDestinationMember destination)
            where TSourceMember : MemberInfo
            where TDestinationMember : MemberInfo
        {
            return AddItem<TSourceMember, TDestinationMember>(source, destination, false);
        }

        /// <summary>
        /// Clears map from all the mappings.
        /// </summary>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        public ISimpleMapper Clear()
        {
            this.items.Clear();
            this.subMaps.Clear();

            return this;
        }

        /// <summary>
        /// Create complex mapping between members in source and destination type.
        /// </summary>
        /// <typeparam name="TSource">The type of the source instance.</typeparam>
        /// <typeparam name="TDestination">The type of the destination instance.</typeparam>
        /// <param name="sourceMember">The expression to select source member.</param>
        /// <param name="destinationMember">The expression to select destination member.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        public ISimpleMapper Complex<TSource, TDestination>(Expression<Func<TSource, object>> sourceMember, Expression<Func<TDestination, object>> destinationMember) where TDestination : new()
        {
            string sourceMemberName = ExtractMemberName(sourceMember);
            string destinationMemberName = ExtractMemberName(destinationMember);

            return Complex<TSource, TDestination>(sourceMemberName, destinationMemberName);
        }

        /// <summary>
        /// Creates complex mapping between members in source and destination type.
        /// </summary>
        /// <typeparam name="TSource">The type of the source instance.</typeparam>
        /// <typeparam name="TDestination">The type of the destination instance.</typeparam>
        /// <param name="sourceMemberName">The source member name.</param>
        /// <param name="destinationMemberName">The destination member name.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        public ISimpleMapper Complex<TSource, TDestination>(string sourceMemberName, string destinationMemberName) where TDestination : new()
        {
            Type sourceType = typeof(TSource);
            Type destinationType = typeof(TDestination);

            return Complex(sourceMemberName, sourceType, destinationMemberName, destinationType);
        }

        /// <summary>
        /// Create complex mapping between members in source and destination type.
        /// </summary>
        /// <param name="sourceMemberName">The source member name.</param>
        /// <param name="sourceType">The source type.</param>
        /// <param name="destinationMemberName">The destination member name.</param>
        /// <param name="destinationType">The destination type.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        public ISimpleMapper Complex(string sourceMemberName, Type sourceType, string destinationMemberName, Type destinationType)
        {
            return AddItem(sourceMemberName, sourceType, destinationMemberName, destinationType, true);
        }

        /// <summary>
        /// Creates complex mapping between members in source and destination type.
        /// </summary>
        /// <typeparam name="TSourceMember">The type of the source <see cref="MemberInfo"/>.</typeparam>
        /// <typeparam name="TDestinationMember">The type of the destination <see cref="MemberInfo"/>.</typeparam>
        /// <param name="source">A instance of <typeparamref name="TSourceMember"/>.</param>
        /// <param name="destination">A instance of <typeparamref name="TDestinationMember"/>.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        public ISimpleMapper Complex<TSourceMember, TDestinationMember>(TSourceMember source, TDestinationMember destination)
            where TSourceMember : MemberInfo
            where TDestinationMember : MemberInfo
        {
            return AddItem<TSourceMember, TDestinationMember>(source, destination, true);
        }

        /// <summary>
        /// Marks the mapping as ignored. If you want to remove mapping completely,
        /// use <see cref="Remove"/> method.
        /// </summary>
        /// <typeparam name="T">The type of the source or destination.</typeparam>
        /// <param name="expression">The member selector expression.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        public ISimpleMapper Ignore<T>(Expression<Func<T, object>> expression)
        {
            string name = ExtractMemberName(expression);

            return Ignore(name);
        }

        /// <summary>
        /// Marks the mapping as ignored. If you want to remove mapping completely,
        /// use <see cref="Remove"/> method.
        /// </summary>
        /// <param name="item">A mapping to mark ignored.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        public ISimpleMapper Ignore(MappedItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.Map != this)
            {
                throw new ArgumentException("mapping is not from this setup.", "mapping");
            }

            item.IsIgnored = true;

            return this;
        }

        /// <summary>
        /// Marks the mapping where provided source or destination member name is equal to
        /// provided member name as ignored. If you want to remove mapping completely, 
        /// use <see cref="Remove"/> method.
        /// </summary>
        /// <param name="memberName">A name of the source or destination member to mark as ignored.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        public ISimpleMapper Ignore(string memberName)
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                throw new ArgumentNullException(nameof(memberName));
            }

            var mapping = Items.Where(x => x.Source.Name == memberName || x.Destination.Name == memberName)
                               .FirstOrDefault();

            if (mapping != null)
            {
                mapping.IsIgnored = true;
            }

            return this;
        }

        /// <summary>
        /// Marks the mappings where provided member is either source or destination as ignored. If you want to remove mapping completely,
        /// use <see cref="Remove"/> method.
        /// </summary>
        /// <typeparam name="TMember">The type of an member to ignore; either <see cref="PropertyInfo"/> or <see cref="MethodInfo"/>.</typeparam>
        /// <param name="member">A source or destination member to ignore.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        public ISimpleMapper Ignore<TMember>(TMember member) where TMember : MemberInfo
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            if (member is PropertyInfo || member is MethodInfo)
            {
                return Ignore(member.Name);
            }
            else
            {
                throw new NotSupportedException($"Mapping of {member.GetType().FullName} is not supported.");
            }
        }

        /// <summary>
        /// Remove mapping by provided selector expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public ISimpleMapper Remove<T>(Expression<Func<T, object>> expression)
        {
            string name = ExtractMemberName(expression);

            return Remove(name);
        }

        /// <summary>
        /// Removes provided item from the map.
        /// </summary>
        /// <param name="item">A mapping to remove.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        public ISimpleMapper Remove(MappedItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.Map != this)
            {
                throw new ArgumentException("MappedItem is not from this setup.", "item");
            }

            int index = this.items.IndexOf(item);

            if (index >= 0)
            {
                this.items.RemoveAt(index);
                item.Remove();
            }

            return this;
        }

        /// <summary>
        /// Removes mapping by the provided source or destination member name.
        /// </summary>
        /// <param name="memberName">A name of the source or destination member to remove.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        public ISimpleMapper Remove(string memberName)
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                throw new ArgumentNullException(nameof(memberName));
            }

            for (int i = items.Count - 1; i >= 0; i--)
            {
                MappedItem m = items[i];

                if (m.Source.Name == memberName ||
                    m.Destination.Name == memberName)
                {
                    //// if item is complex, remove the sub map
                    if (m.IsComplex)
                    {
                        subMaps.Remove(m.Key);
                    }

                    items.RemoveAt(i);
                    m.Remove();
                }
            }

            return this;
        }

        /// <summary>
        /// Removes mapping by the provided source or destination member
        /// </summary>
        /// <typeparam name="TMember">The type of an member to ignore; either <see cref="PropertyInfo"/> or <see cref="MethodInfo"/>.</typeparam>
        /// <param name="member">A source or destination member to remove.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        public ISimpleMapper Remove<TMember>(TMember member) where TMember : MemberInfo
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            if (member is PropertyInfo || member is MethodInfo)
            {
                return Remove(member.Name);
            }
            else
            {
                throw new NotSupportedException($"Mapping of {member.GetType().FullName} is not supported.");
            }
        }

        /// <summary>
        /// Restores previously ignored member.
        /// </summary>
        /// <typeparam name="T">The type of the source or destination.</typeparam>
        /// <param name="expression">The member selector expression.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        public ISimpleMapper Unignore<T>(Expression<Func<T, object>> expression)
        {
            string memberName = ExtractMemberName(expression);

            return Unignore(memberName);
        }

        /// <summary>
        /// Restores previously ignored member.
        /// </summary>
        /// <param name="mapping">The <see cref="MappedItem"/> to unignore.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        public ISimpleMapper Unignore(MappedItem mapping)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            if (mapping.Map != this)
            {
                throw new ArgumentException("mapping is not from this setup.", "mapping");
            }

            mapping.IsIgnored = false;

            return this;
        }

        /// <summary>
        /// Restores previously ignored member.
        /// </summary>
        /// <param name="memberName">The name of the member.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        public ISimpleMapper Unignore(string memberName)
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                throw new ArgumentNullException(nameof(memberName));
            }

            var mapping = Items.Where(x => x.Source.Name == memberName || x.Destination.Name == memberName)
                               .FirstOrDefault();

            if (mapping != null)
            {
                mapping.IsIgnored = false;
            }

            return this;
        }

        /// <summary>
        /// Restores previously ignored member.
        /// </summary>
        /// <typeparam name="TMember">The type of an member to ignore; either <see cref="PropertyInfo"/> or <see cref="MethodInfo"/>.</typeparam>
        /// <param name="member">A source or destination member to ignore.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        public ISimpleMapper Unignore<TMember>(TMember member) where TMember : MemberInfo
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            if (member is PropertyInfo || member is MethodInfo)
            {
                return Unignore(member.Name);
            }
            else
            {
                throw new NotSupportedException($"Mapping of {member.GetType().FullName} is not supported.");
            }
        }

        /// <summary>
        /// Unignores all ignored mappings.
        /// </summary>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        public ISimpleMapper UnignoreAll()
        {
            var ignored = Items.Where(x => x.IsIgnored);

            foreach (var item in ignored)
            {
                item.IsIgnored = false;
            }

            return this;
        }

        /// <summary>
        /// Gets sub map for the provided complex map item.
        /// </summary>
        /// <param name="item">The complex map item.</param>
        /// <returns>A sub map between complex types.</returns>
        internal SimpleMapper GetSubMap(MappedItem item)
        {
            Debug.Assert(item != null);
            Debug.Assert(item.Map == this);
            Debug.Assert(item.IsComplex);

            if (subMaps.TryGetValue(item.Key, out SimpleMapper subMap))
            {
                return subMap;
            }

            return null;
        }

        /// <summary>
        /// Extracts member name from the expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression<see cref="Expression{Func{T, object}}"/></param>
        /// <returns>A name of the member or empty string.</returns>
        private static string ExtractMemberName<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var unaryExpression = expression.Body as UnaryExpression;

            MemberExpression memberExpression;
            if (unaryExpression != null)
            {
                var methodCall = unaryExpression.Operand as MethodCallExpression;

                if (methodCall != null)
                {
                    return methodCall.Method.Name;
                }
                else
                {
                    memberExpression = unaryExpression.Operand as MemberExpression;

                    if (memberExpression != null)
                    {
                        return memberExpression.Member.Name;
                    }
                }
            }

            memberExpression = expression.Body as MemberExpression;

            if (memberExpression != null)
            {
                return memberExpression.Member.Name;
            }

            return string.Empty;
        }

        /// <summary>
        /// Add simple or complex item that maps source member to destination member.
        /// </summary>
        /// <param name="sourceMemberName">The sourceMemberName<see cref="string"/></param>
        /// <param name="sourceType">The sourceType<see cref="Type"/></param>
        /// <param name="destinationMemberName">The destinationMemberName<see cref="string"/></param>
        /// <param name="destinationType">The destinationType<see cref="Type"/></param>
        /// <param name="complex">The complex<see cref="bool"/></param>
        /// <returns>The <see cref="SimpleMapper"/></returns>
        private SimpleMapper AddItem(string sourceMemberName, Type sourceType, string destinationMemberName, Type destinationType, bool complex)
        {
            if (string.IsNullOrWhiteSpace(sourceMemberName))
            {
                throw new ArgumentNullException(nameof(sourceMemberName));
            }

            if (string.IsNullOrWhiteSpace(destinationMemberName))
            {
                throw new ArgumentNullException(nameof(destinationMemberName));
            }

            if (sourceType == null)
            {
                throw new ArgumentNullException(nameof(sourceType));
            }

            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            MemberInfo sm = sourceType.GetMember(sourceMemberName).FirstOrDefault();
            MemberInfo dm = destinationType.GetMember(destinationMemberName).FirstOrDefault();
            MappedItem.Item sourceItem = null;
            MappedItem.Item destinationItem = null;
            MappedItem mapping = null;

            if (sm != null && dm != null)
            {
                sourceItem = new MappedItem.Item(sm, true);
                destinationItem = new MappedItem.Item(dm, false);
                mapping = new MappedItem(sourceItem, destinationItem, this);

                this.items.Add(mapping);
            }

            if (mapping != null)
            {
                if (complex)
                {
                    SimpleMapper subMap = MapResolver.Resolve(sourceItem.Type, destinationItem.Type);

                    mapping.IsComplex = true;

                    subMaps.Add(mapping.Key, subMap);
                }
            }

            return this;
        }

        /// <summary>
        /// Add simple or complex item that maps source member to destination member.
        /// </summary>
        /// <typeparam name="TSourceMember"></typeparam>
        /// <typeparam name="TDestinationMember"></typeparam>
        /// <param name="source">The source<see cref="TSourceMember"/></param>
        /// <param name="destination">The destination<see cref="TDestinationMember"/></param>
        /// <param name="complex">The complex<see cref="bool"/></param>
        /// <returns>The <see cref="SimpleMapper"/></returns>
        private SimpleMapper AddItem<TSourceMember, TDestinationMember>(TSourceMember source, TDestinationMember destination, bool complex)
            where TSourceMember : MemberInfo
            where TDestinationMember : MemberInfo
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            var sourceItem = new MappedItem.Item(source, true);
            var destinationItem = new MappedItem.Item(destination, false);

            MappedItem mapping = new MappedItem(sourceItem, destinationItem, this);
            this.items.Add(mapping);

            if (complex)
            {
                SimpleMapper subMap = MapResolver.Resolve(sourceItem.Type, destinationItem.Type);

                mapping.IsComplex = true;

                subMaps.Add(mapping.Key, subMap);
            }

            return this;
        }

        #endregion
    }
}
