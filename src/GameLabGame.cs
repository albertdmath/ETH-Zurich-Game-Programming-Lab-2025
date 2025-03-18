using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;

//using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

//local imports
using src.GameObjects;

namespace GameLab
{
    public class GameLabGame : Game
    {
        protected static Random rng = new Random();
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;

        // Private fields:
        private Model arena, playerModel;
        private Dictionary<ProjectileType, Model> projectileModels = new Dictionary<ProjectileType, Model>();
        private RingOfDoom ring;
        private LinkedList<Projectile> activeProjectiles = new LinkedList<Projectile>();
        private LinkedList<Projectile> hitProjectiles = new LinkedList<Projectile>();

        // Player settings
        private static int NUM_PLAYERS = 2;
        private Player[] players = new Player[NUM_PLAYERS];
        private Vector3 playerSpawnOrientation = new Vector3(0,0,-1);

        // Camera settings
        private Matrix view = Matrix.CreateLookAt(new Vector3(0f, 10, 7), new Vector3(0, 0, 0), Vector3.Up);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(45f), // Field of view in radians (e.g., 45 degrees)
            16f / 9f, // Aspect ratio (change as needed)
            0.1f, // Near clipping plane
            1000f // Far clipping plane
        );

        // Arena transformations
        private Matrix arenaScaling = Matrix.CreateScale(new Vector3(0.5f));
        //private Matrix arenaTranslation = Matrix.CreateTranslation(new Vector3(0));

        // Player transformations
        private Matrix playerScaling = Matrix.CreateScale(new Vector3(1.5f));
        private Matrix playerTranslation = Matrix.CreateTranslation(new Vector3(0, 0.2f, 0));

        // Projectile transformations:
        private Matrix projectileRotation = Matrix.CreateRotationX((float)-Math.PI / 2);

        private static float timeUntilNextProjectile = 5.0f+(float)rng.NextDouble()*10; // Random interval before next projectile

        public GameLabGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        private void initializePlayers()
        {
            int n = players.Length;
            float[] playerStartPositions = { -0.75f, -0.25f, 0.25f, 0.75f };
            switch (n)
            {
                case 4:
                    players[3] = new Player(new Vector3(playerStartPositions[3], 0, 0),new InputController(PlayerIndex.Three));
                    goto case 3;
                case 3:
                    players[2] = new Player(new Vector3(playerStartPositions[2], 0, 0),new InputController(PlayerIndex.Two));
                    goto case 2;
                case 2:
                    //players[1] = new Player(new Vector3(playerStartPositions[1], 0, 0),new InputController(PlayerIndex.One));
                    players[1] = new Player(new Vector3(playerStartPositions[1], 0, 0),new InputKeyboard());
                    goto case 1;
                case 1: 
                    players[0] = new Player(new Vector3(playerStartPositions[0], 0, 0),new Input());
                    goto default;
                default:
                break;
            }
        }
        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = false; // Enable full screen
            _graphics.ApplyChanges();

            // Initialize the ring of doom:
            int planeWidth = 10, planeHeight = 10;
            this.ring = new RingOfDoom(planeWidth, planeHeight);
                
            initializePlayers();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // Load the player/projectile models
            // Textured arena model currently named test TODO change that and remove old arena model too
            arena = Content.Load<Model>("arena");
            playerModel = Content.Load<Model>("player");
            font = Content.Load<SpriteFont>("font");

            // Load the projectile models
            projectileModels.Add(ProjectileType.Frog, Content.Load<Model>("frog"));
            projectileModels.Add(ProjectileType.Swordfish, Content.Load<Model>("fish"));
            //it should be a tomato
            projectileModels.Add(ProjectileType.Tomato, Content.Load<Model>("fish"));

            // Load Sounds:
            Sounds.bgMusic = Content.Load<Song>("Audio/yoga-dogs-all-good-folks");
            MediaPlayer.Play(Sounds.bgMusic);
            MediaPlayer.IsRepeating = true;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Spawn a new projectile:
            if ((timeUntilNextProjectile -= dt) <= 0)
            {
                timeUntilNextProjectile = (float)rng.NextDouble() * 5f;

                ProjectileType[] values = Enum.GetValues(typeof(ProjectileType))
                                      .Cast<ProjectileType>()
                                      .Where(p => p != ProjectileType.None)
                                      .ToArray();
                Vector3 origin = ring.RndCircPoint();
                Vector3 direction = players[rng.Next(0, NUM_PLAYERS)].Position - origin;
                ProjectileType type = (ProjectileType)values.GetValue(rng.Next(values.Length));
                Projectile newProjectile = Projectile.createProjectile(type, origin, direction);
                if(type == ProjectileType.Frog)
                {
                    Projectile.frogCount++;
                }

                if(!(type == ProjectileType.Frog && Projectile.frogCount > Projectile.maxFrogs))
                {
                    activeProjectiles.AddLast(newProjectile);
                }
            }

