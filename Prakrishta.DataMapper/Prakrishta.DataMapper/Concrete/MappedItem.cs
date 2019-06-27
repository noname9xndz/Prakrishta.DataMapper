//----------------------------------------------------------------------------------
// <copyright file="MappedItem.cs" company="Prakrishta Technologies">
//     Copyright (c) 2019 Prakrishta Technologies. All rights reserved.
// </copyright>
// <author>Arul Sengottaiyan</author>
// <date>6/27/2019</date>
// <summary>Class that represents mapping between two members.</summary>
//-----------------------------------------------------------------------------------

namespace Prakrishta.DataMapper.Concrete
{
    using System;
    using System.Reflection;
    using Prakrishta.DataMapper.Abstracts;
    using Prakrishta.DataMapper.Enums;

    /// <summary>
    /// Class that represents mapping between two members.
    /// </summary>
    public sealed class MappedItem
    {
        #region |Private Fields|

        /// <summary>
        /// Defines the mutex
        /// </summary>
        private readonly object mutex = new object();

        /// <summary>
        /// Defines the key
        /// </summary>
        private string key;

        #endregion

        #region |Constructors|

        /// <summary>
        /// Initializes a new instance of the <see cref="MappedItem"/> class.
        /// </summary>
        /// <param name="source">The <see cref="MappingItem"/> representing source member.</param>
        /// <param name="destination">The <see cref="MappingItem"/> representing destination member.</param>
        /// <param name="map">The <see cref="SimpleMapper"/> this item belongs to.</param>
        internal MappedItem(Item source, Item destination, SimpleMapper map)
        {
            this.Source = source ?? throw new ArgumentNullException(nameof(source));
            this.Destination = destination ?? throw new ArgumentNullException(nameof(destination));            
            this.Map = map ?? throw new ArgumentNullException(nameof(map));
            this.IsIgnored = false;
            this.IsComplex = false;
        }

        #endregion

        #region |Properties|

        /// <summary>
        /// Gets the Destination
        /// Gets <see cref="Item"/> representing destination member.
        /// </summary>
        public Item Destination { get; }

        /// <summary>
        /// Gets or sets a value indicating whether IsIgnored
        /// Gets whether or not mapping is ignored.
        /// </summary>
        public bool IsIgnored { get; internal set; }

        /// <summary>
        /// Gets the Map
        /// Gets <see cref="Map"/> where this
        /// mapping belongs to. Returns <c>null</c>, if removed
        /// from map.
        /// </summary>
        public ISimpleMapper Map { get; private set; }

        /// <summary>
        /// Gets the Source
        /// Gets <see cref="Item"/> representing source member.
        /// </summary>
        public Item Source { get; }

        /// <summary>
        /// Gets or sets a value indicating whether IsComplex
        /// Gets or sets whether or not complex conversion
        /// should be performed.
        /// </summary>
        internal bool IsComplex { get; set; }

        /// <summary>
        /// Gets the value to use as dictionary key.
        /// </summary>
        internal string Key
        {
            get
            {
                if (key == null)
                {
                    key = string.Concat(Source.Type.FullName, ";", Destination.Type.FullName);
                }

                return key;
            }
        }

        #endregion

        #region |Methods|

        /// <summary>
        /// Sets <see cref="Map"/> to null when removed from map.
        /// </summary>
        internal void Remove()
        {
            this.Map = null;
        }

        #endregion

        #region |Nested Class|
        /// <summary>
        /// Class that represents member in <see cref="MappedItem"/>.
        /// </summary>
        public sealed class Item
        {
            #region |Private Fields|

            /// <summary>
            /// Defines the memberType
            /// </summary>
            private MemberType? memberType = null;

            /// <summary>
            /// Defines the type
            /// </summary>
            private Type type = null;

            #endregion

            #region |Constructors|

