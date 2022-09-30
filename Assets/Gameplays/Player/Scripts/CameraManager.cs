using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public GameData data;
    public CinemachineVirtualCamera openWorld3D;
    public CinemachineVirtualCamera linear3D;
    public CinemachineVirtualCamera XWay2D;
    public CinemachineVirtualCamera ZWay2D;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        openWorld3D.Priority = 0;
        linear3D.Priority = 0;
        XWay2D.Priority = 0;
        ZWay2D.Priority = 0;

        if (!GameManager.death) {
            switch (GameManager.dimension){
                case DimensionType.Normal3D:
                //3D
                if (data.openWorldAsDefault) {
                    //オープンワールド
                    openWorld3D.Priority = 1;
                } else {
                    linear3D.Priority = 1;
                }
                break;

                case DimensionType.XWay2D:
                //2D(X方向)
                XWay2D.Priority = 1;
                break;
                
                case DimensionType.ZWay2D:
                //2D(Z方向)
                ZWay2D.Priority = 1;
                break;

                case DimensionType.FreeWay3D:
                //3D(方向指定)
                linear3D.Priority = 1;
                break;
            }
        }
    }
}
