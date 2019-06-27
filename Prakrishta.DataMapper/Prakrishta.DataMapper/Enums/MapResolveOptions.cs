//----------------------------------------------------------------------------------
// <copyright file="MapResolveOptions.cs" company="Prakrishta Technologies">
//     Copyright (c) 2019 Prakrishta Technologies. All rights reserved.
// </copyright>
// <author>Arul Sengottaiyan</author>
// <date>6/27/2019</date>
// <summary>Supported options resolving map</summary>
//-----------------------------------------------------------------------------------

namespace Prakrishta.DataMapper.Enums
{
    using System;

    /// <summary>
    /// Supported options resolving map.
    /// </summary>
    [Flags]
    public enum MapResolveOptions : int
    {
        /// <summary>
        /// None. Private setter is not used.
        /// </summary>
        None = 0,
        /// <summary>
        /// To use private setter of the property.
        /// </summary>
        UsePrivateSetter = 1
    }
}
