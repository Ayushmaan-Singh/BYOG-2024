using UnityEngine;

namespace AstekUtility.DesignPattern.MediatorPattern
{
	/// <summary>
	/// Concrete implementation of this required top make payload of data to be sent
	/// </summary>
	/// <typeparam name="TData"></typeparam>
	public abstract class Payload<TData>:IVisitor
	{
		public abstract TData Content { get; set; }

		public abstract void Visit<T>(T visitable) where T : Component, IVisitable;
	}
}