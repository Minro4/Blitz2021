using System;
using System.Collections.Generic;
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
        List<Chariot> chariots;

        public CartManager()
        {
            chariots = new List<Chariot>();
        }

        public List<UnitAction> updateCart(GameMessage message, MinerManager minersManager, MapManager mapManager)
        {
            var karts = message.getMyCrew().get(Unit.UnitType.CART);
            cleanCarts(karts, message);
            List<UnitAction> cartAction = new List<UnitAction>();
            List<Unit> miners = minersManager.getMiningMiners();

            var travelingChariots = chariots.Where(chariot => chariot.state == Chariot.State.TRAVEL).ToList();
            var availableMiners = miners.Where(miner =>
            {
                return travelingChariots.Find(
                    chariot => chariot.targerPickUp.Equals(miner.position)) == null;
            }).ToList();
            availableMiners = availableMiners.Where(miner =>
            {
                return mapManager.getMineableTile(message.map, miner.position).Where(position => !position.isOccupied(message)).Count() != 0;
            }).ToList();

            var sortedMiners = availableMiners.OrderBy(o => -o.blitzium).ToList();

            var waitingChariots = chariots.Where(chariot => chariot.isWaitting()).ToList();

            for (int i = 0; i < waitingChariots.Count() && i < sortedMiners.Count; i++)
            {
                var targetPosition = mapManager.getMineableTile(message.map, sortedMiners[i].position).Where(position => !position.isOccupied(message)).ToList();
                waitingChariots[i].setGoal(targetPosition[0], sortedMiners[i].position);
            }

            return chariots.Select(chariot => chariot.selectAction(chariot.findChariot(karts),message,mapManager)).ToList();
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
}