using System.Collections;
using UnityEngine;
using YG;

namespace EachOneMatters.Systems
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _cameraPositionToLevel;
        [SerializeField] private float _xAngleOffsetMobile = 50;
        [SerializeField] private float _yAngleOffsetMobile = 180;
        [SerializeField] private float _fieldOfViewDesktop = 70;
        [SerializeField] private float _zoomStep = 0.5f;
        [SerializeField] private float _speed = 5;
        [SerializeField] private float _yOffset = 0.7f;
        [SerializeField] private int _unitsPerZoomLevel = 10;

        private int _lastZoomLevel;
        private Vector3 _baseOffset;
        private Coroutine _coroutine;

        private void Awake()
        {
            _baseOffset = _cameraPositionToLevel.localPosition;
        }

        public void ConfigureCameraForPlatform()
        {
            if (YG2.envir.isDesktop == false)
            {
                Quaternion angle = Quaternion.Euler(_xAngleOffsetMobile, _yAngleOffsetMobile, 0);
                _cameraPositionToLevel.localRotation = angle;
                AdjustCameraForMobile();
                _cameraPositionToLevel.localPosition = _baseOffset;
            }
            else
            {
                _camera.fieldOfView = _fieldOfViewDesktop;
            }

            _camera.transform.parent = _cameraPositionToLevel;
            _camera.transform.localPosition = Vector3.zero;
            _camera.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        public void OnUnitCountChanged(int newUnitCount)
        {
            int currentZoomLevel = newUnitCount / _unitsPerZoomLevel;

            if (currentZoomLevel > _lastZoomLevel)
            {
                int zoomDifference = currentZoomLevel - _lastZoomLevel;

                _baseOffset.y += zoomDifference * _zoomStep;
                _baseOffset.z += zoomDifference * _zoomStep;

                _lastZoomLevel = currentZoomLevel;

                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                }

                _coroutine = StartCoroutine(UpdateCameraPosition());
            }
            else if (currentZoomLevel < _lastZoomLevel)
            {
                int zoomDifference = _lastZoomLevel - currentZoomLevel;

                _baseOffset.y -= zoomDifference * _zoomStep;
                _baseOffset.z -= zoomDifference * _zoomStep;

                _lastZoomLevel = currentZoomLevel;

                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                }

                _coroutine = StartCoroutine(UpdateCameraPosition());
            }
        }

        private void AdjustCameraForMobile()
        {
            float halfBridgeWidth = 15f;
            float fov = 80f;

            float distance = halfBridgeWidth / Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
            _baseOffset = new Vector3(0, distance * _yOffset, _baseOffset.z);
            _camera.fieldOfView = fov;
            _camera.transform.localPosition = _baseOffset;
        }

        private IEnumerator UpdateCameraPosition()
        {
            float elapsed = 0f;

            while (elapsed < _speed)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _speed;
                t = Mathf.SmoothStep(0f, 1f, t);
                _cameraPositionToLevel.localPosition = Vector3.Lerp(_cameraPositionToLevel.localPosition, _baseOffset, t);
                yield return null;
            }

            _cameraPositionToLevel.localPosition = _baseOffset;
            _coroutine = null;
        }
    }
}