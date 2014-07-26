using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pathfinding {
    class Tile {
        private static int idCounter = 0;

        public readonly int Id;
        public TileType Type;

        private bool hasCustomColor = false;
        private Color customColor;
        public Color Color {
            get {
                if(hasCustomColor) {
                    return customColor;
                } else {
                    if(Type == TileType.START) {
                        return Color.Green;
                    } else if(Type == TileType.END) {
                        return Color.Red;
                    } else if(Type == TileType.OPEN) {
                        return Color.LightGray;
                    } else if(Type == TileType.CLOSED) {
                        return Color.DarkGray;
                    }
                    return Color.White;
                }
            }
            set {
                hasCustomColor = true;
                customColor = value;
            }
        }

        public Tile(TileType tt) {
            Type = tt;
            Id = idCounter;
            idCounter++;
        }

        public void ResetColor() {
            hasCustomColor = false;
        }
    }

    enum TileType {
        START, END, OPEN, CLOSED
    }
}
