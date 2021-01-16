using System;
using System.Collections.Generic;
using System.Text;
using Blitz2021;
using static Blitz2021.GameCommand;
using static Blitz2021.GameCommand.UnitAction;
using static Blitz2021.Map;

namespace Blitz2020
{
    class Outlaw
    {
        public enum State
        {
            KILL,
            RETURN,
            WAITTING
        }

        public string id;
        public State state;
        public Position targetPosition = new Position(0, 0);
        public Position targetKilling = new Position(0, 0);
        public string targetToKillID;


        public Outlaw(string id)
        {
            this.id = id;
            state = State.WAITTING;
        }

        public void setGoal(Position targetPosition, Position targetKilling)
        {
            this.targetKilling = targetKilling;
            this.targetPosition = targetPosition;
            state = State.KILL;
        }


        public UnitAction selectAction(GameMessage message, Unit unit)
        {
            if (state == State.KILL)
            {
                if (targetPosition.Equals(unit.position) || Pathfinding.path(unit.position, targetKilling) == 1)
                {
                    List<Position> available = MapManager.getMineableTileNotOccupied(message, targetKilling);
                    if (available.Count > 0)
                        targetPosition = available[0];

                    state = State.WAITTING;
                    targetPosition = getRandomPosition(message.map.getMapSize());
                    return new UnitAction(UnitActionType.ATTACK, id, targetKilling);
                }
                else
                {
                    return new UnitAction(UnitActionType.MOVE, id, targetPosition);
                }
            }
            else if (state == State.WAITTING)
            {
                if (Pathfinding.path(unit.position, targetKilling) < 2)
                {
                    targetPosition = getRandomPosition(message.map.getMapSize());
                    return new UnitAction(UnitActionType.MOVE, id, targetPosition);
                }
                else
                {
                    return new UnitAction(UnitActionType.MOVE, id, targetPosition);
                }
            }

            return new UnitAction(UnitActionType.NONE, id, new Position(0, 0));
        }

        public bool isWaitting()
        {
            return state == State.WAITTING;
        }

        public Position getRandomPosition(int size)
        {
            Random rand = new Random();
            return new Position(rand.Next(size), rand.Next(size));
        }

        public Unit findOutlaw(List<Unit> units)
        {
            for (int x = 0; x < units.Count; x++)
            {
                if (units[x].id == this.id)
                    return units[x];
            }

            return null;
        }
    }
}