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

        public BaseManager(MapManager mapManager)
        {
            this.mapManager = mapManager;
        }

        public void update(List<GameCommand.Action> actions, GameMessage message)
        {
            if (message.tick < message.totalTick / 2)
            {
                expentionStrat(actions,message);
            }
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

        public void expentionStrat(List<GameCommand.Action> actions, GameMessage message)
        {
            Crew crew = message.getMyCrew();

            var minerEfficiency = this.minerEfficiency(message);

            if (minerEfficiency > 1)
            {
                if (crew.blitzium >= crew.prices.CART)
                {
                    var availableMines = mapManager.getAllMineNotOccupied(message);
                    if (availableMines.Count > 0)
                    {
                        var action = new BuyAction(Unit.UnitType.CART);
                        actions.Add(action);
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
        }
    }
}