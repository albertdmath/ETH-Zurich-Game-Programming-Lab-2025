using Microsoft.Xna.Framework;

public class Light {

    public Vector3 color {get; set;}

    public Vector3 direction{get; set;}

    public Matrix lightSpaceMatrix {get; set;}
    public Light(Vector3 color, Vector3 direction){
        this.color = color; 
        this.direction = Vector3.Normalize(-direction); 

        Vector3 lightPos = (-direction)*5;

        Matrix lightProjection = Matrix.CreateOrthographic(50.0f, 50.0f, 10f, 150.0f);

        Matrix lightView = Matrix.CreateLookAt(lightPos,new Vector3(0.0f,0.0f,0.0f), Vector3.Up);

        this.lightSpaceMatrix = lightView * lightProjection; 
    }
}