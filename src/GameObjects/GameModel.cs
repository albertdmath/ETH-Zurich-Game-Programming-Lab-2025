using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace src.GameObjects;

/** Superclass for all moving objects **/
public class GameModel {
    public Vector3 Position { get; set; }
    public Vector3 Orientation { get; set; }
    protected Model Model { get; set; }
    public Hitbox Hitbox { get; set; }

    public GameModel() {}
}
