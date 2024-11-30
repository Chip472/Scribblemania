using UnityEngine;

[ExecuteAlways]
public class BackgroundScaler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundSprite;

    private void Start()
    {
        ScaleBackground();
    }

    private void Update()
    {
        ScaleBackground();
    }

    private void ScaleBackground()
    {
        if (backgroundSprite == null)
        {
            Debug.LogWarning("BackgroundScaler: SpriteRenderer not assigned!");
            return;
        }

        float spriteWidth = backgroundSprite.sprite.bounds.size.x;
        float spriteHeight = backgroundSprite.sprite.bounds.size.y;
        float spriteAspect = spriteWidth / spriteHeight;

        float screenAspect = (float)Screen.width / (float)Screen.height;

        Vector3 scale = transform.localScale;

        if (screenAspect >= spriteAspect)
        {
            scale.x = scale.y * screenAspect / spriteAspect;
        }
        else
        {
            scale.y = scale.x * spriteAspect / screenAspect;
        }

        transform.localScale = scale;
    }
}
