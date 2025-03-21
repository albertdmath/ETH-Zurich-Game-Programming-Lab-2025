using Accord.Math.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace src.GameObjects
{
    public class Mob
    {
        // Zombies
        private const int N_ZOMBIES = 100;
        public static Zombie[] active = new Zombie[N_ZOMBIES];
        private List<Zombie>[] sortedZombies = new List<Zombie>[24 * 24];

        // Ellipse properties
        private float timeAlive;
        private const float TIME_BETWEEN_CLOSING = 10f;
        private const float CLOSING_TIME = 5f;
        private const int N_CLOSINGS = 4;
        public Ellipse Ellipse { get; private set; }
        private float startMajorAxis, startMinorAxis;
        private float endMajorAxis, endMinorAxis = 1f;
        private Vector3 endCenter;

        private Random random = new();
        private Model model;
        private float closing = 0;

        public Mob(float height, float width, Model model) {
            // Set the major and minor axes of the ellipse
            this.startMajorAxis = width / 2; // Half the width of the plane
            this.startMinorAxis = height / 2; // Half the height of the plane
            this.endMajorAxis = this.endMinorAxis * width / height;
            //inside the ellipse
            this.endCenter = GetRandomPointInside();
            // Create the ellipse
            this.Ellipse = new Ellipse(startMinorAxis, startMajorAxis, Vector3.Zero);

            this.model = model;
            SpawnMob();
        }

        public Vector3 GetRandomPointInside()
        {
            // Generate a random angle between 0 and 2π
            float angle = (float)(random.NextDouble() * 2 * Math.PI);

            // Generate a random radius between 0 and 1
            float radius = (float)Math.Sqrt(random.NextDouble());

            // Calculate the x and y coordinates
            float x = radius * startMajorAxis * (float)Math.Cos(angle);
            float z = radius * startMinorAxis * (float)Math.Sin(angle);

            return new Vector3(x, 0, z);
        }


        private void SpawnMob() {
            for(int i = 0; i<N_ZOMBIES; i++)
            {
                float randomFloat = (float)(random.NextDouble() *2f* Math.PI);
                active[i] = new Zombie(
                    new Vector3(8f*(float)Math.Sin(randomFloat), 0.2f, 8f*(float)Math.Cos(randomFloat)), 
                    Ellipse, 
                    model
                );
            }
        }

        public void Update (float dt) {
            CloseRing(dt);
            MobPhysics();
            foreach (Zombie zombie in active) zombie.updateWrap(dt);
        }

        private void CloseRing(float dt)
        {
            // Wait until the time between closings has passed
            if (closing >= N_CLOSINGS || (timeAlive += dt) < TIME_BETWEEN_CLOSING) return;

            // Calculate the progress of the current closing (normalized to [0, 1])
            float progress = Math.Clamp((timeAlive - TIME_BETWEEN_CLOSING) / CLOSING_TIME, 0, 1);

            // Calculate the total progress across all closings
            float totalProgress = (closing + progress) / N_CLOSINGS;

            // Update the ellipse
            Ellipse.Set(
                MathHelper.Lerp(startMinorAxis, endMinorAxis, totalProgress), // Minor axis
                MathHelper.Lerp(startMajorAxis, endMajorAxis, totalProgress), // Major axis
                Vector3.Lerp(Vector3.Zero, endCenter, totalProgress) // Center
            );

            // If the closing is complete, reset for the next closing
            if (progress >= 1f)
            {
                closing++;
                timeAlive = 0f;
            }
        }

        private void MobPhysics(){

             // Update zombie (mob) physics
            sortedZombies = new List<Zombie>[24*24];
            for(int i = 0;i<24*24;i++)
                sortedZombies[i] = new List<Zombie>();
            foreach (Zombie zombie in active) 
                sortedZombies[(int)Math.Round(zombie.Position.X)+11+((int)Math.Round(zombie.Position.Y)+11)*24].Add(zombie);
            // MOve all zombies(mob)
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

        public void Draw(Matrix view, Matrix projection) {
            foreach (Zombie zombie in active)
                zombie.Draw(view, projection);
        }
    }
    
}   