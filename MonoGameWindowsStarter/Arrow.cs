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
    public class Arrow
    {
        Texture2D texture;

        /// <summary>
        /// Arrow bounding rectangle
        /// </summary>
        public BoundingRectangle Bounds;

        Player player;

        float speed = -15;

        bool isFlying = false;

        float release = 0;


        /// <summary>
        /// Class to represent a spider that falls from the sky
        /// </summary>
        /// <param name="game">The game the spider belongs to</param>
        /// <param name="r">Random object used for spawning</param>
        public Arrow(Player player)
        {
            this.player = player;
            Bounds.Width = 50;
            Bounds.Height = 200;
            Bounds.Y = 0;
            Bounds.X = 0;
        }

        /// <summary>
        /// Loads the spider's image from content pipeline
        /// </summary>
        /// <param name="content"></param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("arrow-re");
        }

        /// <summary>
        /// Updates the spider
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            Bounds.Y += speed;

            if (isFlying && Math.Abs(Bounds.Y) > release + 600)
            {
                 isFlying = false;
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

        public void spawnArrow()
        {
            if (!isFlying)
            {
                Bounds.X = player.Bounds.X + 10;
                Bounds.Y = player.Bounds.Y;
                release = Math.Abs(player.Bounds.Y);
                isFlying = true;
            }
        }

        /// <summary>
        /// Checks if the arrow has collided with the spider
        /// </summary>
        /// <param name="spider">Whether the arrow has collided with the spider</param>
        /// <returns></returns>
        public bool collidesWithSpider(Spider spider)
        {
            if ((spider.Bounds.X < Bounds.X + Bounds.Width) && (Bounds.X < (spider.Bounds.X + spider.Bounds.Width)) && (spider.Bounds.Y < Bounds.Y + Bounds.Height) && (Bounds.Y < spider.Bounds.Y + spider.Bounds.Height))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the arrow has collided with the bat
        /// </summary>
        /// <param name="spider">Whether the player has collided with the bat</param>
        /// <returns></returns>
        public bool collidesWithBat(Bat bat)
        {
            if ((bat.Bounds.X < Bounds.X + Bounds.Width) && (Bounds.X < (bat.Bounds.X + bat.Bounds.Width)) && (bat.Bounds.Y < Bounds.Y + Bounds.Height) && (Bounds.Y < bat.Bounds.Y + bat.Bounds.Height))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

}

