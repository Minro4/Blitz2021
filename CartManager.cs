using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Blitz2021;
using static Blitz2021.GameCommand;
using static Blitz2021.GameCommand.UnitAction;
using static Blitz2021.Map;


namespace Blitz2020
{
    class CartManager
    {
        private readonly double distanceValueDiminisher = 30;
        private readonly int far = 150;
        private readonly int quantityStackCap = 100;
        private readonly int isPublicPenalty = 25;

        List<Chariot> chariots;

        public CartManager()
        {
            chariots = new List<Chariot>();
        }

        public List<UnitAction> updateCart(GameMessage message, MinerManager minersManager, MapManager mapManager)
        {
            try
            {
                var karts = message.getMyCrew().get(Unit.UnitType.CART);
                cleanCarts(karts, message);
                foreach (Chariot chariot in chariots)
                {
                    chariot.updateState(message);
                }

                List<Unit> miners = minersManager.getMiningMiners();
                List<Depot> depots = message.map.depots.ToList();

                var travelingChariots = chariots.Where(chariot => chariot.state == Chariot.State.TRAVEL).ToList();

                var ressources = miners.Select(Ressource.fromMiner).ToList();
                //ressources.AddRange(depots.Select(Ressource.fromDepot));

                var availableResources = ressources.Where(resource =>
                {
                    var travelingTo = travelingChariots.Where(chariot => chariot.targerPickUp.Equals(resource.position)).ToList();
                    var dist = Pathfinding.bestPos(message, message.getMyCrew().homeBase, resource.position);
                    if (dist == null)
                        return false;

                    resource.bestAdjPos = dist.Item1;
                    resource.distToBestAdj = dist.Item2;
                    var adjustedGold = Math.Min(resource.blitzium, quantityStackCap) - (resource.isPublic ? isPublicPenalty : 0);
                    var minerFutureGold = adjustedGold + resource.distToBestAdj * resource.growth;
                    minerFutureGold -= travelingTo.Count * 25;
                    resource.value = minerFutureGold - ((double) resource.distToBestAdj / far * distanceValueDiminisher);
                    return minerFutureGold > 0;
                }).ToList();
                availableResources = availableResources.Where(miner =>
                {
                    return MapManager.getMineableTile(message.map, miner.position).Where(position => !position.isOccupied(message)).Count() != 0;
                }).ToList();

                var sortedResources = availableResources.OrderBy(o => -o.value).ToList();

                var waitingChariots = chariots.Where(chariot => chariot.isWaitting()).ToList();

                for (int i = 0; i < waitingChariots.Count() && i < sortedResources.Count; i++)
                {
                    var cartPos = waitingChariots[i].findChariot(karts).position;
                    waitingChariots[i].setGoal(sortedResources[i].bestAdjPos, sortedResources[i]);
                }

                return chariots.Select(chariot => chariot.selectAction(chariot.findChariot(karts), message)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("throw: " + ex);
                Console.WriteLine("Trace: " + ex.StackTrace);
                return new List<UnitAction>();
            }
        }

        private void cleanCarts(List<Unit> karts, GameMessage message)
        {
            //Remove dead chariots
            chariots = chariots.Where(chariot => { return karts.Find(kart => kart.id == chariot.id) != null; }).ToList();

            //Add new chariots
            var newKarts = karts.Where((kart) => { return chariots.Find((chariot) => chariot.id == kart.id) == null; })
                .ToList();

            var newChariots = newKarts.Select(kart => new Chariot(kart.id, message.getMyCrew().homeBase));
            chariots.AddRange(newChariots);
        }
    }

    public class Ressource
    {
        public Position position;
        public int blitzium;
        public int growth;
        public bool isPublic;
        public double value;

        public Position bestAdjPos;
        public int distToBestAdj;

        public static Ressource fromMiner(Unit miner)
        {
            return new Ressource()
            {
                position = miner.position,
                blitzium = miner.blitzium,
                growth = 1,
                isPublic = false
            };
        }

        public static Ressource fromDepot(Depot depot)
        {
            return new Ressource()
            {
                position = depot.position,
                blitzium = depot.blitzium,
                growth = 0,
                isPublic = true
            };
        }
    }
}