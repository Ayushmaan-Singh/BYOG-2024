using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace AstekUtility
{
	/// <summary>
	///     Extends to provide some additional functionality to Vector3 types
	/// </summary>
	public static class Vector3Extensions
	{
		/// <summary>
		/// Sets any x y z _values of a Vector3
		/// </summary>
		public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
			=> new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);

		/// <summary>
		///     Adds to any x y z _values of a Vector3
		/// </summary>
		public static Vector3 Add(this Vector3 vector, float x = 0, float y = 0, float z = 0)
			=> new Vector3(vector.x + x, vector.y + y, vector.z + z);

		/// <summary>
		/// Returns distance between 2 points using the more optimized method of root of squared magnitude method
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		/// <returns></returns>
		public static float SqrMagnitudeDistance(this Vector3 source, Vector3 destination) => Mathf.Sqrt((source - destination).sqrMagnitude);

		/// <summary>
		///     Returns a Boolean indicating whether the current Vector3 is in a given range from another Vector3
		/// </summary>
		/// <param name="current">The current Vector3 position</param>
		/// <param name="target">The Vector3 position to compare against</param>
		/// <param name="range">The range value to compare against</param>
		/// <returns>True if the current Vector3 is in the given range from the target Vector3, false otherwise</returns>
		public static bool InRangeOf(this Vector3 current, Vector3 target, float range) => (current - target).sqrMagnitude <= range * range;

		public static Vector3 SetPrecision(this Vector3 current, int precision) =>
			new Vector3(current.x.SetPrecision(precision), current.y.SetPrecision(precision), current.z.SetPrecision(precision));
	}

	public static class TransformExtensions
	{
		/// <summary>
		///     Retrieves all the children of a given Transform.
		/// </summary>
		/// <remarks>
		///     This method can be used with LINQ to perform operations on all child Transforms. For example,
		///     you could use it to find all children with a specific tag, to disable all children, etc.
		///     Transform implements IEnumerable and the GetEnumerator method which returns an IEnumerator of all its children.
		/// </remarks>
		/// <param name="parent">The Transform to retrieve children from.</param>
		/// <returns>An IEnumerable&lt;Transform&gt; containing all the child Transforms of the parent.</returns>
		public static IEnumerable<Transform> Children(this Transform parent)
		{
			foreach (Transform child in parent)
			{
				yield return child;
			}
		}

		/// <summary>
		///     Resets transform's position, scale and rotation
		/// </summary>
		/// <param name="transform">Transform to use</param>
		public static void Reset(this Transform transform)
		{
			transform.position = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}

		/// <summary>
		///     Destroys all child game objects of the given transform.
		/// </summary>
		/// <param name="parent">The Transform whose child game objects are to be destroyed.</param>
		public static void DestroyChildren(this Transform parent)
		{
			parent.ForEveryChild(child => Object.Destroy(child.gameObject));
		}

		/// <summary>
		///     Immediately destroys all child game objects of the given transform.
		/// </summary>
		/// <param name="parent">The Transform whose child game objects are to be immediately destroyed.</param>
		public static void DestroyChildrenImmediate(this Transform parent)
		{
			parent.ForEveryChild(child => Object.DestroyImmediate(child.gameObject));
		}

		/// <summary>
		///     Enables all child game objects of the given transform.
		/// </summary>
		/// <param name="parent">The Transform whose child game objects are to be enabled.</param>
		public static void EnableChildren(this Transform parent)
		{
			parent.ForEveryChild(child => child.gameObject.SetActive(true));
		}

		/// <summary>
		///     Disables all child game objects of the given transform.
		/// </summary>
		/// <param name="parent">The Transform whose child game objects are to be disabled.</param>
		public static void DisableChildren(this Transform parent)
		{
			parent.ForEveryChild(child => child.gameObject.SetActive(false));
		}

		/// <summary>
		///     Executes a specified action for each child of a given transform.
		/// </summary>
		/// <param name="parent">The parent transform.</param>
		/// <param name="action">The action to be performed on each child.</param>
		/// <remarks>
		///     This method iterates over all child transforms in reverse order and executes a given action on them.
		///     The action is a delegate that takes a Transform as parameter.
		/// </remarks>
		public static void ForEveryChild(this Transform parent, Action<Transform> action)
		{
			for (int i = parent.childCount - 1; i >= 0; i--)
			{
				action(parent.GetChild(i));
			}
		}
	}

	public static class GameObjectExtensions
	{
		/// <summary>
		///     Gets a component of the given type attached to the GameObject. If that type of component does not exist, it adds
		///     one.
		/// </summary>
		/// <remarks>
		///     This method is useful when you don't know if a GameObject has a specific type of component,
		///     but you want to work with that component regardless. Instead of checking and adding the component manually,
		///     you can use this method to do both operations in one line.
		/// </remarks>
		/// <typeparam name="T">The type of the component to get or add.</typeparam>
		/// <param name="gameObject">The GameObject to get the component from or add the component to.</param>
		/// <returns>The existing component of the given type, or a new one if no such component exists.</returns>
		public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
		{
			T component = gameObject.GetComponent<T>();
			if (!component) component = gameObject.AddComponent<T>();

			return component;
		}

		/// <summary>
		///     Returns the object itself if it exists, null otherwise.
		/// </summary>
		/// <remarks>
		///     This method helps differentiate between a null reference and a destroyed Unity object. Unity's "== null" check
		///     can incorrectly return true for destroyed objects, leading to misleading behaviour. The OrNull method use
		///     Unity's "null check", and if the object has been marked for destruction, it ensures an actual null reference is
		///     returned,
		///     aiding in correctly chaining operations and preventing NullReferenceExceptions.
		/// </remarks>
		/// <typeparam name="T">The type of the object.</typeparam>
		/// <param name="obj">The object being checked.</param>
		/// <returns>The object itself if it exists and not destroyed, null otherwise.</returns>
		public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;

		/// <summary>
		/// Get component of type T if they are on the object, on the parent of object or on the children of object
		/// </summary>
		/// <param name="go"></param>
		/// <param name="component"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static bool TryGetComponentFromParentOrChildren<T>(this GameObject go, out T component) where T : MonoBehaviour
		{
			component = null;
			if (!go)
				return false;

			// Check if the current object has the component
			component = go.GetComponentInParent<T>(true);
			if (component)
				return true;

			// Check children
			component = go.GetComponentInChildren<T>(true);
			if (component)
				return true;

			// Component not found in hierarchy
			return false;
		}

		/// <summary>
		///     Destroys all children of the game object
		/// </summary>
		/// <param name="gameObject">GameObject whose children are to be destroyed.</param>
		public static void DestroyChildren(this GameObject gameObject)
		{
			gameObject.transform.DestroyChildren();
		}

		/// <summary>
		///     Immediately destroys all children of the given GameObject.
		/// </summary>
		/// <param name="gameObject">GameObject whose children are to be destroyed.</param>
		public static void DestroyChildrenImmediate(this GameObject gameObject)
		{
			gameObject.transform.DestroyChildrenImmediate();
		}

		/// <summary>
		///     Enables all child GameObjects associated with the given GameObject.
		/// </summary>
		/// <param name="gameObject">GameObject whose child GameObjects are to be enabled.</param>
		public static void EnableChildren(this GameObject gameObject)
		{
			gameObject.transform.EnableChildren();
		}

		/// <summary>
		///     Disables all child GameObjects associated with the given GameObject.
		/// </summary>
		/// <param name="gameObject">GameObject whose child GameObjects are to be disabled.</param>
		public static void DisableChildren(this GameObject gameObject)
		{
			gameObject.transform.DisableChildren();
		}

		/// <summary>
		///     Resets the GameObject's transform's position, rotation, and scale to their default _values.
		/// </summary>
		/// <param name="gameObject">GameObject whose transformation is to be reset.</param>
		public static void ResetTransformation(this GameObject gameObject)
		{
			gameObject.transform.Reset();
		}

		/// <summary>
		///     Returns the hierarchical path in the Unity scene hierarchy for this GameObject.
		/// </summary>
		/// <param name="gameObject">The GameObject to get the path for.</param>
		/// <returns>
		///     A string representing the full hierarchical path of this GameObject in the Unity scene.
		///     This is a '/'-separated string where each part is the name of a parent, starting from the root parent and ending
		///     with the name of the specified GameObjects parent.
		/// </returns>
		public static string Path(this GameObject gameObject)
			=> "/" + string.Join("/", gameObject.GetComponentsInParent<Transform>().Select(t => t.name).Reverse().ToArray());

		/// <summary>
		///     Returns the full hierarchical path in the Unity scene hierarchy for this GameObject.
		/// </summary>
		/// <param name="gameObject">The GameObject to get the path for.</param>
		/// <returns>
		///     A string representing the full hierarchical path of this GameObject in the Unity scene.
		///     This is a '/'-separated string where each part is the name of a parent, starting from the root parent and ending
		///     with the name of the specified GameObject itself.
		/// </returns>
		public static string PathFull(this GameObject gameObject) => gameObject.Path() + "/" + gameObject.name;

		/// <summary>
		///     Recursively sets the provided layer for this GameObject and all of its descendants in the Unity scene hierarchy.
		/// </summary>
		/// <param name="gameObject">The GameObject to set layers for.</param>
		/// <param name="layer">The layer number to set for GameObject and all of its descendants.</param>
		public static void SetLayersRecursively(this GameObject gameObject, int layer)
		{
			gameObject.layer = layer;
			gameObject.transform.ForEveryChild(child => child.gameObject.SetLayersRecursively(layer));
		}

		public static GameObject SetIsActive(this GameObject gameObject, bool isActive)
		{
			gameObject.SetActive(false);
			return gameObject;
		}
	}

	public static class IEnumeratorExtensions
	{
		/// <summary>
		/// Converts an IEnumerator<T> to an IEnumerable<T>.
		/// </summary>
		/// <param name="e">An instance of IEnumerator<T></param>
		/// <returns>An IEnumerable<T> with the same elements as the input instance</returns>
		public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> e)
		{
			while (e.MoveNext())
			{
				yield return e.Current;
			}
		}
	}

	public static class ListExtension
	{
		public static List<T[]> KCombinationOfSize<T>(this List<T> collection, int combinationSize)
		{
			List<T[]> results = new List<T[]>();
			List<T> copy = new List<T>(collection);
			PossibleCombinations(copy, new T[combinationSize], 0, collection.Count - 1, 0, combinationSize, results);
			return results;
		}

		private static void PossibleCombinations<T>(List<T> collection, T[] data, int start, int end, int index, int r, List<T[]> storage)
		{
			// Base case: if index equals r, print the combination
			if (index == r)
			{
				storage.Add((T[])data.Clone());
				return;
			}

			// Generate combinations recursively
			for (int i = start; i <= end && end - i + 1 >= r - index; i++)
			{
				data[index] = collection[i];                                               // Fill data [] with elements of arr [] for the current combination
				PossibleCombinations(collection, data, i + 1, end, index + 1, r, storage); // Recursively generate combinations
			}
		}
	}

	public static class LayerMaskExtension
	{
		public static bool IsInLayer(this GameObject gameObject, LayerMask layerMask) => (layerMask.value & 1 << gameObject.layer) > 0;
	}

	public static class TypeExtensions
	{
		public static void InvokeMethodByName<T>(this Type scriptToInvokeMethodFrom, T objectInstance, string functionName, params object[] parameters) where T : class
		{
			ParameterInfo[] parametersRequired = scriptToInvokeMethodFrom.GetMethod(functionName)?.GetParameters();
			if (parametersRequired != null && parametersRequired.Length == parameters.Length)
			{
				for (int i = 0; i < parametersRequired.Length; i++)
				{
					if (parametersRequired[i].ParameterType != parameters[i].GetType())
					{
						Debug.LogError($"Parameter Type Mismatch: {parametersRequired[i].GetType()} != {parameters[i].GetType()}");
						return;
					}
				}
			}
			scriptToInvokeMethodFrom.GetMethod(functionName)?.Invoke(objectInstance, parameters);
		}
	}

	public static class AnimationCurveExtension
	{
		public static float TimeFromValue(this AnimationCurve c, float value, float precision = 1e-6f)
		{
			float minTime = c.keys[0].time;
			float maxTime = c.keys[^1].time;
			float best = (maxTime + minTime) / 2;
			float bestVal = c.Evaluate(best);
			int it = 0;
			const int maxIt = 1000;
			float sign = Mathf.Sign(c.keys[^1].value - c.keys[0].value);
			while (it < maxIt && Mathf.Abs(minTime - maxTime) > precision)
			{
				if ((bestVal - value) * sign > 0)
				{
					maxTime = best;
				}
				else
				{
					minTime = best;
				}
				best = (maxTime + minTime) / 2;
				bestVal = c.Evaluate(best);
				it++;
			}
			return best;
		}
	}

	/// <summary>
	///     Extensions method for data type float
	/// </summary>
	public static class FloatExtension
	{
		/// <summary>
		///     Return a float rounded to said Precision
		/// </summary>
		/// <param name="value"></param>
		/// <param name="precision"></param>
		/// <returns></returns>
		public static float SetPrecision(this float value, int precision) => (float)Math.Round(value, precision);

		public static bool Approx(this float f1, float f2) => Mathf.Approximately(f1, f2);
	}

	/// <summary>
	/// Extension method that converts an AsyncOperation into a Task.
	/// </summary>
	/// <param name="asyncOperation">The AsyncOperation to convert.</param>
	/// <returns>A Task that represents the completion of the AsyncOperation.</returns>
	public static class AsyncOperationExtensions
	{
		public static Task AsTask(this AsyncOperation asyncOperation)
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			asyncOperation.completed += _ => tcs.SetResult(true);
			return tcs.Task;
		}
	}

	public static class StringExtensions
	{
		/// <summary>
		/// Computes the FNV-1a hash for the input string. 
		/// The FNV-1a hash is a non-cryptographic hash function known for its speed and good distribution properties.
		/// Useful for creating Dictionary keys instead of using strings.
		/// https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
		/// </summary>
		/// <param name="str">The input string to hash.</param>
		/// <returns>An integer representing the FNV-1a hash of the input string.</returns>
		public static int ComputeFNV1aHash(this string str)
		{
			uint hash = 2166136261;
			foreach (char c in str)
			{
				hash = (hash ^ c) * 16777619;
			}
			return unchecked((int)hash);
		}
	}

	public static class EnumerableExtensions
	{
		public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
		{
			foreach (T item in sequence)
			{
				action(item);
			}
		}
	}
}