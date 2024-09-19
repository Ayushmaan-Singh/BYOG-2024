using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AstekUtility.DesignPattern.MediatorPattern
{
	public abstract class Mediator<T> : MonoBehaviour where T : Component, IVisitable
	{
		private readonly List<T> _entities = new List<T>();

		public void Register(T entity)
		{
			if (!_entities.Contains(entity))
			{
				_entities.Add(entity);
				OnRegistered(entity);
			}
		}
		protected virtual void OnRegistered(T entity)
		{
			//noap
		}

		public void Deregister(T entity)
		{
			if (_entities.Contains(entity))
			{
				_entities.Remove(entity);
				OnDeregistered(entity);
			}
		}
		protected virtual void OnDeregistered(T entity)
		{
			//noap
		}

		/// <summary>
		/// Send information to one entity
		/// </summary>
		public void Message(T source, T target, IVisitor message)
		{
			_entities.FirstOrDefault(entity => entity.Equals(target))?.Accept(message);
		}

		/// <summary>
		/// Send information to all entity based on a predicate condition
		/// </summary>
		public void Broadcast(T source, IVisitor message, Func<T, bool> predicate = null)
		{
			_entities.Where(target => source != target & SenderConditionMet(target, predicate) && MediatorConditionMet(target))
				.ForEach(target => target.Accept(message));
		}

		private bool SenderConditionMet(T target, Func<T, bool> predicate) => predicate == null || predicate(target);
		
		/// <summary>
		/// Concrete implementation of this should define any condition for mediator to meet before sending this information
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		protected abstract bool MediatorConditionMet(T target);
	}
}