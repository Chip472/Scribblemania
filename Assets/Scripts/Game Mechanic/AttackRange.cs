using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    bool isAttacking = false;
    PlayerMovement player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player = collision.gameObject.GetComponent<PlayerMovement>();
            if (player != null && transform.parent.GetComponent<PotatoeAttack>().isAttacking)
            {
                StartCoroutine(player.GetHitCoolDown(2f, transform.parent.GetComponent<PotatoeAttack>().potatoAttackDam));
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !isAttacking && transform.parent.GetComponent<PotatoeAttack>().isAttacking)
        {
            isAttacking = true;
            StartCoroutine(DelayDamage(player));
        }
    }

    IEnumerator DelayDamage(PlayerMovement player)
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(player.GetHitCoolDown(1f, transform.parent.GetComponent<PotatoeAttack>().potatoAttackDam));
        isAttacking = false;
    }
}
