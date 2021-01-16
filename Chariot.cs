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
		public string id;
		public State state;
		public Position target;

		public enum State
		{
			TRAVEL, PICKUP, RETURN, DROP,WAITTING
		}

		public UnitAction selectAction(Unit a) 
		{
			if (state == State.TRAVEL)
			{
				if (target.Equals(a.position))
				{
					state = State.PICKUP;
					return new UnitAction(UnitActionType.MOVE, id, target);
				}
				else 
				{ 
				
				}
					

				return new UnitAction(UnitActionType.MOVE, id, target);
			}
			else if (state == State.TRAVEL) 
			{ 
			
			}

			return new UnitAction(UnitActionType.MOVE, id, target);
		}

		public Chariot(string id) {
			done = true;
			this.id = id;
		}

		public static UnitAction goTo(Unit kart, Position target){
			UnitAction action;
			if (estPerimetre(kart.position, target)) {
				action = new UnitAction(UnitActionType.PICKUP, kart.id, target);
				//done = true;
			}
			else {
				action = new UnitAction(UnitActionType.MOVE, kart.id, target);
				//done = false;
			}
			return action;
		}
		public static UnitAction goToBase(Unit kart, Position target) {
			UnitAction action;
			if (estPerimetre(kart.position, target))
			{
				action = new UnitAction(UnitActionType.DROP, kart.id, target);
				//done = true;
			}
			else
			{
				action = new UnitAction(UnitActionType.MOVE, kart.id, target);
				//done = false;
			}
			return action;
		}
		public static bool estPerimetre(Position kartPosition, Position target) {
			bool answer=false;
			int distanceX =Math.Abs( kartPosition.x - target.x);
			int distanceY = Math.Abs(kartPosition.y - target.y);
			if (distanceX <= 1 && distanceY <= 1) {
				answer = true;
			}

			return answer;
		}
	}

}