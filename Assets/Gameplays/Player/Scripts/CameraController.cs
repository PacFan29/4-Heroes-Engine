using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    new CinemachineVirtualCamera camera;
    CinemachineOrbitalTransposer transposer;
    Transform targetPlayer;
    Vector3 cameraRotation;

    public bool is3D;
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<CinemachineVirtualCamera>();
        if (camera.GetCinemachineComponent<CinemachineOrbitalTransposer>() != null) {
            transposer = camera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlayerInfo[] allPlayers = GameObject.FindObjectsOfType<PlayerInfo>();
        for (int i = 0; i < allPlayers.Length; i++){
            //1P着目
            if (allPlayers[i].playerNumber == 0){
                targetPlayer = allPlayers[i].gameObject.transform;
            }
        }

        BossManager[] goArray = GameObject.FindObjectsOfType<BossManager>();
        Transform bossObj = goArray.Length > 0 ? goArray[0].gameObject.transform : null;
        
        if (is3D && (!GameManager.death || GameManager.falling)) {
            camera.LookAt = targetPlayer;
        } else if (GameManager.death) {
            camera.LookAt = null;
        } else {
            camera.LookAt = null;
        }

        if (GameManager.death) {
            camera.Follow = null;
        } else {
            camera.Follow = targetPlayer;

            cameraRotation.x = Input.GetAxisRaw("Camera X") * 600;
            cameraRotation.z = Input.GetAxisRaw("Camera Y") * 30;

            if (cameraRotation.magnitude >= 0.1 && transposer != null && is3D){
                transposer.m_Heading.m_Bias += cameraRotation.x * 3f; //Biasを操作
                transposer.m_FollowOffset.y -= cameraRotation.z / 8f; //Follow Offsetを操作

                if (transposer.m_FollowOffset.y < -12f) {
                    transposer.m_FollowOffset.y = -12f;
                } else if (transposer.m_FollowOffset.y > 40f) {
                    transposer.m_FollowOffset.y = 40f;
                }
            }
        }

        /*
        Yrotation 0 = Offset (0, 10, -25)
        Yrotation 90 = Offset (-25, 10, 0)
        Yrotation 180 = Offset (0, 10, 25)
        Yrotation -90 = Offset (25, 10, 0)

        Offset Y = X or Z * 0.4f
        */
    }
}
