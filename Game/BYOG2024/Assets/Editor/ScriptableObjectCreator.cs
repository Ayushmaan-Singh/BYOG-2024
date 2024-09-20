using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.Editor.Extensions
{
	public class ScriptableObjectCreator : OdinMenuEditorWindow
	{
		private static HashSet<Type> _SCRIPTABLEOBJECTTYPES
		{
			get
			{
				IEnumerable<Type> temp = AssemblyUtilities.GetTypes(AssemblyCategory.Scripts)
					.Where(t =>
						t.IsClass &&
						typeof(ScriptableObject).IsAssignableFrom(t) &&
						!typeof(EditorWindow).IsAssignableFrom(t) &&
						!typeof(UnityEditor.Editor).IsAssignableFrom(t));
				return (HashSet<Type>)GetHashSet(temp);
			}
		}
		// Ref: https://stackoverflow.com/questions/34858338/how-to-elegantly-convert-ienumerablet-to-hashsett-at-runtime-without-knowing
		public static IEnumerable<T> GetHashSet<T>(IEnumerable<T> source)
		{
			return new HashSet<T>(source);
		}

		[MenuItem("Assets/Create Scriptable Object...", priority = -10000)]
		private static void ShowDialog()
		{
			string path = "Assets";
			Object obj = Selection.activeObject;
			if (obj && AssetDatabase.Contains(obj))
			{
				path = AssetDatabase.GetAssetPath(obj);
				if (!Directory.Exists(path))
				{
					path = Path.GetDirectoryName(path);
				}
			}

			var window = CreateInstance<ScriptableObjectCreator>();
			window.ShowUtility();
			window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
			window.titleContent = new GUIContent(path);
			
			if (path != null)
				window.targetFolder = path.Trim('/');
		}

		private ScriptableObject previewObject;
		private string targetFolder;
		private Vector2 scroll;

		private Type SelectedType
		{
			get
			{
				var m = this.MenuTree.Selection.LastOrDefault();
				return m?.Value as Type;
			}
		}

		protected override OdinMenuTree BuildMenuTree()
		{
			this.MenuWidth = 270;
			this.WindowPadding = Vector4.zero;

			OdinMenuTree tree = new OdinMenuTree(false)
			{
				Config =
				{
					DrawSearchToolbar = true
				},
				DefaultMenuStyle = OdinMenuStyle.TreeViewStyle
			};
			tree.AddRange(_SCRIPTABLEOBJECTTYPES.Where(x => !x.IsAbstract), GetMenuPathForType);
			tree.SortMenuItemsByName();
			tree.Selection.SelectionConfirmed += x => this.CreateAsset();
			tree.Selection.SelectionChanged += e =>
			{
				if (this.previewObject && !AssetDatabase.Contains(this.previewObject))
				{
					DestroyImmediate(this.previewObject);
				}
				if (e != SelectionChangedType.ItemAdded)
				{
					return;
				}
				var t = this.SelectedType;
				if (t != null && !t.IsAbstract)
				{
					this.previewObject = CreateInstance(t) as ScriptableObject;
				}
			};

			return tree;
		}

		private string GetMenuPathForType(Type t)
		{

			var path = "";

			if (t.Namespace != null)
			{
				foreach (var part in t.Namespace.Split('.'))
				{
					path += part.SplitPascalCase() + "/";
				}
			}

			return path + t.Name.SplitPascalCase();
		}

		protected override IEnumerable<object> GetTargets()
		{
			yield return this.previewObject;
		}

		protected override void DrawEditor(int index)
		{
			this.scroll = GUILayout.BeginScrollView(this.scroll);
			{
				base.DrawEditor(index);
			}
			GUILayout.EndScrollView();

			if (this.previewObject)
			{
				GUILayout.FlexibleSpace();
				SirenixEditorGUI.HorizontalLineSeparator(1);
				if (GUILayout.Button("Create Asset", GUILayoutOptions.Height(30)))
				{
					this.CreateAsset();
				}
			}
		}

		private void CreateAsset()
		{
			if (!this.previewObject)
				return;
			
			string dest = this.targetFolder + "/" + this.MenuTree.Selection.First().Name + ".asset";
			dest = AssetDatabase.GenerateUniqueAssetPath(dest);
			ProjectWindowUtil.CreateAsset(this.previewObject, dest);
			EditorApplication.delayCall += this.Close;
		}
	}
}