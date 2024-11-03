using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AstekUtility.CardinalDirection;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
namespace AstekUtility.Gameplay
{
	public class ContextSteering : MonoBehaviour
	{
		[Header("Context Steering Data")]
		[SerializeField] protected ContextSolver contextSolver;
		[SerializeField] protected Detector[] detectors;
		[SerializeField] protected SteeringBehaviour[] steeringBehaviours;

		[Space]
		[SerializeField, MaxValue(360)] protected int dirMapResolution = 16;

		[Header("Main Model Data")]
		[SerializeField] protected Transform mainModel;
		[SerializeField] protected Collider[] colliders;

		protected AIData _aiData;
		protected Vector3[] directionXZ;
		public Vector3 Direction { get; private set; } = Vector3.zero;

		private void Awake()
		{
			Init();
		}

		protected virtual void Init()
		{
			_aiData = new AIData();
			InitDirections();
			contextSolver = new ContextSolver(directionXZ, mainModel);

			foreach (SteeringBehaviour behavior in steeringBehaviours)
			{
				new SteeringBehaviour.Builder()
					.InitAIData(_aiData)
					.InitMainModel(mainModel)
					.InitDirectionXZ(directionXZ)
					.Build(behavior);
			}
			foreach (Detector detector in this.detectors)
			{
				new Detector.Builder()
					.InitAIData(_aiData)
					.InitMainModel(mainModel)
					.Build(detector);
			}
		}

		protected void InitDirections()
		{
			directionXZ = new Vector3[dirMapResolution];
			float directionInterval = Mathf.PI * 2 / dirMapResolution;
			for (int i = 0; i < dirMapResolution; i++)
			{
				float currentAngle = i * directionInterval;
				directionXZ[i] = new Vector3(Mathf.Cos(currentAngle), 0, Mathf.Sin(currentAngle));
			}
		}

		private void FixedUpdate()
		{
			Direction = UpdateDirection();
		}

		private Vector3 UpdateDirection()
		{
			foreach (Detector detector in detectors)
			{
				detector.Detect();
			}

			if (_aiData.CurrentTarget)
			{
				if (!_aiData.CurrentTarget)
				{
					//Stopping Logic
					return Vector3.zero;
				}
				return contextSolver.GetDirectionToMove(steeringBehaviours);
			}
			if (_aiData.GetTargetsCount() > 0)
			{
				//Target acquisition logic
				_aiData.CurrentTarget = _aiData.Targets[0];
			}

			return Vector3.zero;
		}

		private void OnDrawGizmos()
		{
			contextSolver.OnDrawGizmos();
		}
	}
}