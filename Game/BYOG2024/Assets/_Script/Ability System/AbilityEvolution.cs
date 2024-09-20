using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Entity.Abilities
{
	public class AbilityEvolution : SerializedScriptableObject
	{
		[OdinSerialize, InlineProperty] private Dictionary<AbilityBase, AbilityBase> abilityEvolutionTable
			= new Dictionary<AbilityBase, AbilityBase>();

		public AbilityBase NextEvolution(AbilityBase ability)
		{
			return abilityEvolutionTable.GetValueOrDefault(ability);
		}
	}
}