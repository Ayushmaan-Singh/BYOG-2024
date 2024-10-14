using UnityEngine;
namespace Combat
{
	public class Consumable : MonoBehaviour
	{
		[field:SerializeField] public ConsumableEntities EntityType { get; private set; }
	}
}