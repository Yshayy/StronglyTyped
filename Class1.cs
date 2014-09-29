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

    public class Typed<U, T> : Value<T>, IValidator<T> where U : Value<T>, IValidator<T>, new()
    {
        private List<Action<T>> mGuards = new List<Action<T>>();

        public Typed(){      
        }
        public Typed(T value) : base(value){
        }

        protected internal Typed<U, T> Guard(Predicate<T> guard)
        {
            return Guard((x)=> {
                if(!guard(x)) throw new InvalidOperationException(string.Format("Invalid data for type {0}:{1}", typeof(U), mValue));
            });
            
        }

        protected Typed<U, T> Guard(Action<T> @try)
        {
            mGuards.Add(@try);
            return this;
        }

        public static implicit operator U(Typed<U, T> value)
        {
            return Of(value.mValue);
        }

        public static U operator |(Typed<U, T> value, Func<T,T> transform)
        {
            return Of(transform(value.mValue));
        }

        public static implicit operator Typed<U, T>(T value)
        {
            return new Typed<U, T>(value);
        }
        public static U Of(T value)
        {
            var u =  new U() { mValue = value };
            u.Validate();
            return u;
        }
        void IValidator<T>.Validate()
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
