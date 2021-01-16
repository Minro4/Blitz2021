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
        public void buy(List<GameCommand.Action> actions, GameMessage message)
        {
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
                    var action = new BuyAction(Unit.UnitType.MINER);
                    actions.Add(action);
                }
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

            return totalCost / (carts.Count * 25);
        }
    }
}