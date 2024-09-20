using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Abilities
{
	public class Gluttony : AbilityBase
	{
		[SerializeField] private GameObject gluttonyGO;
		[SerializeField] private ParticleSystem[] gluttonyVfx;
		
		public override void Execute()
		{
			gluttonyGO.SetActive(true);
			foreach (ParticleSystem particle in gluttonyVfx)
			{
				particle.Play();
			}
		}
		public override void CancelExecution()
		{			
			foreach (ParticleSystem particle in gluttonyVfx)
			{
				particle.Stop();
			}
		}
	}
}
