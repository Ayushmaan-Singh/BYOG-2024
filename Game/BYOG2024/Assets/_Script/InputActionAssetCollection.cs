using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine.InputSystem;

namespace Global
{
	public class InputActionAssetCollection : SerializedScriptableObject
	{
		[OdinSerialize, InlineProperty] public Dictionary<InputActionType, InputActionAsset> _inputActionAssetCollection = new Dictionary<InputActionType, InputActionAsset>();

		public InputActionAsset this[InputActionType actionAsset] => _inputActionAssetCollection[actionAsset];

		public Dictionary<InputActionType, InputActionAsset>.KeyCollection Keys => _inputActionAssetCollection.Keys;
		public Dictionary<InputActionType, InputActionAsset>.ValueCollection Values => _inputActionAssetCollection.Values;
		public int Count => _inputActionAssetCollection.Count;

		public bool TryGet(InputActionType type, out InputActionAsset prefab) => _inputActionAssetCollection.TryGetValue(type, out prefab);
	}

	/// <summary>
	/// Add more here depending on 
	/// </summary>
	public enum InputActionType
	{
		Main
	}
}