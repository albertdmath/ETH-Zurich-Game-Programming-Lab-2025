using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using src.GameObjects;


/* HUD that shows player hp and potentially other information though we prefer to keep things visual on the player models;
 * Can be scaled through the menuStateManager TO SOME DEGREE:
 * menuStateManager.HUD_SCALE multiplications are for that, the big constants next to them are trial and error fix positions
 */
public class HUD
{
    private readonly MenuStateManager menuStateManager = MenuStateManager.GetMenuStateManager();
    private readonly GameStateManager gameStateManager = GameStateManager.GetGameStateManager();
    private List<Texture2D> playerHP; // heart models
    private List<Texture2D> playerHats; // hat models for fashion
    private Texture2D hudBackground; // backdrop, might need to be changed cause idk color theory
    private Texture2D winMessage;
    private Texture2D mainBanner;
    private Texture2D tutorial;

    private bool boo = false;

    private int boomCounter = 0;

List<Texture2D> countdown = new List<Texture2D>();

    private float timePassed = 0;

        int screenWidth;
    int screenHeight;
    private List<Vector2> offsets = new List<Vector2>(); // we put the 4 corner positions as offsets in here
    public HUD(List<Texture2D> playerHP, List<Texture2D> playerHats, Texture2D hudBackground, Texture2D winMessage,  List<Texture2D> countdown, Texture2D mainBanner, Texture2D tutorial, int screenWidth, int screenHeight) 
    {
        this.playerHP = playerHP;
        this.playerHats = playerHats;
        this.hudBackground = hudBackground;
        this.winMessage = winMessage;
        this.screenWidth = screenWidth;
        this.screenHeight = screenHeight;
        this.mainBanner = mainBanner;
        this.tutorial = tutorial;
        offsets.Add(new Vector2(50,40));
        offsets.Add(new Vector2(screenWidth - 4300*menuStateManager.HUD_SCALE,40));
        offsets.Add(new Vector2(50, screenHeight - 300));
        offsets.Add(new Vector2(screenWidth - 4300*menuStateManager.HUD_SCALE, screenHeight-300));
        this.countdown = countdown;
    }

    // Draws the player HUD depending on the number of players, 
    public void DrawPlayerHud(SpriteBatch spriteBatch)
    {
        if(!menuStateManager.MAIN_MENU_IS_OPEN)
        {
            for(int i = 0; i < menuStateManager.NUM_PLAYERS; i++)
            {
                // Draws backdrop
                //spriteBatch.Draw(hudBackground, offsets[i], null, Color.White, 0f, Vector2.Zero, menuStateManager.HUD_SCALE * 2f, SpriteEffects.None, 0f);
                // Draws player hats
                spriteBatch.Draw(playerHats[i], offsets[i] - new Vector2(400*menuStateManager.HUD_SCALE, 0) , null, Color.White, 0f, Vector2.Zero, menuStateManager.HUD_SCALE, SpriteEffects.None, 0f);
                // Draws player hearts
                for (int j = 1; j <= gameStateManager.players[i].Life; j++)
                {
                    spriteBatch.Draw(playerHP[i], offsets[i] + new Vector2(600*menuStateManager.HUD_SCALE*j + 466*menuStateManager.HUD_SCALE, 0), null, Color.White, 0f, Vector2.Zero, menuStateManager.HUD_SCALE, SpriteEffects.None, 0f);
                }
            }
        }
    }
    
        public void resetCountdown(){
            this.timePassed = 0;
            this.boo = false;
            this.boomCounter = 0;
        }


    public void DrawTutorial(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(tutorial, new Vector2(0,0), null, Color.White, 0f, Vector2.Zero, screenWidth/2560.0f, SpriteEffects.None, 0f);
    }
    public bool DrawBanner(SpriteBatch spriteBatch, GraphicsDevice graphics)
    {

        if (menuStateManager.MAIN_MENU_IS_OPEN)
        {
            // Get a random ass texture so we use it as background for the backdrop
            Texture2D pixel = new Texture2D(graphics, 1, 1);
            pixel.SetData(new[] { Color.White });


            // Black transparent backdrop
            //spriteBatch.Draw(pixel, backgroundRectangle, Color.Black * 0.45f);
            // Winning message, just did a png cause it's easier to design this way
            spriteBatch.Draw(mainBanner, new Vector2(screenWidth/2 - 800, screenHeight/3 - 500), Color.White); 
            // Draw player hat of the correct color so people know *who* won
            return true;
        }
        return false;
    }

