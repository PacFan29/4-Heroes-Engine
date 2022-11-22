using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    //Code by Gabriel Aguiar Prod.
    //Tutorial : https://www.youtube.com/watch?v=7vvycc2iX6E

    public PlayerInfo info;

    [Header("Shader Related")]
    public Material mat;

    private bool isTrailActive;
    private SkinnedMeshRenderer[] skinnedMeshRenderers;

    // Update is called once per frame
    void Update()
    {
        if (info.speedUp && !isTrailActive) {
            isTrailActive = true;
            StartCoroutine(ActivateTrail());
        }
    }

    IEnumerator ActivateTrail() {
        while (info.speedUp) {
            if (skinnedMeshRenderers == null) {
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            }

            for (int i = 0; i < skinnedMeshRenderers.Length; i++) {
                GameObject obj = new GameObject();
                obj.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);

                MeshRenderer mr = obj.AddComponent<MeshRenderer>();
                MeshFilter mf = obj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);

                mf.mesh = mesh;
                mr.material = mat;

                StartCoroutine(AnimateMaterialFloat(mr.material, 0, 0.05f, 0.02f));

                Destroy(obj, 3f);
            }

            yield return new WaitForSeconds(0.05f);
        }
        isTrailActive = false;
    }

    IEnumerator AnimateMaterialFloat(Material mat, float goal, float rate, float refreshRate) {
        float valueToAnimate = mat.GetFloat("Alpha");

        while (valueToAnimate > goal) {
            valueToAnimate -= rate;
            mat.SetFloat("Alpha", valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
