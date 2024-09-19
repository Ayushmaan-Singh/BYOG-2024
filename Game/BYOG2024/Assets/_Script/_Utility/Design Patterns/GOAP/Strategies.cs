namespace AstekUtility.DesignPattern.GOAP
{
	public interface Strategies
	{
		bool CanPerform { get; }
		bool Complete { get; }

		void Start()
		{
			//noap
		}

		void Update(float deltaTime)
		{
			//noap
		}

		void Stop()
		{
			//noap
		}
	}
}