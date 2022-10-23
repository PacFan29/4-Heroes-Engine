using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SplineMesh {
    public class Experiment : MonoBehaviour
    {
        public Spline rail;
        public float offset = 0.3f;

        private float rate = 0.0f;
        // Start is called before the first frame update
        void Start()
        {
            ;
        }

        // Update is called once per frame
        void Update()
        {
            rate += Time.deltaTime;

            if (rate > rail.nodes.Count - 1) {
                rate = 0.0f;
            }

            CurveSample sample = rail.GetSample(rate);
            this.transform.localRotation = sample.Rotation;
            this.transform.position = rail.gameObject.transform.position + sample.location + (transform.up * offset);
        }
    }
}
