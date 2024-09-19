using Unity.Mathematics;
using UnityEngine;
namespace AstekUtility
{
    /// <summary>
    ///     This class converts data type
    ///     Like Vector3 to float3 and vice versa
    /// </summary>
    public static class DataTypeConversion
	{
		public static float3 Vector3ToFloat3(this Vector3 vector)
		{
			return new float3(vector.x, vector.y, vector.z);
		}

		public static Vector3 Float3ToVector3(this float3 vector)
		{
			return new Vector3(vector.x, vector.y, vector.z);
		}

		public static int3 Vector3ToInt3(this Vector3Int vector)
		{
			return new int3(vector.x, vector.y, vector.z);
		}

		public static Vector3Int Int3ToVector3(this int3 vector)
		{
			return new Vector3Int(vector.x, vector.y, vector.z);
		}

		public static float2 Vector2ToFloat2(this Vector2 vector)
		{
			return new float2(vector.x, vector.y);
		}

		public static Vector2 Float2ToVector2(this float2 vector)
		{
			return new Vector2(vector.x, vector.y);
		}

		public static int2 Vector2ToInt2(this Vector2Int vector)
		{
			return new int2(vector.x, vector.y);
		}

		public static Vector2Int Int2ToVector2(this int2 vector)
		{
			return new Vector2Int(vector.x, vector.y);
		}
	}
}