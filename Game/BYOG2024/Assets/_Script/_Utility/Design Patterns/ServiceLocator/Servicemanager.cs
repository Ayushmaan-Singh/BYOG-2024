using System;
using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility.DesignPattern.ServiceLocatorTool
{
	public sealed class ServiceManager
	{
		private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
		public IEnumerable<object> RegisteredServices => _services.Values;

		public ServiceManager()
		{
			
		}
		
		public bool TryGet<T>(out T service) where T : class
		{
			Type type = typeof(T);
			if (_services.TryGetValue(type,out object serviceObj))
			{
				service = serviceObj as T;
				return true;
			}
			
			service=null;
			return false;
		}
		
		public T Get<T>() where T : class
		{
			Type type = typeof(T);
			if (_services.TryGetValue(type, out object service))
			{
				return service as T;
			}

			throw new ArgumentException($"ServiceManager.Get: Service of type {type.FullName} not registered");
		}

		public ServiceManager Register<T>(T service)
		{
			Type type = typeof(T);
			if (!_services.TryAdd(type, service))
			{
				Debug.LogError($"ServiceManager.Register: Service of type{type.FullName} already registered");
			}

			return this;
		}

		public ServiceManager Register(Type type, object service)
		{
			if (!type.IsInstanceOfType(service))
				throw new ArgumentException($"Type of service does not match type of service interface {nameof(service)}");

			if (!_services.TryAdd(type, service))
				Debug.LogError($"ServiceManager.Register: Services of type {type.FullName} already registered");

			return this;
		}

		public ServiceManager Deregister<T>(T service)
		{
			Type type = typeof(T);
			if (!_services.Remove(type))
			{
				Debug.LogError($"ServiceManager.Deregister: Service of type{type.FullName} is not registered");
			}

			return this;
		}

		public bool TryDeregister<T>(T service)
		{
			return _services.Remove(typeof(T));
		}
	}
}