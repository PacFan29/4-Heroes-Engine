using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SplineMesh {
    public class CookieChain : MonoBehaviour
    {
        public GameObject startCookie;
        public Spline cookies;

        private PlayerInfo player;
        private bool started = false;
        private float rate = 0;
        
        void OnTriggerEnter(Collider col) {
            if (col.gameObject.tag == "Player") {
                player = col.GetComponent<PlayerInfo>();

                GameManager.Coins++;
                player.scoreIncrease(20);
                started = true;

                startCookie.SetActive(false);

                player.VelocitySetUp(Vector3.zero);
                player.axisInput = false;
                player.activePhysics = false;
            }
        }

        void Update() {
            if (started) {
                rate += Time.deltaTime;

                if (rate > cookies.nodes.Count - 1) {
                    player.axisInput = true;
                    player.activePhysics = true;
                    Destroy(gameObject);
                }

                CurveSample sample = cookies.GetSample(rate);
                player.gameObject.transform.position = this.transform.position + sample.location;
                player.skin.localRotation = sample.Rotation;
            }
        }
    }
}