using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinding {
    class AStarTile {
        private TileGrid Grid;

        public static float HScoreMultiplier = 2;

        public Tile BaseTile { get; private set; }
        public AStarTile Parent;

        public bool IsInPath = false;

        public int FScore { //the lower the score, the sooner this tile will be processed by A*
            get {
                return (int)(GScore + (HScore * HScoreMultiplier));
            }
        }

        public int GScore {//Distance to START
            get {
                if(Parent != null) {
                    return Parent.GScore + 1;
                } else {
                    return 1;
                }
            }
        }

        private int hScore = -1;
        public int HScore {// Estimated distance to END
            get {
                if(hScore == -1) {
                    hScore = Grid.DiagonalDistanceToEnd(BaseTile);
                }
                return hScore;
            }
        }

        public AStarTile(TileGrid baseGrid, Tile baseTile, AStarTile parent) {
            Grid = baseGrid;
            BaseTile = baseTile;
            Parent = parent;
        }

        public void AddSelfAndParentToPath() {
            IsInPath = true;
            if(Parent != null) { 
                Parent.AddSelfAndParentToPath();
            }
        }
    }
}
