using System.Collections.Generic;
using System.Linq;
using Blitz2021;

namespace Blitz2020
{
    public class Rules {
        public int MAX_MINER_CARGO;
        public int MAX_CART_CARGO;
        public int MAX_MINER_MOVE_CARGO;
    }

    public class GameMessage
    {
        public int tick;

        public int totalTick;

        public string crewId;
        public List<Crew> crews;

        public Map map;

        public Rules rules;

        public Dictionary<string, Crew> getCrewsMapById
        {
            get { return this.crews.ToDictionary(p => p.id, p => p); }
        }
    }
}