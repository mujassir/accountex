using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace AccountEx.Common
{
    public static class DynamicQueryable
    {
        public static IQueryable<T> Search<T>(this IQueryable<T> source, string[] columns, string keyword)
        {
            return columns.Aggregate(source, (current, item) => current.Like(item, keyword));
        }

        public static IQueryable<T> Like<T>(this IQueryable<T> source, string propertyName, string keyword)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "param");
            var memberAccess = Expression.MakeMemberAccess(parameter, type.GetProperty(propertyName));

            var constant = Expression.Constant("%" + keyword + "%");
            var contains = memberAccess.Type.GetMethod("Contains");

            var methodExp = Expression.Call(memberAccess, contains, Expression.Constant(keyword));
            var lambda = Expression.Lambda<Func<T, bool>>(methodExp, parameter);
            return source.Where(lambda);

        }

        public static IQueryable Join(this IQueryable outer, System.Collections.IEnumerable inner,
            string outerSelector, string innerSelector, string resultsSelector, params object[] values)
        {
            if (inner == null) throw new ArgumentNullException("inner");
            if (outerSelector == null) throw new ArgumentNullException("outerSelector");
            if (innerSelector == null) throw new ArgumentNullException("innerSelector");
            if (resultsSelector == null) throw new ArgumentNullException("resultsSelector");

            var outerSelectorLambda = DynamicExpression.ParseLambda(itType: outer.ElementType, resultType: null, expression: outerSelector, baseType: null, values: values);
            var enumerable = inner as object[] ?? inner.Cast<object>().ToArray();
            var innerSelectorLambda = DynamicExpression.ParseLambda(itType: enumerable.AsQueryable().ElementType, resultType: null, expression: innerSelector, baseType: null, values: values);
            var parameters = new ParameterExpression[] { Expression.Parameter(outer.ElementType, "outer"), Expression.Parameter(enumerable.AsQueryable().ElementType, "inner") };
            var resultsSelectorLambda = DynamicExpression.ParseLambda(parameters: parameters, baseType: null, resultType: null, expression: resultsSelector, values: values);
            return outer.Provider.CreateQuery(
                Expression.Call(typeof(Queryable), "Join", new Type[]
                                                               {
                                                                   outer.ElementType, enumerable.AsQueryable().ElementType, outerSelectorLambda.Body.Type, resultsSelectorLambda.Body.Type
                                                               }, outer.Expression, enumerable.AsQueryable().Expression, Expression.Quote(outerSelectorLambda), Expression.Quote(innerSelectorLambda), Expression.Quote(resultsSelectorLambda)));
        }       //The generic overload.    

        public static IQueryable<T> Join<T>(this IQueryable<T> outer, IEnumerable<T> inner, string outerSelector, string innerSelector, string resultsSelector, params object[] values)
        {
            return (IQueryable<T>)Join((IQueryable)outer, (System.Collections.IEnumerable)inner, outerSelector, innerSelector, resultsSelector, values);
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, params object[] values)
        {
            return (IQueryable<T>)Where((IQueryable)source, predicate, values);
        }

        public static IQueryable Where(this IQueryable source, string predicate, params object[] values)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            var lambda = DynamicExpression.ParseLambda(source.ElementType, typeof(bool), predicate, null, values);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Where",
                    new Type[] { source.ElementType },
                    source.Expression, Expression.Quote(lambda)));
        }

        public static IQueryable Select(this IQueryable source, string selector, params object[] values)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            var lambda = DynamicExpression.ParseLambda(source.ElementType, null, selector, null, values);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Select",
                    new Type[] { source.ElementType, lambda.Body.Type },
                    source.Expression, Expression.Quote(lambda)));
        }



        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, params object[] values)
        {
            return (IQueryable<T>)OrderBy((IQueryable)source, ordering, values);
        }

        public static IQueryable OrderBy(this IQueryable source, string ordering, params object[] values)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (ordering == null) throw new ArgumentNullException("ordering");
            var parameters = new ParameterExpression[] {
                Expression.Parameter(source.ElementType, "") };
            var parser = new ExpressionParser(parameters, ordering, values);
            var orderings = parser.ParseOrdering();
            var queryExpr = source.Expression;
            var methodAsc = "OrderBy";
            var methodDesc = "OrderByDescending";
            foreach (var o in orderings)
            {
                queryExpr = Expression.Call(
                    typeof(Queryable), o.Ascending ? methodAsc : methodDesc,
                    new Type[] { source.ElementType, o.Selector.Type },
                    queryExpr, Expression.Quote(Expression.Lambda(o.Selector, parameters)));
                methodAsc = "ThenBy";
                methodDesc = "ThenByDescending";
            }
            return source.Provider.CreateQuery(queryExpr);
        }

        public static IQueryable Take(this IQueryable source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Take",
                    new Type[] { source.ElementType },
                    source.Expression, Expression.Constant(count)));
        }

        public static IQueryable Skip(this IQueryable source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Skip",
                    new Type[] { source.ElementType },
                    source.Expression, Expression.Constant(count)));
        }

        public static IQueryable GroupBy(this IQueryable source, string keySelector, string elementSelector, params object[] values)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            if (elementSelector == null) throw new ArgumentNullException("elementSelector");
            var keyLambda = DynamicExpression.ParseLambda(source.ElementType, null, keySelector, null, values);
            var elementLambda = DynamicExpression.ParseLambda(source.ElementType, null, elementSelector, null, values);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "GroupBy",
                    new Type[] { source.ElementType, keyLambda.Body.Type, elementLambda.Body.Type },
                    source.Expression, Expression.Quote(keyLambda), Expression.Quote(elementLambda)));
        }

        public static bool Any(this IQueryable source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return (bool)source.Provider.Execute(
                Expression.Call(
                    typeof(Queryable), "Any",
                    new Type[] { source.ElementType }, source.Expression));
        }

        public static int Count(this IQueryable source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return (int)source.Provider.Execute(
                Expression.Call(
                    typeof(Queryable), "Count",
                    new Type[] { source.ElementType }, source.Expression));
        }
    }


    public abstract class DynamicClass
    {
        public override string ToString()
        {
            var props = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var sb = new StringBuilder();
            sb.Append("{");
            for (var i = 0; i < props.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(props[i].Name);
                sb.Append("=");
                sb.Append(props[i].GetValue(this, null));
            }
            sb.Append("}");
            return sb.ToString();
        }
    }

    public class DynamicProperty
    {
        string _name;
        Type _type;

        public DynamicProperty(string name, Type type, CustomAttributeBuilder attributeBuilder = null)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (type == null) throw new ArgumentNullException("type");
            _name = name;
            _type = type;
            CustomAttributeBuilder = attributeBuilder;

        }

        public string Name
        {
            get { return _name; }
        }

        public Type Type
        {
            get { return _type; }
        }

        public CustomAttributeBuilder CustomAttributeBuilder
        {
            get;
            private set;
        }

    }

    public static class DynamicExpression
    {
        public static Expression Parse(Type resultType, string expression, params object[] values)
        {
            var parser = new ExpressionParser(null, expression, values);
            return parser.Parse(resultType);
        }

        public static LambdaExpression ParseLambda(Type itType, string expression, Type baseType = null, params object[] values)
        {
            return ParseLambda(new ParameterExpression[] { Expression.Parameter(itType, "") }, null, expression, baseType, values);
        }

        public static LambdaExpression ParseLambda(Type itType, Type resultType, string expression, Type baseType = null, params object[] values)
        {
            return ParseLambda(new ParameterExpression[] { Expression.Parameter(itType, "") }, resultType, expression, baseType, values);
        }

        public static LambdaExpression ParseLambda(ParameterExpression[] parameters, Type resultType, string expression, Type baseType = null, params object[] values)
        {
            var parser = new ExpressionParser(parameters, expression, values);
            return Expression.Lambda(parser.Parse(resultType, baseType), parameters);
        }

        public static Expression<Func<T, TS>> ParseLambda<T, TS>(string expression, params object[] values)
        {
            return (Expression<Func<T, TS>>)ParseLambda(typeof(T), typeof(TS), expression, null, values);
        }

        public static Type CreateClass(params DynamicProperty[] properties)
        {
            return ClassFactory.Instance.GetDynamicClass(properties);
        }

        public static Type CreateClass(IEnumerable<DynamicProperty> properties)
        {
            return ClassFactory.Instance.GetDynamicClass(properties);
        }

        public static Type CreateClass(Type resultType = null, Type baseType = null, params DynamicProperty[] properties)
        {
            return ClassFactory.Instance.GetDynamicClass(properties, resultType, baseType);
        }

        public static Type CreateClass(IEnumerable<DynamicProperty> properties, Type resultType = null, Type baseType = null)
        {
            return ClassFactory.Instance.GetDynamicClass(properties, resultType, baseType);
        }
    }

    internal class DynamicOrdering
    {
        public Expression Selector;
        public bool Ascending;
    }

    internal class Signature : IEquatable<Signature>
    {
        public DynamicProperty[] Properties;
        public int HashCode;

        public Signature(IEnumerable<DynamicProperty> properties)
        {
            Properties = properties.ToArray();
            HashCode = 0;
            foreach (var p in properties)
            {
                HashCode ^= p.Name.GetHashCode() ^ p.Type.GetHashCode();
            }
        }

        public override int GetHashCode()
        {
            return HashCode;
        }

        public override bool Equals(object obj)
        {
            return obj is Signature ? Equals((Signature)obj) : false;
        }

        public bool Equals(Signature other)
        {
            if (Properties.Length != other.Properties.Length) return false;
            for (var i = 0; i < Properties.Length; i++)
            {
                if (Properties[i].Name != other.Properties[i].Name ||
                    Properties[i].Type != other.Properties[i].Type) return false;
            }
            return true;
        }
    }

    public class ClassFactory
    {
        public static readonly ClassFactory Instance = new ClassFactory();

        static ClassFactory() { }  // Trigger lazy initialization of static fields

        ModuleBuilder _module;
        Dictionary<Signature, Type> _classes;
        int _classCount;
        ReaderWriterLock _rwLock;

        private ClassFactory()
        {
            var name = new AssemblyName("DynamicClasses");
            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
#if ENABLE_LINQ_PARTIAL_TRUST
            new ReflectionPermission(PermissionState.Unrestricted).Assert();
#endif
            try
            {
                _module = assembly.DefineDynamicModule("Module");
            }
            finally
            {
#if ENABLE_LINQ_PARTIAL_TRUST
                PermissionSet.RevertAssert();
#endif
            }
            _classes = new Dictionary<Signature, Type>();
            _rwLock = new ReaderWriterLock();
        }

        public Type GetDynamicClass(IEnumerable<DynamicProperty> properties)
        {
            _rwLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                var signature = new Signature(properties);
                Type type;
                if (!_classes.TryGetValue(signature, out type))
                {
                    type = CreateDynamicClass(signature.Properties);
                    _classes.Add(signature, type);
                }
                return type;
            }
            finally
            {
                _rwLock.ReleaseReaderLock();
            }
        }

        public Type GetDynamicClass(IEnumerable<DynamicProperty> properties, Type resultType = null, Type baseType = null)
        {
            _rwLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                var signature = new Signature(properties);
                Type type;
                if (!_classes.TryGetValue(signature, out type))
                {
                    if (resultType == null)
                        type = CreateDynamicClass(signature.Properties);
                    else
                    {
                        type = CreateDynamicClass(signature.Properties, resultType, baseType);
                    }

                    _classes.Add(signature, type);
                }
                return type;
            }
            finally
            {
                _rwLock.ReleaseReaderLock();
            }
        }

        Type CreateDynamicClass(DynamicProperty[] properties)
        {
            var cookie = _rwLock.UpgradeToWriterLock(Timeout.Infinite);
            try
            {
                var typeName = "DynamicClass" + (_classCount + 1);
#if ENABLE_LINQ_PARTIAL_TRUST
                new ReflectionPermission(PermissionState.Unrestricted).Assert();
#endif
                try
                {
                    var tb = _module.DefineType(typeName, TypeAttributes.Class |
                        TypeAttributes.Public, typeof(object));

                    var fields = GenerateProperties(tb, properties);
                    GenerateEquals(tb, fields);
                    GenerateGetHashCode(tb, fields);
                    var result = tb.CreateType();
                    return result;
                }
                finally
                {
#if ENABLE_LINQ_PARTIAL_TRUST
                    PermissionSet.RevertAssert();
#endif
                }
            }
            finally
            {
                _classCount++;
                _rwLock.DowngradeFromWriterLock(ref cookie);
            }
        }

        Type CreateDynamicClass(DynamicProperty[] properties, Type resultType, Type baseType = null)
        {
            var cookie = _rwLock.UpgradeToWriterLock(Timeout.Infinite);
            try
            {
                var typeName = "DynamicClass" + (_classCount + 1);
#if ENABLE_LINQ_PARTIAL_TRUST
                new ReflectionPermission(PermissionState.Unrestricted).Assert();
#endif
                try
                {

                    TypeBuilder tb = null;

                    if (baseType != null)
                    {
                        tb = _module.DefineType(typeName, TypeAttributes.Class |
                                                                      TypeAttributes.Public, baseType);
                    }
                    else
                    {
                        tb = _module.DefineType(typeName, TypeAttributes.Class |
                                                                          TypeAttributes.Public, typeof(object));
                    }

                    tb.AddInterfaceImplementation(resultType);
                    var fields = GenerateProperties(tb, properties, resultType);

                    GenerateEquals(tb, fields);
                    GenerateGetHashCode(tb, fields);


                    var result = tb.CreateType();
                    return result;
                }
                finally
                {
#if ENABLE_LINQ_PARTIAL_TRUST
                    PermissionSet.RevertAssert();
#endif
                }
            }
            finally
            {
                _classCount++;
                _rwLock.DowngradeFromWriterLock(ref cookie);
            }
        }

        FieldInfo[] GenerateProperties(TypeBuilder tb, DynamicProperty[] properties, Type returnType = null)
        {
            FieldInfo[] fields = new FieldBuilder[properties.Length];
            for (var i = 0; i < properties.Length; i++)
            {
                var dp = properties[i];
                PrintPropertyInfo(dp);
                var fb = tb.DefineField("_" + dp.Name, dp.Type, FieldAttributes.Private);
                var pb = tb.DefineProperty(dp.Name, PropertyAttributes.HasDefault, dp.Type, null);

                if (dp.CustomAttributeBuilder != null)
                    pb.SetCustomAttribute(dp.CustomAttributeBuilder);



                var mbGet = tb.DefineMethod("get_" + dp.Name,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                    dp.Type, Type.EmptyTypes);
                var genGet = mbGet.GetILGenerator();
                genGet.Emit(OpCodes.Ldarg_0);
                genGet.Emit(OpCodes.Ldfld, fb);
                genGet.Emit(OpCodes.Ret);

                if (returnType != null)
                {
                    var returnTypeMi = returnType.GetMethod(mbGet.Name);

                    if (returnTypeMi != null)
                        tb.DefineMethodOverride(mbGet, returnTypeMi);
                }

                var mbSet = tb.DefineMethod("set_" + dp.Name,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                    null, new Type[] { dp.Type });
                var genSet = mbSet.GetILGenerator();
                genSet.Emit(OpCodes.Ldarg_0);
                genSet.Emit(OpCodes.Ldarg_1);
                genSet.Emit(OpCodes.Stfld, fb);
                genSet.Emit(OpCodes.Ret);

                if (returnType != null)
                {
                    var returnTypeMi = returnType.GetMethod(mbSet.Name);

                    if (returnTypeMi != null)
                        tb.DefineMethodOverride(mbSet, returnTypeMi);
                }

                pb.SetGetMethod(mbGet);
                pb.SetSetMethod(mbSet);



                fields[i] = fb;
            }
            return fields;
        }

        [Conditional("DEBUG")]
        void PrintPropertyInfo(DynamicProperty dp)
        {
            Debug.WriteLine(String.Format("Building Property {0} of Type {1}", dp.Name, dp.Type.Name));
        }


        void GenerateEquals(TypeBuilder tb, FieldInfo[] fields)
        {
            var mb = tb.DefineMethod("Equals",
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(bool), new Type[] { typeof(object) });
            var gen = mb.GetILGenerator();
            var other = gen.DeclareLocal(tb);
            var next = gen.DefineLabel();
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Isinst, tb);
            gen.Emit(OpCodes.Stloc, other);
            gen.Emit(OpCodes.Ldloc, other);
            gen.Emit(OpCodes.Brtrue_S, next);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ret);
            gen.MarkLabel(next);
            foreach (var field in fields)
            {
                var ft = field.FieldType;
                var ct = typeof(EqualityComparer<>).MakeGenericType(ft);
                next = gen.DefineLabel();
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
                gen.Emit(OpCodes.Ldloc, other);
                gen.Emit(OpCodes.Ldfld, field);
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("Equals", new Type[] { ft, ft }), null);
                gen.Emit(OpCodes.Brtrue_S, next);
                gen.Emit(OpCodes.Ldc_I4_0);
                gen.Emit(OpCodes.Ret);
                gen.MarkLabel(next);
            }
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Ret);
        }

        void GenerateGetHashCode(TypeBuilder tb, FieldInfo[] fields)
        {
            var mb = tb.DefineMethod("GetHashCode",
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(int), Type.EmptyTypes);
            var gen = mb.GetILGenerator();
            gen.Emit(OpCodes.Ldc_I4_0);
            foreach (var field in fields)
            {
                var ft = field.FieldType;
                var ct = typeof(EqualityComparer<>).MakeGenericType(ft);
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("GetHashCode", new Type[] { ft }), null);
                gen.Emit(OpCodes.Xor);
            }
            gen.Emit(OpCodes.Ret);
        }
    }

    public sealed class ParseException : Exception
    {
        int _position;

        public ParseException(string message, int position)
            : base(message)
        {
            _position = position;
        }

        public int Position
        {
            get { return _position; }
        }

        public override string ToString()
        {
            return string.Format(Res.ParseExceptionFormat, Message, _position);
        }
    }

    internal class ExpressionParser
    {
        struct Token
        {
            public TokenId Id;
            public string Text;
            public int Pos;
        }

        enum TokenId
        {
            Unknown,
            End,
            Identifier,
            StringLiteral,
            IntegerLiteral,
            RealLiteral,
            Exclamation,
            Percent,
            Amphersand,
            OpenParen,
            CloseParen,
            Asterisk,
            Plus,
            Comma,
            Minus,
            Dot,
            Slash,
            Colon,
            LessThan,
            Equal,
            GreaterThan,
            Question,
            OpenBracket,
            CloseBracket,
            Bar,
            ExclamationEqual,
            DoubleAmphersand,
            LessThanEqual,
            LessGreater,
            DoubleEqual,
            GreaterThanEqual,
            DoubleBar
        }

        interface ILogicalSignatures
        {
            void F(bool x, bool y);
            void F(bool? x, bool? y);
        }

        interface IArithmeticSignatures
        {
            void F(int x, int y);
            void F(uint x, uint y);
            void F(long x, long y);
            void F(ulong x, ulong y);
            void F(float x, float y);
            void F(double x, double y);
            void F(decimal x, decimal y);
            void F(int? x, int? y);
            void F(uint? x, uint? y);
            void F(long? x, long? y);
            void F(ulong? x, ulong? y);
            void F(float? x, float? y);
            void F(double? x, double? y);
            void F(decimal? x, decimal? y);
        }

        interface IRelationalSignatures : IArithmeticSignatures
        {
            void F(string x, string y);
            void F(char x, char y);
            void F(DateTime x, DateTime y);
            void F(TimeSpan x, TimeSpan y);
            void F(char? x, char? y);
            void F(DateTime? x, DateTime? y);
            void F(TimeSpan? x, TimeSpan? y);
        }

        interface IEqualitySignatures : IRelationalSignatures
        {
            void F(bool x, bool y);
            void F(bool? x, bool? y);
            void F(Guid x, Guid y);
            void F(Guid? x, Guid? y);
        }

        interface IAddSignatures : IArithmeticSignatures
        {
            void F(DateTime x, TimeSpan y);
            void F(TimeSpan x, TimeSpan y);
            void F(DateTime? x, TimeSpan? y);
            void F(TimeSpan? x, TimeSpan? y);
        }

        interface ISubtractSignatures : IAddSignatures
        {
            void F(DateTime x, DateTime y);
            void F(DateTime? x, DateTime? y);
        }

        interface INegationSignatures
        {
            void F(int x);
            void F(long x);
            void F(float x);
            void F(double x);
            void F(decimal x);
            void F(int? x);
            void F(long? x);
            void F(float? x);
            void F(double? x);
            void F(decimal? x);
        }

        interface INotSignatures
        {
            void F(bool x);
            void F(bool? x);
        }

        interface IEnumerableSignatures
        {
            void Where(bool predicate);
            void Any();
            void Any(bool predicate);
            void All(bool predicate);
            void Count();
            void Count(bool predicate);
            void Min(object selector);
            void Max(object selector);
            void Sum(int selector);
            void Sum(int? selector);
            void Sum(long selector);
            void Sum(long? selector);
            void Sum(float selector);
            void Sum(float? selector);
            void Sum(double selector);
            void Sum(double? selector);
            void Sum(decimal selector);
            void Sum(decimal? selector);
            void Average(int selector);
            void Average(int? selector);
            void Average(long selector);
            void Average(long? selector);
            void Average(float selector);
            void Average(float? selector);
            void Average(double selector);
            void Average(double? selector);
            void Average(decimal selector);
            void Average(decimal? selector);
        }

        static readonly Type[] PredefinedTypes = {
            typeof(Object),
            typeof(Boolean),
            typeof(Char),
            typeof(String),
            typeof(SByte),
            typeof(Byte),
            typeof(Int16),
            typeof(UInt16),
            typeof(Int32),
            typeof(UInt32),
            typeof(Int64),
            typeof(UInt64),
            typeof(Single),
            typeof(Double),
            typeof(Decimal),
            typeof(DateTime),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(Math),
            typeof(Convert)
        };

        static readonly Expression TrueLiteral = Expression.Constant(true);
        static readonly Expression FalseLiteral = Expression.Constant(false);
        static readonly Expression NullLiteral = Expression.Constant(null);

        private const string KeywordIt = "it";
        private const string KeywordIif = "iif";
        static readonly string KeywordNew = "new";

        static Dictionary<string, object> _keywords;

        Dictionary<string, object> _symbols;
        IDictionary<string, object> _externals;
        Dictionary<Expression, string> _literals;
        ParameterExpression _it;
        string _text;
        int _textPos;
        int _textLen;
        char _ch;
        Token _token;

        private Type _interfaceType = null;
        private Type _baseType = null;

        public ExpressionParser(ParameterExpression[] parameters, string expression, object[] values)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            if (_keywords == null) _keywords = CreateKeywords();
            _symbols = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            _literals = new Dictionary<Expression, string>();
            if (parameters != null) ProcessParameters(parameters);
            if (values != null) ProcessValues(values);
            _text = expression;
            _textLen = _text.Length;
            SetTextPos(0);
            NextToken();
        }

        void ProcessParameters(ParameterExpression[] parameters)
        {
            foreach (var pe in parameters)
                if (!String.IsNullOrEmpty(pe.Name))
                    AddSymbol(pe.Name, pe);
            if (parameters.Length == 1 && String.IsNullOrEmpty(parameters[0].Name))
                _it = parameters[0];
        }

        void ProcessValues(object[] values)
        {
            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                if (i == values.Length - 1 && value is IDictionary<string, object>)
                {
                    _externals = (IDictionary<string, object>)value;
                }
                else
                {
                    AddSymbol("@" + i.ToString(System.Globalization.CultureInfo.InvariantCulture), value);
                }
            }
        }

        void AddSymbol(string name, object value)
        {
            if (_symbols.ContainsKey(name))
                throw ParseError(Res.DuplicateIdentifier, name);
            _symbols.Add(name, value);
        }

        public Expression Parse(Type resultType, Type baseType = null)
        {
            //Update base resultType

            _interfaceType = resultType;
            _baseType = baseType;

            var exprPos = _token.Pos;
            var expr = ParseExpression(resultType);
            if (resultType != null)
                if ((expr = PromoteExpression(expr, resultType, true)) == null)
                    throw ParseError(exprPos, Res.ExpressionTypeMismatch, GetTypeName(resultType));
            ValidateToken(TokenId.End, Res.SyntaxError);
            return expr;
        }

