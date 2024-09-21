using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Abilities
{
	public class OrbitalBombardment : AbilityBase
	{
		[Header("Ability Data")]
		[SerializeField] private float damagePerHit;

		[Header("Visuals")]
		[SerializeField] private ParticleSystem[] particleSystems;
		[SerializeField] private ParticleSystem[] indicators;

		private bool _indicatorOn=false;
		
		private void Update()
		{
			switch (_currentState)
			{

				case State.Usable:

					if (!_indicatorOn)
					{
						_indicatorOn = true;
						foreach (ParticleSystem indicator in indicators)
						{
							indicator.Play();
						}
					}

					break;

				case State.Unusable:
					break;
				case State.InProgress:
					
					if (_indicatorOn)
					{
						_indicatorOn = false;
						foreach (ParticleSystem indicator in indicators)
						{
							indicator.Stop();
						}
					}
					
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public override void Execute()
		{
			throw new System.NotImplementedException();
		}
		public override void CancelExecution()
		{
			throw new System.NotImplementedException();
		}
	}
}