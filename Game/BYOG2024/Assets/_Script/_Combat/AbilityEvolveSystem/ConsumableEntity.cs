using System;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Entity.Abilities;
using UnityEngine;
using UnityEngine.Events;
namespace Combat
{
	/// <summary>
	/// GameObject with this component signify that it can be consumed and the type of resource given on consumption
	/// </summary>
	public class ConsumableEntity : MonoBehaviour
	{
		[SerializeField] private ConsumableEntityType[] entityTypes;

		[Header("Events"),Space]
		[SerializeField] private UnityEvent<ConsumableEntity> onConsume;
		[SerializeField] private UnityEvent<ConsumableEntity> onConsumeGluttony;
		[SerializeField] private UnityEvent<ConsumableEntity> onConsumeBeelzebub;
		
		public ConsumableEntityType[] EntityTypes => entityTypes;

		public void Consume(object sender = null)
		{
			onConsume?.Invoke(this);

			switch (sender)
			{
				case Gluttony gluttony:
					onConsumeGluttony?.Invoke(this);
					break;
				
				case Beelzebub beelzebub:
					onConsumeBeelzebub?.Invoke(this);
					break;
			}
		}
	}
}