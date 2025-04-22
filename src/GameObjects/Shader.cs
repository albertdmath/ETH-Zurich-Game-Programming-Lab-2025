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

    public void setNormalMatrix(Matrix view, Matrix world){
      Matrix normalMatrix = Matrix.Transpose(Matrix.Invert(world * view));
      effect.Parameters["NormalMatrix"].SetValue(normalMatrix);
    }
    
  public void setMetallic(float metallic){
    effect.Parameters["metallic"].SetValue(metallic);
  }

  
  public void setRoughness(float roughness){
    effect.Parameters["roughness"].SetValue(roughness);
  }
    public void setOpacityValue(float opacity){
      effect.Parameters["OpacityVal"].SetValue(opacity);
    }

}

public class PhongShading : Shader {

    public PhongShading(Effect effect) : base(effect){

    }




    public void setCameraPosition(Vector3 CameraPos){
        effect.Parameters["CameraPosition"].SetValue(CameraPos);
    }
}

public class PBR:PhongShading {
  
  public PBR(Effect effect) : base(effect) {

  } 

  public void setNormalTexture(Texture2D normalMap){
    effect.Parameters["NormalPosSampler+NormalTexture"].SetValue(normalMap);
  }

  public void setViewInverse(Matrix viewInverse){
    effect.Parameters["ViewInverse"].SetValue(viewInverse);
  }

   public void setFragPosTexture(Texture2D fragPosTexture){
    effect.Parameters["FragPosSampler+FragPosTexture"].SetValue(fragPosTexture);
  }

  public void setRoughnessTexture(Texture2D roughnessTexture){
    effect.Parameters["RoughnessSampler+RoughnessTexture"].SetValue(roughnessTexture);
  }

  public void setMetallicTexture(Texture2D metallicTexture){
    effect.Parameters["MetallicSampler+MetallicTexture"].SetValue(metallicTexture);
  }
  
}