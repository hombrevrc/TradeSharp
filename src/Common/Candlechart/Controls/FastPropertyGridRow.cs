using System.Reflection;

namespace Candlechart.Controls
{
    class FastPropertyGridRow
    {
        public string Title { get; set; }

        public object Value { get; set; }

        public PropertyInfo Property { get; set; }

        public object StringValue { get; set; }
    }
}
