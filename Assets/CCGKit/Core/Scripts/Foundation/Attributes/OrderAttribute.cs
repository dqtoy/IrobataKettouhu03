using System;

namespace CCGKit
{
    /// <summary>
    /// Custom attribute that allows to control the ordering of fields retrieved via reflection.
    /// リフレクションによって取得されたフィールドの順序を制御できるカスタム属性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class OrderAttribute : Attribute
    {
        /// <summary>
        /// The order of this field.
        /// このフィールドの順序。
        /// </summary>
        private readonly int order;

        /// <summary>
        /// The order of this field.
        /// このフィールドの順序。
        /// </summary>
        public int Order { get { return order; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="order">The order of the field.</param>
        public OrderAttribute(int order)
        {
            this.order = order;
        }
    }
}
