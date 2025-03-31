using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Shader {
   public Effect  effect {get; set;}

   public Shader(Effect effect){
    this.effect = effect; 
   }

   public void setViewMatrix(Matrix view){
     effect.Parameters["View"].SetValue(view);
   }

      public void setProjectionMatrix(Matrix projection){
        effect.Parameters["Projection"].SetValue(projection);
   }

   
      public void setWorldMatrix(Matrix world){
        effect.Parameters["World"].SetValue(world);
   }

         public void setLightSpaceMatrix(Matrix lightSpace){
        effect.Parameters["LightViewProjection"].SetValue(lightSpace);
   }

   public void setLight(Light light){
    effect.Parameters["LightDirection"].SetValue(light.direction);
    effect.Parameters["LightColor"].SetValue(light.color);
   }

   
    public void setTexture(Texture2D tex){
        effect.Parameters["TextureSampler+ModelTexture"].SetValue(tex);
    }

    public void setShadowTexture(Texture2D shadowMap){
      effect.Parameters["ShadowSampler"].SetValue(shadowMap);
    }

}

public class PhongShading : Shader {

    public PhongShading(Effect effect) : base(effect){

    }

    public void setCameraPosition(Vector3 CameraPos){
        effect.Parameters["CameraPosition"].SetValue(CameraPos);
    }
}