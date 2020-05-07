using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;


namespace MonoGameWindowsStarter
{
    public class Player : ISprite
    {
        /// <summary>
        /// A spritesheet containing a santa image
        /// </summary>
        Texture2D[] spritesheet;

        /// <summary>
        /// The portion of the spritesheet that is the santa
        /// </summary>
        public Rectangle sourceRect = new Rectangle
        {
            X = 0,
            Y = 0,
            Width = 407,
            Height = 536
        };

        public BoundingRectangle Bounds => new BoundingRectangle(Position - origin, 75, 144);

        /// <summary>
        /// The origin of the santa sprite
        /// </summary>
        Vector2 origin = new Vector2(-20, 1); //61

        /// <summary>
        /// The angle the santa should tilt
        /// </summary>
        float angle = 0;

        /// <summary>
        /// The player's position in the world
        /// </summary>
        public Vector2 Position = new Vector2(200, 800);

        /// <summary>
        /// How fast the player moves
        /// </summary>
        public float Speed { get; set; } = 300;

        private SpriteEffects effects = SpriteEffects.None;

        private int frameNumber = 0;

        // A timer for animations
        TimeSpan animationTimer = new TimeSpan(0);

        // The speed of the walking animation
        const int FRAME_RATE = 100;

        // The duration of a player's jump, in milliseconds
        const int JUMP_TIME = 500;

        // A timer for jumping
        TimeSpan jumpTimer = new TimeSpan(0);

        bool isJumping = false;

        float v0 = 0f;
        float a = -9.8f;
        float t = 0;

        Vector2 Velocity;

        Dictionary<float, int> seenPlatforms = new Dictionary<float, int>();

        /// <summary>
        /// Constructs a player
        /// </summary>
        /// <param name="spritesheet">The player's spritesheet</param>
        public Player(Texture2D[] spritesheet)
        {
            this.spritesheet = spritesheet;
            Velocity = Vector2.Zero;
        }

        /// <summary>
        /// Updates the player position based on GamePad or Keyboard input
        /// </summary>
        /// <param name="gameTime">The GameTime object</param>
        public void Update(GameTime gameTime)
        {
            // Override with keyboard input
            var keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A))
            {
                Position.X += (float)gameTime.ElapsedGameTime.TotalSeconds * Speed * -1;
                effects = SpriteEffects.FlipHorizontally;
            }
            if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
            {
                Position.X += (float)gameTime.ElapsedGameTime.TotalSeconds * Speed * 1;
                effects = SpriteEffects.None;
            }

            // Caclulate the tilt of the santa
            angle = 0.00f * Velocity.X;

            // Move the santa
            //Position.X += (float)gameTime.ElapsedGameTime.TotalSeconds * Speed * Velocity.X;

            // Update the frame number of the player jumping
            animationTimer += gameTime.ElapsedGameTime;
            if (animationTimer.TotalMilliseconds > FRAME_RATE * 2)
            {
                frameNumber++;
                if (frameNumber > 9)
                {
                    frameNumber = 0;
                }
                animationTimer = new TimeSpan(0);
            }

            Position.Y -= Velocity.Y * 6;
            jump(gameTime);
            keepPlayerInBounds();
        }

        /// <summary>
        /// Draws the player sprite
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Render the santa
#if VISUAL_DEBUG
            VisualDebugging.DrawRectangle(spriteBatch, Bounds, Color.Red);
#endif
            spriteBatch.Draw(spritesheet[frameNumber], Position, sourceRect, Color.White, angle, origin, 0.3f, effects, 0.7f);
        }

        private void keepPlayerInBounds()
        {
            if (Position.X < 0)
            {
                Position.X = 600;
            }

            if (Position.X > 600)
            {
                Position.X = 0;
            }
        }

        public void jump(GameTime gameTime)
        {
            if (!isJumping)
            {
                isJumping = true;
                v0 = 3;
            }
            else
            {
                Velocity.Y = (float)(v0 + (a * t));
                t += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public void CheckForPlatformCollision(IEnumerable<IBoundable> platforms, AxisList world, Random random, Sprite pix)
        {

            foreach (Platform platform in platforms)
            {
                if (Bounds.CollidesWith(platform.Bounds))
                {
                    Position.Y = platform.Bounds.Y -this.Bounds.Height - 1;
                    Velocity.Y = 0;
                    t = 0;
                    isJumping = false;

                    if (!seenPlatforms.ContainsKey(platform.Bounds.X))
                    {
                        seenPlatforms.Add(platform.Bounds.X, 1);

                        world.SpawnNewPlatforms(this, random, pix, (List<Platform>)platforms);
                        return;
                    }
                }
            }
        }
    }
}