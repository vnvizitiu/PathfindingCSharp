using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Pathfinding {
    public class Game1 : Microsoft.Xna.Framework.Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        TileGrid TileGrid;

        public static SpriteFont SimpleFont;
        public static Texture2D EmptyPixel;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 730;
            graphics.PreferredBackBufferWidth = 700;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
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

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            TileGrid.Draw(spriteBatch, 0, 30, 700, 700);

            string drawString = "";

            if(TileGrid.Source == TileGridSource.FILE) {
                drawString += "Loaded level.bmp";
            } else {
                drawString += "Couldn't find level.bmp";
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(SimpleFont, drawString, new Vector2(0, 0), Color.White);
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
                TileGrid.GenRandomGrid(25);
            }
        }
    }
}