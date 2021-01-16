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

        public List<GameCommand.Action> mine(List<Position> mines){
            List<GameCommand.Action> actions = new List<GameCommand.Action>();
            foreach (Unit miner in miningMiners){
                actions.Add(new UnitAction(UnitActionType.MINE, miner.id, getAdjacentMine(mines)));
            }
            return actions;
        }
        public List<GameCommand.Action> getActions(Blitz2020.GameMessage gameMessage){
            List<Unit> availableMiners = getAvailableMiners( gameMessage);
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

        public List<Unit> getMiningMiners(){
            return miningMiners;
        }
        
        private List<Unit> getAvailableMiners(Blitz2020.GameMessage gameMessage){
            List<Unit> availableMiners = new List<Unit>();
            List<Unit> nextMiners = new List<Unit>();
            foreach (Unit miner in miners){
                if (miner.isMoving){
                    if (miner.path.Count == 0){
                        miningMiners.Add(miner);
                    }
                    else if(miner.path.Last().isOccupied(gameMessage)) {
                        nextMiners.Add(miner);
                        availableMiners.Add(miner);
                    }
                }
                else{
                    nextMiners.Add(miner);
                    availableMiners.Add(miner);
                }
            }
            miners = nextMiners;
            return availableMiners;
        }
        private Position getAdjacentMine(List<Position> mines){
            return new Position(0,0);
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
        private List<Unit> miningMiners;
        private List<Position> availableMiningSpots;

    }
}