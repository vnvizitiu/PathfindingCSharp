using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinding {
    class AStarTile {
        private const float DiagonalDistanceUnit = 1.41421f; // Sqrt(1^2 + 1^2)

        private TileGrid Grid;

        public static float HScoreMultiplier = -1;

        public Tile BaseTile { get; private set; }

        private AStarTile parent;
        public AStarTile Parent {
            get { return parent; }
            set {
                if (value != parent) {
                    parent = value;
                    if(parent != null) {
                        var par = Grid.GetCoordinates(Parent.BaseTile);
                        var me = Grid.GetCoordinates(BaseTile);
                        var dist = par.X != me.X && par.Y != me.Y ? DiagonalDistanceUnit : 1; //If both X AND Y differ parent is diagonal from me, otherwise it is horizontal/vertical
                        GScore = Parent.GScore + dist;
                    } else {
                        GScore = 1;
                    }
                }
            }
        }

        public bool IsInPath = false;

        public int FScore { //the lower the score, the sooner this tile will be processed by A*
            get {
                return (int)(GScore + (HScore * HScoreMultiplier));
            }
        }

        public float GScore { get; protected set; } = 1; //Distance to START

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
