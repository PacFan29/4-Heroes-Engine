using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingSpreader : MonoBehaviour
{
    public GameObject ringLoss;
    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }
    
    public void SpreadRings(int rings, bool hyper) {
        source.Play();

        int ring_amount = 1;
        int ring_counter = 0;
        float ring_angle = 11.25f;
        bool flip = false;
        float speed = 20f;

        if (hyper) {
            ring_angle = 22.5f;
            if (rings > 8) ring_amount = (int)Math.Floor((float)rings / 8f);
        }

        while (ring_counter < rings) {
            MovingRingManager oneRing = Instantiate(ringLoss, transform.position, Quaternion.identity).GetComponent<MovingRingManager>();
            oneRing.angle = ring_angle;
            oneRing.speed = speed;
            if (hyper) {
                oneRing.amount = ring_amount;
                oneRing.hyper = true;
            }
            oneRing.setSpeed();

            ring_angle *= -1;
            if (flip) {
                if (hyper) {
                    ring_angle += 45f;
                } else {
                    ring_angle += 22.5f;
                }
            }
            flip = !flip;

            ring_counter++;
            
            if (hyper && ring_counter == 8) {
                break;
            } else if (ring_counter == 16) {
                speed = 10f;
                ring_angle = 11.25f;
            } else if (ring_counter == 32) {
                break;
            }
        }
    }
}
