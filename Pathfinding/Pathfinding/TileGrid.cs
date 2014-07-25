using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pathfinding {
    class TileGrid {
        public Tile[,] Grid;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public TileGrid(int width, int height) {
            Width = width;
            Height = height;

            Grid = new Tile[width, height];
        }

        public List<Tile> GetNeighbours(Tile t) {
            for(int x = 0; x < Width; x++) {
                for(int y = 0; y < Height; y++) {
                    Tile current = Grid[x, y];

                    if(current.Equals(t)) {
                        List<Tile> returnList = new List<Tile>();
                        if(x > 0) {
                            returnList.Add(Grid[x - 1, y]);//Left
                        }
                        if(x < Width - 1) {
                            returnList.Add(Grid[x + 1, y]);//Right
                        }
                        if(y > 0) {
                            returnList.Add(Grid[x, y - 1]);//Top
                        }
                        if(y < Height - 1) {
                            returnList.Add(Grid[x, y + 1]);//Bottom
                        }
                        return returnList;
                    }
                }
            }
            return null;
        }

        public void GenEmptyGrid() {
            for(int x = 0; x < Width; x++) {
                for(int y = 0; y < Height; y++) {
                    Grid[x, y] = new Tile(TileType.OPEN);
                }
            }
            Grid[1, 1] = new Tile(TileType.START);
            Grid[Width - 2, Height - 2] = new Tile(TileType.END);
            Grid[Width / 2, Height / 2] = new Tile(TileType.CLOSED);
        }

        public void GenRandomGrid(Double closedPercentage) {
            Random ran = new Random();
            for(int x = 0; x < Width; x++) {
                for(int y = 0; y < Height; y++) {
                    if(ran.NextDouble() > (closedPercentage / 100)) {
                        Grid[x, y] = new Tile(TileType.OPEN);
                    } else {
                        Grid[x, y] = new Tile(TileType.CLOSED);
                    }
                }
            }
            Grid[ran.Next(Width), ran.Next(Height)] = new Tile(TileType.START);
            Grid[ran.Next(Width), ran.Next(Height)] = new Tile(TileType.END);

            if(!IsValidGrid()) {
                GenRandomGrid(closedPercentage);
            }
        }

        public bool IsValidGrid() {
            bool hasStart = false;
            bool hasEnd = false;

            for(int x = 0; x < Width; x++) {
                for(int y = 0; y < Height; y++) {
                    if(Grid[x, y] != null) {
                        if(Grid[x, y].Type == TileType.START) {
                            hasStart = true;
                        } else if(Grid[x, y].Type == TileType.END) {
                            hasEnd = true;
                        }
                    } else {
                        return false;
                    }
                }
            }
            return (hasStart && hasEnd);
        }

        public void Draw(SpriteBatch sb, int gridX, int gridY, int gridWidth, int gridHeight) {
            int pixelWidth = gridWidth / Width;
            int pixelHeight = gridHeight / Height;

            sb.Begin();

            for(int x = 0; x < Width; x++) {
                for(int y = 0; y < Height; y++) {
                    Tile current = Grid[x, y];
                    sb.Draw(Game1.EmptyPixel, new Rectangle(((x * pixelWidth) + gridX), ((y * pixelHeight) + gridY), pixelWidth, pixelHeight), current.Color);
                }
            }
            sb.End();
        }
    }
}
