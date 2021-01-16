using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Blitz2021;
using static Blitz2021.GameCommand;
using static Blitz2021.GameCommand.UnitAction;
using static Blitz2021.Map;


namespace Blitz2020
{
    class OutlawManager
    {
        List<Outlaw> outlaws;

        public OutlawManager() 
        {
            outlaws = new List<Outlaw>();
        }


        public List<UnitAction> updateOutlaw(GameMessage message, MinerManager minersManager, MapManager mapManager, bool timeKill)
        {
            var newOutlaws = message.getMyCrew().get(Unit.UnitType.OUTLAW);
            cleanOutlaws(newOutlaws, message);
            List<UnitAction> cartAction = new List<UnitAction>();

            if (timeKill) 
            {
                List<Position> ennemie = message.getEnemieMiner();
                for (int x = 0; x < outlaws.Count; x++) 
                {
                    if (ennemie.Count > 0) 
                    {
                       List<Position> posibleTile = MapManager.getMineableTileNotOccupied(message, new Position(ennemie[0].x, ennemie[0].y));




                        outlaws[x].setGoal(posibleTile[0], ennemie[0]);
                        ennemie.Remove(ennemie[0]);
                    }
                }
            }

            return outlaws.Select(outlaw => outlaw.selectAction(message,outlaw.findOutlaw(newOutlaws))).ToList();

        }





        private void cleanOutlaws(List<Unit> newOutlaws, GameMessage message)
        {
            //Remove dead chariots
            outlaws = outlaws.Where((outlaw) => { return newOutlaws.Find(outlaw1 => outlaw1.id == outlaw.id) != null; }).ToList();
            
            //Add new chariots
            var newKarts = newOutlaws.Where((newOutlaw) => { return outlaws.Find((outlaw) => outlaw.id == newOutlaw.id) == null; })
                .ToList();

            var outlawNew = newKarts.Select(outlaw => new Outlaw(outlaw.id));
            outlaws.AddRange(outlawNew);
        }

    }


}
