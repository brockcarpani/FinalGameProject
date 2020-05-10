using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Audio;

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

        Spider spider;
        Bat bat;

        Sprite pix;

        Arrow arrow;

        float lastPlatformY = 0;

        int score = 0;

        float spiderDelay;
        bool updateSpider = false;
        float batDelay;
        bool updateBat = false;
        bool drawSpider = false;
        bool drawBat = false;
        
        int spawnLocation;

        SpriteFont font;

        SoundEffect indianaJonesMusic;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            platforms = new List<Platform>();
            spider = new Spider(this, random);
            bat = new Bat(this, random);
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
            backgroundRect.Height = 4608; //graphics.PreferredBackBufferHeight;  //height of frame
            backgroundRect.X = 0;
            backgroundRect.Y = -2560; //0;

            spiderDelay = 10; //10 seconds
            batDelay = 20; //20 seconds

            spider.Initialize();
            bat.Initialize();

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

            // Load and play music
            indianaJonesMusic = Content.Load<SoundEffect>("Indiana Jones");
            indianaJonesMusic.Play();

            // Load font
            font = Content.Load<SpriteFont>("font");

            // Background
            backgroundTexture = Content.Load<Texture2D>("Background-3584");

            // Player
            Texture2D[] playerTextures = new Texture2D[10];
            for (int i = 0; i < 10; i++)
            {
                playerTextures[i] = Content.Load<Texture2D>("Jump__00" + i);
            }
            player = new Player(playerTextures);

            // Arrow
            arrow = new Arrow(player);
            arrow.LoadContent(Content);

            // Platforms
            Texture2D pixel = Content.Load<Texture2D>("sand");
            pix = new Sprite(new Rectangle(0, 0, 100, 25), pixel);
            platforms.Add(new Platform(new BoundingRectangle(-100, 999, 1000, 25), pix));

            // Add the platforms to the axis list
            world = new AxisList();
            foreach (Platform platform in platforms)
            {
                world.AddGameObject(platform);
            }

            //Spider
            spider.LoadContent(Content);
            //Bat
            bat.LoadContent(Content);

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

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                arrow.spawnArrow();
            }

            // TODO: Add your update logic here
            player.Update(gameTime);

            spawnLocation = (int)player.Bounds.Y - 650;

            float tempPlatformY = lastPlatformY;
            lastPlatformY = player.CheckForPlatformCollision(platforms, world, random, pix, lastPlatformY); //

            if (tempPlatformY != lastPlatformY)
            {
                score++;
            }
            spawnLocation = (int)player.Bounds.Y - 650;
            ////////////////////////////////
            // Timer logic and spider update - starts updating (falling) after 15 seconds
            var timer = (float)gameTime.ElapsedGameTime.TotalSeconds;
            spiderDelay -= timer;
            if (spiderDelay <= 0)
            {
                drawSpider = true;
                updateSpider = true;
            }
            if (updateSpider)
            {
                spider.Update(gameTime);
            }
            ///////////////////////////////
            
            ////////////////////////////////
            // Timer logic and bat update - starts updating (falling) after 25 seconds
            var time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            batDelay -= time;
            if (batDelay <= 0)
            {
                drawBat = true;
                updateBat = true;
            }
            if (updateBat)
            {
                bat.Update(gameTime);
            }
            ///////////////////////////////

            arrow.Update(gameTime);

            if (player.isAboveSpider(spider) || arrow.collidesWithSpider(spider))
            {
                spider.Bounds.Y = spawnLocation;
                spider.Bounds.X = RandomizeEnemy(); 
            }
            if (player.collidesWithSpider(spider))
            {
                Console.WriteLine("Game Over");
            }
            if (player.isAboveBat(bat) || arrow.collidesWithBat(bat))
            {
                bat.Bounds.Y = spawnLocation;
                bat.Bounds.X = RandomizeEnemy();
            }
            if (player.collidesWithBat(bat))
            {
                Console.WriteLine("Game Over");
            }

            if (player.Bounds.Y <= -2000)
            {
                restartLevel();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            Vector2 offset = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2) - new Vector2(player.Position.X, lastPlatformY - 300);
            var t = Matrix.CreateTranslation(0, offset.Y, 0);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, t);
           
            spriteBatch.Draw(backgroundTexture, backgroundRect, Color.White);

            spriteBatch.DrawString(font, "Score: " + score, new Vector2(0, lastPlatformY - 800), Color.White);

            // Draw the player
            player.Draw(spriteBatch, gameTime);

            // Draw the arrow
            arrow.Draw(spriteBatch);

            // Draw the spider
            if (drawSpider)
            {
                spider.Draw(spriteBatch);
            }
            //Draw the bat
            if (drawBat)
            {
                bat.Draw(spriteBatch);
            }
            

            // Draw the platforms 
            platforms.ForEach(platform =>
            {
                platform.Draw(spriteBatch);
            });

            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Randomizes an ememy respawn X value
        /// </summary>
        /// <returns>the x value for the randomized position</returns>
        private int RandomizeEnemy()
        {
            int temp = random.Next(0, graphics.PreferredBackBufferWidth - (int)spider.Bounds.Width);
            return temp;
        }

        private void restartLevel()
        {
            Initialize();

            platforms = new List<Platform>();

            platforms.Add(new Platform(new BoundingRectangle(-100, 999, 1000, 25), pix));

            // Add the platforms to the axis list
            world = new AxisList();
            foreach (Platform platform in platforms)
            {
                world.AddGameObject(platform);
            }
        }
    }
}
