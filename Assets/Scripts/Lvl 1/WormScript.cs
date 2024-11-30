using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormScript : MonoBehaviour
{
    public AudioSource ackSFX;
    public AudioSource spawnTreeSFX;

    public GameObject smallTree;
    public GameObject bigTree;
    public Transform props;

    public Vector3 spawnPos;

    public void SpawnSmallTree()
    {
        smallTree.SetActive(true);
    }

    public void SpawnBigTree()
    {
        smallTree.SetActive(false);

        Instantiate(bigTree, spawnPos, Quaternion.identity, props);
        bigTree.GetComponent<TreeAtCliff>().ahSound = ackSFX;
        bigTree.GetComponent<TreeAtCliff>().worm = gameObject;
        bigTree.GetComponent<TreeAtCliff>().isCompleteTutor = true;
    }

    public void PlayMicrowaveSound()
    {
        spawnTreeSFX.Play();
    }

    public void HideSelf()
    {
        gameObject.SetActive(false);
    }
}
