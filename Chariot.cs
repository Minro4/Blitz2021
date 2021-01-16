using System;
using System.Collections.Generic;
using System.Linq;
using Blitz2021;
using static Blitz2021.GameCommand;
using static Blitz2021.GameCommand.UnitAction;
using static Blitz2021.Map;

namespace Blitz2020
{

	public class Chariot
	{
		public bool done;
		public Chariot() {
			done = true;
		}

		public UnitAction goTo(Unit kart, Position target){
			UnitAction action;
			if (estPerimetre(kart, target)) {
				action = new UnitAction(UnitActionType.PICKUP, kart.id, target);
				done = true;
			}
			else {
				action = new UnitAction(UnitActionType.MOVE, kart.id, target);
				done = false;
			}
			return action;
		}
		public UnitAction goToBase(Unit kart, Position target) {
			UnitAction action;
			if (estPerimetre(kart, target))
			{
				action = new UnitAction(UnitActionType.DROP, kart.id, target);
				done = true;
			}
			else
			{
				action = new UnitAction(UnitActionType.MOVE, kart.id, target);
				done = false;
			}
			return action;
		}
		public bool estPerimetre(Unit kart, Position target) {
			bool answer=false;
			
			if (kart.path.Count==0) {
				answer = true;
			}

			return answer;
		}
	}

}