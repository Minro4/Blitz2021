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
        public string id;
        public State state;
        public Position targetPosition;
        public Position targerPickUp;
        public Position basePosition;

        public enum State
        {
            TRAVEL,
            RETURN,
            WAITTING
        }

        public Chariot(string id, Position basePosition)
        {
            this.id = id;
            this.basePosition = basePosition;
            this.state = State.WAITTING;
        }

        public void setGoal(Position targetPosition, Position targetPickUp)
        {
            this.targerPickUp = targetPickUp;
            this.targetPosition = targetPosition;
            state = State.TRAVEL;
        }

        public bool isWaitting()
        {
            return state == State.WAITTING;
        }

        public UnitAction selectAction(Unit a, GameMessage message)
        {
            if (state == State.TRAVEL)
            {
                if (targetPosition.Equals(a.position))
                {
                    state = State.RETURN;
                    List<Position> available = MapManager.getMineableTileNotOccupied(message, basePosition);
                    if (available.Count > 0)
                        targetPosition = available[0];

                    return new UnitAction(UnitActionType.PICKUP, id, targerPickUp);
                }
                else
                {
                    return new UnitAction(UnitActionType.MOVE, id, targetPosition);
                }
            }
            else if (state == State.RETURN)
            {
                if (targetPosition.isOccupied(message))
                {
                    List<Position> available = MapManager.getMineableTileNotOccupied(message, basePosition);
                    if (available.Count > 0)
                        targetPosition = available[0];
                }

                if (targetPosition.Equals(a.position) || Pathfinding.path(a.position, basePosition).Count == 1)
                {
                    state = State.WAITTING;
                    return new UnitAction(UnitActionType.DROP, id, basePosition);
                }
                else
                {
                    return new UnitAction(UnitActionType.MOVE, id, targetPosition);
                }
            }
            else if (state == State.WAITTING)
            {
                return new UnitAction(UnitActionType.NONE, id, basePosition);
            }

            return new UnitAction(UnitActionType.NONE, id, basePosition);
        }

        public Unit findChariot(List<Unit> karts)
        {
            return karts.Find(kart => kart.id == id);
        }
    }
}