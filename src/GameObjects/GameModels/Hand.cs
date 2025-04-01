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
    public class Hand : GameModel
    {
        public bool IsCatching { get; set; } = false;
        // Private fields:
        private float timeSpentCatching = 0f;
        private Player player;
        private const float CATCH_DURATION = 0.6f; 
        
        public Hand(Player player, DrawModel model,float scale) : base(model,scale)
        {
            this.player=player;
            OnBody();
        }

        // Moves the hand in a half circle around the body
        public void Move(float dt)
        {
            timeSpentCatching += dt;
            Vector3 orthogonalHolderOrientation = new Vector3(-player.Orientation.Z, player.Orientation.Y, player.Orientation.X);
            Position = new Vector3(0f,0.2f,0f)+player.Position + Vector3.Transform(new Vector3(0.35f,0,0f),Matrix.CreateRotationY((float)Math.Atan2(-1f*Orientation.X,-1f*Orientation.Z)+(float)Math.PI/CATCH_DURATION*timeSpentCatching));
            Orientation = player.Orientation;
        }
        // Places the hand next to the body
        private void OnBody()
        {
            Vector3 orthogonalHolderOrientation = new Vector3(-player.Orientation.Z, player.Orientation.Y, player.Orientation.X);
            Position = new Vector3(0f,0.2f,0f)+ player.Position + orthogonalHolderOrientation * 0.2f;
            Orientation = player.Orientation;
        }
       

        // Update function called each update
        //this needs to be stilla adjusted for hitting yourself
        public override void Update(float dt)
        {
            if(timeSpentCatching>CATCH_DURATION)
            {
                timeSpentCatching = 0f;
                IsCatching = false;
            }
            if(IsCatching)
                Move(dt);
            else OnBody();
        }
    }
}