using UnityEngine;
using System.Collections;

public class GameExplosionObject : MonoBehaviour {
    public Vector3 Force;
    public GameObject Prefab;
    public int Num;
    public int Scale = 20;
    public AudioClip[] Sounds;
    public float LifeTimeObject = 2;
    public bool RandomScale;
    
    private void Start() {

        if (Sounds.Length > 0) {
            AudioSource.PlayClipAtPoint(Sounds[Random.Range(0, Sounds.Length)], transform.position,
                                        (float)GameProfiles.Current.GetAudioEffectsVolume());
        }
        if (Prefab) {
            for (int i = 0; i < Num; i++) {
                float scaleHalf = Scale/2;
                Vector3 pos = new Vector3(Random.Range(-scaleHalf, scaleHalf), Random.Range(-scaleHalf, Scale), Random.Range(-scaleHalf, scaleHalf)) / scaleHalf;
                GameObject obj = GameObjectHelper.CreateGameObject(Prefab, transform.position + pos, transform.rotation, true);
                GameObjectHelper.DestroyGameObject(obj, LifeTimeObject, true);
                
                float scale = Scale;
                if (RandomScale) {
                    scale = Random.Range(1, Scale);
                }

                if (scale > 0)
                    obj.transform.localScale = new Vector3(scale, scale, scale);
                if (obj.rigidbody) {
                    Vector3 force = new Vector3(Random.Range(-Force.x, Force.x), Random.Range(-Force.y, Force.y), Random.Range(-Force.z, Force.z));
                    obj.rigidbody.AddForce(force);
                }
            }
        }
    }

}
