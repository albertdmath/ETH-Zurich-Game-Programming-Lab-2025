using Microsoft.Xna.Framework;

/*
    The idea for this class is, that the hitbox of a model consists of a set of bounding boxes.
    For the player we would have individual boxes for arms, legs, head etc. alltogether forming the hitbox.
*/

public interface Hitbox 
{
    public abstract bool Intersects(Hitbox other);
    public abstract bool IntersectsWith(OBB other);
    public abstract bool IntersectsWith(Sphere other);
}