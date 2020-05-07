using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace MonoGameWindowsStarter
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D backgroundTexture;
        Rectangle backgroundRect;

        Player player;

        List<Platform> platforms;
        AxisList world;

        Random random = new Random();

        Sprite pix;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            platforms = new List<Platform>();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //background image is 200 x 512
            graphics.PreferredBackBufferWidth = 600;  //1.5 X width
            graphics.PreferredBackBufferHeight = 1024;  //1.5 X height
            graphics.ApplyChanges();

            backgroundRect.Width = graphics.PreferredBackBufferWidth;  //width of frame
            backgroundRect.Height = graphics.PreferredBackBufferHeight;  //height of frame
            backgroundRect.X = 0;
            backgroundRect.Y = 0;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

#if VISUAL_DEBUG
            VisualDebugging.LoadContent(Content);
#endif

            // Background
            backgroundTexture = Content.Load<Texture2D>("Background");

            // Player
            Texture2D[] playerTextures = new Texture2D[10];
            for (int i = 0; i < 10; i++)
            {
                playerTextures[i] = Content.Load<Texture2D>("Jump__00" + i);
            }
            player = new Player(playerTextures);

            // Platforms
            Texture2D pixel = Content.Load<Texture2D>("pixel");
            pix = new Sprite(new Rectangle(0, 0, 100, 25), pixel);
            platforms.Add(new Platform(new BoundingRectangle(0, 999, 600, 25), pix));

            // Add the platforms to the axis list
            world = new AxisList();
            foreach (Platform platform in platforms)
            {
                world.AddGameObject(platform);
            }

            world.SpawnNewPlatforms(player, random, pix, platforms);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            player.Update(gameTime);
            player.CheckForPlatformCollision(platforms, world, random, pix);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            Vector2 offset = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, player.Position.Y) - player.Position;
            var t = Matrix.CreateTranslation(0, offset.Y, 0);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, t);
            //spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, backgroundRect, Color.White);

            // Draw the player
            player.Draw(spriteBatch, gameTime);

            // Draw the platforms 
            platforms.ForEach(platform =>
            {
                platform.Draw(spriteBatch);
            });

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
