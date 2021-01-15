using System.Collections.Generic;
using static Blitz2021.Map;
using static Blitz2021.Unit;

namespace Blitz2021
{
    public class GameCommand
    {
        public abstract class Action
        {
            public enum ActionType
            {
                BUY, UNIT
            }

            public ActionType type;
        }

        public class BuyAction : Action
        {
            public UnitType unitType;

            public BuyAction(UnitType unitType)
            {
                this.type = ActionType.BUY;
                this.unitType = unitType;
            }
        }

        public class UnitAction : Action
        {
            public enum UnitActionType
            {
                MOVE, ATTACK, PICKUP, MINE, DROP, NONE
            }
            public Position target;
            public UnitActionType action;
            public string unitId;

            public UnitAction(UnitActionType action, string unitId, Position target)
            {
                this.type = ActionType.UNIT;
                this.unitId = unitId;
                this.target = target;
                this.action = action;
            }
        }

        public GameCommand(List<Action> actions)
        {
            this.actions = actions;
        }

        public List<Action> actions;
    }
}