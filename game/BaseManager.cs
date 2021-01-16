using Blitz2020;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Blitz2021.GameCommand;

namespace Blitz2021
{
    public class BaseManager
    {
        private MapManager mapManager;
        private MinerManager minerManager;

        public BaseManager(MapManager mapManager, MinerManager minerManager)
        {
            this.mapManager = mapManager;
            this.minerManager = minerManager;
        }

        public List<GameCommand.Action> update(GameMessage message)
        {
            return expentionStrat(message);
        }

        public double minerEfficiency(GameMessage message)
        {
            Crew crew = message.getMyCrew();
            var miners = crew.get(Unit.UnitType.MINER);
            var distances = miners.Select((Unit miner) => Pathfinding.path(crew.homeBase, miner.position).Count);
            var timeCosts = distances.Select(distance => distance * 2);
            var totalCost = timeCosts.Aggregate(0, (acc, x) => acc + x);

            var carts = crew.get(Unit.UnitType.CART);

            if (carts.Count == 0)
                return totalCost / 0.01;
            return (double) totalCost / (carts.Count * 25);
        }

        public List<GameCommand.Action> expentionStrat(GameMessage message)
        {
            var actions = new List<GameCommand.Action>();
            Crew crew = message.getMyCrew();

            var minerEfficiency = this.minerEfficiency(message);

            if (minerEfficiency > 1)
            {
                if (crew.blitzium >= crew.prices.CART)
                {
                    var availableMines = mapManager.getAllMineNotOccupied(message);
                    var mineDistances = availableMines.Select(pos => Pathfinding.path(pos, crew.homeBase).Count).ToList();
                    if (mineDistances.Count > minerManager.getMovingMiners().Count)
                    {
                        var bestMineDist = mineDistances.Min();
                        var tickLeft = message.totalTick - message.tick;
                        if ((tickLeft - bestMineDist) > message.getMyCrew().prices.MINER)
                        {
                            var action = new BuyAction(Unit.UnitType.CART);
                            actions.Add(action);
                        }
                    }
                }
              
            }
            else
            {
                if (crew.blitzium >= crew.prices.MINER)
                {
                    var action = new BuyAction(Unit.UnitType.MINER);
                    actions.Add(action);
                }
            }

            return actions;
        }

        public int growthRate()
        {
            return this.minerManager.getMiningMiners().Count;
        }
    }
}