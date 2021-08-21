using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public static class DirectionCalculator
    {
        public static int Up = 1;
        public static int Down = -1;
        public static int Right = 1;
        public static int Left = -1;

        public static Vector2Int Calculate(Vector2Int position, Direction direction, float puzzleRotation)
        {
            var targetPosition = new Vector2Int();
            if (puzzleRotation == 0 || puzzleRotation == 360)
            {
                switch (direction)
                {
                    case Direction.Down:
                        targetPosition.x = position.x;
                        targetPosition.y = position.y - 1;
                        break;
                    case Direction.Up:
                        targetPosition.x = position.x;
                        targetPosition.y = position.y + 1;
                        break;
                }
            }
            else if (puzzleRotation == 180)
            {
                switch (direction)
                {
                    case Direction.Down:
                        targetPosition.x = position.x;
                        targetPosition.y = position.y + 1;
                        break;
                    case Direction.Up:
                        targetPosition.x = position.x;
                        targetPosition.y = position.y - 1;
                        break;
                }
            }
            else if (puzzleRotation == 90)
            {
                switch (direction)
                {
                    case Direction.Down:
                        targetPosition.x = position.x-1;
                        targetPosition.y = position.y;
                        break;
                    case Direction.Up:
                        targetPosition.x = position.x + 1;
                        targetPosition.y = position.y;
                        break;
                }
            }
            else if (puzzleRotation == 270)
            {
                switch (direction)
                {
                    case Direction.Down:
                        targetPosition.x = position.x + 1;
                        targetPosition.y = position.y;
                        break;
                    case Direction.Up:
                        targetPosition.x = position.x - 1;
                        targetPosition.y = position.y;
                        break;
                }
            }
            return targetPosition;
        }
    }
}
