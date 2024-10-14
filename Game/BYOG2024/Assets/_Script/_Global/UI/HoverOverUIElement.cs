using System;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Global.UI
{
	public class HoveringOverThisElement : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
	{

		private void OnDisable()
		{
			ServiceLocator.Global?.Get<HoverOverUIManager>()?.Deregister(gameObject);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			ServiceLocator.Global.Get<HoverOverUIManager>()?.Register(gameObject);
		}
		public void OnPointerExit(PointerEventData eventData)
		{
			ServiceLocator.Global.Get<HoverOverUIManager>()?.Deregister(gameObject);
		}
	}
}