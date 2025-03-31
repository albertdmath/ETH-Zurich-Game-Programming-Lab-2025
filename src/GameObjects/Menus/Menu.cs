using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
//WE DONT NEED THIS IF WE WILL USE MYRA ANYWAY
namespace src.GameObjects{
    public class Menu{

        private bool isActive;
        private SpriteFont font;
        private Texture2D backgroundTexture;
        private Rectangle backgroundRectangle;
        private GraphicsDevice graphicsDevice;
        public Menu(SpriteFont font, GraphicsDevice graphicsDevice){
            this.graphicsDevice = graphicsDevice;
            this.font = font;
            isActive = false;

            backgroundTexture = new Texture2D(graphicsDevice,1,1);
            backgroundTexture.SetData(new[] {new Color(0,0,0,150)});
            backgroundRectangle = new Rectangle(200,200,600,400);
        }
        
        public void Update(KeyboardState keyboardState, KeyboardState previousKeyboardState){
            if(keyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter)){
                isActive=!isActive;
            }
        }
        public void Draw(SpriteBatch spriteBatch){
            if(!isActive)return;
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture,backgroundRectangle,Color.Wheat);

            spriteBatch.DrawString(font, "PAUSE", new Vector2(350,150),Color.Red);
            spriteBatch.DrawString(font, "Press Enter to Resume", new Vector2(280, 250), Color.White);
            spriteBatch.DrawString(font, "Press Esc to Quit", new Vector2(300, 300), Color.White);
            spriteBatch.End();
        }
        public bool IsActive(){
            return isActive;
        }
    }
}