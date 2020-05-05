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
        Texture2D spritesheet;

        /// <summary>
        /// The portion of the spritesheet that is the santa
        /// </summary>
        public Rectangle sourceRect = new Rectangle
        {
            X = 0,
            Y = 0,
            Width = 259,
            Height = 171
        };

        /// <summary>
        /// The origin of the santa sprite
        /// </summary>
        Vector2 origin = new Vector2(130, 1);

        /// <summary>
        /// The angle the santa should tilt
        /// </summary>
        float angle = 0;

        /// <summary>
        /// The player's position in the world
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// How fast the player moves
        /// </summary>
        public float Speed { get; set; } = 300;

        private SpriteEffects effects = SpriteEffects.None;

        /// <summary>
        /// Constructs a player
        /// </summary>
        /// <param name="spritesheet">The player's spritesheet</param>
        public Player(Texture2D spritesheet)
        {
            this.spritesheet = spritesheet;
            this.Position = new Vector2(200, 200);
        }

        /// <summary>
        /// Updates the player position based on GamePad or Keyboard input
        /// </summary>
        /// <param name="gameTime">The GameTime object</param>
        public void Update(GameTime gameTime)
        {
            Vector2 direction = Vector2.Zero;

            // Use GamePad for input
            var gamePad = GamePad.GetState(0);

            // The thumbstick value is a vector2 with X & Y between [-1f and 1f] and 0 if no GamePad is available
            direction.X = gamePad.ThumbSticks.Left.X;

            // We need to inverty the Y axis
            direction.Y = -gamePad.ThumbSticks.Left.Y;

            // Override with keyboard input
            var keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A))
            {
                direction.X -= 1;
                effects = SpriteEffects.FlipHorizontally;
            }
            if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
            {
                direction.X += 1;
                effects = SpriteEffects.None;
            }
            if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W))
            {
                direction.Y -= 1;
            }
            if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S))
            {
                direction.Y += 1;
            }

            // Caclulate the tilt of the santa
            angle = 0.3f * direction.X;

            // Move the santa
            Position += (float)gameTime.ElapsedGameTime.TotalSeconds * Speed * direction;
        }

        /// <summary>
        /// Draws the player sprite
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Render the santa
            spriteBatch.Draw(spritesheet, Position, sourceRect, Color.White, angle, origin, 1f, effects, 0.7f);
        }

    }
}