﻿using Blitz2020;
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
        private int miniumBank = 0;

        public BaseManager(MapManager mapManager, MinerManager minerManager)
        {
            this.mapManager = mapManager;
            this.minerManager = minerManager;
        }

        public List<GameCommand.Action> update(GameMessage message)
        {
            try
            {
                return expentionStrat(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("throw: " + ex);
                Console.WriteLine("Trace: " + ex.StackTrace);
                return new List<GameCommand.Action>();
            }
        }

        public double minerEfficiency(GameMessage message)
        {
            Crew crew = message.getMyCrew();
            var miners = crew.get(Unit.UnitType.MINER);
            var distances = miners.Select((Unit miner) => Pathfinding.bestPos(message, crew.homeBase, miner.position)).Where(t => t != null)
                .Select(d => d.Item2);
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
            var miners = crew.get(Unit.UnitType.MINER);

            var minerEfficiency = this.minerEfficiency(message);

            if (shouldBuyOutlaw(message, minerEfficiency, miners.Count))
            {
                if (crew.blitzium >= crew.prices.OUTLAW + miniumBank)
                {
                    var action = new BuyAction(Unit.UnitType.OUTLAW);
                    actions.Add(action);
                    spawnedOutlaw = true;
                }
            }
            else if (minerEfficiency > 1)
            {
                if (crew.blitzium >= crew.prices.CART + miniumBank)
                {
                    var action = new BuyAction(Unit.UnitType.CART);
                    actions.Add(action);
                }
            }
            else
            {
                if (crew.blitzium >= crew.prices.MINER + miniumBank)
                {
                    var tickLeft = message.totalTick - message.tick;

                    var availableMines = mapManager.getAllMineNotOccupied(message);
                    var mineDistances = availableMines.Select(pos => Pathfinding.bestPos(message,crew.homeBase,pos)).Where(d => d!= null).Select(d => d.Item2).ToList();
                    if (mineDistances.Count > minerManager.getMovingMiners().Count)
                    {
                        var bestMineDist = mineDistances.Min();

                        if ((tickLeft - (bestMineDist * 3)) > message.getMyCrew().prices.MINER + message.getMyCrew().prices.CART)
                        {
                            var action = new BuyAction(Unit.UnitType.MINER);
                            actions.Add(action);
                        }
                    }
                }
            }

            return actions;
        }

        private bool shouldBuyOutlaw(GameMessage message, double minerEfficiency, int nbrMiners)
        {
            var nbrAvailableMines = mapManager.getAllMineNotOccupied(message).Count;
            if (nbrAvailableMines > 0)
                return (!spawnedOutlaw && minerEfficiency < 1 && nbrMiners >= 3);

            return !spawnedOutlaw && minerEfficiency < 1;
        }

        public int growthRate()
        {
            return this.minerManager.getMiningMiners().Count;
        }

        public bool timeToKill(GameMessage message)
        {
            var crewNbr = message.crews.Count;
            var tickLeft = message.totalTick - message.tick;

            if (crewNbr <= 2)
            {
                if (spawnedOutlaw && message.getMyCrew().blitzium >= 50 && tickLeft >= 150)
                {
                    miniumBank = 50;
                    return true;
                }

                miniumBank = 0;
                return false;
            }
            else
            {
                var otherBestPotential = message.getOtherCrews().Select(c => c.potential(message)).Max();
                var ourPotential = message.getMyCrew().potential(message);

                if (spawnedOutlaw && message.getMyCrew().blitzium >= 50 && tickLeft >= 250 && (otherBestPotential * 1) > ourPotential)
                {
                    miniumBank = 50;
                    return true;
                }

                miniumBank = 0;
                return false;
            }
        }
    }
}