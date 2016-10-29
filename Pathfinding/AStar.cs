using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pathfinding {
    class AStar : PathFinder {

        private TileGrid Grid;

        private List<AStarTile> openList = new List<AStarTile>();
        private List<AStarTile> closedList = new List<AStarTile>();

        public AStar(TileGrid grid) : base(grid) {
            Grid = grid;
            openList.Add(new AStarTile(Grid, Grid.Start, null));
        }

        public override void DoStep() {
            if(!IsDone) {

                AStarTile current = GetLowestFScoreTileFromOpenList(); //Returns null when openlist is empty
                if(current == null) { //If openlist is emtpy a* is done
                    IsDone = true;
                } else {
                    openList.Remove(current);
                    closedList.Add(current);

                    if(current.BaseTile.Type == TileType.END){
                        IsDone = true;
                        current.AddSelfAndParentToPath();
                    }else{
                        foreach(Tile neighbour in Grid.GetNeighbours(current.BaseTile, NeighbourOrder.STANDARD)){
                            if(neighbour.Type != TileType.CLOSED && !IsInClosedList(neighbour)) {
                                if(!IsInOpenList(neighbour)) { 
                                    openList.Add(new AStarTile(Grid, neighbour, current));
                                }else{
                                    AStarTile NewPathTile = new AStarTile(Grid, neighbour, current);
                                    if(NewPathTile.GScore < GetFromOpenList(neighbour).GScore){ //If this neighbour is already in the open list, but this route is faster, replace the old one
                                        openList.Remove(GetFromOpenList(neighbour));
                                        openList.Add(NewPathTile);
                                    }
                                }
                            }
                        }
                    }
                    SetColors(current.BaseTile);
                }

                base.DoStep();
            }
        }

        private AStarTile GetFromOpenList(Tile t) {
            AStarTile returnTile = null;
            foreach(AStarTile AStarTile in openList) {
                if(AStarTile.BaseTile.Equals(t)) {
                    returnTile = AStarTile;
                    break;
                }
            }
            return returnTile;
        }

        private bool IsInOpenList(Tile t) {
            bool isInList = false;
            foreach(AStarTile AStarTile in openList) {
                if(AStarTile.BaseTile.Equals(t)) {
                    isInList = true;
                    break;
                }
            }
            return isInList;
        }

        private bool IsInClosedList(Tile t) {
            bool isInList = false;
            foreach(AStarTile AStarTile in closedList) {
                if(AStarTile.BaseTile.Equals(t)) {
                    isInList = true;
                    break;
                }
            }
            return isInList;
        }

        private AStarTile GetLowestFScoreTileFromOpenList() { //Catchy names is half the work
            AStarTile returnTile = null;
            foreach(AStarTile current in openList) {
                if(returnTile != null) {
                    if(returnTile.FScore > current.FScore) {
                        returnTile = current;
                    }
                } else {
                    returnTile = current;
                }
            }
            return returnTile;
        }

        private void SetColors(Tile current) {
            Grid.ResetColors();
            foreach(AStarTile t in openList) {
                t.BaseTile.Color = new Color(128, 255, 128);
            }
            foreach(AStarTile t in closedList) {
                if(t.IsInPath) {
                    t.BaseTile.Color = new Color(128, 128, 255);
                } else {
                    float gradient = (255f / (Grid.Height + Grid.Width));
                    float R = (255f - (t.GScore * gradient));
                    t.BaseTile.Color = new Color((int)R, 0, 0);
                }
            }
            current.Color = new Color(0, 0, 255);
        }
    }
}
