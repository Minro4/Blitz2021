﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blitz2020;
using Blitz2021;
using static Blitz2021.GameCommand;
using static Blitz2021.GameCommand.UnitAction;
using static Blitz2021.Map;


public class MapManager
{
    public List<Mine> Mines;

    public MapManager()
    {
        Mines = new List<Mine>();
    }


    public List<Mine> getAllMine(Map map)
    {
        int mapSize = map.getMapSize();
        List<Mine> Mines = new List<Mine>();
        for (int x = 0; x < mapSize; x++)
        for (int y = 0; y < mapSize; y++)
        {
            Position position = new Position(x, y);
            if (map.getTileTypeAt(position) == TileType.MINE)
            {
                Mines.Add(new Mine(x, y, getMineableTile(map, position)));
            }
        }

        this.Mines = Mines;
        return Mines;
    }

    public List<Position> getAllMineNotOccupied(GameMessage message)
    {
        var map = message.map;
        int mapSize = map.getMapSize();
        List<Position> pos = new List<Position>();
        for (int x = 0; x < mapSize; x++)
        for (int y = 0; y < mapSize; y++)
        {
            Position position = new Position(x, y);
            if (map.getTileTypeAt(position) == TileType.MINE)
            {
                var mineableTiles = getMineableTile(map, position);
                var unocc = mineableTiles.Where((mineableTile) => !mineableTile.isOccupied(message));
                pos.AddRange(unocc);
            }
        }

        return pos;
    }

    public static List<Position> getMineableTileNotOccupied(GameMessage message, Position P) 
    {
        List<Position> tiles = getMineableTile(message.map, P);
        List<Position> freeTiles = new List<Position>();

        for (int x = 0; x < tiles.Count; x++)
        {
            if (!tiles[x].isOccupied(message) && !isInsideEnnemieBase(message,tiles[x]))
            {
                freeTiles.Add(tiles[x]);
            }
        }

        return freeTiles;
    }

    public static bool isInsideEnnemieBase(GameMessage message, Position position)
    {
        var ennemieCrews = message.crews.Where(c => c.id != message.crewId).ToList();
        return ennemieCrews.Find((crew) => isInRange(position, crew.homeBase, 7)) != null;
    }

    private static bool isInRange(Position p1, Position p2, int range)
    {
        return p1.x > p2.x - range / 2 && p1.x < p2.x + range / 2 && p1.y > p2.y - range / 2 && p1.y < p2.y + range / 2;
    }
    
    public static List<Position> getMineableTile(Map map, Position P) 
    {
        int mapSize = map.getMapSize();
        List<Position> adjasentTile = new List<Position>();

        if (P.x + 1 < mapSize)
        {
            if (testCell(map, P.x + 1, P.y))
                adjasentTile.Add(new Position(P.x + 1, P.y));
        }

        if (P.x - 1 > 0)
        {
            if (testCell(map, P.x - 1, P.y))
                adjasentTile.Add(new Position(P.x - 1, P.y));
        }

        if (P.y + 1 < mapSize)
        {
            if (testCell(map, P.x, P.y + 1))
                adjasentTile.Add(new Position(P.x, P.y + 1));
        }

        if (P.y - 1 > 0)
        {
            if (testCell(map, P.x, P.y - 1))
                adjasentTile.Add(new Position(P.x, P.y - 1));
        }

        return adjasentTile;
    }

    private static bool testCell(Map map, int x, int y)
    {
        Position P = new Position(x, y);

        return isMineable(map.getTileTypeAt(P));
    }


    private static bool isMineable(TileType tileType)
    {
        switch (tileType)
        {
            case TileType.EMPTY:
                return true;
            case TileType.WALL:
                return false;
            case TileType.MINE:
                return false;
            case TileType.BASE:
                return false;
            default:
                return false;
        }
    }
}

public class Mine
{
    public Position Mines;
    public List<Position> Mineable;

    public Mine(int x, int y, List<Position> Mineable)
    {
        this.Mines = new Position(x, y);
        this.Mineable = Mineable;
    }
}
