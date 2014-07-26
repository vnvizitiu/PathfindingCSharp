using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinding {
    abstract class PathFinder {

        protected TileGrid Grid;
        public int StepCount = 0;
        public bool IsDone = false;

        public PathFinder(TileGrid grid) {
            Grid = grid;

            if(!grid.IsValidGrid()) {
                throw new Exception("Non-valid grid!");
            }
        }

        public virtual void DoStep() {
            StepCount++;
        }

        public void RunTillDone() {
            while(!IsDone) {
                DoStep();
            }
        }
    }
}
