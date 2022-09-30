using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningCoinController : MonoBehaviour
{
    public GameObject coinObject;
    public int counts;
    public float distance;
    public float rotateSpeed;
    public GameObject hideObject;

    private GameObject[] allParts = new GameObject[360];
    private double[] directions = new double[360];
    // Start is called before the first frame update
    void Start()
    {
        if (counts < 1){
            counts = 1;
        } //else if (counts > 16){
            //counts = 16;
        //}
        for (int i = 0; i < counts; i++){
            allParts[i] = Instantiate(coinObject, this.transform);
            directions[i] = (360.0 * i) / counts;
        }
        hideObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        int count = 0;

        for (int i = 0; i < counts; i++){
            if (allParts[i] != null){
                if (!allParts[i].GetComponent<CoinManager>().magnetised) {
                    allParts[i].transform.position = this.transform.position;
                    allParts[i].transform.position += 
                    new Vector3((float)Math.Sin(DirectionSet(directions[i])) * distance, 
                    0, (float)Math.Cos(DirectionSet(directions[i])) * distance);

                    directions[i] += rotateSpeed;
                    directions[i] %= 360;
                }

                count++;
            }
        }

        if (count <= 0){
            Destroy(gameObject);
        }
    }

    double DirectionSet (double radian){
        return Math.PI * radian / 180.0;
    }
}
