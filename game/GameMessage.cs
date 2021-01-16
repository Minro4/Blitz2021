using System.Collections.Generic;
using System.Linq;
using Blitz2021;
using static Blitz2021.GameCommand;
using static Blitz2021.GameCommand.UnitAction;
using static Blitz2021.Map;


namespace Blitz2020
{
    public class Rules
    {
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

        public Crew getMyCrew()
        {
            return getCrewById(crewId);
        }

        public Crew getCrewById(string id)
        {
            return crews.Find((crew) => id.Equals(crew.id));
        }


        public Crew getBestCrew() 
        {
            Crew bestOtherCrew = null;
            bool start = true;
            for (int x = 0; x < crews.Count; x++)
            {
                if (crews[x].id != crewId)
                {
                    if (start)
                    {
                        bestOtherCrew = crews[x];
                        start = false;
                    }

                    if (bestOtherCrew.totalBlitzium < crews[x].totalBlitzium)
                    {
                        bestOtherCrew = crews[x];
                    }
                }
            }

            return bestOtherCrew;
        }

        public List<Position> getEnemieMiner() 
        {
            List<Position> Enemieminers = new List<Position>();
            Crew bestCrew = getBestCrew();

            if(bestCrew != null)
            {
                List<Unit> miner = bestCrew.get(Unit.UnitType.MINER);
                for (int x = 0; x < miner.Count; x++)
                {
                    Enemieminers.Add(miner[x].position);
                }
            }
            
            return Enemieminers;

        }

    }
}