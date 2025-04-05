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
        private DrawModel playerModel;
        private DrawModel playerHandModel;

        private List<DrawModel> playerHatModels = new List<DrawModel>();
        private List<DrawModel> mobModels = new List<DrawModel>();
        public static Dictionary<ProjectileType, DrawModel> projectileModels = new Dictionary<ProjectileType, DrawModel>();
        private List<Texture2D> playerHP = new List<Texture2D>();
        private List<Texture2D> playerHats = new List<Texture2D>();
        private Texture2D hudBackground;
        private Texture2D winMessage;

        private GameStateManager gameStateManager;
        private MenuStateManager menuStateManager;

        private HUD hud;

        // Shader variables for shading shadows
        RenderTarget2D shadowMap;
        private Light Sun;
        PhongShading lightingShader;
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
            arenaModel = new DrawModel(Content.Load<Model>("arena"));
            playerModel = new DrawModel(Content.Load<Model>("Player/player_body"));
            playerHandModel = new DrawModel(Content.Load<Model>("Player/hand"));

            playerHatModels.Add(new DrawModel(Content.Load<Model>("Player/player1_hat")));
            playerHatModels.Add(new DrawModel(Content.Load<Model>("Player/player2_hat")));
            playerHatModels.Add(new DrawModel(Content.Load<Model>("Player/player3_hat")));
            playerHatModels.Add(new DrawModel(Content.Load<Model>("Player/player4_hat")));

            mobModels.Add(new DrawModel(Content.Load<Model>("mob1")));
            mobModels.Add(new DrawModel(Content.Load<Model>("mob2")));
            mobModels.Add(new DrawModel(Content.Load<Model>("mob3")));

            projectileModels.Add(ProjectileType.Frog, new DrawModel(Content.Load<Model>("frog")));
            projectileModels.Add(ProjectileType.Swordfish, new DrawModel(Content.Load<Model>("swordfish")));
            projectileModels.Add(ProjectileType.Tomato, new DrawModel(Content.Load<Model>("tomato")));
            projectileModels.Add(ProjectileType.Coconut, new DrawModel(Content.Load<Model>("coconut")));
            // This should be a banana
            projectileModels.Add(ProjectileType.Banana, new DrawModel(Content.Load<Model>("bananapeel")));
            // This should be a turtle
            projectileModels.Add(ProjectileType.Turtle, new DrawModel(Content.Load<Model>("turtle_shell")));
            projectileModels.Add(ProjectileType.TurtleWalking, new DrawModel(Content.Load<Model>("turtle")));


            font = Content.Load<SpriteFont>("font");
            
            playerHP.Add(Content.Load<Texture2D>("HUD/blue_heart"));
            playerHP.Add(Content.Load<Texture2D>("HUD/pink_heart"));
            playerHP.Add(Content.Load<Texture2D>("HUD/green_heart"));
            playerHP.Add(Content.Load<Texture2D>("HUD/yellow_heart"));

            playerHats.Add(Content.Load<Texture2D>("HUD/hat1"));
            playerHats.Add(Content.Load<Texture2D>("HUD/hat2"));
            playerHats.Add(Content.Load<Texture2D>("HUD/hat3"));
            playerHats.Add(Content.Load<Texture2D>("HUD/hat4"));

            hudBackground = Content.Load<Texture2D>("HUD/HUD_background");
            winMessage = Content.Load<Texture2D>("HUD/win_message");

            // Shader setup
            lightingShader = new PhongShading(Content.Load<Effect>("lightingWithShadow"));
            shadowShader = new Shader(Content.Load<Effect>("shadowMap"));
            Sun = new Light(new Vector3(0.99f, 0.98f, 0.82f), -new Vector3(3.0f, 9.0f, 7.0f));
            shadowMap = new RenderTarget2D(_graphics.GraphicsDevice, 2048, 2048, false, SurfaceFormat.Single, DepthFormat.Depth24);

            shadowShader.setLightSpaceMatrix(Sun.lightSpaceMatrix);
            lightingShader.setLightSpaceMatrix(Sun.lightSpaceMatrix);
            lightingShader.setCameraPosition(cameraPosition);
            lightingShader.setViewMatrix(view);
            lightingShader.setProjectionMatrix(projection);
            lightingShader.setLight(Sun);
            lightingShader.setOpacityValue(0.5f);

            // Initialize gamestate here:
            gameStateManager.Initialize(arenaModel, playerHatModels, playerModel, playerHandModel, mobModels, projectileModels);
            gameStateManager.StartNewGame();

            _menu = new MyMenu(this);

            hud = new HUD(playerHP, playerHats, hudBackground, winMessage, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);

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

        protected override void Draw(GameTime gameTime)
        {
            gameStateManager.DrawGame(shadowShader, lightingShader, view, projection, GraphicsDevice, shadowMap);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            hud.DrawPlayerHud(_spriteBatch);
            hud.DrawWin(_spriteBatch, GraphicsDevice);
            // Draw menu
            _menu.Draw();

            _spriteBatch.End();
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            base.Draw(gameTime);
        }
    }
}
