using UnityEngine;
namespace AstekUtility.CardinalDirection
{

	public enum Direction8Sides
	{
		North = 0,
		South = 1,
		West = 2,
		East = 3,
		NorthEast = 4,
		NorthWest = 5,
		SouthEast = 6,
		SouthWest = 7
	}

	public static class DirectionHelper8Sides
	{
		public static Direction8Sides GetDirectionOppositeTo(this Direction8Sides direction)
		{
			switch (direction)
			{
				case Direction8Sides.North:
					return Direction8Sides.South;

				case Direction8Sides.South:
					return Direction8Sides.North;

				case Direction8Sides.West:
					return Direction8Sides.East;

				case Direction8Sides.East:
					return Direction8Sides.West;

				case Direction8Sides.NorthEast:
					return Direction8Sides.SouthWest;

				case Direction8Sides.NorthWest:
					return Direction8Sides.SouthEast;

				case Direction8Sides.SouthEast:
					return Direction8Sides.NorthWest;

				case Direction8Sides.SouthWest:
					return Direction8Sides.NorthEast;

				default:
					return direction;
			}
		}

		//returns the direction of target position relative to initial position
		public static Direction8Sides GetDirectionOnTargetPos(Vector2Int initialPos, Vector2Int targetPos)
		{
			if (initialPos.x - targetPos.x > 0 && initialPos.y == targetPos.y)
			{
				return Direction8Sides.West;
			}
			if (initialPos.x - targetPos.x < 0 && initialPos.y == targetPos.y)
			{
				return Direction8Sides.East;
			}
			if (initialPos.y - targetPos.y > 0 && initialPos.x == targetPos.x)
			{
				return Direction8Sides.South;
			}
			if (initialPos.y - targetPos.y < 0 && initialPos.x == targetPos.x)
			{
				return Direction8Sides.North;
			}
			if (initialPos.x - targetPos.x < 0 && initialPos.y - targetPos.y < 0 && Mathf.Abs(initialPos.x - targetPos.x) == Mathf.Abs(initialPos.y - targetPos.y))
			{
				return Direction8Sides.NorthEast;
			}
			if (initialPos.x - targetPos.x < 0 && initialPos.y - targetPos.y > 0 && Mathf.Abs(initialPos.x - targetPos.x) == Mathf.Abs(initialPos.y - targetPos.y))
			{
				return Direction8Sides.SouthEast;
			}
			if (initialPos.x - targetPos.x > 0 && initialPos.y - targetPos.y > 0 && Mathf.Abs(initialPos.x - targetPos.x) == Mathf.Abs(initialPos.y - targetPos.y))
			{
				return Direction8Sides.SouthWest;
			}
			if (initialPos.x - targetPos.x > 0 && initialPos.y - targetPos.y < 0 && Mathf.Abs(initialPos.x - targetPos.x) == Mathf.Abs(initialPos.y - targetPos.y))
			{
				return Direction8Sides.NorthWest;
			}
			Debug.LogError("Invalid Direction");
			return (Direction8Sides)10;
		}

		public static Vector2Int GetVectorinDirection(Vector2Int initialPos, Vector2Int targetPos)
		{
			Direction8Sides dir = GetDirectionOnTargetPos(initialPos, targetPos);

			switch (dir)
			{
				case Direction8Sides.North:
					return new Vector2Int(0, 1);

				case Direction8Sides.South:
					return new Vector2Int(0, -1);

				case Direction8Sides.West:
					return new Vector2Int(-1, 0);

				case Direction8Sides.East:
					return new Vector2Int(1, 0);

				case Direction8Sides.NorthEast:
					return new Vector2Int(1, 1);

				case Direction8Sides.NorthWest:
					return new Vector2Int(-1, 1);

				case Direction8Sides.SouthEast:
					return new Vector2Int(1, -1);

				case Direction8Sides.SouthWest:
					return new Vector2Int(-1, -1);

				default:
					return new Vector2Int(-1000, -1000);
			}
		}

		public static Vector2Int GetVectorinDirection(Direction8Sides dir)
		{
			switch (dir)
			{
				case Direction8Sides.North:
					return new Vector2Int(0, 1);

				case Direction8Sides.South:
					return new Vector2Int(0, -1);

				case Direction8Sides.West:
					return new Vector2Int(-1, 0);

				case Direction8Sides.East:
					return new Vector2Int(1, 0);

				case Direction8Sides.NorthEast:
					return new Vector2Int(1, 1);

				case Direction8Sides.NorthWest:
					return new Vector2Int(-1, 1);

				case Direction8Sides.SouthEast:
					return new Vector2Int(1, -1);

				case Direction8Sides.SouthWest:
					return new Vector2Int(-1, -1);

				default:
					return new Vector2Int(-1000, -1000);
			}
		}

		public static Direction8Sides MapInputTo8Direction(Vector2 input)
		{
			if (input.x > 0 && input.y == 0)
			{
				return Direction8Sides.East;
			}
			if (input.x < 0 && input.y == 0)
			{
				return Direction8Sides.West;
			}
			if (input.y > 0 && input.x == 0)
			{
				return Direction8Sides.North;
			}
			if (input.y < 0 && input.x == 0)
			{
				return Direction8Sides.South;
			}
			if (input.x > 0 && input.y > 0)
			{
				return Direction8Sides.NorthEast;
			}
			if (input.x < 0 && input.y > 0)
			{
				return Direction8Sides.NorthWest;
			}
			if (input.x > 0 && input.y < 0)
			{
				return Direction8Sides.SouthEast;
			}
			if (input.x < 0 && input.y < 0)
			{
				return Direction8Sides.SouthWest;
			}
			Debug.LogError("Invalid Direction");
			return (Direction8Sides)10;
		}
	}
}