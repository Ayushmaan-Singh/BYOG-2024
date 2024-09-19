using System;
using System.Collections.Generic;
namespace AstekUtility.Assembly
{

	/// <summary>
	///     Contains Assembly Used In Our Game
	///     Only Contains Assembly Required By Our Own Scripts
	/// </summary>
	public static class GameAssembliesUtils
	{

		/// <summary>
		///     Custom Assemblies In Our Game
		///     Need To Be Populated Manually
		/// </summary>
		public enum CustomAssembliesTypes { }
		/// <summary>
		///     Assemblies Predefined by unity
		/// </summary>
		public enum PredefinedAssembliesTypes
		{
			AssemblyCSharp,
			AssemblyCSharpEditor,
			AssemblyCSharpEditorFirstPass,
			AssemblyCSharpFirstPass
		}

		/// <summary>
		///     Get predefined assembly type via string name
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		public static PredefinedAssembliesTypes? GetPredefinedAssembliesTypes(string assemblyName)
		{
			return assemblyName switch
			{
				"Assembly-CSharp" => PredefinedAssembliesTypes.AssemblyCSharp,
				"Assembly-CSharp-firstpass" => PredefinedAssembliesTypes.AssemblyCSharpFirstPass,
				"Assembly-CSharp-Editor" => PredefinedAssembliesTypes.AssemblyCSharpEditor,
				"Assembly-CSharp-Editor-firstpass" => PredefinedAssembliesTypes.AssemblyCSharpEditorFirstPass,
				_ => null
			};
		}

		/// <summary>
		///     Get Custom assembly type via string name
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		public static CustomAssembliesTypes? GetCustomAssembliesTypes(string assemblyName)
		{
			return assemblyName switch
			{
				_ => null
			};
		}
#if UNITY_EDITOR

		/// <summary>
		///     Method looks through a given assembly and adds types that fulfill a certain interface to the provided collection.
		/// </summary>
		/// <param name="assemblyTypes">Array of Type objects representing all the types in the assembly.</param>
		/// <param name="interfaceType">Type representing the interface to be checked against.</param>
		/// <param name="results">Collection of types where result should be added.</param>
		private static void AddTypesFromAssembly(Type[] assemblyTypes, Type interfaceType, ICollection<Type> results)
		{
			if (assemblyTypes == null) return;
			for (int i = 0; i < assemblyTypes.Length; i++)
			{
				Type type = assemblyTypes[i];
				if (type != interfaceType && interfaceType.IsAssignableFrom(type))
				{
					results.Add(type);
				}
			}
		}

		/// <summary>
		///     Gets class, interface, structs of type from assemblies
		///     Only Use in Editor and not runtime
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static List<Type> GetType(Type type)
		{
			System.Reflection.Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

			Dictionary<PredefinedAssembliesTypes, Type[]> predefinedAssemblyTypes = new Dictionary<PredefinedAssembliesTypes, Type[]>();
			Dictionary<CustomAssembliesTypes, Type[]> customAssemblyTypes = new Dictionary<CustomAssembliesTypes, Type[]>();
			List<Type> types = new List<Type>();

			for (int i = 0; i < assemblies.Length; i++)
			{
				PredefinedAssembliesTypes? predefinedAssemblyTypeTemp = GetPredefinedAssembliesTypes(assemblies[i].GetName().Name);
				CustomAssembliesTypes? customAssemblyTypeTemp = GetCustomAssembliesTypes(assemblies[i].GetName().Name);

				if (predefinedAssemblyTypeTemp != null)
				{
					predefinedAssemblyTypes.Add((PredefinedAssembliesTypes)predefinedAssemblyTypeTemp, assemblies[i].GetTypes());
				}
				else if (customAssemblyTypeTemp != null)
				{
					customAssemblyTypes.Add((CustomAssembliesTypes)predefinedAssemblyTypeTemp, assemblies[i].GetTypes());
				}
			}

			foreach (PredefinedAssembliesTypes predefAssembly in predefinedAssemblyTypes.Keys)
			{
				predefinedAssemblyTypes.TryGetValue(predefAssembly, out Type[] assemblyTypes);
				AddTypesFromAssembly(assemblyTypes, type, types);
			}

			foreach (CustomAssembliesTypes customAssembly in predefinedAssemblyTypes.Keys)
			{
				customAssemblyTypes.TryGetValue(customAssembly, out Type[] assemblyTypes);
				AddTypesFromAssembly(assemblyTypes, type, types);
			}

			return types;
		}
#endif
	}
}