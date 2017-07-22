using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pathfinding {

    public class Game1 : Microsoft.Xna.Framework.Game {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Rectangle GridDrawArea = new Rectangle(0, 30, 700, 700);

        private TileGrid TileGrid;
        private PathFinder PathFinder;

        private SimState CurrentState = SimState.MENU_DIRECTION_SELECT;

        public static SpriteFont SimpleFont;
        public static Texture2D EmptyPixel;

        private KeyboardState LastKeyState;

        private AllowDirection SelectedDirection;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 730;
            graphics.PreferredBackBufferWidth = 700;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            LastKeyState = Keyboard.GetState();
            base.Initialize();
        }

        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            SimpleFont = Content.Load<SpriteFont>("SimpleFont");

            EmptyPixel = new Texture2D(GraphicsDevice, 1, 1);
            EmptyPixel.SetData<Color>(new Color[] { Color.White });

            setupLevel();
        }

        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime) {
            // Allows the game to exit
            if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();
            Vector2 mPos = new Vector2(mState.X, mState.Y);

            if(CurrentState != SimState.STARTED) {
                if(GridDrawArea.Contains((int)mPos.X, (int)mPos.Y)) {
                    if(mState.LeftButton == ButtonState.Pressed) {
                        TileGrid.LeftClick(mPos, GridDrawArea);
                    } else if(mState.RightButton == ButtonState.Pressed) {
                        TileGrid.RightClick(mPos, GridDrawArea);
                    }
                }
            } else {
                if(Keyboard.GetState().IsKeyDown(Keys.Enter)) {
                    PathFinder.RunTillDone();
                }
                PathFinder.DoStep();
            }
            
            if(CurrentState == SimState.MENU_DEPTHFIRST_OPTIONS) {
                if(kState.IsKeyDown(Keys.F1) && LastKeyState.IsKeyUp(Keys.F1)) {
                    ((DepthFirst)(PathFinder)).SetNeighbourOrder(NeighbourOrder.STANDARD);
                    CurrentState = SimState.STARTED;
                } else if(kState.IsKeyDown(Keys.F2) && LastKeyState.IsKeyUp(Keys.F2)) {
                    ((DepthFirst)(PathFinder)).SetNeighbourOrder(NeighbourOrder.RANDOM);
                    CurrentState = SimState.STARTED;
                } else if(kState.IsKeyDown(Keys.F3) && LastKeyState.IsKeyUp(Keys.F3)) {
                    ((DepthFirst)(PathFinder)).SetNeighbourOrder(NeighbourOrder.SMART);
                    CurrentState = SimState.STARTED;
                }
            }

            if(CurrentState == SimState.MENU_ASTAR_OPTIONS) {
                if(kState.IsKeyDown(Keys.F1) && LastKeyState.IsKeyUp(Keys.F1)) {
                    AStarTile.HScoreMultiplier = 1;
                    CurrentState = SimState.STARTED;
                } else if(kState.IsKeyDown(Keys.F2) && LastKeyState.IsKeyUp(Keys.F2)) {
                    AStarTile.HScoreMultiplier = 2;
                    CurrentState = SimState.STARTED;
                }
            }

            if(CurrentState == SimState.MENU_ALGORITHM_SELECT) {
                if(kState.IsKeyDown(Keys.F1) && LastKeyState.IsKeyUp(Keys.F1)) {
                    PathFinder = new DepthFirst(TileGrid, SelectedDirection);
                    CurrentState = SimState.MENU_DEPTHFIRST_OPTIONS;
                } else if(kState.IsKeyDown(Keys.F2) && LastKeyState.IsKeyUp(Keys.F2)) {
                    PathFinder = new AStar(TileGrid, SelectedDirection);
                    CurrentState = SimState.MENU_ASTAR_OPTIONS;
                } else if(kState.IsKeyDown(Keys.F3) && LastKeyState.IsKeyUp(Keys.F3)) {
                    TileGrid.GenRandomGrid(25);
                }
            }

            if (CurrentState == SimState.MENU_DIRECTION_SELECT)
            { 
                if(kState.IsKeyDown(Keys.F1) && LastKeyState.IsKeyUp(Keys.F1)) {
                    SelectedDirection = AllowDirection.NONDIAGONAL;
                    CurrentState = SimState.MENU_ALGORITHM_SELECT;
                } else if(kState.IsKeyDown(Keys.F2) && LastKeyState.IsKeyUp(Keys.F2)) {
                    SelectedDirection = AllowDirection.FULL;
                    CurrentState = SimState.MENU_ALGORITHM_SELECT;
                }
            }

            LastKeyState = kState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            TileGrid.Draw(spriteBatch, GridDrawArea);

            string drawString = "";
            if(CurrentState != SimState.STARTED) {
                if(TileGrid.Source == TileGridSource.FILE) {
                    drawString += "Loaded level.bmp | ";
                } else {
                    drawString += "Couldn't find level.bmp | ";
                }
            }

            if (CurrentState == SimState.MENU_DIRECTION_SELECT) {
                drawString += "Allow directions: Non-Diagonal=F1 (4d) - Full=F2 (8d)";
            } else if(CurrentState == SimState.MENU_ALGORITHM_SELECT) {
                drawString += "Depth Firs=F1 - A*=F2 - Randomize level=F3";
            } else if(CurrentState == SimState.MENU_DEPTHFIRST_OPTIONS) {
                drawString += "Standard=F1 - Random=F2 - Smart(ish)=F3";
            } else if(CurrentState == SimState.MENU_ASTAR_OPTIONS) {
                drawString += "Precise=F1 - Fast=F2";
            } else if(CurrentState == SimState.STARTED) {
                drawString += "Stepcount=" + PathFinder.StepCount + " T=" + PathFinder.TimeRunningMillis + "ms Press ENTER for instant resolve";
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(SimpleFont, drawString, new Vector2(0, 3), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void setupLevel() {
            if(File.Exists("level.bmp")) {
                System.Drawing.Bitmap img = new System.Drawing.Bitmap("level.bmp");
                TileGrid = new TileGrid(img.Width, img.Height);
                TileGrid.GenFromFile(img);
            } else {
                TileGrid = new TileGrid(100, 100);
                TileGrid.GenEmptyGrid();
            }
        }
    }

    internal enum SimState {
        MENU_DIRECTION_SELECT, MENU_ALGORITHM_SELECT, MENU_DEPTHFIRST_OPTIONS, MENU_ASTAR_OPTIONS, STARTED
    }
}