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

        private Tile start = null;
        public Tile Start {
            get {
                if(start != null) {
                    return start;
                } else {
                    if(IsValidGrid()) {
                        for(int x = 0; x < Width; x++) {
                            for(int y = 0; y < Height; y++) {
                                if(Grid[x, y] != null) {
                                    if(Grid[x, y].Type == TileType.START) {
                                        start = Grid[x, y];
                                        return start;
                                    }
                                }
                            }
                        }
                    }
                    return null;
                }
            }
        }

        private Tile end = null;
        public Tile End {
            get {
                if(end != null) {
                    return end;
                } else {
                    if(IsValidGrid()) {
                        for(int x = 0; x < Width; x++) {
                            for(int y = 0; y < Height; y++) {
                                if(Grid[x, y] != null) {
                                    if(Grid[x, y].Type == TileType.END) {
                                        end = Grid[x, y];
                                        return end;
                                    }
                                }
                            }
                        }
                    }
                    return null;
                }
            }
        }

        public TileGridSource Source;

        public TileGrid(int width, int height) {
            Width = width;
            Height = height;

            Grid = new Tile[width, height];
        }

        public List<Tile> GetNeighbours(Tile t, NeighbourOrder order) {
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

                        if(order == NeighbourOrder.RANDOM) {
                            Random ran = new Random();
                            int n = returnList.Count;
                            while(n > 1) {
                                n--;
                                int k = ran.Next(n + 1);
                                var value = returnList[k];
                                returnList[k] = returnList[n];
                                returnList[n] = value;
                            }
                        } else if(order == NeighbourOrder.SMART) {
                            var SmartList = new List<Tile>();
                            var SmartListScores = new int[returnList.Count];

                            for(int i = 0; i < returnList.Count(); i++) {
                                SmartListScores[i] = NonDiagonalDistanceToEnd(returnList[i]);
                            }
                            for(int j = 0; j < returnList.Count(); j++) {
                                int lowestIndex = -1;
                                int lowest = Int32.MaxValue;
                                for(int i = 0; i < SmartListScores.Count(); i++) {
                                    if(SmartListScores[i] != -1) {
                                        if(lowest > SmartListScores[i]) {
                                            lowest = SmartListScores[i];
                                            lowestIndex = i;
                                        }
                                    }
                                }
                                SmartList.Add(returnList[lowestIndex]);
                                SmartListScores[lowestIndex] = -1;
                            }
                            returnList = SmartList;
                        }
                        return returnList;
                    }
                }
            }
            return null;
        }

        public void GenEmptyGrid() {
            Source = TileGridSource.EMPTY;
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
            Source = TileGridSource.RANDOM;
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

        public void GenFromFile(System.Drawing.Bitmap img) {
            Source = TileGridSource.FILE;

            if(img.Width != Width || img.Height != Height){
                GenEmptyGrid();
                return;
            }

            for (int x = 0; x < Width; x++){
                for (int y = 0; y < Height; y++){
                    System.Drawing.Color pixel = img.GetPixel(x, y);

                    if (pixel.R == 0 && pixel.G == 0 && pixel.B == 0){ //#000000 - Black
                        Grid[x, y] = new Tile(TileType.CLOSED);
                    } else if(pixel.R == 255 && pixel.G == 255 && pixel.B == 255) { //#FFFFFF - White
                        Grid[x, y] = new Tile(TileType.OPEN);
                    } else if(pixel.R == 255 && pixel.G == 0 && pixel.B == 0) { //#FF0000 - Red
                        Grid[x, y] = new Tile(TileType.END);
                    } else if(pixel.R == 0 && pixel.G == 255 && pixel.B == 0) { //#00FF00 - Green
                        Grid[x, y] = new Tile(TileType.START);
                    } else {
                        Grid[x, y] = new Tile(TileType.OPEN);
                    }
                }
            }

            if(!IsValidGrid()){
                GenEmptyGrid();
            }
        }

        public bool HasStart() {
            for(int x = 0; x < Width; x++) {
                for(int y = 0; y < Height; y++) {
                    if(Grid[x, y] != null) {
                        if(Grid[x, y].Type == TileType.START) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool HasEnd() {
            for(int x = 0; x < Width; x++) {
                for(int y = 0; y < Height; y++) {
                    if(Grid[x, y] != null) {
                        if(Grid[x, y].Type == TileType.END) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool IsValidGrid() {
            bool hasStart = false;
            bool hasEnd = false;
            bool hasNulls = false;

            for(int x = 0; x < Width; x++) {
                for(int y = 0; y < Height; y++) {
                    if(Grid[x, y] != null) {
                        if(Grid[x, y].Type == TileType.START) {
                            hasStart = true;
                        } else if(Grid[x, y].Type == TileType.END) {
                            hasEnd = true;
                        }
                    } else {
                        hasNulls = true;
                    }
                }
            }
            return (!hasNulls && (hasStart && hasEnd));
        }

        public Vector2 GetCoordinates(Tile t) {
            for(int x = 0; x < Width; x++) {
                for(int y = 0; y < Height; y++) {
                    if(Grid[x, y].Equals(t)) {
                        return new Vector2(x, y);
                    }
                }
            }
            return new Vector2(-1, -1);
        }

        public Vector2 GetGridCoordinates(Vector2 drawCoordinates, Rectangle drawArea) {
            int pixelWidth = drawArea.Width / Width;
            int pixelHeight = drawArea.Height / Height;

            int gridX = (int)Math.Floor((drawCoordinates.X - drawArea.X) / pixelWidth);
            int gridY = (int)Math.Floor((drawCoordinates.Y - drawArea.Y) / pixelHeight);

            return new Vector2(gridX, gridY);
        }

        public int DiagonalDistanceToEnd(Tile t) {
            int tileX = (int) GetCoordinates(t).X;
            int tileY = (int) GetCoordinates(t).Y;
            int endX = (int)GetCoordinates(End).X;
            int endY = (int)GetCoordinates(End).Y;
            
            return (int) (Math.Sqrt((Math.Pow(Math.Abs(tileX - endX), 2) + Math.Pow(Math.Abs(tileY - endY), 2))));
        }

        public int NonDiagonalDistanceToEnd(Tile t) {
            int tileX = (int)GetCoordinates(t).X;
            int tileY = (int)GetCoordinates(t).Y;
            int endX = (int)GetCoordinates(End).X;
            int endY = (int)GetCoordinates(End).Y;

            return (Math.Abs(tileX - endX) + Math.Abs(tileY - endY));
        }

        public void ResetColors() {
            foreach(Tile t in Grid) {
                t.ResetColor();
            }
        }

        public void LeftClick(Vector2 click, Rectangle drawArea) {
            Vector2 gridCoordinates = GetGridCoordinates(click, drawArea);
            Tile clickedTile = Grid[(int)gridCoordinates.X, (int)gridCoordinates.Y];

            TileType replaceType;
            if(!HasStart()) {
                replaceType = TileType.START;
            } else if(!HasEnd()) {
                replaceType = TileType.END;
            } else {
                replaceType = TileType.CLOSED;
            }

            if(clickedTile.Type != TileType.START && clickedTile.Type != TileType.END) {
                clickedTile.Type = replaceType;
            }
        }

        public void RightClick(Vector2 click, Rectangle drawArea) {
            Vector2 gridCoordinates = GetGridCoordinates(click, drawArea);
            Tile clickedTile = Grid[(int)gridCoordinates.X, (int)gridCoordinates.Y];

            clickedTile.Type = TileType.OPEN;
        }

        public void Draw(SpriteBatch sb, Rectangle drawArea) {
            int pixelWidth = drawArea.Width / Width;
            int pixelHeight = drawArea.Height / Height;

            sb.Begin();

            for(int x = 0; x < Width; x++) {
                for(int y = 0; y < Height; y++) {
                    Tile current = Grid[x, y];
                    sb.Draw(Game1.EmptyPixel, new Rectangle(((x * pixelWidth) + drawArea.X), ((y * pixelHeight) + drawArea.Y), pixelWidth, pixelHeight), current.Color);
                }
            }
            sb.End();
        }
    }

    enum TileGridSource { 
        EMPTY, RANDOM, FILE
    }

    enum NeighbourOrder { 
        STANDARD, RANDOM, SMART
    }
}
