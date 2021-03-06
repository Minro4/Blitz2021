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
            minerFollower = new Unit();
            nextIsFollower = false;
        }


        public List<GameCommand.Action> getActions(Blitz2020.GameMessage gameMessage, List<Position> mines)
        {
            try
            {
                List<GameCommand.Action> actions = new List<GameCommand.Action>();
                List<Unit> movingMiners = new List<Unit>();
                foreach (Unit miner in miners)
                {
                    if (miner.position.Equals(miner.target))
                    {
                        miningMiners.Add(miner);
                    }
                    else
                    {
                        movingMiners.Add(miner);
                    }
                }

                miners = movingMiners;
                actions.AddRange(mine(mines));
                foreach (Unit miner in miners)
                {
                    if (!miner.isMoving)
                    {
                        if (availableMiningSpots.Count > 0 && miner.inactivity < 2)
                        {
                            miner.target = availableMiningSpots[getClosestSpotId(availableMiningSpots, miner)];
                            miner.isMoving = true;
                        }
                        else
                        {
                            var outlaws = gameMessage.getMyCrew().get(Unit.UnitType.OUTLAW);
                            if (outlaws.Count > 0)
                            {
                                var outlaw = outlaws[0];
                                var notOcc = MapManager.getMineableTileNotOccupied(gameMessage, outlaw.position);
                                if (notOcc.Count > 0)
                                {
                                    miner.target = notOcc[0];
                                }
                                else
                                {
                                    miner.target = Blitz2020.Bot.getRandomPosition(gameMessage.map.getMapSize());
                                    miner.inactivity = 0;
                                }
                            }
                            else
                            {
                                miner.target = Blitz2020.Bot.getRandomPosition(gameMessage.map.getMapSize());
                                miner.inactivity = 0;
                            }
                        }
                    }

                    actions.Add(new UnitAction(UnitActionType.MOVE, miner.id, miner.target));
                }

                return actions;
            }
            catch (Exception ex)
            {
                Console.WriteLine("throw: " + ex);
                Console.WriteLine("Trace: " + ex.StackTrace);
                return new List<GameCommand.Action>();
            }
        }

        public void setMiners(List<Unit> newMiners)
        {
            foreach (Unit miner in newMiners)
            {
                int i = miners.FindIndex(mine => mine.id == miner.id);
                int j = miningMiners.FindIndex(mine => mine.id == miner.id);
                if (i != -1)
                {
                    if (miners[i].position.Equals(miner.position))
                    {
                        if (miners[i].inactivity > 2)
                        {
                            miners[i].isMoving = false;
                        }
                        else
                        {
                            miners[i].inactivity++;
                        }
                    }
                    else
                    {
                        miners[i].position = miner.position;
                        miners[i].inactivity = 0;
                    }
                }
                else if (j != -1)
                {
                    miningMiners[j].blitzium = miner.blitzium;
                }
                else
                {
                    miner.isMoving = false;
                    miner.inactivity = 0;
                    miners.Add(miner);
                }
            }

            miners = miners.Where(miner => (newMiners.Find(newMiner => newMiner.id == miner.id) != null)).ToList();
            miningMiners = miningMiners.Where(miner => (newMiners.Find(newMiner => newMiner.id == miner.id) != null)).ToList();
        }

        public void setAvailableMiningSpots(List<Position> possibleSpots)
        {
            availableMiningSpots = possibleSpots;
        }

        public List<Unit> getMiningMiners()
        {
            return miningMiners;
        }

        public List<Unit> getMovingMiners()
        {
            return miners;
        }

        private Position getAdjacentMine(List<Position> mines, Unit miner)
        {
            foreach (Position mine in mines)
            {
                if (Math.Abs(miner.position.x - mine.x) + Math.Abs(miner.position.y - mine.y) == 1)
                {
                    return mine;
                }
            }

            return new Position(0, 0);
        }

        private int getClosestSpotId(List<Position> list, Unit miner)
        {
            int id = 0;
            int i = 0;
            float minimum = 99999;
            foreach (Position spot in list)
            {
                float dist = Pathfinding.path(miner.position, spot);
                if (dist < minimum)
                {
                    id = i;
                    minimum = dist;
                }

                i++;
            }

            return id;
        }

        private List<GameCommand.Action> mine(List<Position> mines)
        {
            List<GameCommand.Action> actions = new List<GameCommand.Action>();

            if (miningMiners.Count > 0)
            {
                foreach (Unit miner in miningMiners)
                {
                    if (miner.blitzium < 50)
                    {
                        actions.Add(new UnitAction(UnitActionType.MINE, miner.id, getAdjacentMine(mines, miner)));
                    }
                }
            }

            return actions;
        }

        public static void deployFollower(Position target)
        {
            nextIsFollower = true;
            followerTarget = target;
        }

        private List<Unit> miners;
        private List<Unit> miningMiners;
        private List<Position> availableMiningSpots;
        private static bool nextIsFollower;
        private static Position followerTarget;
        private Unit minerFollower;
    }
}