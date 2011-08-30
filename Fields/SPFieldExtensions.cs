using System;
using System.Reflection;
using Microsoft.SharePoint;

namespace AnjLab.SharePoint.RichControls.Fields
{
    public static class SPFieldExtensions
    {
        public static void SetFieldAttribute(this SPField field, string attribute, string value)
        {
            var baseType = field.GetType();
            var mi = baseType.GetMethod("SetFieldAttributeValue", BindingFlags.Instance | BindingFlags.NonPublic);
            mi.Invoke(field, new object[] { attribute, value });
        }

        public static string GetFieldAttribute(this SPField field, string attribute)
        {
            var baseType = field.GetType();
            var mi = baseType.GetMethod("GetFieldAttributeValue",
                                        BindingFlags.Instance | BindingFlags.NonPublic,
                                        null,
                                        new Type[] { typeof(String) },
                                        null);

            var obj = mi.Invoke(field, new object[] { attribute });

            return obj == null ? "" : obj.ToString();
        }
    }
}
