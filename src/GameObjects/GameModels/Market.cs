using GameLab;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;

namespace src.GameObjects
{
    public class Market : GameModel
    {
        public static Market[] active = new Market[4];
        public ProjectileType Type { get; private set; }

        public Market(Vector3 position, ProjectileType type, DrawModel model, float scaling) : base(model, scaling)
        {

            this.Position = position;
            this.Orientation = Vector3.Normalize(-Position);
            this.Type = type;
        }

        public static void CreateMarket(float width, float height)
        {
            //this should be the positions of the corners
            Vector3[] positions = new Vector3[]
            {
                new Vector3(-10, 0, 10),
                new Vector3(10, 0, 10),
                new Vector3(-10, 0, -10),
                new Vector3(10, 0, -10)
            };
            ProjectileType[] types = SelectRandomTypes();

            for (int i = 0; i < 4; i++)
                active[i] = new Market(positions[i], types[i], GameLabGame.projectileModels[types[i]], 1f);
        }

        private static ProjectileType[] SelectRandomTypes()
        {
            List<ProjectileType> availableTypes = new List<ProjectileType>(Projectile.ProjectileProbability.Keys);
            ProjectileType[] selected = new ProjectileType[4];
            Random rng = new Random();
            float totalWeight = availableTypes.Sum(type => Projectile.ProjectileProbability[type]);

            for (int i = 0; i < 4; i++)
            {
                // Refill if empty
                if (!availableTypes.Any()) 
                    availableTypes = new List<ProjectileType>(Projectile.ProjectileProbability.Keys);
            
                float randomValue = (float)rng.NextDouble() * totalWeight;

                foreach (var type in availableTypes)
                {
                    randomValue -= Projectile.ProjectileProbability[type];
                    if (randomValue > 0) continue;
                    
                    selected[i] = type;
                    availableTypes.Remove(type);
                    totalWeight -= Projectile.ProjectileProbability[type];
                    break;
                }
            }

            return selected;
        }
    }
}