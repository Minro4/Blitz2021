using System;
using System.Collections.Generic;
using System.Linq;
using Blitz2021;
using static Blitz2021.GameCommand;
using static Blitz2021.GameCommand.UnitAction;
using static Blitz2021.Map;
using static MapManager;
using static Blitz2021.MinerManager;


namespace Blitz2020
{
    public class Bot
    {
        public static string NAME = "GUY";
        MinerManager minerMan;
        bool firstPass = false;
        public Bot()
        {
            minerMan = new MinerManager();
        }

        /*
        * Here is where the magic happens, for now the moves are random. I bet you can do better ;)
        *
        * No path finding is required, you can simply send a destination per unit and the game will move your unit towards
        * it in the next turns.
        */
        public GameCommand nextMove(GameMessage gameMessage)
        {
            Crew myCrew = gameMessage.getCrewsMapById[gameMessage.crewId];
            int mapSize = gameMessage.map.getMapSize();

            MapManager mapManager = new MapManager();
            mapManager.getAllMine(gameMessage.map);

            if (!firstPass){
                minerMan.addMiner(myCrew.units[0]);
                firstPass = true;
            }
            minerMan.setAvailableMiningSpots(mapManager.Mines.SelectMany((mine => mine.Mineable)).ToList());

            List<GameCommand.Action> actions = minerMan.getActions(gameMessage);

            var baseManager = new BaseManager();
            baseManager.buy(actions, gameMessage);

            return new GameCommand(actions);
        }

        public Position getRandomPosition(int size)
        {
            Random rand = new Random();
            return new Position(rand.Next(size), rand.Next(size));
        }
    }
}