using Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AstekUtility.Gameplay
{
	public class CrossfadeAnimState : ScriptableObject
	{
		[SerializeField] private string crossFadeAnimStateName;
		[SerializeField] private int layerIndex;
		[SerializeField,Range(0f,1f)] private float layerWeight;
		[SerializeField] private bool useNormalizedTime = false;

		[Title("Normalized Transition settings")]
		[SerializeField, Range(0f, 1f), ShowIf("@useNormalizedTime==true")] private float normalizedTimeDuration;
		[SerializeField, Range(0f, 1f), ShowIf("@useNormalizedTime==true")] private float normalizedTimeOffset;

		[Title("Fixed Time Transition settings")]
		[SerializeField, ShowIf("@useNormalizedTime==false")] private float fixedTimeDuration;
		[SerializeField, ShowIf("@useNormalizedTime==false")] private float fixedTimeOffset;

		private int stateID => Animator.StringToHash(crossFadeAnimStateName);

		public void Play(EntityAnimationController controller)
		{
			if (!controller)
				return;
			
			controller.SetWeight(layerIndex,layerWeight);
			if (useNormalizedTime)
				controller.CrossFade(stateID, layerIndex, normalizedTimeDuration:normalizedTimeDuration, normalizedTimeOffset:normalizedTimeOffset);
			else
				controller.CrossFade(stateID, layerIndex, fixedTimeDuration:fixedTimeDuration, fixedTimeOffset:fixedTimeOffset);
		}
	}
}