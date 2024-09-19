using System.Reflection;
using UnityEngine;
namespace AstekUtility.Arrays
{
	public class ArrayUtils<T>
	{
		public static T[] FlattenArray(T[,] grid, int size)
		{
			T[] arr = new T[size];

			int i = 0;
			for (int y = 0; y < grid.GetLength(1); y++)
			{
				for (int x = 0; x < grid.GetLength(0); x++)
				{
					arr[i] = grid[x, y];
				}
			}

			return arr;
		}

		public static int FlattenedArrayCoord(Vector2Int pos, int width)
		{
			return width * pos.x * pos.y;
		}

		#region Radix Sort

		public static void RadixSort(int[] array)
		{
			// Find the maximum value in the array
			int maxValue = int.MinValue;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] > maxValue)
				{
					maxValue = array[i];
				}
			}

			// Do the counting sort for each digit, starting from the least significant
			for (int exp = 1; maxValue / exp > 0; exp *= 10)
			{
				// Init the count array
				int[] count = new int[10];

				// Count the number of elements with each digit in the current position
				for (int i = 0; i < array.Length; i++)
				{
					int digit = array[i] / exp % 10;
					count[digit]++;
				}

				// Calculate the _running sum of counts to get the positions of elements in the sorted array
				for (int i = 1; i < count.Length; i++)
				{
					count[i] += count[i - 1];
				}

				// Build the sorted array by placing elements in the correct positions based on their digit
				int[] sortedArray = new int[array.Length];
				for (int i = array.Length - 1; i >= 0; i--)
				{
					int digit = array[i] / exp % 10;
					sortedArray[count[digit] - 1] = array[i];
					count[digit]--;
				}

				// Copy the sorted array back to the original array
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = sortedArray[i];
				}
			}
		}

		public static void RadixSort(float[] array)
		{
			// Find the maximum value in the array
			float maxValue = float.MinValue;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] > maxValue)
				{
					maxValue = array[i];
				}
			}

			// Do the counting sort for each digit, starting from the least significant
			for (int exp = 1; maxValue / exp > 0; exp *= 10)
			{
				// Init the count array
				int[] count = new int[10];

				// Count the number of elements with each digit in the current position
				for (int i = 0; i < array.Length; i++)
				{
					int digit = (int)(array[i] / exp % 10);
					count[digit]++;
				}

				// Calculate the _running sum of counts to get the positions of elements in the sorted array
				for (int i = 1; i < count.Length; i++)
				{
					count[i] += count[i - 1];
				}

				// Build the sorted array by placing elements in the correct positions based on their digit
				float[] sortedArray = new float[array.Length];
				for (int i = array.Length - 1; i >= 0; i--)
				{
					int digit = (int)(array[i] / exp % 10);
					sortedArray[count[digit] - 1] = array[i];
					count[digit]--;
				}

				// Copy the sorted array back to the original array
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = sortedArray[i];
				}
			}
		}

        /// <summary>
        ///     Never use this version in runtime
        /// </summary>
        /// <param name="array"></param>
        /// <param name="fieldName"></param>
        public static void RadixSort(T[] array, string fieldName)
		{
			// Get the field to sort by using reflection
			FieldInfo field = typeof(T).GetField(fieldName);

			// Find the maximum value of the field
			float maxValue = float.MinValue;
			for (int i = 0; i < array.Length; i++)
			{
				float value = (float)field.GetValue(array[i]);
				if (value > maxValue)
				{
					maxValue = value;
				}
			}

			// Do the counting sort for each digit, starting from the least significant
			for (int exp = 1; maxValue / exp > 0; exp *= 10)
			{
				// Init the count array
				int[] count = new int[10];

				// Count the number of objects with each digit in the current position
				for (int i = 0; i < array.Length; i++)
				{
					float value = (float)field.GetValue(array[i]);
					int digit = (int)(value / exp % 10);
					count[digit]++;
				}

				// Calculate the _running sum of counts to get the positions of objects in the sorted array
				for (int i = 1; i < count.Length; i++)
				{
					count[i] += count[i - 1];
				}

				// Copy the objects into the sorted array in the correct order
				T[] sorted = new T[array.Length];
				for (int i = array.Length - 1; i >= 0; i--)
				{
					float value = (float)field.GetValue(array[i]);
					int digit = (int)(value / exp % 10);
					sorted[count[digit] - 1] = array[i];
					count[digit]--;
				}

				// Copy the sorted array back into the original array
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = sorted[i];
				}
			}
		}

		#endregion
	}
}