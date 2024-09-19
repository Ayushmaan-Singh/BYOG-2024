using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
namespace _Scripts._Utility.Blackboard
{
	public class TestData : SerializedMonoBehaviour
	{
		[DrawWithUnity]public List<DictElement> test;
	}
}