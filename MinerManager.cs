using System;
using System.Collections.Generic;
using System.Linq;
using Blitz2021;
using static Blitz2021.GameCommand;
using static Blitz2021.GameCommand.UnitAction;
using static Blitz2021.Map;

namespace Blitz2021
{
    public class MinerManager
    {
        public MinerManager()
        {
            miners = new List<Unit>();
        }

        public List<GameCommand.Action> getActions(){
            List<Unit> availableMiners = getAvailableMiners();
            List<GameCommand.Action> actions = new List<GameCommand.Action>();

            foreach (Unit miner in availableMiners){
                actions.Add(new UnitAction(UnitActionType.MOVE, miner.id, availableMiningSpots[getClosestSpotId(miner)]));
            }
            return actions;
        }

        public void addMiner(Unit newMiner){
            miners.Add(newMiner);
        }

        public void addMiners(List<Unit> newMiners){
            miners.AddRange(newMiners);
        }
        
        public void setAvailableMiningSpots(List<Position> possibleSpots){
            availableMiningSpots = possibleSpots;
        }
        
        private List<Unit> getAvailableMiners(){
            List<Unit> availableMiners = new List<Unit>();

            foreach (Unit miner in miners){
                if (miner.isOccupied){
                    //if (miner.path.Last() ){
                    //    availableMiners.Add(miner);
                    //}
                }
                else{
                    availableMiners.Add(miner);
                }
            }
            return availableMiners;
        }

        private int getClosestSpotId(Unit miner){       
            int id = 0;
            int i = 0;
            int minimum = 99999;
            foreach (Position spot in availableMiningSpots){
                int dist = Pathfinding.path(miner.position,spot).Count;
                if (dist < minimum){
                    id = i;
                    minimum = dist;
                }
                i++;
            } 
            return id;
        }


        private List<Unit> miners;
        private List<Position> availableMiningSpots;

    }
}