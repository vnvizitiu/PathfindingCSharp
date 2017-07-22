using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinding {
    abstract class PathFinder {

        protected TileGrid Grid;
        public int StepCount = 0;

        private bool isDone = false;
        public bool IsDone {
            set {
                isDone = value;
                endTime = Environment.TickCount;
            }
            get {
                return isDone;
            }     
        }

        private int startTime;
        private int endTime;

        public long TimeRunningMillis {
            get {
                if(IsDone) {
                    return endTime - startTime;
                } else {
                    return Environment.TickCount - startTime;
                }
            }
        }

        public AllowDirection AllowDirection { get; }

        public PathFinder(TileGrid grid, AllowDirection allowDirection) {
            Grid = grid;

            if(!grid.IsValidGrid()) {
                throw new Exception("Non-valid grid!");
            }
            startTime = Environment.TickCount;
            AllowDirection = allowDirection;
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
