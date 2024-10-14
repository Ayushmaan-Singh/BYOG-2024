using System;
using System.Collections.Generic;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
namespace Global.UI
{
	public class HoverOverUIManager : MonoBehaviour
	{
		[ShowInInspector, InlineProperty] private readonly List<GameObject> mousePointerOverTheseUI = new List<GameObject>();

		private void Awake()
		{
			ServiceLocator.Global.Register(this);
		}

		private void OnDestroy()
		{
			ServiceLocator.Global?.Deregister(this);
		}

		public void Register(GameObject obj) => mousePointerOverTheseUI.Add(obj);
		public void Deregister(GameObject obj) => mousePointerOverTheseUI.Remove(obj);

		public GameObject GetObjectWithTag(string searchTag)
		{
			int count = mousePointerOverTheseUI.Count;
			for (int i = 0; i < count; i++)
			{
				if (mousePointerOverTheseUI[i].CompareTag(searchTag))
				{
					return mousePointerOverTheseUI[i];
				}
			}
			return null;
		}

		public List<GameObject> GetObjectsWithTag(string searchTag)
		{
			int count = mousePointerOverTheseUI.Count;
			List<GameObject> objects = new List<GameObject>();
			for (int i = 0; i < count; i++)
			{
				if (mousePointerOverTheseUI[i].CompareTag(searchTag))
				{
					objects.Add(mousePointerOverTheseUI[i]);
				}
			}
			return objects;
		}

		public GameObject GetObjectWithComponent<T>()
		{
			int count = mousePointerOverTheseUI.Count;
			for (int i = 0; i < count; i++)
			{
				if (mousePointerOverTheseUI[i].GetComponent<T>() != null)
				{
					return mousePointerOverTheseUI[i];
				}
			}
			return null;
		}

		public List<GameObject> GetObjectsWithComponent<T>()
		{
			int count = mousePointerOverTheseUI.Count;
			List<GameObject> objects = new List<GameObject>();
			for (int i = 0; i < count; i++)
			{
				if (mousePointerOverTheseUI[i].GetComponent<T>() != null)
				{
					objects.Add(mousePointerOverTheseUI[i]);
				}
			}
			return objects;
		}
	}
}