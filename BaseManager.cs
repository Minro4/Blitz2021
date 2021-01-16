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
        private bool spawnedOutlaw = false;

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
            var distances = miners.Select((Unit miner) => Pathfinding.path(crew.homeBase, miner.position));
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
                    var action = new BuyAction(Unit.UnitType.CART);
                    actions.Add(action);
                }
            }
            else
            {
                if (crew.blitzium >= crew.prices.MINER)
                {
                    var availableMines = mapManager.getAllMineNotOccupied(message);
                    var mineDistances = availableMines.Select(pos => Pathfinding.path(pos, crew.homeBase)).ToList();
                    if (mineDistances.Count > minerManager.getMovingMiners().Count)
                    {
                        var bestMineDist = mineDistances.Min();
                        var tickLeft = message.totalTick - message.tick;
                        if ((tickLeft - bestMineDist) > message.getMyCrew().prices.MINER + message.getMyCrew().prices.CART)
                        {
                            var action = new BuyAction(Unit.UnitType.MINER);
                            actions.Add(action);
                        }
                    }
                }
            }

            if (actions.Count == 0)
            {
                if (!spawnedOutlaw && crew.blitzium >= crew.prices.OUTLAW)
                {
                    var action = new BuyAction(Unit.UnitType.OUTLAW);
                    actions.Add(action);
                    spawnedOutlaw = true;
                }
            }

            return actions;
        }

        public int growthRate()
        {
            return this.minerManager.getMiningMiners().Count;
        }

        public bool timeToKill(GameMessage message)
        {
            var tickLeft = message.totalTick - message.tick;
            var percent = (double) message.tick / message.totalTick;
            if (message.getMyCrew().blitzium >= 50 && percent >= 0.25 && tickLeft >= 100)
                return true;

            return false;
        }
    }
}