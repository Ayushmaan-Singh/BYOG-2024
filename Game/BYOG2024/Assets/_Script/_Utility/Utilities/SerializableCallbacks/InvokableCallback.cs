using System;
public class InvokableCallback<TReturn> : InvokableCallbackBase<TReturn>
{

	public Func<TReturn> func;

	/// <summary> Constructor </summary>
	public InvokableCallback(object target, string methodName)
	{
		if (target == null || string.IsNullOrEmpty(methodName))
		{
			func = () => default(TReturn);
		}
		else
		{
			func = (Func<TReturn>)Delegate.CreateDelegate(typeof(Func<TReturn>), target, methodName);
		}
	}

	public TReturn Invoke()
	{
		return func();
	}

	public override TReturn Invoke(params object[] args)
	{
		return func();
	}
}

public class InvokableCallback<T0, TReturn> : InvokableCallbackBase<TReturn>
{

	public Func<T0, TReturn> func;

	/// <summary> Constructor </summary>
	public InvokableCallback(object target, string methodName)
	{
		if (target == null || string.IsNullOrEmpty(methodName))
		{
			func = x => default(TReturn);
		}
		else
		{
			func = (Func<T0, TReturn>)Delegate.CreateDelegate(typeof(Func<T0, TReturn>), target, methodName);
		}
	}

	public TReturn Invoke(T0 arg0)
	{
		return func(arg0);
	}

	public override TReturn Invoke(params object[] args)
	{
		return func((T0)args[0]);
	}
}

public class InvokableCallback<T0, T1, TReturn> : InvokableCallbackBase<TReturn>
{

	public Func<T0, T1, TReturn> func;

	/// <summary> Constructor </summary>
	public InvokableCallback(object target, string methodName)
	{
		if (target == null || string.IsNullOrEmpty(methodName))
		{
			func = (x, y) => default(TReturn);
		}
		else
		{
			func = (Func<T0, T1, TReturn>)Delegate.CreateDelegate(typeof(Func<T0, T1, TReturn>), target, methodName);
		}
	}

	public TReturn Invoke(T0 arg0, T1 arg1)
	{
		return func(arg0, arg1);
	}

	public override TReturn Invoke(params object[] args)
	{
		return func((T0)args[0], (T1)args[1]);
	}
}

public class InvokableCallback<T0, T1, T2, TReturn> : InvokableCallbackBase<TReturn>
{

	public Func<T0, T1, T2, TReturn> func;

	/// <summary> Constructor </summary>
	public InvokableCallback(object target, string methodName)
	{
		if (target == null || string.IsNullOrEmpty(methodName))
		{
			func = (x, y, z) => default(TReturn);
		}
		else
		{
			func = (Func<T0, T1, T2, TReturn>)Delegate.CreateDelegate(typeof(Func<T0, T1, T2, TReturn>), target, methodName);
		}
	}

	public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2)
	{
		return func(arg0, arg1, arg2);
	}

	public override TReturn Invoke(params object[] args)
	{
		return func((T0)args[0], (T1)args[1], (T2)args[2]);
	}
}

public class InvokableCallback<T0, T1, T2, T3, TReturn> : InvokableCallbackBase<TReturn>
{

	public Func<T0, T1, T2, T3, TReturn> func;

	/// <summary> Constructor </summary>
	public InvokableCallback(object target, string methodName)
	{
		if (target == null || string.IsNullOrEmpty(methodName))
		{
			func = (x, y, z, w) => default(TReturn);
		}
		else
		{
			func = (Func<T0, T1, T2, T3, TReturn>)Delegate.CreateDelegate(typeof(Func<T0, T1, T2, T3, TReturn>), target, methodName);
		}
	}

	public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
	{
		return func(arg0, arg1, arg2, arg3);
	}

	public override TReturn Invoke(params object[] args)
	{
		return func((T0)args[0], (T1)args[1], (T2)args[2], (T3)args[3]);
	}
}