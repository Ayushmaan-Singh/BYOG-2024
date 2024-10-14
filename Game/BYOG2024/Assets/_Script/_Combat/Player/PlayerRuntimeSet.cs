using System.Collections.Generic;
using Entity.Player;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Combat.Player
{
	public class PlayerRuntimeSet : ScriptableObject
	{
		[ShowInInspector, EnableIf("@false")] private readonly List<PlayerMediator> _players = new List<PlayerMediator>();

		[ShowInInspector, EnableIf("@false")] private PlayerMediator _owner;

		public PlayerMediator Owner => _owner;
		public List<PlayerMediator> PlayersInGame => _players;

		public PlayerRuntimeSet RegisterPlayer(PlayerMediator mediator)
		{
			if (!_players.Contains(mediator))
				_players.Add(mediator);

			return this;
		}

		public PlayerRuntimeSet DeregisterPlayer(PlayerMediator mediator)
		{
			if (_players.Contains(mediator))
				_players.Remove(mediator);

			return this;
		}

		public PlayerRuntimeSet SetOwner(PlayerMediator mediator)
		{
			if (!_players.Contains(mediator) && mediator != null)
				_players.Add(mediator);

			_owner = mediator;
			return this;
		}

	}
}