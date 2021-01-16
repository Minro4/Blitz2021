using System;
using System.Collections.Generic;
using static Blitz2021.Map;

namespace Blitz2021
{
    public static class Pathfinding
    {
        public static List<Position> path(Position p1, Position p2)
        {
            var pos = new List<Position>();
            var l = Math.Abs(p1.x - p2.x) + Math.Abs(p1.y - p2.y);
            for (int i = 0; i < l; i++)
            {
                pos.Add(new Position(0,0));
            }

            return pos;
        }
    }
}