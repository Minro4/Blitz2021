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
        private MinerManager minerMan;
        private MapManager mapManager;
        private BaseManager baseManager;
        private CartManager cartManager;
        private OutlawManager outlawManager;
        bool firstPass = false;

        public Bot()
        {
            mapManager = new MapManager();
            minerMan = new MinerManager();
            baseManager = new BaseManager(mapManager, minerMan);
            cartManager = new CartManager();
            outlawManager = new OutlawManager();
        }

        /*
        * Here is where the magic happens, for now the moves are random. I bet you can do better ;)
        *
        * No path finding is required, you can simply send a destination per unit and the game will move your unit towards
        * it in the next turns.
        */
        public GameCommand nextMove(GameMessage gameMessage)
        {
            try
            {
                Pathfinding.initialize(gameMessage);
                List<GameCommand.Action> actions = new List<GameCommand.Action>();
                Crew myCrew = gameMessage.getCrewsMapById[gameMessage.crewId];
                int mapSize = gameMessage.map.getMapSize();


                mapManager.getAllMine(gameMessage.map);

                minerMan.setMiners(myCrew.units.Where(Unit => Unit.type == 0).ToList());
                minerMan.setAvailableMiningSpots(mapManager.getAllMineNotOccupied(gameMessage));

                actions.AddRange(minerMan.getActions(gameMessage, mapManager.Mines.Select(mine => mine.Mines).ToList()));

                actions.AddRange(baseManager.update(gameMessage));
                actions.AddRange(cartManager.updateCart(gameMessage, minerMan, mapManager));
                actions.AddRange(outlawManager.updateOutlaw(gameMessage, minerMan, mapManager, baseManager.timeToKill(gameMessage)));
                return new GameCommand(actions);
            }
            catch (Exception ex)
            {
                Console.WriteLine("throw: " + ex);
                Console.WriteLine("Trace: " + ex.StackTrace);
                return new GameCommand(new List<GameCommand.Action>());
            }
        }

        public Position getRandomPosition(int size)
        {
            Random rand = new Random();
            return new Position(rand.Next(size), rand.Next(size));
        }
    }
}