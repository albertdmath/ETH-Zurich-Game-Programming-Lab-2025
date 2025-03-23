

using Microsoft.Xna.Framework;

public class Light {

    public Vector3 color {get; set;}

    public Vector3 direction{get; set;}
    public Light(Vector3 color, Vector3 direction){
        this.color = color; 
        this.direction = Vector3.Normalize(-direction); 
    }
}