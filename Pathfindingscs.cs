using System;
using System.Collections.Generic;
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
        public static void initialize(GameMessage gameMessage){
            if (pathFinder == null){
                var gridSize = new GridSize(columns : gameMessage.map.getMapSize(), rows : gameMessage.map.getMapSize());
                var cellSize = new Size(Distance.FromMeters(1), Distance.FromMeters(1));
                var traversalVelocity = Velocity.FromKilometersPerHour(100);

                grid = Grid.CreateGridWithLateralConnections(gridSize, cellSize, traversalVelocity);

                for ( int i = 0; i < gameMessage.map.getMapSize();i++){
                    for (int j= 0; j < gameMessage.map.getMapSize(); j++){
                        Map.Position pos = new Map.Position(i,j);
                        if (gameMessage.map.getTileTypeAt(pos) == TileType.WALL || MapManager.isInsideEnnemieBase(gameMessage,pos)){
                            grid.
                        }
                    }
                }
                pathFinder = new PathFinder();
            }
        }

        public static int path(Map.Position p1, Map.Position p2)
        {
            



            return 0;
        }
    }
}