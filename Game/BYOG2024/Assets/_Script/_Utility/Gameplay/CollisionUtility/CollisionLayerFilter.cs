using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstekUtility.Gameplay
{
	public class CollisionLayerFilter : MonoBehaviour
	{
		[SerializeField] private LayerMask includeLayer;

		public bool CanCollide(GameObject obj) => obj.IsInLayer(includeLayer);
		public void SetLayers(LayerMask layers) => includeLayer = layers;
	}
}