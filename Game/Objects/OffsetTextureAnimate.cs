using UnityEngine;
using System.Collections;

public class OffsetTextureAnimate : GameObjectBehavior {

    public float scrollSpeedX = 0.015f;
    public float  scrollSpeedY = 0.015f;
    public float scrollSpeedXMaterial2 = 0.015f;
    public float scrollSpeedYMaterial2 = 0.015f;
    
    void  Update() {
    
        float offsetX = Time.time * scrollSpeedX;
    
        float offsetY = Time.time * scrollSpeedY;
    
        float offset2X = Time.time * scrollSpeedXMaterial2;
    
        float offset2Y = Time.time * scrollSpeedYMaterial2;
    
        transform.renderer.material.SetTextureOffset("_BumpMap", new Vector2(offsetX, offsetY));
    
        transform.renderer.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
    
        if (transform.renderer.materials.Length > 0) {
        
            transform.renderer.materials[1].SetTextureOffset("_MainTex", new Vector2(offset2X, offset2Y));
    
            transform.renderer.materials[1].SetTextureOffset("_BumpMap", new Vector2(offset2X, offset2Y));
        }
    
    }
}