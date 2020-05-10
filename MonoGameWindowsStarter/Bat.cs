using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace MonoGameWindowsStarter
{
    public class Bat
    {
        Game1 game;

        /// <summary>
        /// spider's texture
        /// </summary>
        Texture2D texture;

        /// <summary>
        /// random for the spawing of the spider
        /// </summary>
        Random random;

        /// <summary>
        /// spider's bounding rectangle
        /// </summary>
        public BoundingRectangle Bounds;

        /// <summary>
        /// game width and height
        /// </summary>
        int gameWidth = 600;
        int gameHeight = 1024;

        /// <summary>
        /// Class to represent a spider that falls from the sky
        /// </summary>
        /// <param name="game">The game the spider belongs to</param>
        /// <param name="r">Random object used for spawning</param>
        public Bat(Game1 game, Random r)
        {
            this.game = game;
            random = r;
        }

        /// <summary>
        /// Initlizes bounding rectangle
        /// </summary>
        public void Initialize()
        {
            Bounds.Width = 45;
            Bounds.Height = 45;
            Bounds.Y = 0;
            Bounds.X = RandomizeX();//randomize
        }

        /// <summary>
        /// Loads the spider's image from content pipeline
        /// </summary>
        /// <param name="content"></param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("bat");
        }

        /// <summary>
        /// Updates the spider
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            Bounds.Y += random.Next(2, 4);

            if (Bounds.Y > (gameHeight - (int)Bounds.Height))
            {
                Bounds.Y = 0;
                Bounds.X = RandomizeX();
                if (Bounds.X + Bounds.Width > gameWidth)
                {
                    Bounds.X = gameWidth - Bounds.Width;
                }
                if (Bounds.X < 0)
                {
                    Bounds.X = 0;
                }
            }
        }

        /// <summary>
        /// Draws the spider onto the screen
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Bounds, Color.White);
        }

        /// <summary>
        /// Randomizes the spiders X location
        /// </summary>
        /// <returns></returns>
        public int RandomizeX()
        {
            int temp;
            temp = random.Next(0, gameWidth - (int)Bounds.Width);
            return temp;
        }

        /// <summary>
        /// Sends the spider back up to the top
        /// </summary>
        public void SendBack()
        {
            this.Initialize();
        }

    }

}

