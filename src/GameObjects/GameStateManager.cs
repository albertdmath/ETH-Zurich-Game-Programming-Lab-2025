using System;
using System.Collections.Generic;
using System.Linq;
using GameLab;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace src.GameObjects
{
    /** This class manages menu state. It looks weird cause it uses Singleton pattern. If you refactor I will cut of your prenats.
      * This holds the state of the game, active stuff and so on AND the functionality to reset it.
      */
    public class GameStateManager
    {
        private const float ARENA_SCALE = 5f;
        private const float TOMATO_SCALE = 1;
        private const float SWORDFISH_SCALE = 0.9f;
        private const float FROG_SCALE = 0.7f;
        private const float COCONUT_SCALE = 0.3f;
        private const float BANANA_SCALE = 1f;
        private const float TURTLE_SCALE = 0.3f;
        private const float MJOELNIR_SCALE = 1.5f;
        private const float SPEAR_SCALE = 0.9f;


        private const float TOMATO_HEIGHT = 0f;
        private const float SWORDFISH_HEIGHT = 0f;
        private const float FROG_HEIGHT = 0f;
        private const float COCONUT_HEIGHT = 0f;
        private const float BANANA_HEIGHT = 0f;
        private const float TURTLE_HEIGHT = 0f;
        private const float MJOELNIR_HEIGHT = 0.2f;
        private const float SPEAR_HEIGHT = 0f;


        // Model references for initializing the instances
        private DrawModel arenaModel;
        private List<DrawModel> marketModels = new List<DrawModel>();
        private List<DrawModel> playerHatModels;
        private DrawModel playerModel;
        private DrawModel playerModelShell;
        private DrawModel playerHandModel;
        private DrawModel indicatorModel;

        private List<DrawModel> mobModels;
        private List<DrawModel> areaDamageModels;
        private Dictionary<ProjectileType, DrawModel> projectileModels;
        private GameModel arena;

        // This is the mob that might or might not be angry
        private Mob mob;
        public readonly List<Player> players = new List<Player>();
        public readonly List<Player> livingPlayers = new List<Player>();
        public readonly List<Projectile> projectiles = new List<Projectile>();
        private MenuStateManager menuStateManager;
        private readonly List<AreaDamage> areaDamages = new List<AreaDamage>();
        // Singleton instancing
        private GameStateManager() { }

        private static readonly GameStateManager instance = new();

        public static GameStateManager GetGameStateManager()
        {
            return instance;
        }

        public void Initialize(DrawModel arenaModel, List<DrawModel> marketModels, List<DrawModel> playerHatModels, DrawModel playerModel, DrawModel playerModelShell, DrawModel playerHandModel, DrawModel indicatorModel,  List<DrawModel> mobModels, List<DrawModel> areaDamageModels, Dictionary<ProjectileType, DrawModel> projectileModels)
        {
            this.menuStateManager = MenuStateManager.GetMenuStateManager();
            this.arenaModel = arenaModel;
            this.marketModels = marketModels;
            this.playerHatModels = playerHatModels;
            this.playerModel = playerModel;
            this.playerHandModel = playerHandModel;
            this.mobModels = mobModels;
            this.areaDamageModels = areaDamageModels;
            this.projectileModels = projectileModels;
            this.indicatorModel = indicatorModel;
            this.playerModelShell = playerModelShell;

            arena = new GameModel(arenaModel, ARENA_SCALE);
        }

        public void InitializeMob() { mob = new Mob(mobModels); }

        public void InitializePlayers()
        {
            players.Clear();
            livingPlayers.Clear();
            float[] playerStartPositions = { -1.5f, -0.5f, 0.5f, 1.5f };
            float scaling = 0.5f;
            /*
            Input[] inputs = { new InputDual(new Input(),new InputController(PlayerIndex.One)), 
                new InputDual(new InputKeyboard(),new InputController(PlayerIndex.Two)), 
                new InputController(PlayerIndex.Three), new InputController(PlayerIndex.Four)};
            for(int i = 0; i<MenuStateManager.GetMenuStateManager().NUM_PLAYERS; ++i)
                players.Add(new Player(new Vector3(playerStartPositions[i], 0, 0), inputs[i], 0, mob.Ellipse, playerModels[i], scaling));
            SRY BOUT THAT*/
            players.Add(new Player(new Vector3(playerStartPositions[0], 0, 0), new InputControllerKeyboard(0), 0, mob.Ellipse, playerModel, playerModelShell, playerHandModel, playerHatModels[0], indicatorModel, scaling));
            //players.Add(new Player(new Vector3(playerStartPositions[1], 0, 0), new InputKeyboard(), 1, mob.Ellipse, playerModels[1], scaling));
            for(int i=1;i<menuStateManager.NUM_PLAYERS;++i){
                    players.Add(new Player(new Vector3(playerStartPositions[i], 0, 0), (GamePad.GetState(i).IsConnected) ? new InputController((PlayerIndex)i) : new InputKeyboard(),i,mob.Ellipse,playerModel, playerModelShell, playerHandModel, playerHatModels[i], indicatorModel, scaling));
            }

            foreach (Player player in players)
                livingPlayers.Add(player);
        }

        public Projectile CreateProjectile(ProjectileType type, Vector3 origin, Vector3 target)
        {
            Projectile projectile;
            switch (type)
            {
                case ProjectileType.Frog:
                    projectile = new Frog(type, origin, target, projectileModels[ProjectileType.Frog], FROG_SCALE, FROG_HEIGHT);
                    break;
                case ProjectileType.Swordfish:
                    projectile = new Swordfish(type, origin, target, projectileModels[ProjectileType.Swordfish], SWORDFISH_SCALE, SWORDFISH_HEIGHT);
                    break;
                case ProjectileType.Tomato:
                    projectile = new Tomato(type, origin, target, projectileModels[ProjectileType.Tomato], TOMATO_SCALE, TOMATO_HEIGHT);
                    break;
                case ProjectileType.Coconut:
                    projectile = new Coconut(type, origin, target, projectileModels[ProjectileType.Coconut], COCONUT_SCALE, COCONUT_HEIGHT);
                    break;
                case ProjectileType.Banana:
                    projectile = new Banana(type, origin, target, projectileModels[ProjectileType.Banana], BANANA_SCALE, BANANA_HEIGHT);
                    break;
                case ProjectileType.Turtle:
                    projectile = new Turtle(type, origin, target, projectileModels[ProjectileType.Turtle], projectileModels[ProjectileType.TurtleWalking], TURTLE_SCALE, TURTLE_HEIGHT);
                    break;
                case ProjectileType.Spear:
                    projectile = new Spear(type, origin, target, projectileModels[ProjectileType.Spear], SPEAR_SCALE, SPEAR_HEIGHT);
                    break;
                case ProjectileType.Mjoelnir:
                    projectile = new Mjoelnir(type, origin, target, projectileModels[ProjectileType.Mjoelnir], MJOELNIR_SCALE, MJOELNIR_HEIGHT);
                    break;
                default:
                    throw new ArgumentException("Invalid projectile type: ", type.ToString());
            }
            projectiles.Add(projectile);
            return projectile;
        }
        public void CreateAreaDamage(Vector3 position, float scale,Player player,ProjectileType type)
        {
            if(type == ProjectileType.Mjoelnir)
                areaDamages.Add(new AreaDamage(position,player,areaDamageModels[0],scale));
            else
                areaDamages.Add(new AreaDamage(position,player,areaDamageModels[1],scale));
        }

        public void UpdateGame(float dt)
        {

            // Update area damage
            foreach(AreaDamage areaDamage in areaDamages)
                areaDamage.updateWrap(dt);
                areaDamages.RemoveAll(x => x.ToBeDeleted);

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
                projectile.ToBeDeleted = projectile.ToBeDeleted || MathF.Abs(projectile.Position.X) > GameLabGame.ARENA_HEIGHT || MathF.Abs(projectile.Position.Z) > GameLabGame.ARENA_WIDTH;
            }
            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = i + 1; j < projectiles.Count; j++)
                {
                    if(projectiles[i].Hitbox.Intersects(projectiles[j].Hitbox))
                    {
                        projectiles[i].ToBeDeleted = projectiles[j].DestroysOtherProjectiles;
                        projectiles[j].ToBeDeleted = projectiles[i].DestroysOtherProjectiles;
                    }
                }
            }

            // Early delete projectiles for efficiency
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
            foreach (Projectile projectile in projectiles.Where(x => x.Holder == null || x.DestroysOtherProjectiles))
            {
                foreach (Player player in players.Where(x => x.Life > 0))
                {
                    if (projectile.Hitbox.Intersects(player.Hitbox))
                        projectile.OnPlayerHit(player);
                }
            }
            // Check for areaDamage-player intersections
            foreach (AreaDamage areaDamage in areaDamages.Where(x => x.timeSinceCreation <= 0.1f))
            {
                foreach (Player player in players.Where(x => x.Life > 0))
                {
                    if (areaDamage.Intersects(player))
                        player.LoseLife();
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

        public void DrawGame(Shader shadowShader, PBR lightingShader, Matrix view, Matrix projection, GraphicsDevice graphicsDevice, RenderTarget2D shadowMap)
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
                //player.Hitbox.DebugDraw(graphicsDevice, view, projection);
            }
            mob.Draw(view, projection, shadowShader, true);

            lightingShader.setShadowTexture(shadowMap);
            graphicsDevice.SetRenderTarget(null);
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            // Set background color
            graphicsDevice.Clear(Color.DeepSkyBlue);
            
            lightingShader.setMetallic(arena.DrawModel.metallic);
            lightingShader.setRoughness(arena.DrawModel.roughness);
            arena.Draw(view, projection, lightingShader, false);
            // arenaModel.Hitbox.DebugDraw(GraphicsDevice,view,projection);

            // Draw all active projectiles:
            foreach (Projectile projectile in projectiles)
            {
                lightingShader.setMetallic(projectile.DrawModel.metallic);
                lightingShader.setRoughness(projectile.DrawModel.roughness);
                projectile.Draw(view, projection, lightingShader, false);
                // projectile.Hitbox.DebugDraw(GraphicsDevice,view,projection);
            }

            // Draw all Players
            foreach (Player player in players)
            {
                if(player.playerState == Player.PlayerState.Crawling){
                    graphicsDevice.BlendState = BlendState.NonPremultiplied;
                    lightingShader.setOpacityValue(0.4f);
                }else{
                    graphicsDevice.BlendState = BlendState.Opaque;
                    lightingShader.setOpacityValue(1.0f);
                }

                lightingShader.setMetallic(player.DrawModel.metallic);
                lightingShader.setRoughness(player.DrawModel.roughness);
                player.Draw(view, projection, lightingShader, false);
                //player.Hitbox.DebugDraw(graphicsDevice, view, projection);
            }

            // Draw mob
            graphicsDevice.BlendState = BlendState.NonPremultiplied;
            lightingShader.setOpacityValue(1.0f);
            mob.Draw(view, projection, lightingShader, false);
            graphicsDevice.BlendState = BlendState.NonPremultiplied;
            lightingShader.setOpacityValue(0.2f);
            foreach(AreaDamage areaDamage in areaDamages) 
            {
                areaDamage.Draw(view, projection, lightingShader, false);
            }
            
            graphicsDevice.BlendState = BlendState.Opaque;
            lightingShader.setOpacityValue(1.0f);
            // graphicsDevice.BlendState = BlendState.NonPremultiplied;
            // lightingShader.setOpacityValue(0.7f);
            lightingShader.setRoughness(0.3f);
            lightingShader.setMetallic(0.0f);
            // graphicsDevice.BlendState = BlendState.Opaque;
            // lightingShader.setOpacityValue(1.0f);
     
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