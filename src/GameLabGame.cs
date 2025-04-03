using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using src.GameObjects;

namespace GameLab
{
    public class GameLabGame : Game
    {
        private MyMenu _menu;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        private KeyboardState _previousKeyboardState;
        // Private fields:
        private DrawModel arenaModel;
        private List<DrawModel> playerModels = new List<DrawModel>();
        private List<DrawModel> mobModels = new List<DrawModel>();
        public static Dictionary<ProjectileType, DrawModel> projectileModels = new Dictionary<ProjectileType, DrawModel>();

        private GameStateManager gameStateManager;
        private MenuStateManager menuStateManager;

        // TODO remove this stuff with the HUD update (no hearts no stamina no playernames no wife maidenless actually)
        Texture2D playerHearts;
        private Color[] playerColors = {
            new Color(118, 254, 253), // Player 1 color (cyan)
            new Color(254, 144, 209), // Player 2 color (pink)
            new Color(209, 255, 42), // Player 3 color (green)
            new Color(254, 131, 22) // Player 4 color (orange)
        };

        // Shader variables for shading shadows
        RenderTarget2D shadowMap;
        private Light Sun;
        PBR lightingShader;
        Shader shadowShader;

        // Camera settings
        private Vector3 cameraPosition = new Vector3(0f, 9, 7);
        private Matrix view = Matrix.CreateLookAt(new Vector3(0f, 9, 7), new Vector3(0, 0, 0.7f), Vector3.Up);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(45f), // Field of view in radians (e.g., 45 degrees)
            16f / 9f, // Aspect ratio (change as needed)
            0.1f, // Near clipping plane
            1000f // Far clipping plane
        );

        // Arena settings
        public const float ARENA_HEIGHT = 10f, ARENA_WIDTH = 17f;

        public GameLabGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = false; // Enable full screen
            _graphics.ApplyChanges();

            // Get Gamestatemanager instance yay and Menustatemanager too wahoo
            menuStateManager = MenuStateManager.GetMenuStateManager();
            gameStateManager = GameStateManager.GetGameStateManager();
           

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load all of the models
            arenaModel = new DrawModel(Content.Load<Model>("arena"),0.0f,1.0f);

            playerModels.Add(new DrawModel(Content.Load<Model>("player1"),0.0f,0.7f));
            playerModels.Add(new DrawModel(Content.Load<Model>("player2"),0.0f,0.7f));
            playerModels.Add(new DrawModel(Content.Load<Model>("player3"),0.0f,0.7f));
            playerModels.Add(new DrawModel(Content.Load<Model>("player4"),0.0f,0.7f));

            mobModels.Add(new DrawModel(Content.Load<Model>("mob1"),0.0f,0.7f));
            mobModels.Add(new DrawModel(Content.Load<Model>("mob2"),0.0f,0.7f));

            projectileModels.Add(ProjectileType.Frog, new DrawModel(Content.Load<Model>("frog"),0.0f,0.4f));
            projectileModels.Add(ProjectileType.Swordfish, new DrawModel(Content.Load<Model>("swordfish"),0.0f,0.5f));
            projectileModels.Add(ProjectileType.Tomato, new DrawModel(Content.Load<Model>("tomato"),0.0f,0.6f));
            projectileModels.Add(ProjectileType.Coconut, new DrawModel(Content.Load<Model>("coconut"),0.0f,0.9f));
            // This should be a banana
            projectileModels.Add(ProjectileType.Banana, new DrawModel(Content.Load<Model>("coconut"),0.0f,0.9f));
            // This should be a turtle
            projectileModels.Add(ProjectileType.Turtle, new DrawModel(Content.Load<Model>("frog"),0.0f,0.4f));

            font = Content.Load<SpriteFont>("font");
            playerHearts = Content.Load<Texture2D>("player_heart");

            // Shader setup
            //lightingShader = new PhongShading(Content.Load<Effect>("lightingWithShadow"));
            lightingShader = new PBR(Content.Load<Effect>("pbrShading"));
            shadowShader = new Shader(Content.Load<Effect>("shadowMap"));
            Sun = new Light(new Vector3(0.99f, 0.98f, 0.82f)*7.0f, -new Vector3(3.0f, 9.0f, 7.0f));
            shadowMap = new RenderTarget2D(_graphics.GraphicsDevice, 2048, 2048, false, SurfaceFormat.Single, DepthFormat.Depth24);

