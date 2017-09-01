using System;

namespace FastGrid
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValueListAttribute : Attribute
    {
        private bool isEditable;
        public bool IsEditable => isEditable;

        private object[] values;
        public object[] Values => values;

        public ValueListAttribute(bool isEditable, params object[] values)
        {
            this.isEditable = isEditable;
            this.values = values;
        }
    }
}
