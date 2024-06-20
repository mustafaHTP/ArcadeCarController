using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSuspension : MonoBehaviour
{
    [Header("Car RigidBody")]
    [SerializeField] private Rigidbody _carRigidbody;

    [Header("Suspension Config")]
    [Space(5)]
    [SerializeField] private float _suspensionLength;
    [SerializeField] private float _wheelRadius;
    [SerializeField] private float _wheelDisplacementVelocity;
    [SerializeField] private float _suspensionForceStrength;
    [SerializeField] private Transform _wheel;
    [SerializeField] private Transform _baseWheel;

    private Vector3 _suspensionTopPos;
    private float _raycastLength;
    private bool _isSuspensionRayHit;
    private RaycastHit _suspensionHit;

    private void Awake()
    {
        _suspensionTopPos = _baseWheel.position + (_suspensionLength * transform.up);
        //0.1 added because ray not exactly hit ground
        _raycastLength = _suspensionLength + _wheelRadius + 0.1f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(_suspensionTopPos, -1f * transform.up * _raycastLength);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_wheel.position, _wheelRadius);
    }

    private void Update()
    {
        _suspensionTopPos = _baseWheel.position + (_suspensionLength * transform.up);

        Ray suspensionRay = new(_suspensionTopPos, -1f * transform.up);
        _raycastLength = _suspensionLength + _wheelRadius + 0.1f;

        if(Physics.Raycast(suspensionRay, out _suspensionHit, _raycastLength))
        {
            _isSuspensionRayHit = true;

            float deltaPosition = _wheelDisplacementVelocity * Time.deltaTime;
            Vector3 targetPosition = _suspensionHit.point + (_wheelRadius * transform.up);
            _wheel.position = Vector3.Lerp(_wheel.position, targetPosition, deltaPosition);
        }
        else
        {
            _isSuspensionRayHit = false;

            float deltaPosition = _wheelDisplacementVelocity * Time.deltaTime;
            Vector3 targetPosition = _suspensionTopPos - ((_raycastLength - _wheelRadius) * transform.up);
            _wheel.position = Vector3.Lerp(_wheel.position, targetPosition, deltaPosition);
        }
    }

    private void FixedUpdate()
    {
        if (_isSuspensionRayHit)
        {
            _carRigidbody.AddForceAtPosition(
                _wheel.transform.up * _suspensionForceStrength, 
                _wheel.position, 
                ForceMode.Impulse);
        }
    }

}
