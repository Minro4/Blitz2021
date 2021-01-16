using System;
using System.Collections.Generic;
using System.Linq;
using static Blitz2021.Map;
using Roy_T.AStar.Graphs;
using Roy_T.AStar.Primitives;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Paths;
using Blitz2020;

namespace Blitz2021
{
    public static class Pathfinding
    {
        private static PathFinder pathFinder;
        private static Grid grid;

        public static int infinity = 99999;

        private readonly static Velocity traversalVelocity = Velocity.FromKilometersPerHour(100);

        public static void initialize(GameMessage gameMessage)
        {
            var gridSize = new GridSize(columns: gameMessage.map.getMapSize(), rows: gameMessage.map.getMapSize());
            var cellSize = new Size(Distance.FromMeters(1), Distance.FromMeters(1));


            grid = Grid.CreateGridWithLateralConnections(gridSize, cellSize, traversalVelocity);

            for (int i = 0; i < gameMessage.map.getMapSize(); i++)
            {
                for (int j = 0; j < gameMessage.map.getMapSize(); j++)
                {
                    Map.Position pos = new Map.Position(i, j);
                    if (gameMessage.map.getTileTypeAt(pos) == TileType.WALL || MapManager.isInsideEnnemieBase(gameMessage, pos) || pos.isOccupied(gameMessage))
                    {
                        grid.DisconnectNode(new GridPosition(i, j));
                    }
                }
            }

            pathFinder = new PathFinder();
        }

        public static void addBlocker(Map.Position pos)
        {
            grid.DisconnectNode(new GridPosition(pos.x, pos.y));
        }

        public static Tuple<Map.Position, int> bestPos(GameMessage message, Map.Position basePos, Map.Position miner)
        {
            var positions = MapManager.getMineableTileNotOccupied(message, miner);
            Tuple<Map.Position, int> tuples = new Tuple<Map.Position, int>(null, infinity);
            foreach (Map.Position p in positions)
            {
                var dist = path(basePos, p);
                if (dist < tuples.Item2)
                {
                    tuples = new Tuple<Map.Position, int>(p, dist);
                }
            }

            if (tuples.Item2 == infinity)
            {
                return null;
            }

            return tuples;
        }

        public static int path(Map.Position p1, Map.Position p2)
        {
            addNode(p1);
            var path = pathFinder.FindPath(new GridPosition(p1.x, p1.y), new GridPosition(p2.x, p2.y), grid);

            if (path.Type == PathType.Complete)
            {
                return (int) path.Distance.Meters;
            }
            grid.DisconnectNode(new GridPosition(p1.x, p1.y));

            return infinity;
        }

        public static void addNode(Map.Position position)
        {
            grid.AddEdge(new GridPosition(position.x, position.y), new GridPosition(position.x + 1, position.y), traversalVelocity);
            grid.AddEdge(new GridPosition(position.x, position.y), new GridPosition(position.x - 1, position.y), traversalVelocity);
            grid.AddEdge(new GridPosition(position.x, position.y), new GridPosition(position.x, position.y + 1), traversalVelocity);
            grid.AddEdge(new GridPosition(position.x, position.y), new GridPosition(position.x, position.y - 1), traversalVelocity);
        }


        public static bool isAnyPathAvailable(Map.Position currentPosition, List<Map.Position> availableTile) 
        {

            for (int x=0; x < availableTile.Count; x++) 
            {
                if (path(currentPosition, availableTile[x]) < 1000) 
                {
                    return true;
                }
            }

            return false;
        }




    }
}