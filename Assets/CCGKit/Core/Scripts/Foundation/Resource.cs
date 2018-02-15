// Copyright (C) 2016-2017 David Pol. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

namespace CCGKit
{
    /// <summary>
    /// This base class is used across the kit in order to have types with unique identifiers that
    /// increase automatically.
    /// この基本クラスはキット全体で使用され、一意の識別子が自動的に増加する型を持つようにします。
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// The unique identifier of this resource.
        /// このリソースのユニークID
        /// </summary>
        public int id;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The unique identifier of the resource.</param>
        public Resource(int id)
        {
            this.id = id;
        }
    }
}
