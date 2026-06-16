using UnityEngine;


public class torchFlicker : MonoBehaviour
{
  public int minIntensity = 4;
  public int maxIntensity = 6;
  public float minChange = (float)-0.005;
  public float maxChange = (float)0.005;
  public float change = (float)0.0001;
  public int flickerCount = 0;
  public int flickerSpeed = 10;
  public float intense = 5;
  public Light light;

    void Start(){
      light = GetComponent<Light>();
    }
    void Update()
    {
      if (intense + change >= maxIntensity){
        change = 0 - change;
      }
      if (intense + change <= minIntensity){
        change = 0 - change;
      }
      intense = intense += change;
      light.intensity = intense;
      flickerCount += 1;
      if(flickerCount >= flickerSpeed){
        flickerSpeed = Random.Range(80, 140);
        flickerCount = 0;
        change = Random.Range(minChange, maxChange);
      }

    }
}
