using System;
using System.Collections.Generic;
using System.Linq;
using GameLab;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace src.GameObjects
{
    /** This class manages menu state. It looks weird cause it uses Singleton pattern. If you refactor I will cut of your prenats.
      * This holds the state of the game, active stuff and so on AND the functionality to reset it.
      */
    public class GameStateManager
    {
        private const float ARENA_SCALE = 0.5f;
        private const float TOMATO_SCALE = 1;
        private const float SWORDFISH_SCALE = 0.9f;
        private const float FROG_SCALE = 0.7f;
        private const float COCONUT_SCALE = 0.3f;
        private const float BANANA_SCALE = 0.3f;
        private const float TURTLE_SCALE = 0.8f;


        // Model references for initializing the instances
        private DrawModel arenaModel;
        private List<DrawModel> playerModels;
        private List<DrawModel> mobModels;
        private Dictionary<ProjectileType, DrawModel> projectileModels;
        private GameModel arena;

        // This is the mob that might or might not be angry
        private Mob mob;
        public readonly List<Player> players = new List<Player>();
        public readonly List<Player> livingPlayers = new List<Player>();
        public readonly List<Projectile> projectiles = new List<Projectile>();


        // Singleton instancing
        private GameStateManager() { }

        private static readonly GameStateManager instance = new();

        public static GameStateManager GetGameStateManager()
        {
            return instance;
        }

        public void Initialize(DrawModel arenaModel, List<DrawModel> playerModels, List<DrawModel> mobModels, Dictionary<ProjectileType, DrawModel> projectileModels)
        {
            this.arenaModel = arenaModel;
            this.playerModels = playerModels;
            this.mobModels = mobModels;
            this.projectileModels = projectileModels;

            arena = new GameModel(arenaModel, ARENA_SCALE);
        }

        public void InitializeMob() { mob = new Mob(mobModels); }

        public void InitializePlayers()
        {
            players.Clear();
            livingPlayers.Clear();
            float[] playerStartPositions = { -1.5f, -0.5f, 0.5f, 1.5f };
            float scaling = 0.5f;

            players.Add(new Player(new Vector3(playerStartPositions[0], 0, 0), new Input(), 0, mob.Ellipse, playerModels[0], scaling));
            players.Add(new Player(new Vector3(playerStartPositions[1], 0, 0), new InputKeyboard(), 1, mob.Ellipse, playerModels[1], scaling));

            foreach (Player player in players)
                livingPlayers.Add(player);
        }

        public Projectile CreateProjectile(ProjectileType type, Vector3 origin, Vector3 target)
        {
            Projectile projectile;
            switch (type)
            {
                case ProjectileType.Frog:
                    projectile = new Frog(type, origin, target, projectileModels[ProjectileType.Frog], FROG_SCALE);
                    break;
                case ProjectileType.Swordfish:
                    projectile = new Swordfish(type, origin, target, projectileModels[ProjectileType.Swordfish], SWORDFISH_SCALE);
                    break;
                case ProjectileType.Tomato:
                    projectile = new Tomato(type, origin, target, projectileModels[ProjectileType.Tomato], TOMATO_SCALE);
                    break;
                case ProjectileType.Coconut:
                    projectile = new Coconut(type, origin, target, projectileModels[ProjectileType.Coconut], COCONUT_SCALE);
                    break;
                case ProjectileType.Banana:
                    projectile = new Banana(type, origin, target, projectileModels[ProjectileType.Banana], BANANA_SCALE);
                    break;
                case ProjectileType.Turtle:
                    projectile = new Turtle(type, origin, target, projectileModels[ProjectileType.Turtle], TURTLE_SCALE);
                    break;
                default:
                    throw new ArgumentException("Invalid projectile type: ", type.ToString());
            }
            projectiles.Add(projectile);
            return projectile;
        }

        public void UpdateGame(float dt)
        {
            // Move Players
            foreach (Player player in players)
                player.updateWrap(dt);

            // Players bumping into each other
            for (int i = 0; i < players.Count; i++)
                for (int j = i + 1; j < players.Count; j++)
                    players[i].PlayerCollision(players[j]);

            // Update mob
            mob.Update(dt);

            // Move the projectiles
            foreach (Projectile projectile in projectiles)
                projectile.updateWrap(dt);


            // Check for projectile out of bounds and remove
            foreach (Projectile projectile in projectiles)
            {
                // Check if the projectile is out of bounds
                if (MathF.Abs(projectile.Position.X) > GameLabGame.ARENA_HEIGHT || MathF.Abs(projectile.Position.Z) > GameLabGame.ARENA_WIDTH)
                    projectile.ToBeDeleted = true;
                    
            }
            
            projectiles.RemoveAll(x => x.ToBeDeleted);
            
            // Check for projectile-player hand intersections
            foreach (Projectile projectile in projectiles)
            {
                foreach (Player player in players)
                {
                    if (player.Hand.IsCatching && projectile.Hitbox.Intersects(player.Hand.Hitbox))
                        player.Catch(projectile);
                }
            }
            // Check for projectile-player intersections
            foreach (Projectile projectile in projectiles.Where(x => x.Holder == null))
            {
                foreach (Player player in players.Where(x => x.Life > 0))
                {
                    if (projectile.Hitbox.Intersects(player.Hitbox))
                        projectile.OnPlayerHit(player);
                }
            }
            // Check for projectile-mob intersection TODO
            foreach (Projectile projectile in projectiles)
            {
                if (mob.Ellipse.Outside(projectile.Position.X, projectile.Position.Z))
                    projectile.OnMobHit();
            }

            // Check for projectile-ground intersection
            foreach (Projectile projectile in projectiles)
            {
                if (projectile.Position.Y < 0f)
                    projectile.OnGroundHit();
            }

            // Delete stuff again
            projectiles.RemoveAll(x => x.ToBeDeleted);
        }

        public void DrawGame(Shader shadowShader, PhongShading lightingShader, Matrix view, Matrix projection, GraphicsDevice graphicsDevice, RenderTarget2D shadowMap)
        {
            graphicsDevice.SetRenderTarget(shadowMap);
            graphicsDevice.Clear(Color.Black);
            graphicsDevice.RasterizerState = RasterizerState.CullClockwise;

            arena.Draw(view, projection, shadowShader, true);
            // arenaModel.Hitbox.DebugDraw(GraphicsDevice,view,projection);

            // Draw all active projectiles
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(view, projection, shadowShader, true);
                // projectile.Hitbox.DebugDraw(GraphicsDevice,view,projection);
            }

            // Draw all players
            foreach (Player player in players)
            {
                player.Draw(view, projection, shadowShader, true);
                // player.Hitbox.DebugDraw(GraphicsDevice, view, projection);
            }
            mob.Draw(view, projection, shadowShader, true);

            lightingShader.setShadowTexture(shadowMap);
            graphicsDevice.SetRenderTarget(null);
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            // Set background color
            graphicsDevice.Clear(Color.DeepSkyBlue);

            arena.Draw(view, projection, lightingShader, false);
            // arenaModel.Hitbox.DebugDraw(GraphicsDevice,view,projection);

            // Draw all active projectiles:
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(view, projection, lightingShader, false);
                // projectile.Hitbox.DebugDraw(GraphicsDevice,view,projection);
            }

            // Draw all Players
            foreach (Player player in players)
            {
                player.Draw(view, projection, lightingShader, false);
                // player.Hitbox.DebugDraw(GraphicsDevice, view, projection);
            }

            // Draw mob
            graphicsDevice.BlendState = BlendState.NonPremultiplied;
            lightingShader.setOpacityValue(0.7f);
            mob.Draw(view, projection, lightingShader, false);
            graphicsDevice.BlendState = BlendState.Opaque;
            lightingShader.setOpacityValue(1.0f);
     
        }

        public void StartNewGame()
        {
            players.Clear();
            projectiles.Clear();

            InitializeMob();
            InitializePlayers();
        }
    }
}