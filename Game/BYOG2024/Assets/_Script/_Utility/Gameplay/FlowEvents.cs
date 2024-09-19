namespace AstekUtility.Gameplay
{
	#region Application Related Events

	/// <summary>
	///     When application exits
	/// </summary>
	public class OnApplicationExit { }

	/// <summary>
	///     When application is paused
	///     Primarily used in mobile devices
	/// </summary>
	public class OnApplicationPause { }

	/// <summary>
	///     When application is in background process
	///     Primarily used in mobile devices
	/// </summary>
	public class OnApplicationFocus { }

	/// <summary>
	///     When you reach a certain checkpoint in application
	/// </summary>
	public class OnAppCheckpointReached { }

	/// <summary>
	///     When you start from a certain checkpoint in application
	/// </summary>
	public class FromAppCheckPointStart { }

	#endregion

	#region Gameplay Related Events

	/// <summary>
	///     When you reach a certain checkpoint in gameplay
	/// </summary>
	public class OnCheckpointReach { }

	/// <summary>
	///     When you start from a certain checkpoint in gameplay
	/// </summary>
	public class FromCheckpointStart { }

	#endregion
}