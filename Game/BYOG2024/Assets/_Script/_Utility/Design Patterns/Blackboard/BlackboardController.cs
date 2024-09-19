using System;
using ServiceLocator = AstekUtility.DesignPattern.ServiceLocatorTool.ServiceLocator;
using Sirenix.OdinInspector;
using UnityEngine;
namespace AstekUtility.DesignPattern.Blackboard
{
	public class BlackboardController : MonoBehaviour
	{
		[InlineEditor, SerializeField] private BlackBoardData _blackboardData;
		private readonly Blackboard _blackboard = new Blackboard();
		private readonly Arbiter _arbiter = new Arbiter();

		private void Awake()
		{
			ServiceLocator.Global.Register(this);
			_blackboardData.SetValueOnBlackboard(_blackboard);

			#if UNITY_EDITOR
			_blackboard.Debug();
			#endif
		}

		public Blackboard GetBlackboard() => _blackboard;
		public void RegisterExpert(IExpert expert) => _arbiter.RegisterExpert(expert);

		private void Update()
		{
			//Execute all agreed actions from the current iteration
			foreach (Action action in _arbiter.BlackboardInteraction(_blackboard))
			{
				action();
			}
		}
	}
}