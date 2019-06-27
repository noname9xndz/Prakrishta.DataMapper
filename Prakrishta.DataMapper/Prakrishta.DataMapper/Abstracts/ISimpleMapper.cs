using Prakrishta.DataMapper.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Prakrishta.DataMapper.Abstracts
{
    public interface ISimpleMapper
    {
        /// <summary>
        /// Gets items in map.
        /// </summary>
        IEnumerable<MappedItem> Items { get; }

        /// <summary>
        /// Create simple mapping between members in source and destination type.
        /// </summary>
        /// <typeparam name="TSourceMember">The type of the source member. Should be either <see cref="PropertyInfo"/> or <see cref="MethodInfo"/>.</typeparam>
        /// <typeparam name="TDestinationMember">The type of the destination member. Should be either <see cref="PropertyInfo"/> or <see cref="MethodInfo"/>.</typeparam>
        /// <param name="source">The source member info.</param>
        /// <param name="destination">The destination member info.</param>
        /// <returns>A changed <see cref="IMap"/>.</returns>
        ISimpleMapper Add<TSourceMember, TDestinationMember>(TSourceMember sourceMember, TDestinationMember destinationMember)
            where TSourceMember : MemberInfo
            where TDestinationMember : MemberInfo;

        /// <summary>
        /// Create simple mapping between members in source and destination type.
        /// </summary>
        /// <typeparam name="TSource">The type of an source object.</typeparam>
        /// <typeparam name="TDestination">The type of an destination object.</typeparam>
        /// <param name="sourceMemberName">The name of the source member.</param>
        /// <param name="destinationMemberName">The name of the destination member.</param>
        /// <returns>A changed <see cref="Map"/>.</returns>
        /// <exception cref="ArgumentNullException">If <see cref="sourceMemberName"/> or <see cref="destinationMemberName"/> is null, empty or whitespace.</exception>
        /// <exception cref="ArgumentException">If member name is not name of the property or method.</exception>
        ISimpleMapper Add<TSource, TDestination>(string sourceMemberName, string destinationMemberName) where TDestination : new();

        /// <summary>
        /// Create simple mapping between members in source and destination type.
        /// </summary>
        /// <param name="sourceMemberName">The name of the source member.</param>
        /// <param name="sourceType">The type of an source object.</param>
        /// <param name="destinationMemberName">The name of the destination member.</param>
        /// <param name="destinationType">The type of an destination object.</param>
        /// <returns>A changed <see cref="IMap"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <see cref="sourceMemberName"/> or <see cref="destinationMemberName"/> is null, empty or whitespace.
        /// -or-
        /// If <paramref name="sourceType"/> or <paramref name="destinationType"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">If member name is not name of the property or method.</exception>
        ISimpleMapper Add(string sourceMemberName, Type sourceType, string destinationMemberName, Type destinationType);

        /// <summary>
        /// Create simple mapping between members in source and destination type.
        /// </summary>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <typeparam name="TDestination">The type of the destination object.</typeparam>
        /// <param name="sourceMember">The source member selector expression.</param>
        /// <param name="destinationMember">The destination member selector expression.</param>
        /// <returns>A changed <see cref="IMap"/>.</returns>
        ISimpleMapper Add<TSource, TDestination>(Expression<Func<TSource, object>> sourceMember, Expression<Func<TDestination, object>> destinationMember) where TDestination : new();

        /// <summary>
        /// Clears map from all the mappings.
        /// </summary>
        /// <returns>A changed <see cref="ISimpleMapper"/>e.</returns>
        ISimpleMapper Clear();

        /// <summary>
        /// Creates complex mapping between members in source and destination type.
        /// </summary>
        /// <typeparam name="TSourceMember">The type of the source <see cref="MemberInfo"/>.</typeparam>
        /// <typeparam name="TDestinationMember">The type of the destination <see cref="MemberInfo"/>.</typeparam>
        /// <param name="source">A instance of <typeparamref name="TSourceMember"/>.</param>
        /// <param name="destination">A instance of <typeparamref name="TDestinationMember"/>.</param>
        /// <returns>A changed <see cref="IMap"/>.</returns>
        ISimpleMapper Complex<TSourceMember, TDestinationMember>(TSourceMember source, TDestinationMember destination)
            where TSourceMember : MemberInfo
            where TDestinationMember : MemberInfo;

        /// <summary>
        /// Creates complex mapping between members in source and destination type.
        /// </summary>
        /// <typeparam name="TSource">The type of the source instance.</typeparam>
        /// <typeparam name="TDestination">The type of the destination instance.</typeparam>
        /// <param name="sourceMemberName">The source member name.</param>
        /// <param name="destinationMemberName">The destination member name.</param>
        /// <returns>A changed <see cref="IMap"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="sourceMemberName"/> or <see cref="destinationMemberName"/> is null, empty or whitespace.
        /// </exception>
        ISimpleMapper Complex<TSource, TDestination>(string sourceMemberName, string destinationMemberName) where TDestination : new();

        /// <summary>
        /// Create complex mapping between members in source and destination type.
        /// </summary>
        /// <param name="sourceMemberName">The source member name.</param>
        /// <param name="sourceType">The source type.</param>
        /// <param name="destinationMemberName">The destination member name.</param>
        /// <param name="destinationType">The destination type.</param>
        /// <returns>A changed <see cref="IMap"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="sourceMemberName"/> or <see cref="destinationMemberName"/> is null, empty or whitespace.
        /// -or-
        /// If <paramref name="sourceType"/> or <paramref name="destinationType"/> is <c>null</c>.
        /// </exception>
        ISimpleMapper Complex(string sourceMemberName, Type sourceType, string destinationMemberName, Type destinationType);

        /// <summary>
        /// Create complex mapping between members in source and destination type.
        /// </summary>
        /// <typeparam name="TSource">The type of the source instance.</typeparam>
        /// <typeparam name="TDestination">The type of the destination instance.</typeparam>
        /// <param name="sourceMember">The expression to select source member.</param>
        /// <param name="destinationMember">The expression to select destination member.</param>
        /// <returns>A changed <see cref="IMap"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="sourceMember"/> or <paramref name="destinationMember"/> is <c>null</c>.
        /// </exception>
        ISimpleMapper Complex<TSource, TDestination>(Expression<Func<TSource, object>> sourceMember, Expression<Func<TDestination, object>> destinationMember) where TDestination : new();

        /// <summary>
        /// Marks the mappings where provided member is either source or destination as ignored. If you want to remove mapping completely,
        /// use <see cref="Remove"/> method.
        /// </summary>
        /// <typeparam name="TMember">The type of an member to ignore; either <see cref="PropertyInfo"/> or <see cref="MethodInfo"/>.</typeparam>
        /// <param name="member">A source or destination member to ignore.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="member"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="member"/> does not represent <see cref="PropertyInfo"/> or <see cref="MethodInfo"/>.</exception>
        ISimpleMapper Ignore<TMember>(TMember member) where TMember : MemberInfo;

        /// <summary>
        /// Marks the mapping where provided source or destination member name is equal to
        /// provided member name as ignored. If you want to remove mapping completely, 
        /// use <see cref="Remove"/> method.
        /// </summary>
        /// <param name="memberName">A name of the source or destination member to mark as ignored.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="memberName"/> is <c>null</c>, empty string or whitespace.</exception>
        ISimpleMapper Ignore(string memberName);

        /// <summary>
        /// Marks the mapping as ignored. If you want to remove mapping completely,
        /// use <see cref="Remove"/> method.
        /// </summary>
        /// <param name="item">A mapping to mark ignored.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="item"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="item"/> is not from this setup instance.</exception>
        ISimpleMapper Ignore(MappedItem item);

        /// <summary>
        /// Marks the mapping as ignored. If you want to remove mapping completely,
        /// use <see cref="Remove"/> method.
        /// </summary>
        /// <typeparam name="T">The type of the source or destination.</typeparam>
        /// <param name="expression">The member selector expression.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        ISimpleMapper Ignore<T>(Expression<Func<T, object>> expression);

        /// <summary>
        /// Restores previously ignored member.
        /// </summary>
        /// <typeparam name="TMember">The type of an member to ignore; either <see cref="PropertyInfo"/> or <see cref="MethodInfo"/>.</typeparam>
        /// <param name="member">A source or destination member to ignore.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        ISimpleMapper Unignore<TMember>(TMember member) where TMember : MemberInfo;

        /// <summary>
        /// Restores previously ignored member.
        /// </summary>
        /// <param name="memberName">The name of the member.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        ISimpleMapper Unignore(string memberName);

        /// <summary>
        /// Restores previously ignored member.
        /// </summary>
        /// <param name="mapping">The <see cref="MapItem"/> to unignore.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        ISimpleMapper Unignore(MappedItem item);

        /// <summary>
        /// Restores previously ignored member.
        /// </summary>
        /// <typeparam name="T">The type of the source or destination.</typeparam>
        /// <param name="expression">The member selector expression.</param>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        ISimpleMapper Unignore<T>(Expression<Func<T, object>> expression);

        /// <summary>
        /// Unignores all ignored mappings.
        /// </summary>
        /// <returns>A changed <see cref="Map"/> instance.</returns>
        ISimpleMapper UnignoreAll();

        /// <summary>
        /// Removes provided item from the map.
        /// </summary>
        /// <param name="item">A mapping to remove.</param>
        /// <returns>A changed <see cref="IMap"/> instance.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="item"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="item"/> is not from this setup instance.</exception>
        ISimpleMapper Remove<TMember>(TMember member) where TMember : MemberInfo;

        /// <summary>
        /// Removes mapping by the provided source or destination member name.
        /// </summary>
        /// <param name="memberName">A name of the source or destination member to remove.</param>
        /// <returns>A changed <see cref="IMap"/> instance.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="memberName"/> is <c>null</c>, empty string or whitespace.</exception>
        ISimpleMapper Remove(string memberName);

        /// <summary>
        /// Removes mapping by the provided source or destination member
        /// </summary>
        /// <typeparam name="TMember">The type of an member to ignore; either <see cref="PropertyInfo"/> or <see cref="MethodInfo"/>.</typeparam>
        /// <param name="member">A source or destination member to remove.</param>
        /// <returns>A changed <see cref="IMap"/> instance.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="member"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="member"/> does not represent <see cref="PropertyInfo"/> or <see cref="MethodInfo"/>.</exception>
        ISimpleMapper Remove(MappedItem item);

        /// <summary>
        /// Remove mapping by provided selector expression.
        /// </summary>
        /// <typeparam name="T">The type of an source or destination.</typeparam>
        /// <param name="expression">The member selector expression.</param>
        /// <returns>A changed <see cref="IMap"/> instance.</returns>
        ISimpleMapper Remove<T>(Expression<Func<T, object>> expression);

    }
}
