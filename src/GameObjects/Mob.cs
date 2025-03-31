using Accord.Math.Geometry;
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
        private const int N_ZOMBIES = 300;
        public static Zombie[] active = new Zombie[N_ZOMBIES];
        private List<Zombie>[] sortedZombies = new List<Zombie>[24 * 24];

        // Ellipse properties
        private float timeAlive;
        private const float TIME_BETWEEN_CLOSING = 10f;
        private const float CLOSING_TIME = 5f;
        private const int N_CLOSINGS = 4;
        public Ellipse Ellipse { get; private set; }
        private float startMajorAxis, startMinorAxis;
        private float endMajorAxis, endMinorAxis = 3f;
        private Vector3 endCenter;

        private Random random = new();
        private List<Model> models;
        private float closing = 0;
        private float timeUntilNextProjectile;

        public Mob(List<Model> models) {
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
        }

        public Vector3 GetRandomPointInside()
        {
            // Generate a random angle between 0 and 2π
            float angle = (float)(random.NextDouble() * 2 * Math.PI);

            // Generate a random radius between 0 and 1
            float radius = (float)Math.Sqrt(random.NextDouble())*0.3f;

            // Calculate the x and y coordinates
            float x = radius * startMajorAxis * (float)Math.Cos(angle);
            float z = radius * startMinorAxis * (float)Math.Sin(angle);

            return new Vector3(x, 0, z);
        }


        private void SpawnMob() {
            for(int i = 0; i<N_ZOMBIES; i++)
            {
                float angle = (float)(random.NextDouble() * 2f * Math.PI);
                active[i] = new Zombie(
                    new Vector3(startMajorAxis*(float)Math.Sin(angle), 0, startMinorAxis*(float)Math.Cos(angle)) * 1.3f, 
                    Ellipse, 
                    models[i%2], 0.7f
                );
            }
        }

        public void Update (float dt) {
            CloseRing(dt);
            MobPhysics();
            MobPlayerInteraction();
            foreach (Zombie zombie in active) zombie.updateWrap(dt);
            NewMobProjectile(dt);
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
                sortedZombies[(int)Math.Round(zombie.Position.X)+11+((int)Math.Round(zombie.Position.Y)+11)*24].Add(zombie);
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

            foreach (Player player in Player.active)
            { 
                int i = (int)Math.Round(player.Position.X)+11;
                int j = (int)Math.Round(player.Position.Y)+11;
                int iNeighbour = (player.Position.X-(float)i) < 0.5f ? -1 : 1;
                int jNeighbour = (player.Position.Y-(float)j) < 0.5f ? -24 : 24;
                if(player.Life<=0)
                {
                    foreach (Zombie zombie in sortedZombies[i+j*24]) zombie.ForceByPlayer(player);
                    foreach (Zombie zombie in sortedZombies[i+j*24+iNeighbour]) zombie.ForceByPlayer(player);
                    foreach (Zombie zombie in sortedZombies[i+j*24+jNeighbour]) zombie.ForceByPlayer(player);
                    foreach (Zombie zombie in sortedZombies[i+j*24+iNeighbour+jNeighbour]) zombie.ForceByPlayer(player);
                }else{
                    foreach (Zombie zombie in sortedZombies[i+j*24]) player.mobCollision(zombie);
                    foreach (Zombie zombie in sortedZombies[i+j*24+iNeighbour]) player.mobCollision(zombie);
                    foreach (Zombie zombie in sortedZombies[i+j*24+jNeighbour]) player.mobCollision(zombie);
                    foreach (Zombie zombie in sortedZombies[i+j*24+iNeighbour+jNeighbour]) player.mobCollision(zombie);
                }
            }
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

        private void NewMobProjectile(float dt)
        {
            //the probability to shoot is once every 0.1 second
            if ((timeUntilNextProjectile += dt) < 0.01f) return;

            foreach (var entry in Projectile.ProjectileProbability)
            {
                if (random.NextDouble() * 100 > entry.Value) continue;

                Zombie throwingZombie = Mob.active[random.Next(0, Mob.active.Length)];
                while(throwingZombie.projectileHeld != null) throwingZombie = Mob.active[random.Next(0, Mob.active.Length)];

                Player targetPlayer = Player.active[random.Next(0, Player.active.Count)];
                while(targetPlayer.Life <= 0) targetPlayer = Player.active[random.Next(0, Player.active.Count)];

                throwingZombie.Spawn(entry.Key, targetPlayer);
            }

            timeUntilNextProjectile = 0f;
        }


        public void Draw(Matrix view, Matrix projection) {
            foreach (Zombie zombie in active)
                zombie.Draw(view, projection);
        }
    }
    
}   