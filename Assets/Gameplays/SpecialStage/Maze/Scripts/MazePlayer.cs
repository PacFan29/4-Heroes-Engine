using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazePlayer : MazeCharacter
{
    [Header("エフェクト")]
    public GameObject invincibleEffect;

    float invincibleTime = 0f;
    bool isInvincible = false;

    void Update()
    {
        if (invincibleTime > 0) {
            invincibleTime -= Time.deltaTime;
            if (invincibleTime <= 0) {
                isInvincible = false;
            }
        }
        invincibleEffect.SetActive(isInvincible);

        if (Input.GetAxisRaw("Horizontal") >= 0.5f && (canRotate(Vector2.right) || direction == Vector2.left) && direction != Vector2.right) {
            if (direction != Vector2.left) gridPos();
            direction = Vector2.right;
        } else if (Input.GetAxisRaw("Horizontal") <= -0.5f && (canRotate(Vector2.left) || direction == Vector2.right) && direction != Vector2.left) {
            if (direction != Vector2.right) gridPos();
            direction = Vector2.left;
        } else if (Input.GetAxisRaw("Vertical") >= 0.5f && (canRotate(Vector2.up) || direction == Vector2.down) && direction != Vector2.up) {
            if (direction != Vector2.down) gridPos();
            direction = Vector2.up;
        } else if (Input.GetAxisRaw("Vertical") <= -0.5f && (canRotate(Vector2.down) || direction == Vector2.up) && direction != Vector2.down) {
            if (direction != Vector2.up) gridPos();
            direction = Vector2.down;
        }
    }

    public void PowerUp() {
        isInvincible = true;
        invincibleTime = 7.5f;
    }
}
