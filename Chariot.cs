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
        public Position lastPosition;
        public Ressource ressource;
        public int samePositionCtr=0;

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

        public void setGoal(Position targetPosition, Ressource ressource)
        {
            this.targerPickUp = ressource.position;
            this.targetPosition = targetPosition;
            this.ressource = ressource;
            this.lastPosition = new Position(0, 0);
            state = State.TRAVEL;
        }

        public bool isWaitting()
        {
            return state == State.WAITTING;
        }
        
        //Verifies if desired depot was taken
        public void updateState(GameMessage message)
        {
            if (ressource != null && ressource.isPublic && state == State.TRAVEL)
            {
                var depot = message.map.depots.ToList().Find(depot => depot.position.Equals(ressource.position));
                if (depot == null || depot.blitzium == 0)
                {
                    state = State.WAITTING;
                }
            }
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
                    samePositionCtr = 0;
                    return new UnitAction(UnitActionType.PICKUP, id, targerPickUp);
                }
                else if (lastPosition.Equals(a.position))
                {
                    samePositionCtr += 1;
                    if (samePositionCtr >= 3)
                    {
                        List<Position> available = MapManager.getMineableTileNotOccupied(message, targerPickUp);
                        if (available.Count > 0 && Pathfinding.isAnyPathAvailable(a.position, available))
                        {
                            targetPosition = Pathfinding.findAvailablePosition(a.position, available);
                        }
                        else
                        {
                            state = State.WAITTING;
                        }
                    }
                    return new UnitAction(UnitActionType.MOVE, id, targetPosition);
                }
                else 
                {
                    lastPosition = a.position;
                    return new UnitAction(UnitActionType.MOVE, id, targetPosition);
                }
            }
            else if (state == State.RETURN)
            {
                samePositionCtr = 0;
                if (targetPosition.isOccupied(message) || lastPosition.Equals(a.position))
                {
                    List<Position> available = MapManager.getMineableTileNotOccupied(message, basePosition);
                    if (available.Count > 0)
                        targetPosition = available[0];
                }

                if (targetPosition.Equals(a.position) || Pathfinding.path(a.position, basePosition) == 1)
                {
                    state = State.WAITTING;
                    return new UnitAction(UnitActionType.DROP, id, basePosition);
                }
                else
                {
                    lastPosition = a.position;
                    return new UnitAction(UnitActionType.MOVE, id, targetPosition);
                }
            }
            else if (state == State.WAITTING)
            {
                samePositionCtr = 0;
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