using UnityEngine;

namespace AstekUtility.Gameplay
{
	public class OwnerOfThisObject : MonoBehaviour
	{
		[SerializeField] private Transform owner;
		public Transform Owner => owner.OrNull();

		public void SetOwner(Transform newOwner) => owner = newOwner;
	}
}