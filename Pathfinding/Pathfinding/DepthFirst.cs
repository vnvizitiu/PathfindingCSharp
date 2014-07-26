using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pathfinding {
    class DepthFirst : PathFinder{

        private List<Tile> path = new List<Tile>();
        private List<Tile> seen = new List<Tile>();

        private NeighbourOrder TileSelectionMethod;

        public DepthFirst(TileGrid grid, NeighbourOrder no) : base(grid){
            path.Add(grid.Start);
            TileSelectionMethod = no;
        }

        public override void DoStep(){
            if(!IsDone) {
                bool deadEnd = true;
                foreach(Tile t in Grid.GetNeighbours(path.Last(), TileSelectionMethod)) {
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

                SetColors();

                base.DoStep();
            }
        }

        private void SetColors() {
            Grid.ResetColors();
            foreach(Tile t in seen) {
                t.Color = new Color(128, 0, 0);
            }
            foreach(Tile t in path) {
                t.Color = new Color(0, 0, 128);
            }
            path.Last().Color = new Color(64, 64, 255);
            path.First().Color = new Color(0, 255, 0);
        }
    }
}
