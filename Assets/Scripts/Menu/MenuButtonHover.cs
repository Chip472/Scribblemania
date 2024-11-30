using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite normal, hover;
    public Image buttonImg;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        buttonImg.sprite = hover;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        buttonImg.sprite = normal;
    }
}
