using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace src.GameObjects
{
    public class Mob
    {
        private List<Zombie> zombies = new List<Zombie>();
        private List<Zombie>[] sortedZombies = new List<Zombie>[24*24];
        private float totalTimePassed = 0f;

        private Ellipse innerEllipse;
        private Ellipse outerEllipse;

        private Random random = new();

        private Model model;


        public Mob(Ellipse innerEllipse, Ellipse outerEllipse, Model model) {
            this.innerEllipse = innerEllipse;
            this.outerEllipse = outerEllipse;
            this.model = model;
        }

        public void Update (float dt) {

            totalTimePassed+=dt;
            if(totalTimePassed>20f)
            {
                float a = 7.5f - 0.1f*((float)Math.Round(totalTimePassed-20f));
                a = a < 0.5f ? 0.5f : a;
                float b = 4f - 0.05f*((float)Math.Round(totalTimePassed-20f));
                b = b < 0.5f ? 0.5f : b;
                innerEllipse.Set(a-0.2f,b-0.2f);
                outerEllipse.Set(a,b);
            }
            if(zombies.Count<500)
            {
                float randomFloat = (float)(random.NextDouble() *2f* Math.PI);
                zombies.Add(new Zombie(
                    new Vector3(9f*(float)Math.Sin(randomFloat), 0.2f, 8f*(float)Math.Cos(randomFloat)), outerEllipse, model)
                );
            }


             // Update zombie (mob) physics
            sortedZombies = new List<Zombie>[24*24];
            for(int i = 0;i<24*24;i++)
                sortedZombies[i] = new List<Zombie>();
            foreach (Zombie zombie in zombies) 
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
            foreach (Zombie zombie in zombies) zombie.updateWrap(dt);
        }

        public void Draw(Matrix view, Matrix projection) {
            foreach (Zombie zombie in zombies)
                zombie.Draw(view, projection);
        }
    }
    
}   