            // Move all the projectiles
            foreach (Projectile projectile in activeProjectiles) projectile.Update(dt);

            // Move players
            foreach (Player player in players)
            {
                player.update(dt);
            }


            // Super basic hit detection until we can figure out bounding spheres, just using 0.5 which is quite arbitrary for now:
            foreach (Player player in players)
            {
                foreach (Projectile projectile in activeProjectiles)
                {
                    //we should decide how much distance
                    if (player.GrabOrHit(projectile, activeProjectiles))
                        hitProjectiles.AddLast(projectile);
                        //activeProjectiles.Remove(projectile);
                }
            }
            foreach (Projectile projectile in hitProjectiles)
                activeProjectiles.Remove(projectile);
            hitProjectiles = new LinkedList<Projectile>();
            // Postpone the ring closing for the low target, right now functional minimum!!
            /*
            //close the ring of doom
            ring.CloseRing(gameTime);
            this.player.Position = this.ring.RingClamp(this.player.Position);
            */
            base.Update(gameTime);
        }

        private void DrawModel(Model model, Matrix world)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = this.view;
                    effect.Projection = this.projection;

                    if(model == Content.Load<Model>("frog"))
                    {
                        Texture2D frogTexture = Content.Load<Texture2D>("Textures/frogTexture");
                        effect.Texture = frogTexture;
                        effect.TextureEnabled = true;
                    }
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
        private void DrawHealthAndStamina()
        {
            int n = players.Length;
            switch (n)
            {
                case 4:
                    _spriteBatch.DrawString(font, "Lives: " + players[3].Life + "  Stamina: " + (int)players[3].Stamina, new Vector2(1500, 950), Color.Yellow);
                    goto case 3;
                case 3:
                    _spriteBatch.DrawString(font, "Lives: " + players[2].Life + "  Stamina: " + (int)players[2].Stamina, new Vector2(10, 950), Color.Orange);
                    goto case 2;
                case 2:
                    _spriteBatch.DrawString(font, "Lives: " + players[1].Life + "  Stamina: " + (int)players[1].Stamina, new Vector2(1500, 10), Color.White);
                    goto case 1;
                case 1: 
                    _spriteBatch.DrawString(font, "Lives: " + players[0].Life + "  Stamina: " + (int)players[0].Stamina, new Vector2(10, 10), Color.Red);
                    goto default;
                default:
                break;
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DeepSkyBlue); // Background color
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // This resolves upscaling issues when going fullscreen
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Draw the ring of doom:
            //this.ring.DrawRing(_spriteBatch, Content.Load<Texture2D>("ring"));

            DrawModel(arena, arenaScaling);

            // Draw all active projectiles:
            foreach (Projectile projectile in activeProjectiles)
            {
                if(projectile.Type == ProjectileType.Frog) {
                    Matrix frogRotationMatrix = Matrix.CreateRotationY((float)(Math.PI / 2 - Math.Atan2(projectile.Orientation.Z, projectile.Orientation.X)));
                    DrawModel(projectileModels[projectile.Type], projectileRotation * frogRotationMatrix * Matrix.CreateTranslation(projectile.Position));
                }

                if(projectile.Type == ProjectileType.Swordfish) {
                    Matrix frogRotationMatrix = Matrix.CreateRotationY((float) Math.PI- (float) Math.Atan2(projectile.Orientation.Z, projectile.Orientation.X));
                    DrawModel(projectileModels[projectile.Type], projectileRotation * frogRotationMatrix * Matrix.CreateTranslation(projectile.Position));
                }
            }

            // Draw all players:
            foreach (Player player in players)
                DrawModel(playerModel, Matrix.CreateRotationY((float)Math.Atan2(-1f*player.Orientation.X,-1f*player.Orientation.Z))* Matrix.CreateTranslation(player.Position) * playerTranslation);
            //foreach (Player player in players)  
            //   Console.WriteLine(player.Position);
            DrawHealthAndStamina();
            _spriteBatch.End();

            OrientedBoundingBox obb1 = OrientedBoundingBox.ComputeOBB(arena.Meshes[15], arenaScaling);
            BoundingBoxRenderer.DrawOBB(GraphicsDevice, obb1, view, projection);

            OrientedBoundingBox obb2 =  OrientedBoundingBox.ComputeOBB(playerModel.Meshes[1], Matrix.CreateTranslation(players[0].Position) * playerTranslation * playerScaling);
            BoundingBoxRenderer.DrawOBB(GraphicsDevice, obb2 ,view, projection);

            if(obb1.Intersects(obb2)) {
                Console.WriteLine("yay");
            }
            base.Draw(gameTime);
        }
    }
}
