using System;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Combat;
using UnityEngine;
namespace _Script
{
	public class EnemyController : MonoBehaviour
	{
		[field:SerializeField] public EnemyTypes Type { get; private set; }
		[SerializeField] private Rigidbody rb;

		private void Awake()
		{
			ServiceLocator.For(this).Register(this);
		}

		private void OnDestroy()
		{
			ServiceLocator.For(this)?.Deregister(this);
		}
	}
}