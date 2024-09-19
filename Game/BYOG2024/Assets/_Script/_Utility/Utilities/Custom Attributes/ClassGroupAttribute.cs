using System;
namespace AstekUtility.CustomAttribute
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class ClassGroupAttribute : Attribute
	{

        /// <summary>
        /// </summary>
        /// <param name="classGroupName">Name of group this class belongs to</param>
        public ClassGroupAttribute(string classGroupName)
		{
			ClassGroup = classGroupName;
		}

        /// <summary>
        /// </summary>
        /// <param name="classGroup">Name of group this class belongs to</param>
        /// <param name="description">Description of this class group</param>
        public ClassGroupAttribute(string classGroup, string description) : this(classGroup)
		{
			Description = description;
		}
		public string ClassGroup { get; }
		public string Description { get; }
	}
}