using System;
using System.Threading;
using UnityEngine;
namespace AstekUtility.DesignPattern.GOAP
{
	public interface ISensor
	{
		public event Action OnTargetChanged;
		public Vector3 TargetPosition { get; }
		public bool IsTargetInRange { get; }
	}
}