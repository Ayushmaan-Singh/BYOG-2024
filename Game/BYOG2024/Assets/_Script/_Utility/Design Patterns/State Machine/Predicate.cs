using System;
namespace AstekUtility.DesignPattern.StateMachine
{
	public interface IPredicate
	{
		bool Evaluate();
	}

    /// <summary>
    ///     Evaluates given condition to identify if we can change state
    /// </summary>
    public class Predicate : IPredicate
	{
		private readonly Func<bool> Func;

		public Predicate(Func<bool> func)
		{
			Func = func;
		}

		public bool Evaluate()
		{
			return Func.Invoke();
		}
	}

    /// <summary>
    /// Evaluates given condition to identify if we can change state
    /// Can also take an input of generic type.
    /// </summary>
    /// <typeparam name="T">input type for a Func</typeparam>
    public class Predicate<T> : IPredicate
	{
		private readonly Func<T, bool> Func;
		private T _data;

		public Predicate(Func<T, bool> func)
		{
			Func = func;
		}

		public bool Evaluate()
		{
			return Func.Invoke(_data);
		}

		public bool Evaluate(T input)
		{
			_data = input;
			return Evaluate();
		}
	}
}