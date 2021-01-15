using System;

namespace Blitz2021
{
    public class Map
    {
        public class Position
        {
            public Position(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
            public int x;
            public int y;

            public override string ToString()
            {
                return string.Format("P({0},{1})", this.x, this.y);
            }
        }

        public class Depot
        {
            public Position position;
            public int blitzium;
        }

        public enum TileType
        {
            EMPTY, WALL, BASE, MINE
        }

        public String[][] tiles;
        public Depot[] depots;

        public class PointOutOfMapException : Exception
        {
            public PointOutOfMapException(Position position, int size)
            : base(String.Format("Point {0} is out of map, x and y must be greater than 0 and less than {1}.", position, size))
            { }
        }

        public int getMapSize()
        {
            return this.tiles.Length;
        }

        public TileType getTileTypeAt(Position position)
        {
            string rawTile = this.getRawTileValueAt(position);

            switch (rawTile)
            {
                case "EMPTY":
                    return TileType.EMPTY;
                case "WALL":
                    return TileType.WALL;
                case "MINE":
                    return TileType.MINE;
                case "BASE":
                    return TileType.BASE;
                default:
                    throw new ArgumentException(String.Format("'{0}' is not a valid tile", rawTile));
            }
        }

        public String getRawTileValueAt(Position position)
        {
            this.validateTileExists(position);
            return this.tiles[position.x][position.y];
        }

        public void validateTileExists(Position position)
        {
            if (position.x < 0 || position.y < 0 || position.x >= this.getMapSize() || position.y >= this.getMapSize())
            {
                throw new PointOutOfMapException(position, this.getMapSize());
            }
        }
    }
}