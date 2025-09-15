using UnityEngine;
using YG;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _xAngleOffsetMobile = 50;
    [SerializeField] private float _yAngleOffsetMobile = 180;
    [SerializeField] private float _fieldOfView = 110;

    public void ConfigureCameraForPlatform()
    {
        if (YG2.envir.isDesktop == false)
        {
            Quaternion angle = Quaternion.Euler(_xAngleOffsetMobile, _yAngleOffsetMobile, 0);

            _camera.transform.localRotation = angle;
            
            _camera.fieldOfView = _fieldOfView;
        }
    }
}
