using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private GameObject tabletFrame;
    [SerializeField] private Animator tabletFrameAnim;

    public bool isTabletOpened = true;
    public Animator playerAnim;

    private void Update()
    {
        if (isTabletOpened && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            isTabletOpened = false;
            tabletFrame.SetActive(true);
            tabletFrameAnim.SetBool("closeTablet", false);
            playerAnim.SetFloat("RunType", 1);
        }
        else if (!isTabletOpened && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            isTabletOpened = true;
            tabletFrameAnim.SetBool("closeTablet", true);
            playerAnim.SetFloat("RunType", 0);
        }
    }
}
