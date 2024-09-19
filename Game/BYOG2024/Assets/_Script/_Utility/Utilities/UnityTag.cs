using Sirenix.OdinInspector;

#if ODIN_INSPECTOR

namespace AstekUtility.Odin.Utility
{
	[System.Serializable]
	public class UnityTag
	{
		[ValueDropdown(nameof(tagCollection))] public string Tag;
		#if UNITY_EDITOR
		private ValueDropdownList<string> tagCollection
		{
			get
			{
				ValueDropdownList<string> collection = new ValueDropdownList<string>();
				foreach (string tag in UnityEditorInternal.InternalEditorUtility.tags)
				{
					collection.Add(tag);
				}
				return collection;
			}
		}
		#endif
	}
}

#endif