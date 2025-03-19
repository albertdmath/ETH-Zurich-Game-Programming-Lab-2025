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
        public static GameModel arenaModel;
        public static Dictionary<ProjectileType, Model> projectileModels = new Dictionary<ProjectileType, Model>();
        private RingOfDoom ring;
        private LinkedList<Projectile> activeProjectiles = new LinkedList<Projectile>();
        private LinkedList<Projectile> hitProjectiles = new LinkedList<Projectile>();

        // Player settings
        private static int NUM_PLAYERS = 3;
        public static Player[] players = new Player[NUM_PLAYERS];
        private LinkedList<Player> activePlayers = new LinkedList<Player>();
        private List<Zombie> zombies = new List<Zombie>();
        private List<Zombie>[] sortedZombies = new List<Zombie>[24*24];
        private Vector3 playerSpawnOrientation = new Vector3(0,0,-1);

        // Camera settings
        private Matrix view = Matrix.CreateLookAt(new Vector3(0f, 9, 7), new Vector3(0, 0, 1), Vector3.Up);
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

        // Projectile transformations:
        private Matrix projectileRotation = Matrix.CreateRotationX((float)-Math.PI / 2);

        private static float timeUntilNextProjectile = 5.0f+(float)rng.NextDouble()*10; // Random interval before next projectile

        private Ellipse innerEllipse = new Ellipse(7.2f,3.8f);
        private Ellipse outerEllipse = new Ellipse(7.5f,4f);
        private float totalTimePassed = 0f;

        Random random;//random variable for zombies

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
                    players[3] = new Player(new Vector3(playerStartPositions[3], 0.2f, 0),new InputController(PlayerIndex.Three),innerEllipse,playerModel);
                    goto case 3;
                case 3:
                    players[2] = new Player(new Vector3(playerStartPositions[2], 0.2f, 0),new InputController(PlayerIndex.Two),innerEllipse,playerModel);
                    goto case 2;
                case 2:
                    players[1] = new Player(new Vector3(playerStartPositions[1], 0.2f, 0),new InputController(PlayerIndex.One),innerEllipse,playerModel);
                    goto case 1;
                case 1: 
                    players[0] = new Player(new Vector3(playerStartPositions[0], 0.2f, 0),new Input(),innerEllipse,playerModel);
                    goto default;
                default:
                break;
            }
            for(int i = 0; i<n;i++)
                activePlayers.AddLast(players[i]);
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
                
            base.Initialize();
            random = new Random();
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
            initializePlayers();
            arenaModel = new GameModel(arena);
            arenaModel.Transform = arenaScaling;
            arenaModel.Hitbox.Transform(arenaScaling);
            // Load Sounds:
            /* Sounds.bgMusic = Content.Load<Song>("Audio/yoga-dogs-all-good-folks");
            MediaPlayer.Play(Sounds.bgMusic);
            MediaPlayer.IsRepeating = true; */
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            totalTimePassed+=dt;
            if(totalTimePassed>20f){
                float a = 7.5f - 0.1f*((float)Math.Round(totalTimePassed-20f));
                a = a < 0.5f ? 0.5f : a;
                float b = 4f - 0.05f*((float)Math.Round(totalTimePassed-20f));
                b = b < 0.5f ? 0.5f : b;
                innerEllipse.Set(a-0.2f,b-0.2f);
                outerEllipse.Set(a,b);
            }
            if(zombies.Count<700){
                float randomFloat = (float)(random.NextDouble() *2f* Math.PI);
                zombies.Add(new Zombie(new Vector3(9f*(float)Math.Sin(randomFloat),0.2f,8f*(float)Math.Cos(randomFloat)),outerEllipse,playerModel));
            }
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
                Projectile newProjectile = Projectile.createProjectile(type, origin, direction,projectileModels[type]);
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
            foreach (Projectile projectile in activeProjectiles) projectile.updateWrap(dt);
            sortedZombies = new List<Zombie>[24*24];
            for(int i = 0;i<24*24;i++)
                sortedZombies[i] = new List<Zombie>();
            foreach (Zombie zombie in zombies) sortedZombies[(int)Math.Round(zombie.Position.X)+11+((int)Math.Round(zombie.Position.Y)+11)*24].Add(zombie);
            // MOve all zombies(mob)
            for(int j = 0; j<23;j++){
                for(int i = 0; i<23;i++){
                    List<Zombie> tempList = sortedZombies[i+j*24];
                    for(int k=0; k<tempList.Count;++k){
                        tempList[k].Force(tempList,k);
                        tempList[k].Force(sortedZombies[i+j*24+1],-1);
                        tempList[k].Force(sortedZombies[i+j*24+24],-1);
                        tempList[k].Force(sortedZombies[i+j*24+25],-1);
                    }
                }
            }
            foreach (Zombie zombie in zombies) zombie.updateWrap(dt);

            // Move players
            foreach (Player player in players)
            {
                player.updateWrap(dt);
                if(player.projectileHeld != null && !activeProjectiles.Contains(player.projectileHeld))
                    activeProjectiles.AddLast(player.projectileHeld);
            }


            // Super basic hit detection until we can figure out bounding spheres, just using 0.5 which is quite arbitrary for now:
            foreach (Player player in activePlayers)
            {
                foreach (Projectile projectile in activeProjectiles)
                {
                    //we should decide how much distance
                    if (player.GrabOrHit(projectile))
                        hitProjectiles.AddLast(projectile);
                }
            }
            foreach (Projectile projectile in hitProjectiles)
                activeProjectiles.Remove(projectile);
            foreach (Player player in players)
                if(player.Life<=0) activePlayers.Remove(player);
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
                    //_spriteBatch.DrawString(font, "Lives: " + players[0].Position + "  Hitbox: " + players[0].Hitbox.BoundingBoxes[0].Center, new Vector2(10, 10), Color.Red);
                    goto default;
                default:
                break;
            }
        }

        private void DrawWin(){
            if(activePlayers.Count == 1){
                int n = 0;
                for(int i = 0; i<players.Length;i++)
                    n = activePlayers.Contains(players[i]) ? i:n;
                players[n].notImportant = true;
                _spriteBatch.DrawString(font, "Player " + n + "  wins!", new Vector2(750, 475), Color.Gold);
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

            //DrawModel(arena, arenaScaling);
            arenaModel.Draw(view,projection);
            arenaModel.Hitbox.DebugDraw(GraphicsDevice,view,projection);
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
                projectile.Hitbox.DebugDraw(GraphicsDevice,view,projection);
            }

            // Draw all players:
            foreach (Player player in players){
                player.Draw(view,projection);
                player.Hitbox.DebugDraw(GraphicsDevice,view,projection);
            } 
            foreach (Zombie zombie in zombies)
                zombie.Draw(view,projection);
            //   Console.WriteLine(player.Position);
            DrawHealthAndStamina();
            DrawWin();
            _spriteBatch.End();

            OrientedBoundingBox obb1 = OrientedBoundingBox.ComputeOBB(arena.Meshes[15], arenaScaling);
            BoundingBoxRenderer.DrawOBB(GraphicsDevice, obb1, view, projection);

            OrientedBoundingBox obb2 =  OrientedBoundingBox.ComputeOBB(playerModel.Meshes[1],  Matrix.CreateRotationY((float)Math.Atan2(-1f*players[0].Orientation.X,-1f*players[0].Orientation.Z)) * Matrix.CreateTranslation(players[0].Position) );
            BoundingBoxRenderer.DrawOBB(GraphicsDevice, obb2 ,view, projection);

            if(obb1.Intersects(obb2)) {
                Console.WriteLine("yay");
            }
            base.Draw(gameTime);
        }
    }
}
