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

		public UnitAction goTo(Unit kart, Position target){
			UnitAction action;
			if (estPerimetre(kart, target)) {
				action = new UnitAction(UnitActionType.PICKUP, kart.id, target);
				//done = true;
			}
			else {
				action = new UnitAction(UnitActionType.MOVE, kart.id, target);
				//done = false;
			}
			return action;
		}
		public UnitAction goToBase(Unit kart, Position target) {
			UnitAction action;
			if (estPerimetre(kart, target))
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

		public bool estPerimetre(Unit kart, Position target) {

			bool answer=false;
			
			if (kart.path.Count==0) {
				answer = true;
			}

			return answer;
		}
	}

}