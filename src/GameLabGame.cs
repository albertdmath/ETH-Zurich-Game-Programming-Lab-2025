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
        private DrawModel playerModelShell;

        private DrawModel playerHandModel;
        private DrawModel indicatorModel;

        private List<DrawModel> playerHatModels = new List<DrawModel>();
        private List<DrawModel> mobModels = new List<DrawModel>();
        private List<DrawModel> areaDamageModels = new List<DrawModel>();
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
        PBR lightingShader;

        PhongShading testShader;
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
            arenaModel = new DrawModel("../../../Content/marketplace.dae",Content.Load<Model>("marketplace"),0.0f,1.0f,GraphicsDevice);
            playerModel = new DrawModel("../../../Content/Player/player_body.dae",Content.Load<Model>("Player/player_body"),0.0f,0.3f,GraphicsDevice);
            playerModelShell = new DrawModel("../../../Content/Player/player_body_shell.dae", Content.Load<Model>("Player/player_body_shell"),0.0f,0.3f,GraphicsDevice);

            playerHandModel = new DrawModel("../../../Content/Player/hand.dae", Content.Load<Model>("Player/hand"),0.0f,0.3f,GraphicsDevice);
            indicatorModel = new DrawModel("../../../Content/indicator.dae", Content.Load<Model>("indicator"),0.0f,0.3f,GraphicsDevice);
            playerHatModels.Add(new DrawModel("../../../Content/Player/player1_hat.dae", Content.Load<Model>("Player/player1_hat"),0.0f,0.3f,GraphicsDevice));
            playerHatModels.Add(new DrawModel("../../../Content/Player/player2_hat.dae", Content.Load<Model>("Player/player2_hat"),0.0f,0.3f,GraphicsDevice));
            playerHatModels.Add(new DrawModel("../../../Content/Player/player3_hat.dae", Content.Load<Model>("Player/player3_hat"),0.0f,0.3f,GraphicsDevice));
            playerHatModels.Add(new DrawModel("../../../Content/Player/player4_hat.dae",Content.Load<Model>("Player/player4_hat"),0.0f,0.3f,GraphicsDevice));

            mobModels.Add(new DrawModel("../../../Content/mob1.dae",Content.Load<Model>("mob1"),0.0f,0.3f,GraphicsDevice));
            mobModels.Add(new DrawModel("../../../Content/mob2.dae",Content.Load<Model>("mob2"),0.0f,0.3f,GraphicsDevice));
            mobModels.Add(new DrawModel("../../../Content/mob3.dae",Content.Load<Model>("mob3"),0.0f,0.3f,GraphicsDevice));

            projectileModels.Add(ProjectileType.Frog, new DrawModel("../../../Content/frog.dae",Content.Load<Model>("frog"),0.0f,0.4f,GraphicsDevice));
            projectileModels.Add(ProjectileType.Swordfish, new DrawModel("../../../Content/swordfish.dae",Content.Load<Model>("swordfish"),0.0f,0.5f,GraphicsDevice));
            projectileModels.Add(ProjectileType.Tomato, new DrawModel("../../../Content/tomato.dae",Content.Load<Model>("tomato"),0.0f,0.6f,GraphicsDevice));
            projectileModels.Add(ProjectileType.Coconut, new DrawModel("../../../Content/coconut.dae",Content.Load<Model>("coconut"),0.0f,0.9f,GraphicsDevice));
            projectileModels.Add(ProjectileType.Banana, new DrawModel("../../../Content/bananapeel.dae",Content.Load<Model>("bananapeel"),0.0f,0.9f,GraphicsDevice));
            projectileModels.Add(ProjectileType.Turtle, new DrawModel("../../../Content/turtle_shell.dae",Content.Load<Model>("turtle_shell"),0.0f,0.9f,GraphicsDevice));
            projectileModels.Add(ProjectileType.TurtleWalking, new DrawModel("../../../Content/turtle.dae",Content.Load<Model>("turtle"),0.0f,0.9f,GraphicsDevice));
            projectileModels.Add(ProjectileType.Mjoelnir, new DrawModel("../../../Content/mjoelnir.dae",Content.Load<Model>("mjoelnir"),0.0f,0.9f,GraphicsDevice));
            projectileModels.Add(ProjectileType.Spear, new DrawModel("../../../Content/trident.dae",Content.Load<Model>("trident"),0.0f,0.9f,GraphicsDevice));

            areaDamageModels.Add(new DrawModel("../../../Content/hammer_aoe.dae",Content.Load<Model>("hammer_aoe"),0.0f,0.9f,GraphicsDevice));
            areaDamageModels.Add(new DrawModel("../../../Content/tomato_aoe.dae",Content.Load<Model>("tomato_aoe"),0.0f,0.9f,GraphicsDevice));

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
            //lightingShader = new PhongShading(Content.Load<Effect>("lightingWithShadow"));
            testShader = new PhongShading(Content.Load<Effect>("lighting"));
            lightingShader = new PBR(Content.Load<Effect>("pbrShading"));
            shadowShader = new Shader(Content.Load<Effect>("shadowMap"));
            Sun = new Light(new Vector3(1.2f, 1.2f, 0.82f)*4.5f, -new Vector3(3.0f, 9.0f, 7.0f));
            shadowMap = new RenderTarget2D(_graphics.GraphicsDevice, 2048, 2048, false, SurfaceFormat.Single, DepthFormat.Depth24);

            shadowShader.setLightSpaceMatrix(Sun.lightSpaceMatrix);
            lightingShader.setLightSpaceMatrix(Sun.lightSpaceMatrix);
            lightingShader.setCameraPosition(cameraPosition);
            lightingShader.setViewMatrix(view);
            lightingShader.setProjectionMatrix(projection);
            lightingShader.setLight(Sun);
            lightingShader.setOpacityValue(1.0f);

            testShader.setCameraPosition(cameraPosition);
            testShader.setViewMatrix(view);
            testShader.setProjectionMatrix(projection);
            testShader.setLight(Sun);

            // Initialize gamestate here:
            gameStateManager.Initialize(arenaModel, playerHatModels, playerModel, playerModelShell, playerHandModel, indicatorModel, mobModels, areaDamageModels, projectileModels);
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
            // gameStateManager.ShaderTest(testShader,view,projection,GraphicsDevice);
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
