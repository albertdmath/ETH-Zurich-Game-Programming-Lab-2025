using GameLab;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using Accord.Math.Distances;
using Microsoft.Xna.Framework.Graphics;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Accord.Collections;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Net;

namespace src.GameObjects
{
    /** Anything regarding player characters, from movement to the actual Model files goes here. **/
    public class Player : GameModel
    {
        public int Id { get; set; }
        // Private fields:
        private float playerSpeed = 2f;
        public int Life { get; set; } = 5;
        public float Stamina { get; set; } = 0f;
        public Projectile projectileHeld;
        private float dashTime = 0f;
        private float actionPushedDuration;
        private float stunDuration = 0f;
        public bool notImportant = false;
        private bool mob = false;
        private bool recentlyCaught = false;
        private float immunity = 0f, throwImmunity = 0f;
        private Projectile lastThrownProjectile = null; // Store last thrown projectile

        private Input input;
        private Ellipse ellipse;
        private Vector3 Inertia;

        private GameStateManager gameStateManager;

        public Player(Vector3 position, Input input, int id, Ellipse ellipse, DrawModel model,float scale) : base(model,scale)
        {
            Position = position;
            Orientation = new Vector3(0,0,1f);
            this.input = input;
            this.ellipse = ellipse;
            projectileHeld = null;
            this.Id = id;
            Inertia = new Vector3(0,0,0);
            // Remove hat from hitbox; this is trashcode and needs to be removed / done better at some point
            this.Hitbox.BoundingBoxes.RemoveAt(this.Hitbox.BoundingBoxes.Count - 1);
            gameStateManager = GameStateManager.GetGameStateManager();
        }

        // The player move method:
        public void Move(float dt)
        {
            Vector3 dir = input.Direction();
            //inertia to keep some movement from last update;
            Inertia -= (9f*dt) * Inertia;
            if (stunDuration<=0f && dir.Length() > 0)
            {
                dir = Vector3.Normalize(dir);
                Inertia += (9f*dt)*dir;
                // Limit Inertia to a vector of length 1
                if (Inertia.Length() > 1f)
                    Inertia = Vector3.Normalize(Inertia);
            }
            // Only update orientation if inertia is not 0
            if (Inertia.Length() > 0)
            {
                Orientation = Vector3.Normalize(Inertia);
            }
            // Updating the position of the player
            Position += playerSpeed * Inertia * dt;
        }

        // Method to test for a collision with a projectile and potentially grab it:
        private bool Catch()
        {
            float cosTheta = MathF.Cos(MathHelper.ToRadians(30f)); // Precompute cos(30°)

            foreach (Projectile projectile in gameStateManager.projectiles)
            {
                Vector3 toProjectile = projectile.Position - Position;
                if (toProjectile.LengthSquared() > 2) continue; // Outside radius

                Vector3 directionToProjectile = Vector3.Normalize(toProjectile);
                if (Vector3.Dot(Orientation, directionToProjectile) >= cosTheta) // Inside 60-degree cone
                {
                    projectileHeld = projectile;
                    projectile.Catch(this);
                    /*
                    Console.WriteLine("Grabbing " + projectile.Type);
                    // Here the player speed is set for the movement with projectile in hand
                    playerSpeed = 0.9f;

                    // Handle the equip sound effect based on the projectile type:
                    if(GameLabGame.SOUND_ENABLED) { MusicAndSoundEffects.playProjectileSFX(projectile.Type); }
                    return false;
                } else // the player is hit by the projectile
                {
                    Life -= notImportant ? 0 : 1;
                    
                    */
                    // Handle the hit sound effect:
                    MusicAndSoundEffects.playProjectileSFX(projectile.Type);
                    
                    return true;
                }
            }
            return false;
        }

        public bool GetHit(Projectile projectile)
        {
            // If last man standing return hit but don't subtract life
            if (gameStateManager.livingPlayers.Count == 1)
                return true;
            // Check immunity
            if (throwImmunity > 0 && projectile == lastThrownProjectile)
                return false;
            
            // Otherwise check general immunity
            if (immunity <= 0)
            {
                input.Vibrate();
                Life--;
                immunity = 1f;
                if (Life == 0f)
                {
                    // For now the player is moved down to indacet crawling. Later done with an animation
                    Position = Position - new Vector3(0, 0.2f, 0);
                    playerSpeed = 1f;
                    gameStateManager.livingPlayers.Remove(this);
                }
            }
            return true;
        }