            shadowShader.setLightSpaceMatrix(Sun.lightSpaceMatrix);
            lightingShader.setLightSpaceMatrix(Sun.lightSpaceMatrix);
            lightingShader.setCameraPosition(cameraPosition);
            lightingShader.setViewMatrix(view);
            lightingShader.setProjectionMatrix(projection);
            lightingShader.setLight(Sun);
            lightingShader.setOpacityValue(1.0f);

            // Initialize gamestate here:
            gameStateManager.Initialize(arenaModel, playerModels, mobModels, projectileModels);


            gameStateManager.StartNewGame();

            _menu = new MyMenu(this);

            // Load Sounds:
            MusicAndSoundEffects.loadSFX(Content);
        }


        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            _menu.Update(gameTime, keyboardState, _previousKeyboardState);

            if (_menu.menuisopen())
            {
                _previousKeyboardState = keyboardState;
                base.Update(gameTime);
                return;
            }

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            gameStateManager.UpdateGame(dt);

            _previousKeyboardState = keyboardState;

            base.Update(gameTime);
        }



        private void DrawHealthAndStamina()
        {
            switch (gameStateManager.players.Count)
            {
                case 4:
                    _spriteBatch.DrawString(font, "Player state " + (int)gameStateManager.players[3].Stamina, new Vector2(1500, 950), playerColors[3]);
                    for (int i = 0; i < gameStateManager.players[3].Life; i++)
                    {
                        _spriteBatch.Draw(playerHearts, new Vector2(1500 + 60 * i, 910), null, Color.White, 0f, Vector2.Zero, 0.15f, SpriteEffects.None, 0f);
                    }
                    goto case 3;
                case 3:
                    _spriteBatch.DrawString(font, "Stamina: " + (int)gameStateManager.players[2].Stamina, new Vector2(10, 950), playerColors[2]);
                    for (int i = 0; i < gameStateManager.players[2].Life; i++)
                    {
                        _spriteBatch.Draw(playerHearts, new Vector2(10 + 60 * i, 910), null, Color.White, 0f, Vector2.Zero, 0.15f, SpriteEffects.None, 0f);
                    }
                    goto case 2;
                case 2:
                    _spriteBatch.DrawString(font, "Stamina: " + (int)gameStateManager.players[1].Stamina, new Vector2(1500, 50), playerColors[1]);
                    for (int i = 0; i < gameStateManager.players[1].Life; i++)
                    {
                        _spriteBatch.Draw(playerHearts, new Vector2(1500 + 60 * i, 10), null, Color.White, 0f, Vector2.Zero, 0.15f, SpriteEffects.None, 0f);
                    }
                    goto case 1;
                case 1:
                    _spriteBatch.DrawString(font, "Player state  " + gameStateManager.players[0].playerState, new Vector2(10, 50), playerColors[0]);
                    for (int i = 0; i < gameStateManager.players[0].Life; i++)
                    {
                        _spriteBatch.Draw(playerHearts, new Vector2(10 + 60 * i, 10), null, Color.White, 0f, Vector2.Zero, 0.15f, SpriteEffects.None, 0f);
                    }
                    goto default;
                default:
                    break;
            }
        }


        /*
        private void DrawWin()
        {
            //this can be also used for the hit projectiles
            var alivePlayers = gameStateManager.players.Where(p => p.Life > 0).ToList();
            if (alivePlayers.Count() == 1)
            {
                Texture2D pixel;
                pixel = new Texture2D(_graphics.GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });

                // Define text
                string winMessage = "Player " + (alivePlayers[0].Id+1) + " wins!";

                // Measure text size
                Vector2 textSize = font.MeasureString(winMessage);
                Vector2 textPosition = 
                    new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width/2 - 100, 
                                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height/2 - 50);

                // Define background rectangle position and size
                Vector2 padding = new Vector2(20, 10); // Add some padding around text
                Rectangle backgroundRect = new Rectangle(
                    (int)(0),
                    (int)(textPosition.Y - 50),
                    (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width),
                    (int)(textSize.Y + 100)
                );
                _spriteBatch.Draw(pixel, backgroundRect, Color.Black * 0.5f); // Semi-transparent black
                _spriteBatch.DrawString(font, winMessage, textPosition, Color.Gold);
            }
        }

        */

        protected override void Draw(GameTime gameTime)
        {
            gameStateManager.DrawGame(shadowShader, lightingShader, view, projection, GraphicsDevice, shadowMap);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            DrawHealthAndStamina();
            // Draw menu
            _menu.Draw();

            _spriteBatch.End();
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            base.Draw(gameTime);
        }
    }
}
