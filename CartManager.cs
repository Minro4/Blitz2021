using System;
using System.Collections.Generic;
using System.Text;
using Blitz2021;
using static Blitz2021.GameCommand;
using static Blitz2021.GameCommand.UnitAction;
using static Blitz2021.Map;



namespace Blitz2020
{
    class CartManager
    {
        
        List<Unit> init;
        List<Unit> inTravel;
        List<Unit> inCollect;
        List<Unit> inReturn;
        List<Chariot> chariots;

        public CartManager() 
        {
             init = new List<Unit>();
             inTravel = new List<Unit>();
             inCollect = new List<Unit>();
             inReturn = new List<Unit>();
             chariots = new List<Chariot>();
        }
    


        public void add(Unit kart) 
        {
            chariots.Add(new Chariot(kart.id));        
        }
        

        public List<UnitAction> updateCart(List<Unit> karts, MinerManager minersManager)
        {
            List<UnitAction> cartAction = new List<UnitAction>();
            if (init.Count > 0) 
            {
                List<Unit> miners = minersManager.getMiningMiners();
                for (int x = 0; x < init.Count; x++) 
                {
                    cartAction.Add(Chariot.goTo(init[x], miners[0].position));
                    inTravel.Add(init[x]);
                    init.Remove(init[x]);
                }
            
            }
            
            if (inTravel.Count > 0)
            {
                List<Unit> miners = minersManager.getMiningMiners();
                for (int x = 0; x < inTravel.Count; x++)
                {
                 
                    cartAction.Add(Chariot.goTo(init[x], miners[0].position));
                    inTravel.Add(init[x]);
                    init.Remove(init[x]);
                }

            }



            return cartAction;
        }





    }


    


}
