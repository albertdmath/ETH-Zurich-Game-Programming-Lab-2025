using System;
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
        private List<DrawModel> marketModels = new List<DrawModel>();
        private DrawModel playerModel;
        private DrawModel playerModelShell;
        private DrawModel walkingTurtle;

        private DrawModel playerHandModel;
        private DrawModel indicatorModel;

        private DrawModel jesterAnimated;

        private List<DrawModel> playerHatModels = new List<DrawModel>();
        private List<DrawModel> mobModels = new List<DrawModel>();
        private List<DrawModel> areaDamageModels = new List<DrawModel>();
        public static Dictionary<ProjectileType, DrawModel> projectileModels = new Dictionary<ProjectileType, DrawModel>();
        private List<Texture2D> playerHP = new List<Texture2D>();
        private List<Texture2D> playerHats = new List<Texture2D>();
        private Texture2D hudBackground;
        private Texture2D winMessage;

        private RenderTarget2D depthMap;


        //DEFERRED SHADING
        private RenderTarget2D posMap;
        private RenderTarget2D normalMap;
        private RenderTarget2D albedoMap;
        private RenderTarget2D roughnessMetallicMap;



        //HBAO

        private RenderTarget2D HBAOmap;
        private RenderTarget2D HBAOBlurredMap;

        private VertexBuffer fullscreenVertexBuffer;

        private GameStateManager gameStateManager;
        private MenuStateManager menuStateManager;



        private HUD hud;

        // Shader variables for shading shadows
        RenderTarget2D shadowMap;
        private Light Sun;
        PBR lightingShader;

        PhongShading testShader;
        Shader shadowShader;

        Shader geometryShader;

        Shader depthMapShader;


        //HBAO
        private HBAOShader hBAOShader;

        private Filter HBAOFilter;

        private Texture2D ditherTex;

        // Camera settings
        private Vector3 cameraPosition = new Vector3(0f, 9, 7);
        private Matrix view = Matrix.CreateLookAt(new Vector3(0f, 9, 7), new Vector3(0, 0, 0.7f), Vector3.Up);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(45f), // Field of view in radians (e.g., 45 degrees)
            16f / 9f, // Aspect ratio (change as needed)
            0.1f, // Near clipping plane
            1000f // Far clipping plane
        );

        private Matrix viewInverse;

        private void generateFullScreenVertexBuffer()
        {
            VertexPositionTexture[] fullscreenQuad = new VertexPositionTexture[6] {
                    new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
                    new VertexPositionTexture(new Vector3(-1,  1, 0), new Vector2(0, 0)),
                    new VertexPositionTexture(new Vector3( 1,  1, 0), new Vector2(1, 0)),

                    new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
                    new VertexPositionTexture(new Vector3( 1,  1, 0), new Vector2(1, 0)),
                    new VertexPositionTexture(new Vector3( 1, -1, 0), new Vector2(1, 1)),
                };
            fullscreenVertexBuffer = new VertexBuffer(
            GraphicsDevice,
            typeof(VertexPositionTexture),
            fullscreenQuad.Length,
            BufferUsage.WriteOnly
        );

            fullscreenVertexBuffer.SetData(fullscreenQuad);
        }

        private void Create4x4DitherTexture(GraphicsDevice device)
        {
            const int size = 4;
            var tex = new Texture2D(device, size, size, false, SurfaceFormat.Color);
            var data = new Color[size * size];
            var rnd = new Random(12345);  // seed for reproducibility

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    // 1) random angle in [0, 2π)
                    float angle = (float)(rnd.NextDouble() * Math.PI * 2.0);
                    // 2) random jitter in [0,1]
                    float jitter = (float)rnd.NextDouble();

                    // encode cos/sin into [0,1]
                    float u = 0.5f + 0.5f * (float)Math.Cos(angle);
                    float v = 0.5f + 0.5f * (-(float)Math.Sin(angle));

                    // pack into a Color (RGBA8 UNORM)
                    data[y * size + x] = new Color(u, v, jitter, 0);
                }
            }

            tex.SetData(data);
            this.ditherTex = tex;
        }

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
            // _graphics.PreferMultiSampling = true; // Enable MSAA
            // _graphics.GraphicsProfile = GraphicsProfile.HiDef; // Needed for MSAA > 0
            _graphics.ApplyChanges();
            Console.WriteLine("MultiSampling supported: " + GraphicsDevice.PresentationParameters.MultiSampleCount);

            // Get Gamestatemanager instance yay and Menustatemanager too wahoo
            menuStateManager = MenuStateManager.GetMenuStateManager();
            gameStateManager = GameStateManager.GetGameStateManager();


            base.Initialize();
        }



        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            generateFullScreenVertexBuffer();
            // Load all of the models
            arenaModel = new DrawModel("../../../Content/marketplace.dae", 0.0f, 1.0f, GraphicsDevice);
            playerModel = new DrawModel("../../../Content/Player/player_body.dae", 0.0f, 0.3f, GraphicsDevice);
            playerModelShell = new DrawModel("../../../Content/Player/player_body_shell.dae", 0.0f, 0.3f, GraphicsDevice);
            jesterAnimated = new DrawModel("../../../Content/Player/jester_animated.gltf", 0.0f, 0.3f, GraphicsDevice);
            playerHandModel = new DrawModel("../../../Content/Player/hand.dae", 0.0f, 0.3f, GraphicsDevice);
            indicatorModel = new DrawModel("../../../Content/indicator.dae", 0.0f, 0.3f, GraphicsDevice);
            playerHatModels.Add(new DrawModel("../../../Content/Player/player1_hat.dae", 0.0f, 0.3f, GraphicsDevice));
            playerHatModels.Add(new DrawModel("../../../Content/Player/player2_hat.dae", 0.0f, 0.3f, GraphicsDevice));
            playerHatModels.Add(new DrawModel("../../../Content/Player/player3_hat.dae", 0.0f, 0.3f, GraphicsDevice));
            playerHatModels.Add(new DrawModel("../../../Content/Player/player4_hat.dae", 0.0f, 0.3f, GraphicsDevice));

            marketModels.Add(new DrawModel("../../../Content/market_1.dae",0.0f,0.3f, GraphicsDevice));
            marketModels.Add(new DrawModel("../../../Content/market_2.dae",0.0f,0.3f, GraphicsDevice));
            marketModels.Add(new DrawModel("../../../Content/market_3.dae",0.0f,0.3f, GraphicsDevice));
            marketModels.Add(new DrawModel("../../../Content/market_4.dae",0.0f,0.3f, GraphicsDevice));

            mobModels.Add(new DrawModel("../../../Content/mob1.dae", 0.0f, 0.6f, GraphicsDevice));
            mobModels.Add(new DrawModel("../../../Content/mob2.dae", 0.0f, 0.6f, GraphicsDevice));
            mobModels.Add(new DrawModel("../../../Content/mob3.dae", 0.0f, 0.6f, GraphicsDevice));

            projectileModels.Add(ProjectileType.Frog, new DrawModel("../../../Content/frog.dae", 0.0f, 0.4f, GraphicsDevice));
            projectileModels.Add(ProjectileType.Swordfish, new DrawModel("../../../Content/swordfish.dae", 0.0f, 0.5f, GraphicsDevice));
            projectileModels.Add(ProjectileType.Tomato, new DrawModel("../../../Content/tomato.dae", 0.0f, 0.6f, GraphicsDevice));
            projectileModels.Add(ProjectileType.Coconut, new DrawModel("../../../Content/coconut.dae", 0.0f, 0.9f, GraphicsDevice));
            projectileModels.Add(ProjectileType.Banana, new DrawModel("../../../Content/bananapeel.dae", 0.0f, 0.9f, GraphicsDevice));
            projectileModels.Add(ProjectileType.Turtle, new DrawModel("../../../Content/turtle_shell.dae", 0.0f, 0.9f, GraphicsDevice));
            projectileModels.Add(ProjectileType.Mjoelnir, new DrawModel("../../../Content/mjoelnir.dae", 0.0f, 0.9f, GraphicsDevice));
            projectileModels.Add(ProjectileType.Spear, new DrawModel("../../../Content/trident.dae", 0.0f, 0.9f, GraphicsDevice));
            projectileModels.Add(ProjectileType.Chicken, new DrawModel("../../../Content/rooster.dae", 0.0f, 0.9f, GraphicsDevice));

            walkingTurtle = new DrawModel("../../../Content/turtle.dae", 0.0f, 0.9f, GraphicsDevice);

            areaDamageModels.Add(new DrawModel("../../../Content/hammer_aoe.dae", 0.0f, 0.9f, GraphicsDevice));
            areaDamageModels.Add(new DrawModel("../../../Content/tomato_aoe.dae", 0.0f, 0.3f, GraphicsDevice));

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
            viewInverse = Matrix.Invert(view);
            // Shader setup
            //lightingShader = new PhongShading(Content.Load<Effect>("lightingWithShadow"));
            testShader = new PhongShading(Content.Load<Effect>("lighting"));
            lightingShader = new PBR(Content.Load<Effect>("pbrShading"));
            shadowShader = new Shader(Content.Load<Effect>("shadowMap"));
            depthMapShader = new Shader(Content.Load<Effect>("depthMap"));
            geometryShader = new Shader(Content.Load<Effect>("GeometryPass"));
            hBAOShader = new HBAOShader(Content.Load<Effect>("HBAO"));
            HBAOFilter = new Filter(Content.Load<Effect>("OcclusionBlur"));
            hBAOShader.setStrengthPerRay(0.1875f);
            hBAOShader.setFalloff(0.25f);
            hBAOShader.setDitherScale(GraphicsDevice.PresentationParameters.BackBufferWidth / 4.0f);
            hBAOShader.setBias(0.25f);
            Vector2 renderTargetResolution = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
            hBAOShader.SetRenderTargetResolution(renderTargetResolution);
            //hBAOShader.setupSampleDirections();
            Create4x4DitherTexture(GraphicsDevice);
            hBAOShader.setDitherTexture(ditherTex);

            HBAOFilter.setFilterSize(4);
            HBAOFilter.SetRenderTargetResolution(renderTargetResolution);
            Sun = new Light(new Vector3(1.2f, 1.2f, 0.82f), -new Vector3(3.0f, 9.0f, 7.0f));
            shadowMap = new RenderTarget2D(_graphics.GraphicsDevice, 4096, 4096, false, SurfaceFormat.Single, DepthFormat.Depth24);

            posMap = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            normalMap = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            albedoMap = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            roughnessMetallicMap = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Vector2, DepthFormat.Depth24);
            HBAOmap = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
            HBAOBlurredMap = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Vector4, DepthFormat.None);


            shadowShader.setLightSpaceMatrix(Sun.lightSpaceMatrix);
            lightingShader.setLight(Sun);
            lightingShader.setViewInverse(viewInverse);
            lightingShader.setLightSpaceMatrix(Sun.lightSpaceMatrix);
            lightingShader.setViewMatrix(view);

            testShader.setCameraPosition(cameraPosition);
            testShader.setViewMatrix(view);
            testShader.setProjectionMatrix(projection);
            testShader.setLight(Sun);


            geometryShader.setViewMatrix(view);
            geometryShader.setProjectionMatrix(projection);
            geometryShader.setOpacityValue(1.0f);

            // Initialize gamestate here:
            gameStateManager.Initialize(arenaModel, marketModels, playerHatModels, playerModel, playerModelShell, playerHandModel, indicatorModel, mobModels, areaDamageModels, projectileModels, walkingTurtle);
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
            RenderTargetBinding[] targets = {
                new RenderTargetBinding(posMap),
                new RenderTargetBinding(normalMap),
                new RenderTargetBinding(albedoMap),
                new RenderTargetBinding(roughnessMetallicMap)
            };
            //gameStateManager.DepthMapPass(depthMapShader, view, projection, GraphicsDevice, depthMap, _spriteBatch, true);
            gameStateManager.GeometryPass(geometryShader, shadowShader, view, projection, GraphicsDevice, shadowMap, targets, _spriteBatch, false);
            gameStateManager.HBAOPass(hBAOShader, posMap, normalMap, HBAOmap, fullscreenVertexBuffer, GraphicsDevice, _spriteBatch, false);
            gameStateManager.FilterPass(HBAOFilter, HBAOmap, normalMap, posMap, HBAOBlurredMap, GraphicsDevice, fullscreenVertexBuffer, _spriteBatch, false);
           gameStateManager.DrawGame(lightingShader, GraphicsDevice, fullscreenVertexBuffer, posMap, normalMap, albedoMap, roughnessMetallicMap, shadowMap, HBAOBlurredMap, _spriteBatch, false);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            hud.DrawPlayerHud(_spriteBatch);
            hud.DrawWin(_spriteBatch, GraphicsDevice);
            // Draw menu
            _menu.Draw();

            _spriteBatch.End();
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap; // or whatever your 3D renderer expects
            GraphicsDevice.SetRenderTarget(null); // go back to backbuffer

            base.Draw(gameTime);
        }
    }
}
