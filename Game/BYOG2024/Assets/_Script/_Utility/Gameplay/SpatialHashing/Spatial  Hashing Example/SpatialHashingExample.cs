# if UNITY_EDITOR

using System;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace AstekUtility.Gameplay.SpatialHashing
{
	public class SpatialHashingExample : MonoBehaviour
	{
		#region Setting And References

		[SerializeField] private SpatialHashingManager manager;
		[Header("Particles")]
		[SerializeField] private GameObject particlePrefabs;
		[SerializeField] private int particleCount = 500;
		[SerializeField] private float maxRadius = 1.5f;
		[SerializeField] private float cellSize = 2f;
		private Bounds particleBounds = new Bounds(Vector3.zero, new Vector3(20, 20, 20));

		[Header("Query Setting")]
		[SerializeField] private Transform querySphere;
		[SerializeField] private float queryRadius = 5f;

		[Header("UI controls")]
		[SerializeField] private Slider particleSlider;
		[SerializeField] private Slider cellSizeSlider;
		[SerializeField] private Slider queryRadiusSlider;
		[SerializeField] private TextMeshProUGUI resultsText;
		[SerializeField] private Toggle showGridToggle;

		private bool _showGrid;

		private GameObject queryShareVisual;
		[SerializeField] private Material querySphereMaterial;

		private GameObject[] _particleInstances;
		private Renderer[] _particleRenderers;
		private NativeArray<Particle> _particlesNative;

		#endregion

		private struct Particle
		{
			public float3 Position;
			public float3 Velocity;
			public float Radius;
		}

		private void Start()
		{
			particleSlider.value = particleCount;
			particleSlider.onValueChanged.AddListener(_ => InitializeParticles());
			showGridToggle.onValueChanged.AddListener(value => _showGrid = value);

			cellSizeSlider.value = cellSize;
			cellSizeSlider.onValueChanged.AddListener(value => cellSize = value);

			queryRadiusSlider.value = queryRadius;
			queryRadiusSlider.onValueChanged.AddListener(value => queryRadius = value);

			InitializeParticles();
		}

		private void InitializeParticles()
		{
			if (_particlesNative.IsCreated)
				_particlesNative.Dispose();

			particleCount = Mathf.RoundToInt(particleSlider.value);
			_particleRenderers = new Renderer[particleCount];

			if (_particleInstances != null)
			{
				int count = _particleInstances.Length;
				for (int i = 0; i < count; i++)
				{
					Destroy(_particleInstances[i]);
				}
			}

			_particleInstances = new GameObject[particleCount];

			_particlesNative = new NativeArray<Particle>(particleCount, Allocator.Persistent);


			for (int i = 0; i < particleCount; i++)
			{
				Vector3 pos = new Vector3
				(
					UnityEngine.Random.Range(particleBounds.min.x, particleBounds.max.x),
					UnityEngine.Random.Range(particleBounds.min.y, particleBounds.max.y),
					UnityEngine.Random.Range(particleBounds.min.z, particleBounds.max.z)
				);
				float radius = UnityEngine.Random.Range(0.5f, maxRadius);
				Vector3 velocity = new Vector3
				(
					UnityEngine.Random.Range(-1, 1),
					UnityEngine.Random.Range(-1, 1),
					UnityEngine.Random.Range(-1, 1)
				);

				_particlesNative[i] = new Particle()
				{
					Position = pos,
					Radius = radius,
					Velocity = velocity
				};

				GameObject instance = Instantiate(particlePrefabs, pos, Quaternion.identity);
				instance.transform.localPosition = Vector3.one * radius * 2f;
				instance.transform.SetParent(transform);
				_particleInstances[i] = instance;
				_particleRenderers[i] = instance.GetComponent<Renderer>();
				manager.AddObjects(instance);
			}
		}

		private void Update()
		{
			if (!_particlesNative.IsCreated)
				return;

			//Particle Movement Job
			UpdateParticleJob updateJob = new UpdateParticleJob()
			{
				Particles = _particlesNative,
				BoundsMin = particleBounds.min,
				BoundsMax = particleBounds.max,
				DeltaTime = Time.deltaTime
			};

			JobHandle updateJobHandle = updateJob.Schedule(_particlesNative.Length, 64);

			updateJobHandle.Complete();
			GameObject[] objects = manager.QueryObjects(querySphere.position, cellSize, queryRadius);

			foreach (Renderer pr in _particleRenderers)
			{
				pr.material.color = Color.white;
			}

			foreach (GameObject index in objects)
			{
				index.GetComponent<Renderer>().material.color = Color.red;
			}

			for (int i = 0; i < _particlesNative.Length; i++)
			{
				_particleInstances[i].transform.position = _particlesNative[i].Position;
			}
		}

		private void OnDestroy()
		{
			if (_particlesNative.IsCreated)
				_particlesNative.Dispose();
		}

		[BurstCompile]
		private struct UpdateParticleJob : IJobParallelFor
		{
			public NativeArray<Particle> Particles;
			public float3 BoundsMin;
			public float3 BoundsMax;
			public float DeltaTime;

			public void Execute(int index)
			{
				Particle particle = Particles[index];

				particle.Position += particle.Velocity * DeltaTime;

				//Bounce off the bounds
				//x
				if (particle.Position.x - particle.Radius < BoundsMin.x && particle.Velocity.x < 0 ||
				    particle.Position.x + particle.Radius > BoundsMax.x && particle.Velocity.x > 0)
				{
					particle.Velocity.x = -particle.Velocity.x;
				}
				//y
				if (particle.Position.y - particle.Radius < BoundsMin.y && particle.Velocity.y < 0 ||
				    particle.Position.y + particle.Radius > BoundsMax.y && particle.Velocity.y > 0)
				{
					particle.Velocity.y = -particle.Velocity.y;
				}
				//z
				if (particle.Position.z - particle.Radius < BoundsMin.z && particle.Velocity.z < 0 ||
				    particle.Position.z + particle.Radius > BoundsMax.z && particle.Velocity.z > 0)
				{
					particle.Velocity.z = -particle.Velocity.z;
				}
				Particles[index] = particle;
			}
		}
	}
}

#endif