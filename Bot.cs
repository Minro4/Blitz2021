using System;
using System.Collections.Generic;
using System.Linq;
using Blitz2021;
using static Blitz2021.GameCommand;
using static Blitz2021.GameCommand.UnitAction;
using static Blitz2021.Map;

namespace Blitz2020
{
    public class Bot
    {
        public static string NAME = "MyBot C#";

        public Bot()
        {
            // initialize some variables you will need throughout the game here
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


            List<GameCommand.Action> actions = myCrew.units
                .Select(c => new UnitAction(UnitActionType.MOVE, c.id, this.getRandomPosition(mapSize)))
                .ToList<GameCommand.Action>();

            return new GameCommand(actions);
        }

        public Position getRandomPosition(int size)
        {
            Random rand = new Random();
            return new Position(rand.Next(size), rand.Next(size));
        }
    }
}