    public bool DrawCountdown(SpriteBatch spriteBatch, GraphicsDevice graphics, float dt){
        
            // Get a random ass texture so we use it as background for the backdrop
            Texture2D pixel = new Texture2D(graphics, 1, 1);
            pixel.SetData(new[] { Color.White });

            Rectangle backgroundRectangle = new Rectangle(
                0,
                screenHeight/3-250,
                screenWidth,
                400
            );
            timePassed += dt;
            
            if(timePassed < 1){
                       // Black transparent backdrop
            spriteBatch.Draw(pixel, backgroundRectangle, Color.Transparent);
            spriteBatch.Draw(countdown[0], new Vector2(screenWidth/2 - 600, screenHeight/3 - 400), Color.White); 
            // Draw player hat of the correct color so people know *who* won
                        if(boomCounter == 0){
                MusicAndSoundEffects.playBoomSFX();
                boomCounter++;
            }
            return false;
            } else if( timePassed >= 1 && timePassed <2){
            // Black transparent backdrop
            spriteBatch.Draw(pixel, backgroundRectangle, Color.Transparent);

            spriteBatch.Draw(countdown[1], new Vector2(screenWidth/2 - 600, screenHeight/3 - 400), Color.White); 

                if(boomCounter == 1){
                MusicAndSoundEffects.playBoomSFX();
                boomCounter++;
            }
            // Draw player hat of the correct color so people know *who* won

            return false;
            } else if (timePassed >= 2 && timePassed <3){
            // Black transparent backdrop
            spriteBatch.Draw(pixel, backgroundRectangle, Color.Transparent);

            spriteBatch.Draw(countdown[2], new Vector2(screenWidth/2 - 600, screenHeight/3 - 400), Color.White); 
                        if(boomCounter == 2){
                MusicAndSoundEffects.playBoomSFX();
                boomCounter++;
            }
            return false;
            } else  if (timePassed >= 3 && timePassed <5){
                  spriteBatch.Draw(pixel, backgroundRectangle, Color.Transparent);
            // Black transparent backdro
            spriteBatch.Draw(countdown[3], new Vector2(screenWidth/2 - 600, screenHeight/3 - 400), Color.White); 
            if(!boo){
                MusicAndSoundEffects.playBooSFX();
                boo = true;
            }

                return false;
            }    else {
                resetCountdown();
                return true;
            }
          


    }



    // Draws the winning screen, can be improved in design for sure and also it doesn't scale with screensize but I dont feel like it rn
    public bool DrawWin(SpriteBatch spriteBatch, GraphicsDevice graphics)
    {

        if (gameStateManager.GameIsOver())
        {
            // Get a random ass texture so we use it as background for the backdrop
            Texture2D pixel = new Texture2D(graphics, 1, 1);
            pixel.SetData(new[] { Color.White });

            Rectangle backgroundRectangle = new Rectangle(
                0,
                screenHeight/5,
                screenWidth,
                (int) (800 * (screenWidth/2560.0f))
            );

            // Black transparent backdrop
            spriteBatch.Draw(pixel, backgroundRectangle, Color.Black * 0.45f);
            // Winning message, just did a png cause it's easier to design this way
            spriteBatch.Draw(winMessage, new Vector2(screenWidth/2 - 800, screenHeight/3 - 300), Color.White); 
            // Draw player hat of the correct color so people know *who* won
            spriteBatch.Draw(playerHats[gameStateManager.livingPlayers[0].Id],  new Vector2(screenWidth/2 + (250* (screenWidth/2560.0f)), screenHeight/3-100), null, Color.White, 0f, Vector2.Zero, menuStateManager.HUD_SCALE*4 * (screenWidth/2560.0f), SpriteEffects.None, 0f);
            spriteBatch.Draw(playerHats[gameStateManager.livingPlayers[0].Id],  new Vector2(screenWidth/2 - (1450* (screenWidth/2560.0f)), screenHeight/3-100), null, Color.White, 0f, Vector2.Zero, menuStateManager.HUD_SCALE*4* (screenWidth/2560.0f), SpriteEffects.None, 0f);
            return true;
        }
        return false;
    }
}