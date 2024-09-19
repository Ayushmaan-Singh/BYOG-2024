namespace AstekUtility.StateMachine
{
	public interface ITransition
	{
		IState To { get; }
		IPredicate Condition { get; }
	}

    /// <summary>
    ///     Defines which state we are moving to depending on condition defined through predicate
    /// </summary>
    public class Transition : ITransition
	{

		public Transition(IState to, IPredicate condition)
		{
			To = to;
			Condition = condition;
		}
		public IState To { get; }
		public IPredicate Condition { get; }
	}
}