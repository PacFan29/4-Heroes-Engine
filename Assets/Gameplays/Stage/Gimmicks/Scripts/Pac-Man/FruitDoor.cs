using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitDoor : MonoBehaviour
{
    [Header("フルーツアイコン")]
    public int fruitIndex;
    public Sprite[] sprites = new Sprite[8];
    public SpriteRenderer icon;
    [Header("効果音")]
    public AudioSource source;
    public AudioClip locked;
    public AudioClip unlock;
    [Header("ドア")]
    public GameObject group;

    // Update is called once per frame
    void Update()
    {
        icon.sprite = sprites[fruitIndex];
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.GetComponent<PlayerInfo>() != null) {
            if (GameManager.fruits[fruitIndex] > 0) {
                source.clip = unlock;
                source.Play();
                Destroy(group);
            } else {
                source.clip = locked;
                source.Play();
            }
        }
    }
}
