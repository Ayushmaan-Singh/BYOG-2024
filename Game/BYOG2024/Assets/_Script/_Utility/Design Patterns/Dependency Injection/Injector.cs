using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServiceLocator = AstekUtility.DesignPattern.ServiceLocatorTool.ServiceLocator;
using UnityEngine;

namespace AstekUtility.DesignPattern.DependencyInjection
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
	public sealed class InjectAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.Method)]
	public sealed class ProvideAttribute : Attribute { }

	public interface IDependencyProvider { }

	[DefaultExecutionOrder(-1000)]
	public class Injector : Singleton<Injector>
	{
		protected const BindingFlags k_bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		protected readonly Dictionary<Type, object> _registry = new Dictionary<Type, object>();

		protected override void Awake()
		{
			ServiceLocator.Global.Register(this);
			base.Awake();

			//Find all modules implementing IDependecyProvider
			IEnumerable<IDependencyProvider> providers = FindMonoBehaviors().OfType<IDependencyProvider>();
			foreach (IDependencyProvider provider in providers)
			{
				RegisterProvider(provider);
			}

			//Find all injectable objects and inject their dependencies
			IEnumerable<MonoBehaviour> injectables = FindMonoBehaviors().Where(IsInjectable);
			foreach (MonoBehaviour injectable in injectables)
			{
				Inject(injectable);
			}
		}

		protected void OnDestroy()
		{
			ServiceLocator.Global?.Deregister(this);
		}

		protected void Inject(object instance)
		{
			Type type = instance.GetType();

			//Field Injection
			IEnumerable<FieldInfo> injectableFields = type.GetFields(k_bindingFlags).Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

			foreach (FieldInfo injectableField in injectableFields)
			{
				Type fieldType = injectableField.FieldType;
				object resolvedInstance = Resolve(fieldType);
				if (resolvedInstance == null)
				{
					throw new Exception($"Failed to inject {fieldType.Name} into {type.Name}");
				}

				injectableField.SetValue(instance, resolvedInstance);
#if UNITY_EDITOR
				Debug.Log($"Field Injected {type.Name}.{injectableField.Name}");
#endif
			}

			//Method Injection
			IEnumerable<MethodInfo> injectableMethods = type.GetMethods(k_bindingFlags).Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
			foreach (MethodInfo injectableMethod in injectableMethods)
			{
				Type[] requiredParameters = injectableMethod.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
				object[] resolvedInstances = requiredParameters.Select(Resolve).ToArray();
				if (resolvedInstances.Any(resolvedInstance => resolvedInstance == null))
				{
					throw new Exception($"Failed to inject {type.Name}.{injectableMethod}.Name");
				}

				injectableMethod.Invoke(instance, resolvedInstances);
#if UNITY_EDITOR
				Debug.Log($"Method Injected {type.Name}.{injectableMethod.Name}");
#endif
			}
		}

		protected object Resolve(Type type)
		{
			_registry.TryGetValue(type, out object resolvedInstance);
			return resolvedInstance;
		}

		protected void RegisterProvider(IDependencyProvider provider)
		{
			MethodInfo[] methods = provider.GetType().GetMethods(k_bindingFlags);

			foreach (MethodInfo method in methods)
			{
				if (!Attribute.IsDefined(method, typeof(ProvideAttribute)))
					continue;

				Type returnType = method.ReturnType;
				object provideInstance = method.Invoke(provider, null);

				if (provideInstance != null)
				{
					_registry.Add(returnType, provideInstance);
				}
				else
				{
					throw new Exception($"Provider {provider.GetType().Name} returned null for {returnType.Name}");
				}
			}
		}

		protected static bool IsInjectable(MonoBehaviour obj)
		{
			MemberInfo[] members = obj.GetType().GetMembers(k_bindingFlags);
			return members.Any(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
		}

		protected static MonoBehaviour[] FindMonoBehaviors()
		{
			return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID);
		}
	}
}