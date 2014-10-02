using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CaseClass
{
    public class Value<T>
    {
        internal T mValue { get; set; }
        internal Value()
        {

        }
        public Value(T value)
        {
            mValue = value;
        }

        public static implicit operator T(Value<T> value)
        {
            return value.mValue;
        }
    }

    public interface IValidator<T>
    {
        void Validate();
    }

    public class Typed<TName, TValue> : Value<TValue>, IValidator<TValue> where TName : Value<TValue>, IValidator<TValue>, new()
    {
        private List<Action<TValue>> mGuards = new List<Action<TValue>>();

        public Typed(){      
        }
        public Typed(TValue value) : base(value){
        }

        protected internal Typed<TName, TValue> Guard(Predicate<TValue> guard)
        {
            return Guard((x)=> {
                if(!guard(x)) throw new InvalidOperationException(string.Format("Invalid data for type {0}:{1}", typeof(TName), mValue));
            });
            
        }

        protected Typed<TName, TValue> Guard(Action<TValue> @try)
        {
            mGuards.Add(@try);
            return this;
        }

        public static implicit operator TName(Typed<TName, TValue> value)
        {
            return Of(value.mValue);
        }

        public static TName operator |(Typed<TName, TValue> value, Func<TValue,TValue> transform)
        {
            return Of(transform(value.mValue));
        }

        public static implicit operator Typed<TName, TValue>(TValue value)
        {
            return new Typed<TName, TValue>(value);
        }
        public static TName Of(TValue value)
        {
            var u =  new TName() { mValue = value };
            u.Validate();
            return u;
        }
        void IValidator<TValue>.Validate()
        {
            foreach (var guard in mGuards) guard(mValue);
        }
    }

    public static class TypedExtensions
    {
        public static Typed<U, string> Match<U>(this Typed<U,string> typed, string pattern) where U: Typed<U,string>, new()
        {
            return typed.Guard(x => Regex.IsMatch(x,pattern));
        }

    }

}
