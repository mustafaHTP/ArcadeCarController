using Cinemachine.Utility;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private float _acceleration;
    [SerializeField] private float _topSpeed;
    [SerializeField] private float _rearTopSpeed;
    [SerializeField] private float _steerVelocity;
    [SerializeField] private Rigidbody _rigidbody;

    [Header("Steer Help & Anti Slip")]
    [SerializeField] private bool _isApplyOrthogonalForce;
    [SerializeField] private bool _isApplyAntiSlipForce;
    [SerializeField] private float _antiSlipForce;
    [SerializeField] private float _orthogonalForce;

    private const float MinCarSpeed = 0f;
    private const float MinSteerVelocity = 0f;
    private float _moveInput;
    private float _steerInput;

    private void Awake()
    {
        _rigidbody.transform.parent = null;
    }

    private void Update()
    {
        GetInputs();
        Steer();
    }

    private void FixedUpdate()
    {
        SyncTransformWithRigidbody();
        //Steer();
        HelpSteer();
        MoveCar();

        IsGoesBackwards();
    }

    private void HelpSteer()
    {
        float dotResult = Vector3.Dot(transform.forward.normalized, _rigidbody.velocity.normalized);

        if (Mathf.Abs(dotResult) < 1f && _isApplyAntiSlipForce)
        {
            _rigidbody.AddForce(
                -1f * _rigidbody.velocity * _antiSlipForce * Time.fixedDeltaTime);
        }

        float orthogonalDotResult = Vector3.Dot(transform.right.normalized, _rigidbody.velocity.normalized);

        if (Mathf.Abs(dotResult) > 0f && _isApplyOrthogonalForce)
        {
            _rigidbody.AddForce(
                -1f * _orthogonalForce * orthogonalDotResult * Time.fixedDeltaTime * transform.right);
        }
    }

    private void MoveCar()
    {
        if (!IsExceedTopSpeed() && !IsExceedRearTopSpeed())
        {
            float deltaForce = _moveInput * Time.fixedDeltaTime * _acceleration;
            _rigidbody.AddForce(transform.forward * deltaForce);
        }
    }

    private void Steer()
    {
        float currentSpeed = _rigidbody.velocity.magnitude;
        float lerpedSpeed = Mathf.InverseLerp(MinCarSpeed, _topSpeed, currentSpeed);
        float lerpedSteerVelocity = Mathf.Lerp(MinSteerVelocity, _steerVelocity, lerpedSpeed);
        float deltaRotation = (_steerInput * lerpedSteerVelocity * Time.fixedDeltaTime);

        if (IsGoesBackwards())
        {
            deltaRotation *= -1f;
        }

        transform.Rotate(0f, deltaRotation, 0f);
    }

    private void SyncTransformWithRigidbody()
    {
        transform.position = _rigidbody.position;
    }

    private void GetInputs()
    {
        _moveInput = Input.GetAxis("Vertical");
        _steerInput = Input.GetAxis("Horizontal");
    }

    private bool IsExceedTopSpeed()
    {
        return _rigidbody.velocity.magnitude > _topSpeed;
    }

    private bool IsExceedRearTopSpeed()
    {
        if (!IsGoesBackwards()) return false;

        return Mathf.Abs(_rigidbody.velocity.magnitude) > _rearTopSpeed;
    }

    private bool IsGoesBackwards()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(_rigidbody.velocity);
        float forwardLocalVelocity = localVelocity.z;

        return forwardLocalVelocity < 0f;
    }
}
