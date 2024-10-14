using Sirenix.OdinInspector;
using Sirenix.Utilities;

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

		public override bool Equals(object obj)
		{
			if (obj is UnityTag tag)
				return this == tag;
			else if (obj is string val)
				return this == val;

			return false;
		}

		public override int GetHashCode()
		{
			return Tag.ComputeFNV1aHash();
		}

		public static bool operator ==(UnityTag tag, string val) => tag?.Tag == val;
		public static bool operator !=(UnityTag tag, string val) => !(tag == val);
		
		public static bool operator ==(UnityTag tag1, UnityTag tag2) => tag1?.Tag == tag2?.Tag;
		public static bool operator !=(UnityTag tag1, UnityTag tag2) => !(tag1 == tag2);
	}
}

#endif