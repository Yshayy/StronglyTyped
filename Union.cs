using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseClass
{
    public class None {
        public static None Value;
    }

    public interface IUntypedCasedUnion<T1, T2>
    {
        ICasedUnion<T2, TReturnType> Case<TReturnType>(Func<T1, TReturnType> func);
        ICasedUnion<T1, TReturnType> Case<TReturnType>(Func<T2, TReturnType> func);
        
    }

    public interface ICasedUnion<T>
    {
        void Case(Action<T> action);
    }

    public interface ICasedUnion<T1, T2,TReturnType>
    {
        ICasedUnion<T2, TReturnType> Case(Func<T1, TReturnType> func);
        ICasedUnion<T1, TReturnType> Case(Func<T2, TReturnType> func);
    }

    public interface ICasedUnion<T, TReturnType>
    {
        TReturnType Case(Func<T, TReturnType> func);
    }

    public struct EndCaseUnion<T, TResult> : ICasedUnion<T, TResult>
    {
        private T _value;
        private TResult _result;
        private bool _resolved;
        public EndCaseUnion(T value)
        {
            _result = default(TResult);
            _value = value;
            _resolved = false;
        }

        public EndCaseUnion(TResult result)
        {
            _value = default(T);
            _result = result;
            _resolved = true;
        }

        public TResult Case(Func<T, TResult> func)
        {
            return (!_resolved) ? func(_value) : _result;
        }
    }

    public class Optional<T> : Union<T, None> {
        public Optional(): base(None.Value){}
        public Optional(T Value) : base(Value) { }

        public T Value {get {return _T1;}}
        public bool HasValue {get {return ResolvedType != typeof(None); }}
    }


    public class Union<T1,T2> : IUntypedCasedUnion<T1, T2>
    {
        internal T1 _T1 {private set; get;}
        internal T2 _T2 {private set; get;}
        internal Type ResolvedType {private set; get;}

        internal void initalize(T1 value)
        {
            _T1 = value;
            ResolvedType = typeof(T1);
        }

        internal void initalize(T2 value)
        {
            _T2 = value;
            ResolvedType = typeof(T2);
        }
        internal Union() { }

        public Union(T1 value)
        {
            initalize(value);
        }

        public Union(T2 value)
        {
            initalize(value);
        }

        public ICasedUnion<T2,TReturnType> Case<TReturnType>(Func<T1,TReturnType> func)
        {
            return new CaseUnion<T1, T2, TReturnType>(this).Case(func);
        }

        public ICasedUnion<T1,TReturnType> Case<TReturnType>(Func<T2,TReturnType> func)
        {
            return new CaseUnion<T1, T2, TReturnType>(this).Case(func);
        }
    }

    public class TypedUnion<TName, T1, T2> : Union<T1, T2> where TName : Union<T1, T2>, new()
    {
        public TypedUnion() : base() { }

        public static TName Of(T1 value) 
        {
            var union = new TName();
            union.initalize(value);
            return union;
        }

        public static TName Of(T2 value)
        {
            var union = new TName();
            union.initalize(value);
            return union;
        }
    }

    public class CaseUnion<T1,T2,TReturn> :  ICasedUnion<T1,T2,TReturn>
    {
        private Union<T1, T2> _union; 
        public CaseUnion(Union<T1, T2> union)
        {
            _union = union;
        }


        public ICasedUnion<T2, TReturn> Case(Func<T1, TReturn> func)
        {
            return (typeof(T1) == _union.ResolvedType) ? new EndCaseUnion<T2, TReturn>(func(_union._T1))
                                                 : new EndCaseUnion<T2, TReturn>(_union._T2);
        }

        public ICasedUnion<T1, TReturn> Case(Func<T2, TReturn> func)
        {
            return (typeof(T2) == _union.ResolvedType) ? new EndCaseUnion<T1, TReturn>(func(_union._T2))
                                                 : new EndCaseUnion<T1, TReturn>(_union._T1);
        }

    }

    public static class CaseUnionEx
    {
        public static ICasedUnion<T2, None> Case<T1,T2>(this IUntypedCasedUnion<T1,T2> union, Action<T1> func)
        {
            return union.Case<None>(x =>
                {
                    func(x);
                    return None.Value;
                });
        }
        public static ICasedUnion<T1, None> Case<T1, T2>(this IUntypedCasedUnion<T1, T2> union, Action<T2> func)
        {
            return union.Case<None>(x =>
            {
                func(x);
                return None.Value;
            });
        }
        public static TResult Case<T1,T2,TResult>(this IUntypedCasedUnion<T1, T2> union, 
                                                                       Func<T1,TResult> func1,
                                                                       Func<T2, TResult> func2)
        {
            return union.Case(func1).Case(func2);
        }
    }
}
