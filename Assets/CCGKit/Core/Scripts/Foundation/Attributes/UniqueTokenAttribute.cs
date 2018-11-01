using System.Reflection;

#if UNITY_EDITOR

using UnityEditor;

#endif
using System;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// ユニークなトークンを名前で指定するカスタム属性
    /// </summary>

    //Attributeについては以下を参照
    //http://ufcpp.net/study/csharp/sp_attribute.html
    [AttributeUsage(AttributeTargets.Class)]
    public class UniqueTokenAttribute : Attribute
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="prefix">Prefix.:接頭辞</param>
        /*
        public UniqueTokenAttribute(string prefix) : base(prefix)
        {
            width = 100;
        }
        */
    }
}