        // Method to throw an object:
        private void Throw()
        { 
            float speedUp = 1 + 2 * (float)Math.Pow(Math.Min(actionPushedDuration, 4f), 2f);
            throwImmunity = 1f; 
            lastThrownProjectile = projectileHeld;
            projectileHeld.Throw(speedUp);
            projectileHeld = null;
            Console.WriteLine("Throwing projectile with orientation: " + Orientation+ " and speedup: " +speedUp);
        }

        // Method to dash. Current dash cost 30
        private bool Dash(float dt)
        {
            if (stunDuration<=0f && input.Dash() && dashTime <= 0f && Stamina > 30f)
            {
                dashTime = 0.1f;
                Stamina -= 30f;
            }
            if (dashTime > 0f)
            {
                Position += 6 * playerSpeed * Orientation * dt;
                dashTime -= dt;
                return true;
            }
            else
            {
                return false;
            }
        }
        // Spawning a projectile in hand when part of the mob. Currently only swordfish
        private void Spawn()
        {
            if(input.Action())
            {
                projectileHeld = gameStateManager.CreateProjectile(ProjectileType.Swordfish,Position,Orientation);
                projectileHeld.Catch(this);
                Stamina -= 40f;
            }
        
        }

        public void playerCollision(Player player){
            Vector3 dir = 0.02f * Vector3.Normalize(new Vector3(Position.X-player.Position.X,0f,Position.Z-player.Position.Z));
            while(Hitbox.Intersects(player.Hitbox)){
                Position += dir;
                player.Position -= dir;
                updateHitbox();
                player.updateHitbox();
            }
        }
        public void mobCollision(Zombie zombie){
            Vector3 dir = 0.02f * Vector3.Normalize(new Vector3(Position.X-zombie.Position.X,0f,Position.Z-zombie.Position.Z));
            if(Hitbox.Intersects(zombie.Hitbox))
            {
                while(Hitbox.Intersects(zombie.Hitbox)){
                    Position += dir;
                    updateHitbox();
                }
                Orientation = ellipse.Normal(Position.X,Position.Z);
                Inertia = 1.6f * Orientation;
                stunDuration = 1f;
            }
        }


        // Update function called each update
        //this needs to be stilla adjusted for hitting yourself
        public override void Update(float dt)
        {
            Stamina += dt * 5f;
            Stamina = (Stamina > 100f) ? 100f : Stamina;
            input.EndVibrate(dt);

            if (Life > 0f || mob) // Behaviour when alive
            {   
                stunDuration -= dt;
                immunity -= dt;
                throwImmunity -= dt;

                if(Dash(dt)) return;

                Move(dt);

                if(mob)
                {
                    while(ellipse.Inside(Position.X,Position.Z))
                    Position += ellipse.Normal(Position.X,Position.Z) * dt * -0.1f;
                }

                if (stunDuration>0f) return;

                if (input.Action())
                {
                    //wait until the action is released to throw the projectile
                    if (recentlyCaught) return;

                    actionPushedDuration += dt;
                    if(projectileHeld != null)
                        playerSpeed = 0f; //Aim
                    else 
                    {
                        if(actionPushedDuration<0.7f)
                            recentlyCaught = Catch(); // Catch projectile
                        else if (mob)
                            Spawn(); // Spawn projectile
                    }
                }
                else
                {
                    if (playerSpeed == 0f)
                    {
                        Throw(); // Throw projectile
                        playerSpeed = 2f;
                    }
                    recentlyCaught = false;
                    actionPushedDuration = 0;
                } 
            } 
            else // Crawling
            {
                Move(dt);
                playerSpeed = 1f;

                if (ellipse.Outside(Position.X,Position.Z))
                {
                    mob = true;
                    Position = Position + new Vector3(0, 0.2f, 0);
                    playerSpeed = 2f;
                }
            }
        }

        public override void Draw(Matrix view, Matrix projection, Shader shader, bool shadowDraw)
        {
            // Blink every 0.1 seconds when either stunDuration or immunity are active
            bool shouldDraw = true;
            if(stunDuration > 0)
               shouldDraw = (int)(stunDuration * 10) % 2 == 0;
            
            if(immunity > 0)
               shouldDraw = (int)(immunity * 10) % 2 == 0;

            if (shouldDraw)
                base.Draw(view, projection, shader, shadowDraw);
        }
    }
}