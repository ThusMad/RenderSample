using System;
using System.Collections.Generic;
using Autofac;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Controls;

namespace Scalpio.Controls.Extensions
{
    public class DISource : MarkupExtension
    {
        public static RichTextBox LogSource = new RichTextBox();
        public static Func<Type, object> Resolver { get; set; }
        public Type Type { get; set; }
        public override object ProvideValue(IServiceProvider serviceProvider) => Resolver?.Invoke(Type);
    }
}
