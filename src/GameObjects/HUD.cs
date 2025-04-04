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
    int screenWidth;
    int screenHeight;
    private List<Vector2> offsets = new List<Vector2>(); // we put the 4 corner positions as offsets in here
    public HUD(List<Texture2D> playerHP, List<Texture2D> playerHats, Texture2D hudBackground, Texture2D winMessage, int screenWidth, int screenHeight) 
    {
        this.playerHP = playerHP;
        this.playerHats = playerHats;
        this.hudBackground = hudBackground;
        this.winMessage = winMessage;
        this.screenWidth = screenWidth;
        this.screenHeight = screenHeight;
        offsets.Add(new Vector2(50,40));
        offsets.Add(new Vector2(screenWidth - 3400*menuStateManager.HUD_SCALE,40));
        offsets.Add(new Vector2(50, screenHeight - 200));
        offsets.Add(new Vector2(screenWidth - 3400*menuStateManager.HUD_SCALE, screenHeight-200));
    }

    // Draws the player HUD depending on the number of players, 
    public void DrawPlayerHud(SpriteBatch spriteBatch)
    {
        for(int i = 0; i < menuStateManager.NUM_PLAYERS; i++)
        {
            // Draws backdrop
            spriteBatch.Draw(hudBackground, offsets[i], null, Color.White, 0f, Vector2.Zero, menuStateManager.HUD_SCALE, SpriteEffects.None, 0f);
            // Draws player hats
            spriteBatch.Draw(playerHats[i], offsets[i] - new Vector2(400*menuStateManager.HUD_SCALE, 0) , null, Color.White, 0f, Vector2.Zero, menuStateManager.HUD_SCALE, SpriteEffects.None, 0f);
            // Draws player hearts
            for (int j = 1; j <= gameStateManager.players[i].Life; j++)
            {
                spriteBatch.Draw(playerHP[i], offsets[i] + new Vector2(600*menuStateManager.HUD_SCALE*j + 466*menuStateManager.HUD_SCALE, 0), null, Color.White, 0f, Vector2.Zero, menuStateManager.HUD_SCALE, SpriteEffects.None, 0f);
            }
        }
    }


    // Draws the winning screen, can be improved in design for sure and also it doesn't scale with screensize but I dont feel like it rn
    public void DrawWin(SpriteBatch spriteBatch, GraphicsDevice graphics)
    {

        if (gameStateManager.livingPlayers.Count() == 1)
        {
            // Get a random ass texture so we use it as background for the backdrop
            Texture2D pixel = new Texture2D(graphics, 1, 1);
            pixel.SetData(new[] { Color.White });

            Rectangle backgroundRectangle = new Rectangle(
                0,
                screenHeight/2-250,
                screenWidth,
                400
            );

            // Black transparent backdrop
            spriteBatch.Draw(pixel, backgroundRectangle, Color.Black * 0.5f);
            // Winning message, just did a png cause it's easier to design this way
            spriteBatch.Draw(winMessage, new Vector2(screenWidth/2 - 750, screenHeight/2 - 300), Color.White); 
            // Draw player hat of the correct color so people know *who* won
            spriteBatch.Draw(playerHats[gameStateManager.livingPlayers[0].Id],  new Vector2(screenWidth/2 + 150, screenHeight/2 - 265), null, Color.White, 0f, Vector2.Zero, menuStateManager.HUD_SCALE*3, SpriteEffects.None, 0f);
            spriteBatch.Draw(playerHats[gameStateManager.livingPlayers[0].Id],  new Vector2(screenWidth/2 - 1050, screenHeight/2 - 265), null, Color.White, 0f, Vector2.Zero, menuStateManager.HUD_SCALE*3, SpriteEffects.None, 0f);
        }
    }
}