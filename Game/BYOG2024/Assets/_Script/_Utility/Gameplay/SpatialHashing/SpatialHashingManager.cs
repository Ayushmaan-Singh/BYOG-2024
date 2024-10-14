using System;
using System.Collections.Generic;
using System.Linq;
using AstekUtility.Gameplay.SpatialHashing;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
namespace AstekUtility.Gameplay.SpatialHashing
{
	public class SpatialHashingManager : MonoBehaviour
	{
		private readonly List<GameObject> _particleInstances = new List<GameObject>();

		private struct Particle
		{
			public float3 Position;
			public float3 Velocity;
			public float Radius;
		}

		public GameObject[] QueryObjects(Vector3 position, float cellSize, float radius)
		{
			NativeArray<Particle> particles = new NativeArray<Particle>(_particleInstances.Count, Allocator.TempJob);
			NativeArray<HashAndIndex> hashAndIndices = new NativeArray<HashAndIndex>(_particleInstances.Count, Allocator.TempJob);

			int i = 0;
			foreach (GameObject obj in _particleInstances)
			{
				particles[i] = new Particle()
				{
					Position = obj.transform.position,
					Radius = 3f
				};
				i++;
			}

			//Hashing Job
			HashParticleJob hashJob = new HashParticleJob()
			{
				Particles = particles,
				CellSize = cellSize,
				HashAndIndices = hashAndIndices
			};

			JobHandle hashJobHandle = hashJob.Schedule(particles.Length, 64);

			//Hash sorting job
			SortHashCodesJob sortHashJob = new SortHashCodesJob()
			{
				HashAndIndices = hashAndIndices
			};

			JobHandle sortHashJobHandle = sortHashJob.Schedule(hashJobHandle);

			//Query Job
			QueryJob queryJob = new QueryJob()
			{
				Particles = particles,
				HashAndIndices = hashAndIndices,
				CellSize = cellSize,
				QueryPosition = position,
				QueryRadius = radius,
				ResultIndices = new NativeList<int>(Allocator.TempJob)
			};

			JobHandle queryJobHandle = queryJob.Schedule(sortHashJobHandle);
			queryJobHandle.Complete();

			//Some issue here
			GameObject[] results = new GameObject[queryJob.ResultIndices.Length];
			i = 0;
			foreach (int index in queryJob.ResultIndices)
			{
				results[i] = _particleInstances[index];
				i++;
			}

			if (particles.IsCreated) particles.Dispose();
			if (hashAndIndices.IsCreated) hashAndIndices.Dispose();
			if (queryJob.ResultIndices.IsCreated) queryJob.ResultIndices.Dispose();

			return results;
		}

		public void AddObjects(GameObject obj)
		{
			_particleInstances.Add(obj);
		}

		public void RemoveObjects(GameObject obj)
		{
			_particleInstances.Remove(obj);
		}

		#region Spacial Hashing Job

		private struct HashAndIndex : IComparable<HashAndIndex>
		{
			public int ParticleHash;
			public int Index;

			public int CompareTo(HashAndIndex other)
			{
				return ParticleHash.CompareTo(other.ParticleHash);
			}
		}

		private static int Hash(int3 gridPos)
		{
			unchecked
			{
				return gridPos.x * 73856093 ^ gridPos.y * 19349663 ^ gridPos.z * 83492791;
			}
		}

		private static int3 GridPosition(float3 position, float cellSize)
		{
			return new int3(math.floor(position / cellSize));
		}

		[BurstCompile]
		private struct HashParticleJob : IJobParallelFor
		{
			[ReadOnly] public NativeArray<Particle> Particles;
			public NativeArray<HashAndIndex> HashAndIndices;
			public float CellSize;

			public void Execute(int index)
			{
				Particle particle = Particles[index];
				int3 gridPos = GridPosition(particle.Position, CellSize);
				int hash = Hash(gridPos);

				HashAndIndices[index] = new HashAndIndex()
				{
					ParticleHash = hash,
					Index = index
				};
			}


		}

		[BurstCompile]
		private struct SortHashCodesJob : IJob
		{
			public NativeArray<HashAndIndex> HashAndIndices;
			public void Execute()
			{
				HashAndIndices.Sort();
			}
		}

		[BurstCompile]
		private struct QueryJob : IJob
		{
			[ReadOnly] public NativeArray<Particle> Particles;
			[ReadOnly] public NativeArray<HashAndIndex> HashAndIndices;
			public float3 QueryPosition;
			public float QueryRadius;
			public float CellSize;
			public NativeList<int> ResultIndices;

			public void Execute()
			{
				float radiusSquared = QueryRadius * QueryRadius;
				int3 minGridPos = GridPosition(QueryPosition - QueryRadius, CellSize);
				int3 maxGridPos = GridPosition(QueryPosition + QueryRadius, CellSize);
				for (int x = minGridPos.x; x <= maxGridPos.x; x++)
				{
					for (int y = minGridPos.y; y <= maxGridPos.y; y++)
					{
						for (int z = minGridPos.z; z <= maxGridPos.z; z++)
						{
							int3 gridPos = new int3(x, y, z);
							int hash = Hash(gridPos);

							int startIndex = BinarySearchFirst(HashAndIndices, hash);
							if (startIndex < 0) continue;

							for (int i = startIndex; i < HashAndIndices.Length && HashAndIndices[i].ParticleHash == hash; i++)
							{
								int particleIndex = HashAndIndices[i].Index;
								Particle particle = Particles[particleIndex];
								float3 toParticle = particle.Position - QueryPosition;

								if (math.lengthsq(toParticle) <= radiusSquared)
								{
									ResultIndices.Add(particleIndex);
								}
							}
						}
					}
				}
			}

			private int BinarySearchFirst(NativeArray<HashAndIndex> array, int hash)
			{
				int left = 0;
				int right = array.Length - 1;
				int result = -1;

				while (left <= right)
				{
					int mid = (left + right) / 2;
					int midHash = array[mid].ParticleHash;

					if (midHash == hash)
					{
						result = mid;
						right = mid - 1;
					}
					else if (midHash < hash)
					{
						left = mid + 1;
					}
					else
					{
						right = mid - 1;
					}
				}
				return result;
			}
		}

		#endregion
	}
}