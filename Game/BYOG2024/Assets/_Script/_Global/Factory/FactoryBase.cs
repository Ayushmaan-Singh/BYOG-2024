using Sirenix.OdinInspector;
using UnityEngine;

namespace Global.Factory
{
	public abstract class FactoryBase : SerializedMonoBehaviour
	{
		public abstract T Instantiate<T>() where T:class;
	}
}