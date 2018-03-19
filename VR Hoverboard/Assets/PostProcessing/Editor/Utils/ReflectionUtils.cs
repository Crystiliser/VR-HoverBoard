namespace UnityEditor.PostProcessing
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    public static class ReflectionUtils
    {
        private static Dictionary<KeyValuePair<object, string>, FieldInfo> s_FieldInfoFromPaths = new Dictionary<KeyValuePair<object, string>, FieldInfo>();
        public static FieldInfo GetFieldInfoFromPath(object source, string path)
        {
            FieldInfo field = null;
            KeyValuePair<object, string> kvp = new KeyValuePair<object, string>(source, path);
            if (!s_FieldInfoFromPaths.TryGetValue(kvp, out field))
            {
                string[] splittedPath = path.Split('.');
                System.Type type = source.GetType();
                foreach (string t in splittedPath)
                {
                    field = type.GetField(t, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (null == field)
                        break;
                    type = field.FieldType;
                }
                s_FieldInfoFromPaths.Add(kvp, field);
            }
            return field;
        }
        public static string GetFieldPath<T, TValue>(Expression<System.Func<T, TValue>> expr)
        {
            MemberExpression me;
            switch (expr.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    me = ((expr.Body as UnaryExpression)?.Operand) as MemberExpression;
                    break;
                default:
                    me = expr.Body as MemberExpression;
                    break;
            }
            List<string> members = new List<string>();
            while (null != me)
            {
                members.Add(me.Member.Name);
                me = me.Expression as MemberExpression;
            }
            StringBuilder sb = new StringBuilder();
            for (int i = members.Count - 1; i >= 0; --i)
            {
                sb.Append(members[i]);
                if (0 != i) sb.Append('.');
            }
            return sb.ToString();
        }
        public static object GetFieldValue(object source, string name)
        {
            System.Type type = source.GetType();
            while (null != type)
            {
                FieldInfo f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (null != f)
                    return f.GetValue(source);
                type = type.BaseType;
            }
            return null;
        }
        public static object GetFieldValueFromPath(object source, ref System.Type baseType, string path)
        {
            string[] splittedPath = path.Split('.');
            object srcObject = source;
            foreach (string t in splittedPath)
            {
                FieldInfo fieldInfo = baseType.GetField(t, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (null == fieldInfo)
                {
                    baseType = null;
                    break;
                }
                baseType = fieldInfo.FieldType;
                srcObject = GetFieldValue(srcObject, t);
            }
            return null == baseType ? null : srcObject;
        }
        public static object GetParentObject(string path, object obj)
        {
            string[] fields = path.Split('.');
            if (1 == fields.Length)
                return obj;
            FieldInfo info = obj.GetType().GetField(fields[0], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            obj = info.GetValue(obj);
            return GetParentObject(string.Join(".", fields, 1, fields.Length - 1), obj);
        }
    }
}