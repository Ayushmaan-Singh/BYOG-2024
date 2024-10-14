using System.Collections.Generic;
namespace AstekUtility.Observer
{
	namespace Managed
	{
		public interface ISubject { }

		public class ManagedSubject<T> : ISubject
		{
			public T Data { get; private set; }

			public void Notify(T newValue)
			{
				Data = newValue;
				ObserverManager.Instance.NotifyObservers(GetType());
			}
		}
	}

	namespace Unmanaged
	{
		public interface ISubject<T>
		{
			T Data { get; }

			void Attach(params IObserver<T>[] observer);
			void Detach(params IObserver<T>[] observer);
			void Notify(T changedValue);
		}

		public class UnmanagedSubject<T> : ISubject<T>
		{
			private readonly List<IObserver<T>> _observers = new List<IObserver<T>>();
			private T _data;
			
			public T Data=>_data;

			public void Attach(params IObserver<T>[] observer)
			{
				foreach (IObserver<T> obs in observer)
				{
					if (!_observers.Contains(obs))
					{
						_observers.Add(obs);
					}
				}
			}

			public void Detach(params IObserver<T>[] observer)
			{
				foreach (IObserver<T> obs in observer)
				{
					if (_observers.Contains(obs))
					{
						_observers.Remove(obs);
					}
				}
			}

			public void Notify(T changedValue)
			{
				_data = changedValue;
				foreach (IObserver<T> obs in _observers)
				{
					obs.OnNotify(this);
				}
			}

			public void DetachAll()
			{
				_observers.Clear();
			}
		}
	}

}