using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pathfinding {
    class DepthFirst : PathFinder {

        private List<Tile> path = new List<Tile>();
        private List<Tile> seen = new List<Tile>();

        private NeighbourOrder TileSelectionMethod = NeighbourOrder.STANDARD;

        public DepthFirst(TileGrid grid, AllowDirection allowDirection) : base(grid, allowDirection){
            path.Add(grid.Start);
        }

        public override void DoStep(){
            if(!IsDone && path.Count > 0) {
                bool deadEnd = true;
                foreach(Tile t in Grid.GetNeighbours(path.Last(), TileSelectionMethod, AllowDirection)) {
                    if(t.Type == TileType.END) {
                        deadEnd = false;
                        IsDone = true;
                        break;
                    } else if(t.Type == TileType.OPEN) {
                        if(!seen.Contains(t)) {
                            path.Add(t);
                            seen.Add(t);
                            deadEnd = false;
                            break;
                        }
                    }
                }

                if(deadEnd) {
                    path.RemoveAt(path.Count - 1);
                }

                if(path.Count < 1) {
                    IsDone = true;
                }

                SetColors();

                base.DoStep();
            }
        }

        private void SetColors() {
            Grid.ResetColors();
            foreach(Tile t in seen) {
                t.Color = new Color(128, 64, 64);
            }
            foreach(Tile t in path) {
                t.Color = new Color(128, 255, 128);
            }
            if(path.Count > 0) {
                path.Last().Color = new Color(0, 0, 255);
            }
        }

        public void SetNeighbourOrder(NeighbourOrder no) {
            TileSelectionMethod = no;
        }
    }
}
