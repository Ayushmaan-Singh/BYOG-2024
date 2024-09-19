namespace AstekUtility.StateMachine
{
	public interface IState
	{
		void OnEnter();
		void Update();
		void FixedUpdate();
		void OnExit();
	}

	public abstract class BaseState : IState
	{
		public abstract void OnEnter();

		public abstract void Update();

		public abstract void FixedUpdate();

		public abstract void OnExit();
	}
}