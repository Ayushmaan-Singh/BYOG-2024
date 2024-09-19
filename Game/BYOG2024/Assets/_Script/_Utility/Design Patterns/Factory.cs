namespace AstekUtility.DesignPattern
{
    /// <summary>
    ///     Factory Pattern
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFactory<T>
	{
		public T CreateInstance();
	}

	public interface IFactory
	{
		public void CreateInstance();
	}
}