using System;
using System.Collections.Generic;
using System.Linq;
using Blitz2021;

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
			if (estPerimetre(kart.position, target)) {
				action = new UnitAction(UnitActionType.PICKUP, kart.id, target);
				done = true;
			}
			else {
				action = new UnitAction(UnitActionType.MOVE, kart.id, target);
				done = false;
			}
			return action;
		}
		public UnitAction goToBase() {
			UnitAction action;
			if (estPerimetre(kart.position, target))
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
		public bool estPerimetre(Position kartPosition, Position target) {
			bool answer=false;
			int distanceX =Math.Abs( kartPosition.x - target.x);
			int distanceY = Math.Abs(kartPosition.y - target.y);
			if (distanceX <= 1 && distanceY <= 1) {
				answer == true;
			}

			return answer;
		}
	}

}