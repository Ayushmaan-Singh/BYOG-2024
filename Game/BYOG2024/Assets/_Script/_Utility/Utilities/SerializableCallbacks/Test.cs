﻿using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class Test : MonoBehaviour
{
	private const int ITERATIONS = 100000;
	public float f = 0.5f;
	public string s;
	public Condition condition;
	public SerializableEvent ev;
	public Func<float, bool> DynamicDelegate;
	public Func<float, bool> RegularDelegate;

	private void Start()
	{
		RegularDelegate = TestMethod;
		DynamicDelegate = (Func<float, bool>)Delegate.CreateDelegate(typeof(Func<float, bool>), this, "TestMethod");
		condition.Invoke(f);
	}

	private void Update()
	{
		Stopwatch method = Stopwatch.StartNew();
		bool methodb = false;
		for (int i = 0; i < ITERATIONS; ++i)
		{
			methodb = TestMethod(f);
		}
		method.Stop();

		Stopwatch regularDelegate = Stopwatch.StartNew();
		bool regularDelegateb = false;
		for (int i = 0; i < ITERATIONS; ++i)
		{
			regularDelegateb = RegularDelegate(f);
		}
		regularDelegate.Stop();

		Stopwatch dynamicDelegate = Stopwatch.StartNew();
		bool dynamicDelegateb = false;
		for (int i = 0; i < ITERATIONS; ++i)
		{
			dynamicDelegateb = DynamicDelegate(f);
		}
		dynamicDelegate.Stop();

		Stopwatch serializedDelegate = Stopwatch.StartNew();
		bool serializedDelegateb = false;
		for (int i = 0; i < ITERATIONS; ++i)
		{
			serializedDelegateb = condition.Invoke(f);
		}
		serializedDelegate.Stop();

		Stopwatch serializedEvent = Stopwatch.StartNew();
		for (int i = 0; i < ITERATIONS; ++i)
		{
			ev.Invoke();
		}
		serializedEvent.Stop();

		Debug.Log("Method: " + methodb + method.Elapsed);
		Debug.Log("RegularDelegate: " + regularDelegateb + regularDelegate.Elapsed);
		Debug.Log("DynamicDelegate: " + dynamicDelegateb + dynamicDelegate.Elapsed);
		Debug.Log("SerializedCallback: " + serializedDelegateb + serializedDelegate.Elapsed);
		Debug.Log("SerializedEvent: " + serializedEvent.Elapsed);
	}

	public bool TestMethod(float f)
	{
		return f > 0.5f;
	}

	public bool TestMethod(string a)
	{
		return string.IsNullOrEmpty(a);
	}

	public bool TestMethod2(float f, string a)
	{
		return f > 0.5f && string.IsNullOrEmpty(a);
	}

	public void TestMethod2(string a)
	{
		s = a;
	}
}

[Serializable]
public class Condition : SerializableCallback<float, bool> { }