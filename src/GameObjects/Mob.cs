using GameLab;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace src.GameObjects
{
    public class Mob
    {
        // Zombies
        private const int N_ZOMBIES = 150;
        public static Zombie[] active = new Zombie[N_ZOMBIES];
        private List<Zombie>[] sortedZombies = new List<Zombie>[24 * 24];

        private const float MIN_DISTANCE_OPACITY = 1f;      // distance for min opacity
        private const float MAX_DISTANCE_OPACITY = 5f;     // distance for full opacity
        private const float MIN_OPACITY = 0.3f;     // minimum opacity

        // Ellipse properties
        private float timeAlive;
        private const float TIME_BETWEEN_CLOSING = 15f;
        private const float CLOSING_TIME = 5f;
        private const int N_CLOSINGS = 5;
        public Ellipse Ellipse { get; private set; }
        private float startMajorAxis, startMinorAxis;
        private float endMajorAxis, endMinorAxis = 3f;
        private Vector3 endCenter;

        private List<DrawModel> models;
        private float closing = 0;
        private float timeUntilNextProjectile;

        private GameStateManager gameStateManager;

        public Mob(List<DrawModel> models) {
            // Set the major and minor axes of the ellipse
            this.startMajorAxis = GameLabGame.ARENA_WIDTH * 0.5f; // Half the width of the plane
            this.startMinorAxis = GameLabGame.ARENA_HEIGHT * 0.5f; // Half the height of the plane
            this.endMajorAxis = this.endMinorAxis * GameLabGame.ARENA_WIDTH / GameLabGame.ARENA_HEIGHT;
            //inside the ellipse
            this.endCenter = GetRandomPointInside();
            // Create the ellipse
            this.Ellipse = new Ellipse(startMajorAxis, startMinorAxis, Vector3.Zero);

            this.models = models;
            SpawnMob();

            gameStateManager = GameStateManager.GetGameStateManager();
        }

        private Vector3 GetRandomPointInside()
        {
            // Generate a random angle between 0 and 2π
            float angle = Rng.NextFloat(2 * MathF.PI);

            // Generate a random radius between 0 and 1
            float radius = MathF.Sqrt(Rng.NextFloat())*0.3f;

            // Calculate the x and y coordinates
            float x = radius * startMajorAxis * MathF.Cos(angle);
            float z = radius * startMinorAxis * MathF.Sin(angle);

            return new Vector3(x, 0, z);
        }


        private void SpawnMob() {
            for(int i = 0; i<N_ZOMBIES; i++)
            {
                float angle = Rng.NextFloat(2 * MathF.PI);
                active[i] = new Zombie(
                    new Vector3(startMajorAxis*MathF.Sin(angle), 0, startMinorAxis*MathF.Cos(angle)) * 1.3f, 
                    Ellipse, 
                    models[i%models.Count], 0.7f
                );
            }
        }

        public void updateWrap(float dt, bool MainMenuMode) {
            if(!MainMenuMode)
            {
                MobPlayerInteraction();
                NewMobProjectile(dt);
                CloseRing(dt);
            }
            foreach (Zombie zombie in active) 
            {
                            zombie.updateWrap(dt);
                            zombie.UpdateAnimation(dt);
            }

            MobMarketInteraction();
            MobPhysics();
        }

        private void CloseRing(float dt)
        {
            // Wait until the time between closings has passed
            if (closing >= N_CLOSINGS || (timeAlive += dt) < TIME_BETWEEN_CLOSING) return;

            // Calculate the progress
            float progress = (timeAlive - TIME_BETWEEN_CLOSING) / CLOSING_TIME;

            if (progress <= 1f)
            {
                float totalProgress = (closing + progress) / N_CLOSINGS;

                // Update the ellipse
                Ellipse.Set(
                    MathHelper.Lerp(startMajorAxis, endMajorAxis, totalProgress), // Major axis
                    MathHelper.Lerp(startMinorAxis, endMinorAxis, totalProgress), // Minor axis
                    Vector3.Lerp(Vector3.Zero, endCenter, totalProgress) // Center
                );
                foreach (Zombie zombie in active) zombie.Target = endCenter;
            } 
            else
            {
                // Reset the time and increment the number of closings
                timeAlive = 0f;
                closing++;
            }   
        }

        private void MobPhysics(){

             // Update zombie (mob) physics
            sortedZombies = new List<Zombie>[24*24];
            for(int i = 0;i<24*24;i++)
                sortedZombies[i] = new List<Zombie>();
            foreach (Zombie zombie in active) 
                sortedZombies[(int)Math.Round(zombie.Position.X*0.2f)+11+((int)Math.Round(zombie.Position.Z*0.2f)+11)*24].Add(zombie);
            // Update force for all zombies(mob)
            for(int j = 0; j<23;j++)
            {
                for(int i = 0; i<23;i++)
                {
                    List<Zombie> tempList = sortedZombies[i+j*24];
                    for(int k=0; k<tempList.Count;++k)
                    {
                        tempList[k].Force(tempList,k);
                        tempList[k].Force(sortedZombies[i+j*24+1],-1);
                        tempList[k].Force(sortedZombies[i+j*24+24],-1);
                        tempList[k].Force(sortedZombies[i+j*24+25],-1);
                    }
                }
            }
        }

        private void MobPlayerInteraction(){

            foreach (Player player in gameStateManager.players)
            { 
                if(player.Life>0) continue;

                int i = (int)Math.Round(player.Position.X*0.2f)+11;
                int j = (int)Math.Round(player.Position.Z*0.2f)+11;
                int iNeighbour = (player.Position.X*0.2f-i) < 0.5f ? -1 : 1;
                int jNeighbour = (player.Position.Z*0.2f-j) < 0.5f ? -24 : 24;
                
                foreach (Zombie zombie in sortedZombies[i+j*24]) zombie.ForceByPlayer(player);
                foreach (Zombie zombie in sortedZombies[i+j*24+iNeighbour]) zombie.ForceByPlayer(player);
                foreach (Zombie zombie in sortedZombies[i+j*24+jNeighbour]) zombie.ForceByPlayer(player);
                foreach (Zombie zombie in sortedZombies[i+j*24+iNeighbour+jNeighbour]) zombie.ForceByPlayer(player);
            }
        }

        private void MobMarketInteraction(){

            foreach (Zombie zombie in active)
            { 
                foreach(Market market in gameStateManager.markets){
                    if(zombie.Hitbox.Intersects(market.Hitbox))zombie.ForceByMarket(market);
                }
            }
        }



        private float CalculateOpacity(Zombie zombie)
        {
            float closestDist = MAX_DISTANCE_OPACITY;

            foreach (Player player in gameStateManager.players)
            {
                if (player.Life == 0)
                    closestDist = Math.Min(closestDist, Vector3.DistanceSquared(zombie.Position, player.Position));
            }

            if (closestDist <= MIN_DISTANCE_OPACITY)
                return MIN_OPACITY;

            if (closestDist < MAX_DISTANCE_OPACITY)
            {
                float t = (closestDist -MIN_DISTANCE_OPACITY) / (MAX_DISTANCE_OPACITY - MIN_DISTANCE_OPACITY);
                return MathHelper.Lerp(MIN_OPACITY, 1f, MathHelper.Clamp(t, 0f, 1f));
            }

            return 1f;
        }




        private void NewMobProjectile(float dt)
        {
            //the probability to shoot is once every 0.1 second
            if ((timeUntilNextProjectile += dt) < 0.01f) return;

            foreach (var entry in Projectile.ProjectileProbability)
            {
                if (!Rng.NextBool(entry.Value * 0.01f)) continue;

                Zombie throwingZombie = Mob.active[Rng.NextInt(active.Length)];
                while(throwingZombie.projectileHeld != null) throwingZombie = active[Rng.NextInt(active.Length)];

                Player targetPlayer = gameStateManager.players[Rng.NextInt(gameStateManager.players.Count)];
                while(targetPlayer.Life <= 0) targetPlayer = gameStateManager.players[Rng.NextInt(gameStateManager.players.Count)];

                throwingZombie.Spawn(entry.Key, targetPlayer);
            }

            timeUntilNextProjectile = 0f;
        }

        public void Draw(Matrix view, Matrix projection, Shader shader, GraphicsDevice graphicsDevice, bool shadowDraw) {
            foreach (Zombie zombie in active) {
                shader.setFinalBoneMatrices(zombie.GetFinalBoneMatrices());
                if(!shadowDraw)
                {
                 
                    shader.setRoughness(zombie.DrawModel.roughness);
                    shader.setMetallic(zombie.DrawModel.metallic);
                    shader.setOpacityValue(CalculateOpacity(zombie));
                }

                zombie.Draw(view, projection, shader, graphicsDevice, shadowDraw);
            }
        }
    }
    
}   