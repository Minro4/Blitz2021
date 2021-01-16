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
            miningMiners = new List<Unit>();
        }

        
        public List<GameCommand.Action> getActions(Blitz2020.GameMessage gameMessage,List<Position> mines){
            List<GameCommand.Action> actions = new List<GameCommand.Action>();
            List<Unit> movingMiners = new List<Unit>();
            foreach (Unit miner in miners){
                if (miner.position.Equals(miner.target)){
                    miningMiners.Add(miner);
                }
                else{
                    movingMiners.Add(miner);
                }
            }
            miners = movingMiners;
            actions.AddRange(mine(mines));
            foreach (Unit miner in miners){
                if (!miner.isMoving){
                    miner.target = availableMiningSpots[getClosestSpotId(miner)];
                    miner.isMoving = true;
                }
                actions.Add(new UnitAction(UnitActionType.MOVE, miner.id,miner.target));
            }
            return actions;
        }
        public void setMiners(List<Unit> newMiners){
            foreach (Unit miner in newMiners){
                int i = miners.FindIndex(mine=>mine.id == miner.id);
                int j = miningMiners.FindIndex(mine=>mine.id == miner.id);
                if (i != -1){
                    miners[i].position = miner.position;
                }
                else if ( j != -1) {
                    miningMiners[j].blitzium = miner.blitzium;
                }
                else{
                    miner.isMoving = false;
                    miners.Add(miner);
                }
            }
        }
        
        public void setAvailableMiningSpots(List<Position> possibleSpots){
            availableMiningSpots = possibleSpots;
        }

        public List<Unit> getMiningMiners(){
            return miningMiners;
        }
        
        private Position getAdjacentMine(List<Position> mines,Unit miner){
            foreach (Position mine in mines){
                if (Math.Abs(miner.position.x - mine.x ) + Math.Abs(miner.position.y - mine.y ) == 1){
                    return mine;
                }
            }
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
        private List<GameCommand.Action> mine(List<Position> mines){
            
            List<GameCommand.Action> actions = new List<GameCommand.Action>();
            
            if (miningMiners.Count > 0){
                foreach (Unit miner in miningMiners){
                    if (miner.blitzium < 50){
                        actions.Add(new UnitAction(UnitActionType.MINE, miner.id, getAdjacentMine(mines,miner)));
                    }
                }
            }
            return actions;
        }

        private List<Unit> miners;
        private List<Unit> miningMiners;
        private List<Position> availableMiningSpots;

    }
}