            /// <summary>
            /// Initializes a new instance of the <see cref="Item"/> class.
            /// </summary>
            /// <param name="memberInfo">The <see cref="MemberInfo"/> representing the mapped member.</param>
            /// <param name="isSource"><c>true</c> if represents mapping of the source member; <c>false</c> otherwise.</param>
            internal Item(MemberInfo memberInfo, bool isSource)
            {
                if (memberInfo == null)
                {
                    throw new ArgumentNullException(nameof(memberInfo));
                }

                this.Name = memberInfo.Name;
                this.IsSourceMember = isSource;
                this.MemberInfo = memberInfo;
            }

            #endregion

            #region |Properties|

            /// <summary>
            /// Gets a value indicating whether IsDestinationMember
            /// Gets whether or not this item represents the
            /// destination member of the mapping.
            /// </summary>
            public bool IsDestinationMember
            {
                get { return !IsSourceMember; }
            }

            /// <summary>
            /// Gets a value indicating whether IsSourceMember
            /// Gets whether or not this item represents the
            /// source member of the mapping.
            /// </summary>
            public bool IsSourceMember { get; }

            /// <summary>
            /// Gets the <see cref="MemberType"/> of the mapped member.
            /// </summary>
            public MemberType Member
            {
                get
                {
                    //// determine the member type
                    if (this.memberType == null)
                    {
                        this.memberType = DetermineMemberType(MemberInfo);
                    }

                    return this.memberType.Value;
                }
            }

            /// <summary>
            /// Gets the <see cref="MemberInfo"/> of the mapped member.
            /// </summary>
            public MemberInfo MemberInfo { get; }

            /// <summary>
            /// Gets the name of the mapped member.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Gets the <see cref="Type"/> of the mapped member.
            /// </summary>
            public Type Type
            {
                get
                {
                    //// determine the type
                    if (this.type == null)
                    {
                        this.type = DetermineType(MemberInfo, Member, IsSourceMember);
                    }

                    return this.type;
                }
            }

            #endregion

            #region |Methods|

            /// <summary>
            /// Determines the <see cref="MemberType"/> of provider <see cref="MemberInfo"/>.
            /// </summary>
            /// <param name="info">A <see cref="MemberInfo"/> representing either property or method.</param>
            /// <returns>A <see cref="MemberType"/> of info.</returns>
            private static MemberType DetermineMemberType(MemberInfo info)
            {
                PropertyInfo property = info as PropertyInfo;

                if (property != null)
                {
                    return MemberType.Property;
                }

                MethodInfo method = info as MethodInfo;

                if (method != null)
                {
                    return MemberType.Method;
                }

                throw new InvalidOperationException($"Mapped member, {info.Name}, must be either property or method.");
            }

            /// <summary>
            /// Determines type of the mapped member.
            /// </summary>
            /// <param name="member">The <see cref="MemberInfo"/> representing either property or method.</param>
            /// <param name="memberType">The <see cref="MemberType"/> of the member.</param>
            /// <param name="isSource"><c>true</c> if is source member; <c>false</c> otherwise.</param>
            /// <returns>A determined type of the mapped member.</returns>
            private static Type DetermineType(MemberInfo member, MemberType memberType, bool isSource)
            {
                Type type;

                if (memberType == MemberType.Property)
                {
                    type = ((PropertyInfo)member).PropertyType;
                }
                else
                {
                    MethodInfo method = (MethodInfo)member;

                    var parameters = method.GetParameters();

                    if (isSource)
                    {
                        type = method.ReturnType;

                        //// method to get value should not return void
                        if (type.Equals(typeof(void)))
                        {
                            throw new InvalidOperationException("Source method returns void.");
                        }

                        //// method to get value should not have parameters
                        if (parameters.Length > 0)
                        {
                            throw new InvalidOperationException("Expected get method is not parameterless.");
                        }
                    }
                    else
                    {
                        //// method to set value should have 1 and only 1 parameter
                        if (parameters.Length != 1)
                        {
                            throw new InvalidOperationException("Expected parameter count of set method is not 1.");
                        }

                        type = parameters[0].ParameterType;
                    }
                }

                return type;
            }

            #endregion
        }
        #endregion
    }
}
