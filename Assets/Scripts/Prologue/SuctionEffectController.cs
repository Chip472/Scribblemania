using UnityEngine;

public class SuctionEffectController : MonoBehaviour
{
    public Material suctionMaterial;
    public float suctionSpeed = 1f;
    public float maxStrength = 2f;

    void Update()
    {
        if (suctionMaterial != null)
        {
            float suctionStrength = Mathf.PingPong(Time.time * suctionSpeed, maxStrength);
            suctionMaterial.SetFloat("_Strength", suctionStrength);
        }
    }
}
