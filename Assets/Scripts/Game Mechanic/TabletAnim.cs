using UnityEngine;
using UnityEngine.UI;

public class TabletAnim : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private float minWidth = 0.1f;
    [SerializeField] private float maxWidth = 1.0f;
    [SerializeField] private float speed = 1.0f;

    [SerializeField] private CombatManager manager;

    private Material rawImageMaterial;
    public float currentTopWidth;
    public bool isIncreasing;
    private bool isChanging;

    [SerializeField] private Animator tabletAnimator; 
    [SerializeField] public string animationName;
    [SerializeField] private float animationLength;

    void Start()
    {
        isChanging = false;
        isIncreasing = true;
        rawImageMaterial = rawImage.material;

        if (rawImageMaterial == null)
        {
            Debug.LogError("RawImage does not have a material assigned!");
            enabled = false;
            return;
        }

        if (tabletAnimator != null)
        {
            AnimationClip clip = GetAnimationClip(animationName);
            if (clip != null)
            {
                animationLength = clip.length;
                speed = (maxWidth - minWidth) / animationLength;
            }
        }

        currentTopWidth = minWidth;
    }

    AnimationClip GetAnimationClip(string name)
    {
        foreach (AnimationClip clip in tabletAnimator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }
        Debug.LogError($"Animation '{name}' not found in Animator.");
        return null;
    }

    void FixedUpdate()
    {
        if (isChanging)
        {
            if (isIncreasing)
            {
                currentTopWidth += speed * Time.deltaTime;
            }
            else
            {
                currentTopWidth -= speed * Time.deltaTime;
            }

            currentTopWidth = Mathf.Clamp(currentTopWidth, minWidth, maxWidth);

            if (currentTopWidth == maxWidth || currentTopWidth == minWidth)
            {
                isChanging = false;
            }
        }

        rawImageMaterial.SetFloat("_TopWidth", currentTopWidth);
    }

    public void CloseTablet()
    {
        isChanging = true;
        isIncreasing = false;
    }

    public void OpenTablet()
    {
        isChanging = true;
        isIncreasing = true;
    }

    public void HideSelf()
    {
        manager.isTabletOpened = true;
        gameObject.SetActive(false);
    }
}
