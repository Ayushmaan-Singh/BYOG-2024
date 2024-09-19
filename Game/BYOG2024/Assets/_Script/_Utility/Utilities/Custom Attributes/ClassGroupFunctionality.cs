using System;
using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility.CustomAttribute
{
	[Serializable]
	public class ClassGroupFunctionality
	{
		private List<Type> _classInGroup;

		private void FindClasses()
		{
			// Get all assemblies loaded in the current domain
			System.Reflection.Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			List<object> classes = new List<object>();

			// Iterate over each assembly and type to find classes with the CustomAttribute
			foreach (System.Reflection.Assembly assembly in assemblies)
			{
				foreach (Type type in assembly.GetTypes())
				{
					// Check if the type has the CustomAttribute
					object[] attributes = type.GetCustomAttributes(typeof(ClassGroupAttribute), true);
					if (attributes.Length > 0)
					{
						Debug.Log($"Class with CustomAttribute found: {type.FullName}");
					}
				}
			}
		}
	}
}