using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundedDustEffect : MonoBehaviour
{
    ParticleSystem m_ParticleSystem;
    public GameObject dust;
    public float dustScale = 1f;
    public bool dirDepend;
    private ParticleSystem.Particle[] m_Particles;
    private int maxParticles;
    private GameObject[] allParticles = new GameObject[20];
    // Start is called before the first frame update
    void Start()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
        
        maxParticles = m_ParticleSystem.main.maxParticles;

        for (int i = 0; i < maxParticles; i++){
            allParticles[i] = Instantiate(dust, Vector3.zero, Quaternion.identity, this.transform);
            allParticles[i].transform.localScale = new Vector3(dustScale, dustScale, dustScale);
            var main = allParticles[i].GetComponent<ParticleSystem>().main;
            //main.startSize = 5f;
            main.startLifetime = m_ParticleSystem.main.startLifetime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Particles == null || m_Particles.Length < maxParticles)
        {
            m_Particles = new ParticleSystem.Particle[maxParticles];
        }
        int particleNum = m_ParticleSystem.GetParticles(m_Particles);

        //パーティクルひとつひとつの処理
        for (int i = 0; i < particleNum; i++)
        {
            if (allParticles[i] != null) {
                Vector3 particlePos = m_Particles[i].position;
                if (dirDepend) {
                    allParticles[i].transform.localPosition = this.transform.localPosition + particlePos;
                } else {
                    allParticles[i].transform.position = this.transform.position + particlePos;
                }
                //allParticles[i].transform.position = this.transform.position + (m_Particles[i].position.magnitude * this.transform.forward);
            }
        }
    }
}
