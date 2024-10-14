namespace AstekUtility.DesignPattern.StateMachine
{
	public interface IState
	{
		void OnStateEnter();
		void FrameUpdate();
		void PhysicsUpdate();
		void OnStateExit();
	}

	public abstract class BaseState : IState
	{
		public abstract void OnStateEnter();

		public abstract void FrameUpdate();

		public abstract void PhysicsUpdate();

		public abstract void OnStateExit();
	}
}