#pragma warning disable 0219
        public IEnumerable<DynamicOrdering> ParseOrdering()
        {
            var orderings = new List<DynamicOrdering>();
            while (true)
            {
                var expr = ParseExpression();
                var ascending = true;
                if (TokenIdentifierIs("asc") || TokenIdentifierIs("ascending"))
                {
                    NextToken();
                }
                else if (TokenIdentifierIs("desc") || TokenIdentifierIs("descending"))
                {
                    NextToken();
                    ascending = false;
                }
                orderings.Add(new DynamicOrdering { Selector = expr, Ascending = ascending });
                if (_token.Id != TokenId.Comma) break;
                NextToken();
            }
            ValidateToken(TokenId.End, Res.SyntaxError);
            return orderings;
        }
#pragma warning restore 0219

        // ?: operator
        Expression ParseExpression(Type resultType = null)
        {
            var errorPos = _token.Pos;
            var expr = ParseLogicalOr(resultType);
            if (_token.Id == TokenId.Question)
            {
                NextToken();
                var expr1 = ParseExpression(resultType);
                ValidateToken(TokenId.Colon, Res.ColonExpected);
                NextToken();
                var expr2 = ParseExpression(resultType);
                expr = GenerateConditional(expr, expr1, expr2, errorPos);
            }
            return expr;
        }

        // ||, or operator
        Expression ParseLogicalOr(Type resultType = null)
        {
            var left = ParseLogicalAnd(resultType);
            while (_token.Id == TokenId.DoubleBar || TokenIdentifierIs("or"))
            {
                var op = _token;
                NextToken();
                var right = ParseLogicalAnd(resultType);
                CheckAndPromoteOperands(typeof(ILogicalSignatures), op.Text, ref left, ref right, op.Pos);
                left = Expression.OrElse(left, right);
            }
            return left;


        }

        // &&, and operator
        Expression ParseLogicalAnd(Type resultType = null)
        {
            var left = ParseComparison(resultType);
            while (_token.Id == TokenId.DoubleAmphersand || TokenIdentifierIs("and"))
            {
                var op = _token;
                NextToken();
                var right = ParseComparison(resultType);
                CheckAndPromoteOperands(typeof(ILogicalSignatures), op.Text, ref left, ref right, op.Pos);
                left = Expression.AndAlso(left, right);
            }
            return left;
        }

        // =, ==, !=, <>, >, >=, <, <= operators
        Expression ParseComparison(Type resultType = null)
        {
            var left = ParseAdditive(resultType);
            while (_token.Id == TokenId.Equal || _token.Id == TokenId.DoubleEqual ||
                _token.Id == TokenId.ExclamationEqual || _token.Id == TokenId.LessGreater ||
                _token.Id == TokenId.GreaterThan || _token.Id == TokenId.GreaterThanEqual ||
                _token.Id == TokenId.LessThan || _token.Id == TokenId.LessThanEqual)
            {
                var op = _token;
                NextToken();
                var right = ParseAdditive(resultType);
                var isEquality = op.Id == TokenId.Equal || op.Id == TokenId.DoubleEqual ||
                    op.Id == TokenId.ExclamationEqual || op.Id == TokenId.LessGreater;
                if (isEquality && !left.Type.IsValueType && !right.Type.IsValueType)
                {
                    if (left.Type != right.Type)
                    {
                        if (left.Type.IsAssignableFrom(right.Type))
                        {
                            right = Expression.Convert(right, left.Type);
                        }
                        else if (right.Type.IsAssignableFrom(left.Type))
                        {
                            left = Expression.Convert(left, right.Type);
                        }
                        else
                        {
                            throw IncompatibleOperandsError(op.Text, left, right, op.Pos);
                        }
                    }
                }
                else if (IsEnumType(left.Type) || IsEnumType(right.Type))
                {
                    if (left.Type != right.Type)
                    {
                        Expression e;
                        if ((e = PromoteExpression(right, left.Type, true)) != null)
                        {
                            right = e;
                        }
                        else if ((e = PromoteExpression(left, right.Type, true)) != null)
                        {
                            left = e;
                        }
                        else
                        {
                            throw IncompatibleOperandsError(op.Text, left, right, op.Pos);
                        }
                    }
                }
                else
                {
                    CheckAndPromoteOperands(isEquality ? typeof(IEqualitySignatures) : typeof(IRelationalSignatures),
                        op.Text, ref left, ref right, op.Pos);
                }
                switch (op.Id)
                {
                    case TokenId.Equal:
                    case TokenId.DoubleEqual:
                        left = GenerateEqual(left, right);
                        break;
                    case TokenId.ExclamationEqual:
                    case TokenId.LessGreater:
                        left = GenerateNotEqual(left, right);
                        break;
                    case TokenId.GreaterThan:
                        left = GenerateGreaterThan(left, right);
                        break;
                    case TokenId.GreaterThanEqual:
                        left = GenerateGreaterThanEqual(left, right);
                        break;
                    case TokenId.LessThan:
                        left = GenerateLessThan(left, right);
                        break;
                    case TokenId.LessThanEqual:
                        left = GenerateLessThanEqual(left, right);
                        break;
                }
            }
            return left;
        }

        // +, -, & operators
        Expression ParseAdditive(Type resultType = null)
        {
            var left = ParseMultiplicative(resultType);
            while (_token.Id == TokenId.Plus || _token.Id == TokenId.Minus ||
                _token.Id == TokenId.Amphersand)
            {
                var op = _token;
                NextToken();
                var right = ParseMultiplicative(resultType);
                switch (op.Id)
                {
                    case TokenId.Plus:
                        if (left.Type == typeof(string) || right.Type == typeof(string))
                            goto case TokenId.Amphersand;
                        CheckAndPromoteOperands(typeof(IAddSignatures), op.Text, ref left, ref right, op.Pos);
                        left = GenerateAdd(left, right);
                        break;
                    case TokenId.Minus:
                        CheckAndPromoteOperands(typeof(ISubtractSignatures), op.Text, ref left, ref right, op.Pos);
                        left = GenerateSubtract(left, right);
                        break;
                    case TokenId.Amphersand:
                        left = GenerateStringConcat(left, right);
                        break;
                }
            }
            return left;
        }

        // *, /, %, mod operators
        Expression ParseMultiplicative(Type resultType = null)
        {
            var left = ParseUnary(resultType);
            while (_token.Id == TokenId.Asterisk || _token.Id == TokenId.Slash ||
                _token.Id == TokenId.Percent || TokenIdentifierIs("mod"))
            {
                var op = _token;
                NextToken();
                var right = ParseUnary(resultType);
                CheckAndPromoteOperands(typeof(IArithmeticSignatures), op.Text, ref left, ref right, op.Pos);
                switch (op.Id)
                {
                    case TokenId.Asterisk:
                        left = Expression.Multiply(left, right);
                        break;
                    case TokenId.Slash:
                        left = Expression.Divide(left, right);
                        break;
                    case TokenId.Percent:
                    case TokenId.Identifier:
                        left = Expression.Modulo(left, right);
                        break;
                }
            }
            return left;
        }

        // -, !, not unary operators
        Expression ParseUnary(Type resultType = null)
        {
            if (_token.Id == TokenId.Minus || _token.Id == TokenId.Exclamation ||
                TokenIdentifierIs("not"))
            {
                var op = _token;
                NextToken();
                if (op.Id == TokenId.Minus && (_token.Id == TokenId.IntegerLiteral ||
                    _token.Id == TokenId.RealLiteral))
                {
                    _token.Text = "-" + _token.Text;
                    _token.Pos = op.Pos;
                    return ParsePrimary(resultType);
                }
                var expr = ParseUnary(resultType);
                if (op.Id == TokenId.Minus)
                {
                    CheckAndPromoteOperand(typeof(INegationSignatures), op.Text, ref expr, op.Pos);
                    expr = Expression.Negate(expr);
                }
                else
                {
                    CheckAndPromoteOperand(typeof(INotSignatures), op.Text, ref expr, op.Pos);
                    expr = Expression.Not(expr);
                }
                return expr;
            }
            return ParsePrimary(resultType);
        }

        Expression ParsePrimary(Type resultType = null)
        {
            var expr = ParsePrimaryStart(resultType);
            while (true)
            {
                if (_token.Id == TokenId.Dot)
                {
                    NextToken();
                    expr = ParseMemberAccess(null, expr);
                }
                else if (_token.Id == TokenId.OpenBracket)
                {
                    expr = ParseElementAccess(expr);
                }
                else
                {
                    break;
                }
            }
            return expr;
        }

        Expression ParsePrimaryStart(Type resultType = null)
        {
            switch (_token.Id)
            {
                case TokenId.Identifier:
                    return ParseIdentifier(resultType);
                case TokenId.StringLiteral:
                    return ParseStringLiteral(resultType);
                case TokenId.IntegerLiteral:
                    return ParseIntegerLiteral(resultType);
                case TokenId.RealLiteral:
                    return ParseRealLiteral(resultType);
                case TokenId.OpenParen:
                    return ParseParenExpression(resultType);
                default:
                    throw ParseError(Res.ExpressionExpected);
            }
        }

        Expression ParseStringLiteral(Type resultType = null)
        {
            ValidateToken(TokenId.StringLiteral);
            var quote = _token.Text[0];
            var s = _token.Text.Substring(1, _token.Text.Length - 2);
            var start = 0;
            while (true)
            {
                var i = s.IndexOf(quote, start);
                if (i < 0) break;
                s = s.Remove(i, 1);
                start = i + 1;
            }
            if (quote == '\'')
            {
                if (s.Length != 1)
                    throw ParseError(Res.InvalidCharacterLiteral);
                NextToken();
                return CreateLiteral(s[0], s);
            }
            NextToken();
            return CreateLiteral(s, s);
        }

        Expression ParseIntegerLiteral(Type resultType = null)
        {
            ValidateToken(TokenId.IntegerLiteral);
            var text = _token.Text;
            if (text[0] != '-')
            {
                ulong value;
                if (!UInt64.TryParse(text, out value))
                    throw ParseError(Res.InvalidIntegerLiteral, text);
                NextToken();
                if (value <= (ulong)Int32.MaxValue) return CreateLiteral((int)value, text);
                if (value <= (ulong)UInt32.MaxValue) return CreateLiteral((uint)value, text);
                if (value <= (ulong)Int64.MaxValue) return CreateLiteral((long)value, text);
                return CreateLiteral(value, text);
            }
            else
            {
                long value;
                if (!Int64.TryParse(text, out value))
                    throw ParseError(Res.InvalidIntegerLiteral, text);
                NextToken();
                if (value >= Int32.MinValue && value <= Int32.MaxValue)
                    return CreateLiteral((int)value, text);
                return CreateLiteral(value, text);
            }
        }

        Expression ParseRealLiteral(Type resultType = null)
        {
            ValidateToken(TokenId.RealLiteral);
            var text = _token.Text;
            object value = null;
            var last = text[text.Length - 1];
            if (last == 'F' || last == 'f')
            {
                float f;
                if (Single.TryParse(text.Substring(0, text.Length - 1), out f)) value = f;
            }
            else
            {
                double d;
                if (Double.TryParse(text, out d)) value = d;
            }
            if (value == null) throw ParseError(Res.InvalidRealLiteral, text);
            NextToken();
            return CreateLiteral(value, text);
        }

        Expression CreateLiteral(object value, string text)
        {
            var expr = Expression.Constant(value);
            _literals.Add(expr, text);
            return expr;
        }

        Expression ParseParenExpression(Type resultType = null)
        {
            ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            NextToken();
            var e = ParseExpression();
            ValidateToken(TokenId.CloseParen, Res.CloseParenOrOperatorExpected);
            NextToken();
            return e;
        }

        Expression ParseIdentifier(Type resultType = null)
        {
            ValidateToken(TokenId.Identifier);
            object value;
            if (_keywords.TryGetValue(_token.Text, out value))
            {
                if (value is Type) return ParseTypeAccess((Type)value);
                if (value == (object)KeywordIt) return ParseIt(resultType);
                if (value == (object)KeywordIif) return ParseIif(resultType);
                if (value == (object)KeywordNew) return ParseNew(resultType);
                NextToken();
                return (Expression)value;
            }
            if (_symbols.TryGetValue(_token.Text, out value) ||
                _externals != null && _externals.TryGetValue(_token.Text, out value))
            {
                var expr = value as Expression;
                if (expr == null)
                {
                    expr = Expression.Constant(value);
                }
                else
                {
                    var lambda = expr as LambdaExpression;
                    if (lambda != null) return ParseLambdaInvocation(lambda);
                }
                NextToken();
                return expr;
            }
            if (_it != null) return ParseMemberAccess(null, _it);
            throw ParseError(Res.UnknownIdentifier, _token.Text);
        }

        Expression ParseIt(Type resultType = null)
        {
            if (_it == null)
                throw ParseError(Res.NoItInScope);
            NextToken();
            return _it;
        }

        Expression ParseIif(Type resultType = null)
        {
            var errorPos = _token.Pos;
            NextToken();
            var args = ParseArgumentList();
            if (args.Length != 3)
                throw ParseError(errorPos, Res.IifRequiresThreeArgs);
            return GenerateConditional(args[0], args[1], args[2], errorPos);
        }

        Expression GenerateConditional(Expression test, Expression expr1, Expression expr2, int errorPos)
        {
            if (test.Type != typeof(bool))
                throw ParseError(errorPos, Res.FirstExprMustBeBool);
            if (expr1.Type != expr2.Type)
            {
                var expr1As2 = expr2 != NullLiteral ? PromoteExpression(expr1, expr2.Type, true) : null;
                var expr2As1 = expr1 != NullLiteral ? PromoteExpression(expr2, expr1.Type, true) : null;
                if (expr1As2 != null && expr2As1 == null)
                {
                    expr1 = expr1As2;
                }
                else if (expr2As1 != null && expr1As2 == null)
                {
                    expr2 = expr2As1;
                }
                else
                {
                    var type1 = expr1 != NullLiteral ? expr1.Type.Name : "null";
                    var type2 = expr2 != NullLiteral ? expr2.Type.Name : "null";
                    if (expr1As2 != null && expr2As1 != null)
                        throw ParseError(errorPos, Res.BothTypesConvertToOther, type1, type2);
                    throw ParseError(errorPos, Res.NeitherTypeConvertsToOther, type1, type2);
                }
            }
            return Expression.Condition(test, expr1, expr2);
        }

        Expression ParseNew(Type resultType = null)
        {
            NextToken();
            ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            NextToken();
            var properties = new List<DynamicProperty>();
            var expressions = new List<Expression>();
            while (true)
            {
                var exprPos = _token.Pos;
                var expr = ParseExpression();
                string propName;
                if (TokenIdentifierIs("as"))
                {
                    NextToken();
                    propName = GetIdentifier();
                    NextToken();
                }
                else
                {
                    var me = expr as MemberExpression;
                    if (me == null) throw ParseError(exprPos, Res.MissingAsClause);
                    propName = me.Member.Name;
                }
                expressions.Add(expr);
                properties.Add(new DynamicProperty(propName, expr.Type));
                if (_token.Id != TokenId.Comma) break;
                NextToken();
            }
            ValidateToken(TokenId.CloseParen, Res.CloseParenOrCommaExpected);
            NextToken();

            Type type = null;

            if (_interfaceType != null)
                type = DynamicExpression.CreateClass(properties, _interfaceType, _baseType);
            else
                type = DynamicExpression.CreateClass(properties);

            var bindings = new MemberBinding[properties.Count];
            for (var i = 0; i < bindings.Length; i++)
                bindings[i] = Expression.Bind(type.GetProperty(properties[i].Name), expressions[i]);
            return Expression.MemberInit(Expression.New(type), bindings);
        }

        Expression ParseLambdaInvocation(LambdaExpression lambda)
        {
            var errorPos = _token.Pos;
            NextToken();
            var args = ParseArgumentList();
            MethodBase method;
            if (FindMethod(lambda.Type, "Invoke", false, args, out method) != 1)
                throw ParseError(errorPos, Res.ArgsIncompatibleWithLambda);
            return Expression.Invoke(lambda, args);
        }

        Expression ParseTypeAccess(Type type)
        {
            var errorPos = _token.Pos;
            NextToken();
            if (_token.Id == TokenId.Question)
            {
                if (!type.IsValueType || IsNullableType(type))
                    throw ParseError(errorPos, Res.TypeHasNoNullableForm, GetTypeName(type));
                type = typeof(Nullable<>).MakeGenericType(type);
                NextToken();
            }
            if (_token.Id == TokenId.OpenParen)
            {
                var args = ParseArgumentList();
                MethodBase method;
                switch (FindBestMethod(type.GetConstructors(), args, out method))
                {
                    case 0:
                        if (args.Length == 1)
                            return GenerateConversion(args[0], type, errorPos);
                        throw ParseError(errorPos, Res.NoMatchingConstructor, GetTypeName(type));
                    case 1:
                        return Expression.New((ConstructorInfo)method, args);
                    default:
                        throw ParseError(errorPos, Res.AmbiguousConstructorInvocation, GetTypeName(type));
                }
            }
            ValidateToken(TokenId.Dot, Res.DotOrOpenParenExpected);
            NextToken();
            return ParseMemberAccess(type, null);
        }

        Expression GenerateConversion(Expression expr, Type type, int errorPos)
        {
            var exprType = expr.Type;
            if (exprType == type) return expr;
            if (exprType.IsValueType && type.IsValueType)
            {
                if ((IsNullableType(exprType) || IsNullableType(type)) &&
                    GetNonNullableType(exprType) == GetNonNullableType(type))
                    return Expression.Convert(expr, type);
                if ((IsNumericType(exprType) || IsEnumType(exprType)) &&
                    (IsNumericType(type)) || IsEnumType(type))
                    return Expression.ConvertChecked(expr, type);
            }
            if (exprType.IsAssignableFrom(type) || type.IsAssignableFrom(exprType) ||
                exprType.IsInterface || type.IsInterface)
                return Expression.Convert(expr, type);
            throw ParseError(errorPos, Res.CannotConvertValue,
                GetTypeName(exprType), GetTypeName(type));
        }

        Expression ParseMemberAccess(Type type, Expression instance)
        {
            if (instance != null) type = instance.Type;
            var errorPos = _token.Pos;
            var id = GetIdentifier();
            NextToken();
            if (_token.Id == TokenId.OpenParen)
            {
                if (instance != null && type != typeof(string))
                {
                    var enumerableType = FindGenericType(typeof(IEnumerable<>), type);
                    if (enumerableType != null)
                    {
                        var elementType = enumerableType.GetGenericArguments()[0];
                        return ParseAggregate(instance, elementType, id, errorPos);
                    }
                }
                var args = ParseArgumentList();
                MethodBase mb;
                switch (FindMethod(type, id, instance == null, args, out mb))
                {
                    case 0:
                        throw ParseError(errorPos, Res.NoApplicableMethod,
                            id, GetTypeName(type));
                    case 1:
                        var method = (MethodInfo)mb;
                        //HM removed this as it seemed to be breaking stuff
                        //if (!IsPredefinedType(method.DeclaringType))
                        //    throw ParseError(errorPos, Res.MethodsAreInaccessible, GetTypeName(method.DeclaringType));
                        if (method.ReturnType == typeof(void))
                            throw ParseError(errorPos, Res.MethodIsVoid,
                                id, GetTypeName(method.DeclaringType));
                        return Expression.Call(instance, (MethodInfo)method, args);
                    default:
                        throw ParseError(errorPos, Res.AmbiguousMethodInvocation,
                            id, GetTypeName(type));
                }
            }
            var member = FindPropertyOrField(type, id, instance == null);



            if (member == null)
                throw ParseError(errorPos, Res.UnknownPropertyOrField,
                    id, GetTypeName(type));
            return member is PropertyInfo ?
                Expression.Property(instance, (PropertyInfo)member) :
                Expression.Field(instance, (FieldInfo)member);
        }

        static Type FindGenericType(Type generic, Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == generic) return type;
                if (generic.IsInterface)
                {
                    foreach (var intfType in type.GetInterfaces())
                    {
                        var found = FindGenericType(generic, intfType);
                        if (found != null) return found;
                    }
                }
                type = type.BaseType;
            }
            return null;
        }

        Expression ParseAggregate(Expression instance, Type elementType, string methodName, int errorPos)
        {
            var outerIt = _it;
            var innerIt = Expression.Parameter(elementType, "");
            _it = innerIt;
            var args = ParseArgumentList();
            _it = outerIt;
            MethodBase signature;
            if (FindMethod(typeof(IEnumerableSignatures), methodName, false, args, out signature) != 1)
                throw ParseError(errorPos, Res.NoApplicableAggregate, methodName);
            Type[] typeArgs;
            if (signature.Name == "Min" || signature.Name == "Max")
            {
                typeArgs = new Type[] { elementType, args[0].Type };
            }
            else
            {
                typeArgs = new Type[] { elementType };
            }
            if (args.Length == 0)
            {
                args = new Expression[] { instance };
            }
            else
            {
                args = new Expression[] { instance, Expression.Lambda(args[0], innerIt) };
            }
            return Expression.Call(typeof(Enumerable), signature.Name, typeArgs, args);
        }

        Expression[] ParseArgumentList()
        {
            ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            NextToken();
            var args = _token.Id != TokenId.CloseParen ? ParseArguments() : new Expression[0];
            ValidateToken(TokenId.CloseParen, Res.CloseParenOrCommaExpected);
            NextToken();
            return args;
        }

        Expression[] ParseArguments()
        {
            var argList = new List<Expression>();
            while (true)
            {
                argList.Add(ParseExpression());
                if (_token.Id != TokenId.Comma) break;
                NextToken();
            }
            return argList.ToArray();
        }

        Expression ParseElementAccess(Expression expr)
        {
            var errorPos = _token.Pos;
            ValidateToken(TokenId.OpenBracket, Res.OpenParenExpected);
            NextToken();
            var args = ParseArguments();
            ValidateToken(TokenId.CloseBracket, Res.CloseBracketOrCommaExpected);
            NextToken();
            if (expr.Type.IsArray)
            {
                if (expr.Type.GetArrayRank() != 1 || args.Length != 1)
                    throw ParseError(errorPos, Res.CannotIndexMultiDimArray);
                var index = PromoteExpression(args[0], typeof(int), true);
                if (index == null)
                    throw ParseError(errorPos, Res.InvalidIndex);
                return Expression.ArrayIndex(expr, index);
            }
            MethodBase mb;
            switch (FindIndexer(expr.Type, args, out mb))
            {
                case 0:
                    throw ParseError(errorPos, Res.NoApplicableIndexer,
                        GetTypeName(expr.Type));
                case 1:
                    return Expression.Call(expr, (MethodInfo)mb, args);
                default:
                    throw ParseError(errorPos, Res.AmbiguousIndexerInvocation,
                        GetTypeName(expr.Type));
            }
        }

        static bool IsPredefinedType(Type type)
        {
            foreach (var t in PredefinedTypes) if (t == type) return true;
            return false;
        }

        static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        static Type GetNonNullableType(Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        static string GetTypeName(Type type)
        {
            var baseType = GetNonNullableType(type);
            var s = baseType.Name;
            if (type != baseType) s += '?';
            return s;
        }

        static bool IsNumericType(Type type)
        {
            return GetNumericTypeKind(type) != 0;
        }

        static bool IsSignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == 2;
        }

        static bool IsUnsignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == 3;
        }

        static int GetNumericTypeKind(Type type)
        {
            type = GetNonNullableType(type);
            if (type.IsEnum) return 0;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return 1;
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return 2;
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return 3;
                default:
                    return 0;
            }
        }

        static bool IsEnumType(Type type)
        {
            return GetNonNullableType(type).IsEnum;
        }

        void CheckAndPromoteOperand(Type signatures, string opName, ref Expression expr, int errorPos)
        {
            var args = new Expression[] { expr };
            MethodBase method;
            if (FindMethod(signatures, "F", false, args, out method) != 1)
                throw ParseError(errorPos, Res.IncompatibleOperand,
                    opName, GetTypeName(args[0].Type));
            expr = args[0];
        }

        void CheckAndPromoteOperands(Type signatures, string opName, ref Expression left, ref Expression right, int errorPos)
        {
            var args = new Expression[] { left, right };
            MethodBase method;
            if (FindMethod(signatures, "F", false, args, out method) != 1)
                throw IncompatibleOperandsError(opName, left, right, errorPos);
            left = args[0];
            right = args[1];
        }

        Exception IncompatibleOperandsError(string opName, Expression left, Expression right, int pos)
        {
            return ParseError(pos, Res.IncompatibleOperands,
                opName, GetTypeName(left.Type), GetTypeName(right.Type));
        }

        MemberInfo FindPropertyOrField(Type type, string memberName, bool staticAccess)
        {
            var flags = BindingFlags.Public | BindingFlags.DeclaredOnly |
                (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
            foreach (var t in SelfAndBaseTypes(type))
            {
                var members = t.FindMembers(MemberTypes.Property | MemberTypes.Field,
                    flags, Type.FilterNameIgnoreCase, memberName);
                if (members.Length != 0) return members[0];
            }
            return null;


        }

        int FindMethod(Type type, string methodName, bool staticAccess, Expression[] args, out MethodBase method)
        {
            var flags = BindingFlags.Public | BindingFlags.DeclaredOnly |
                (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
            foreach (var t in SelfAndBaseTypes(type))
            {
                var members = t.FindMembers(MemberTypes.Method,
                    flags, Type.FilterNameIgnoreCase, methodName);
                var count = FindBestMethod(members.Cast<MethodBase>(), args, out method);
                if (count != 0) return count;
            }
            method = null;
            return 0;
        }

        int FindIndexer(Type type, Expression[] args, out MethodBase method)
        {
            foreach (var t in SelfAndBaseTypes(type))
            {
                var members = t.GetDefaultMembers();
                if (members.Length != 0)
                {
                    var methods = members.
                        OfType<PropertyInfo>().
                        Select(p => (MethodBase)p.GetGetMethod()).
                        Where(m => m != null);
                    var count = FindBestMethod(methods, args, out method);
                    if (count != 0) return count;
                }
            }
            method = null;
            return 0;
        }

        static IEnumerable<Type> SelfAndBaseTypes(Type type)
        {
            if (type.IsInterface)
            {
                var types = new List<Type>();
                AddInterface(types, type);
                return types;
            }
            return SelfAndBaseClasses(type);
        }

        static IEnumerable<Type> SelfAndBaseClasses(Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }

        static void AddInterface(List<Type> types, Type type)
        {
            if (!types.Contains(type))
            {
                types.Add(type);
                foreach (var t in type.GetInterfaces()) AddInterface(types, t);
            }
        }

        class MethodData
        {
            public MethodBase MethodBase;
            public ParameterInfo[] Parameters;
            public Expression[] Args;
        }

        int FindBestMethod(IEnumerable<MethodBase> methods, Expression[] args, out MethodBase method)
        {
            var applicable = methods.
                Select(m => new MethodData { MethodBase = m, Parameters = m.GetParameters() }).
                Where(m => IsApplicable(m, args)).
                ToArray();
            if (applicable.Length > 1)
            {
                applicable = applicable.
                    Where(m => applicable.All(n => m == n || IsBetterThan(args, m, n))).
                    ToArray();
            }
            if (applicable.Length == 1)
            {
                var md = applicable[0];
                for (var i = 0; i < args.Length; i++) args[i] = md.Args[i];
                method = md.MethodBase;
            }
            else
            {
                method = null;
            }
            return applicable.Length;
        }

        bool IsApplicable(MethodData method, Expression[] args)
        {
            if (method.Parameters.Length != args.Length) return false;
            var promotedArgs = new Expression[args.Length];
            for (var i = 0; i < args.Length; i++)
            {
                var pi = method.Parameters[i];
                if (pi.IsOut) return false;
                var promoted = PromoteExpression(args[i], pi.ParameterType, false);
                if (promoted == null) return false;
                promotedArgs[i] = promoted;
            }
            method.Args = promotedArgs;
            return true;
        }

        Expression PromoteExpression(Expression expr, Type type, bool exact)
        {
            if (expr.Type == type) return expr;
            if (expr is ConstantExpression)
            {
                var ce = (ConstantExpression)expr;
                if (ce == NullLiteral)
                {
                    if (!type.IsValueType || IsNullableType(type))
                        return Expression.Constant(null, type);
                }
                else
                {
                    string text;
                    if (_literals.TryGetValue(ce, out text))
                    {
                        var target = GetNonNullableType(type);
                        Object value = null;
                        switch (Type.GetTypeCode(ce.Type))
                        {
                            case TypeCode.Int32:
                            case TypeCode.UInt32:
                            case TypeCode.Int64:
                            case TypeCode.UInt64:
                                value = ParseNumber(text, target);
                                break;
                            case TypeCode.Double:
                                if (target == typeof(decimal)) value = ParseNumber(text, target);
                                break;
                            case TypeCode.String:
                                value = ParseEnum(text, target);
                                break;
                        }
                        if (value != null)
                            return Expression.Constant(value, type);
                    }
                }
            }
            if (IsCompatibleWith(expr.Type, type))
            {
                if (type.IsValueType || exact) return Expression.Convert(expr, type);
                return expr;
            }
            return null;
        }

        public static object ParseNumber(string text, TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.SByte:
                    sbyte sb;
                    if (sbyte.TryParse(text, out sb)) return sb;
                    break;
                case TypeCode.Byte:
                    byte b;
                    if (byte.TryParse(text, out b)) return b;
                    break;
                case TypeCode.Int16:
                    short s;
                    if (short.TryParse(text, out s)) return s;
                    break;
                case TypeCode.UInt16:
                    ushort us;
                    if (ushort.TryParse(text, out us)) return us;
                    break;
                case TypeCode.Int32:
                    int i;
                    if (int.TryParse(text, out i)) return i;
                    break;
                case TypeCode.UInt32:
                    uint ui;
                    if (uint.TryParse(text, out ui)) return ui;
                    break;
                case TypeCode.Int64:
                    long l;
                    if (long.TryParse(text, out l)) return l;
                    break;
                case TypeCode.UInt64:
                    ulong ul;
                    if (ulong.TryParse(text, out ul)) return ul;
                    break;
                case TypeCode.Single:
                    float f;
                    if (float.TryParse(text, out f)) return f;
                    break;
                case TypeCode.Double:
                    double d;
                    if (double.TryParse(text, out d)) return d;
                    break;
                case TypeCode.Decimal:
                    decimal e;
                    if (decimal.TryParse(text, out e)) return e;
                    break;
            }
            return null;
        }

        public static object ParseNumber(string text, Type type)
        {
            return ParseNumber(text, Type.GetTypeCode(GetNonNullableType(type)));
        }

        static object ParseEnum(string name, Type type)
        {
            if (type.IsEnum)
            {
                var memberInfos = type.FindMembers(MemberTypes.Field,
                    BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static,
                    Type.FilterNameIgnoreCase, name);
                if (memberInfos.Length != 0) return ((FieldInfo)memberInfos[0]).GetValue(null);
            }
            return null;
        }

        static bool IsCompatibleWith(Type source, Type target)
        {
            if (source == target) return true;
            if (!target.IsValueType) return target.IsAssignableFrom(source);
            var st = GetNonNullableType(source);
            var tt = GetNonNullableType(target);
            if (st != source && tt == target) return false;
            var sc = st.IsEnum ? TypeCode.Object : Type.GetTypeCode(st);
            var tc = tt.IsEnum ? TypeCode.Object : Type.GetTypeCode(tt);
            switch (sc)
            {
                case TypeCode.SByte:
                    switch (tc)
                    {
                        case TypeCode.SByte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Byte:
                    switch (tc)
                    {
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int16:
                    switch (tc)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt16:
                    switch (tc)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int32:
                    switch (tc)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt32:
                    switch (tc)
                    {
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int64:
                    switch (tc)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt64:
                    switch (tc)
                    {
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Single:
                    switch (tc)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                            return true;
                    }
                    break;
                default:
                    if (st == tt) return true;
                    break;
            }
            return false;
        }

        static bool IsBetterThan(Expression[] args, MethodData m1, MethodData m2)
        {
            var better = false;
            for (var i = 0; i < args.Length; i++)
            {
                var c = CompareConversions(args[i].Type,
                    m1.Parameters[i].ParameterType,
                    m2.Parameters[i].ParameterType);
                if (c < 0) return false;
                if (c > 0) better = true;
            }
            return better;
        }

        // Return 1 if s -> t1 is a better conversion than s -> t2
        // Return -1 if s -> t2 is a better conversion than s -> t1
        // Return 0 if neither conversion is better
        static int CompareConversions(Type s, Type t1, Type t2)
        {
            if (t1 == t2) return 0;
            if (s == t1) return 1;
            if (s == t2) return -1;
            var t1T2 = IsCompatibleWith(t1, t2);
            var t2T1 = IsCompatibleWith(t2, t1);
            if (t1T2 && !t2T1) return 1;
            if (t2T1 && !t1T2) return -1;
            if (IsSignedIntegralType(t1) && IsUnsignedIntegralType(t2)) return 1;
            if (IsSignedIntegralType(t2) && IsUnsignedIntegralType(t1)) return -1;
            return 0;
        }

        Expression GenerateEqual(Expression left, Expression right)
        {
            return Expression.Equal(left, right);
        }

        Expression GenerateNotEqual(Expression left, Expression right)
        {
            return Expression.NotEqual(left, right);
        }

        Expression GenerateGreaterThan(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.GreaterThan(
                    GenerateStaticMethodCall("Compare", left, right),
                    Expression.Constant(0)
                );
            }
            return Expression.GreaterThan(left, right);
        }

        Expression GenerateGreaterThanEqual(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.GreaterThanOrEqual(
                    GenerateStaticMethodCall("Compare", left, right),
                    Expression.Constant(0)
                );
            }
            return Expression.GreaterThanOrEqual(left, right);
        }

        Expression GenerateLessThan(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.LessThan(
                    GenerateStaticMethodCall("Compare", left, right),
                    Expression.Constant(0)
                );
            }
            return Expression.LessThan(left, right);
        }

        Expression GenerateLessThanEqual(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.LessThanOrEqual(
                    GenerateStaticMethodCall("Compare", left, right),
                    Expression.Constant(0)
                );
            }
            return Expression.LessThanOrEqual(left, right);
        }

        Expression GenerateAdd(Expression left, Expression right)
        {
            if (left.Type == typeof(string) && right.Type == typeof(string))
            {
                return GenerateStaticMethodCall("Concat", left, right);
            }
            return Expression.Add(left, right);
        }

        Expression GenerateSubtract(Expression left, Expression right)
        {
            return Expression.Subtract(left, right);
        }

        Expression GenerateStringConcat(Expression left, Expression right)
        {
            return Expression.Call(
                null,
                typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) }),
                new[] { left, right });
        }

        MethodInfo GetStaticMethod(string methodName, Expression left, Expression right)
        {
            return left.Type.GetMethod(methodName, new[] { left.Type, right.Type });
        }

        Expression GenerateStaticMethodCall(string methodName, Expression left, Expression right)
        {
            return Expression.Call(null, GetStaticMethod(methodName, left, right), new[] { left, right });
        }

        void SetTextPos(int pos)
        {
            _textPos = pos;
            _ch = _textPos < _textLen ? _text[_textPos] : '\0';
        }

        void NextChar()
        {
            if (_textPos < _textLen) _textPos++;
            _ch = _textPos < _textLen ? _text[_textPos] : '\0';
        }

        void NextToken()
        {
            while (Char.IsWhiteSpace(_ch)) NextChar();
            TokenId t;
            var tokenPos = _textPos;
            switch (_ch)
            {
                case '!':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        t = TokenId.ExclamationEqual;
                    }
                    else
                    {
                        t = TokenId.Exclamation;
                    }
                    break;
                case '%':
                    NextChar();
                    t = TokenId.Percent;
                    break;
                case '&':
                    NextChar();
                    if (_ch == '&')
                    {
                        NextChar();
                        t = TokenId.DoubleAmphersand;
                    }
                    else
                    {
                        t = TokenId.Amphersand;
                    }
                    break;
                case '(':
                    NextChar();
                    t = TokenId.OpenParen;
                    break;
                case ')':
                    NextChar();
                    t = TokenId.CloseParen;
                    break;
                case '*':
                    NextChar();
                    t = TokenId.Asterisk;
                    break;
                case '+':
                    NextChar();
                    t = TokenId.Plus;
                    break;
                case ',':
                    NextChar();
                    t = TokenId.Comma;
                    break;
                case '-':
                    NextChar();
                    t = TokenId.Minus;
                    break;
                case '.':
                    NextChar();
                    t = TokenId.Dot;
                    break;
                case '/':
                    NextChar();
                    t = TokenId.Slash;
                    break;
                case ':':
                    NextChar();
                    t = TokenId.Colon;
                    break;
                case '<':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        t = TokenId.LessThanEqual;
                    }
                    else if (_ch == '>')
                    {
                        NextChar();
                        t = TokenId.LessGreater;
                    }
                    else
                    {
                        t = TokenId.LessThan;
                    }
                    break;
                case '=':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        t = TokenId.DoubleEqual;
                    }
                    else
                    {
                        t = TokenId.Equal;
                    }
                    break;
                case '>':
                    NextChar();
                    if (_ch == '=')
                    {
                        NextChar();
                        t = TokenId.GreaterThanEqual;
                    }
                    else
                    {
                        t = TokenId.GreaterThan;
                    }
                    break;
                case '?':
                    NextChar();
                    t = TokenId.Question;
                    break;
                case '[':
                    NextChar();
                    t = TokenId.OpenBracket;
                    break;
                case ']':
                    NextChar();
                    t = TokenId.CloseBracket;
                    break;
                case '|':
                    NextChar();
                    if (_ch == '|')
                    {
                        NextChar();
                        t = TokenId.DoubleBar;
                    }
                    else
                    {
                        t = TokenId.Bar;
                    }
                    break;
                case '"':
                case '\'':
                    var quote = _ch;
                    do
                    {
                        NextChar();
                        while (_textPos < _textLen && _ch != quote) NextChar();
                        if (_textPos == _textLen)
                            throw ParseError(_textPos, Res.UnterminatedStringLiteral);
                        NextChar();
                    } while (_ch == quote);
                    t = TokenId.StringLiteral;
                    break;
                default:
                    if (Char.IsLetter(_ch) || _ch == '@' || _ch == '_')
                    {
                        do
                        {
                            NextChar();
                        } while (Char.IsLetterOrDigit(_ch) || _ch == '_');
                        t = TokenId.Identifier;
                        break;
                    }
                    if (Char.IsDigit(_ch))
                    {
                        t = TokenId.IntegerLiteral;
                        do
                        {
                            NextChar();
                        } while (Char.IsDigit(_ch));
                        if (_ch == '.')
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            ValidateDigit();
                            do
                            {
                                NextChar();
                            } while (Char.IsDigit(_ch));
                        }
                        if (_ch == 'E' || _ch == 'e')
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            if (_ch == '+' || _ch == '-') NextChar();
                            ValidateDigit();
                            do
                            {
                                NextChar();
                            } while (Char.IsDigit(_ch));
                        }
                        if (_ch == 'F' || _ch == 'f') NextChar();
                        break;
                    }
                    if (_textPos == _textLen)
                    {
                        t = TokenId.End;
                        break;
                    }
                    throw ParseError(_textPos, Res.InvalidCharacter, _ch);
            }
            _token.Id = t;
            _token.Text = _text.Substring(tokenPos, _textPos - tokenPos);
            _token.Pos = tokenPos;
        }

        bool TokenIdentifierIs(string id)
        {
            return _token.Id == TokenId.Identifier && String.Equals(id, _token.Text, StringComparison.OrdinalIgnoreCase);
        }

        string GetIdentifier()
        {
            ValidateToken(TokenId.Identifier, Res.IdentifierExpected);
            var id = _token.Text;
            if (id.Length > 1 && id[0] == '@') id = id.Substring(1);
            return id;
        }

        void ValidateDigit()
        {
            if (!Char.IsDigit(_ch)) throw ParseError(_textPos, Res.DigitExpected);
        }

        void ValidateToken(TokenId t, string errorMessage)
        {
            if (_token.Id != t) throw ParseError(errorMessage);
        }

        void ValidateToken(TokenId t)
        {
            if (_token.Id != t) throw ParseError(Res.SyntaxError);
        }

        Exception ParseError(string format, params object[] args)
        {
            return ParseError(_token.Pos, format, args);
        }

        Exception ParseError(int pos, string format, params object[] args)
        {
            return new ParseException(string.Format(System.Globalization.CultureInfo.CurrentCulture, format, args), pos);
        }

        static Dictionary<string, object> CreateKeywords()
        {
            var d = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            d.Add("true", TrueLiteral);
            d.Add("false", FalseLiteral);
            d.Add("null", NullLiteral);
            d.Add(KeywordIt, KeywordIt);
            d.Add(KeywordIif, KeywordIif);
            d.Add(KeywordNew, KeywordNew);
            foreach (var type in PredefinedTypes) d.Add(type.Name, type);
            return d;
        }
    }

    static class Res
    {
        public const string DuplicateIdentifier = "The identifier '{0}' was defined more than once";
        public const string ExpressionTypeMismatch = "Expression of type '{0}' expected";
        public const string ExpressionExpected = "Expression expected";
        public const string InvalidCharacterLiteral = "Character literal must contain exactly one character";
        public const string InvalidIntegerLiteral = "Invalid integer literal '{0}'";
        public const string InvalidRealLiteral = "Invalid real literal '{0}'";
        public const string UnknownIdentifier = "Unknown identifier '{0}'";
        public const string NoItInScope = "No 'it' is in scope";
        public const string IifRequiresThreeArgs = "The 'iif' function requires three arguments";
        public const string FirstExprMustBeBool = "The first expression must be of type 'Boolean'";
        public const string BothTypesConvertToOther = "Both of the types '{0}' and '{1}' convert to the other";
        public const string NeitherTypeConvertsToOther = "Neither of the types '{0}' and '{1}' converts to the other";
        public const string MissingAsClause = "Expression is missing an 'as' clause";
        public const string ArgsIncompatibleWithLambda = "Argument list incompatible with lambda expression";
        public const string TypeHasNoNullableForm = "Type '{0}' has no nullable form";
        public const string NoMatchingConstructor = "No matching constructor in type '{0}'";
        public const string AmbiguousConstructorInvocation = "Ambiguous invocation of '{0}' constructor";
        public const string CannotConvertValue = "A value of type '{0}' cannot be converted to type '{1}'";
        public const string NoApplicableMethod = "No applicable method '{0}' exists in type '{1}'";
        public const string MethodsAreInaccessible = "Methods on type '{0}' are not accessible";
        public const string MethodIsVoid = "Method '{0}' in type '{1}' does not return a value";
        public const string AmbiguousMethodInvocation = "Ambiguous invocation of method '{0}' in type '{1}'";
        public const string UnknownPropertyOrField = "No property or field '{0}' exists in type '{1}'";
        public const string NoApplicableAggregate = "No applicable aggregate method '{0}' exists";
        public const string CannotIndexMultiDimArray = "Indexing of multi-dimensional arrays is not supported";
        public const string InvalidIndex = "Array index must be an integer expression";
        public const string NoApplicableIndexer = "No applicable indexer exists in type '{0}'";
        public const string AmbiguousIndexerInvocation = "Ambiguous invocation of indexer in type '{0}'";
        public const string IncompatibleOperand = "Operator '{0}' incompatible with operand type '{1}'";
        public const string IncompatibleOperands = "Operator '{0}' incompatible with operand types '{1}' and '{2}'";
        public const string UnterminatedStringLiteral = "Unterminated string literal";
        public const string InvalidCharacter = "Syntax error '{0}'";
        public const string DigitExpected = "Digit expected";
        public const string SyntaxError = "Syntax error";
        public const string TokenExpected = "{0} expected";
        public const string ParseExceptionFormat = "{0} (at index {1})";
        public const string ColonExpected = "':' expected";
        public const string OpenParenExpected = "'(' expected";
        public const string CloseParenOrOperatorExpected = "')' or operator expected";
        public const string CloseParenOrCommaExpected = "')' or ',' expected";
        public const string DotOrOpenParenExpected = "'.' or '(' expected";
        public const string OpenBracketExpected = "'[' expected";
        public const string CloseBracketOrCommaExpected = "']' or ',' expected";
        public const string IdentifierExpected = "Identifier expected";
    }

}