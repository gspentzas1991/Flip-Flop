using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public enum TileShape
    {
        Cursor = 0,
        Diamond = 1,
        Pentagon = 2,
        Hexagon = 3,
        Triangle = 4,
        Gem = 5,
        Star = 6
    }
    
    public enum Direction
    {
        Up=0,
        Down=1,
        Left=2,
        Right=3
    }

    public enum Character
    {
        Q=0,
        E=1
    }

    public enum GameMode
    {
        NormalGame = 0,
        FreePlay = 1
    }
}
