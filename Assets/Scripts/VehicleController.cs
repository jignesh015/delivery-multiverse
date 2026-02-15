using UnityEngine;

namespace DeliveryMultiverse
{
    [RequireComponent(typeof(Rigidbody))]
    public class VehicleController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputManager inputManager;
        [SerializeField] private Transform movementReference;

        [Header("Biome")]
        [SerializeField] private BiomeConfig defaultBiome;

        [Header("Grounding")]
        [SerializeField] private LayerMask groundMask = ~0;
        [SerializeField] private float groundCheckDistance = 0.2f;
        
        [Header("Debug")]
        [SerializeField] private bool isGrounded;

        private Rigidbody m_Rb;
        private float m_AccelerateInput;
        private float m_BrakeInput;
        private float m_SteerInput;
        private bool m_JumpPressed;
        private Vector3 m_DesiredDirection;

        private float m_MaxSpeed;
        private float m_ReverseSpeed;
        private float m_Acceleration;
        private float m_Drag;
        private float m_SteeringSensitivity;
        private float m_GravityMultiplier;
        private float m_JumpForce;
        private float m_ExtraLateralDrift;

        // Brake/Reverse parameters
        private const float BrakeForce = 50f;
        private const float MinSpeedForSteering = 0.5f;

        private void Awake()
        {
            m_Rb = GetComponent<Rigidbody>();
            if (inputManager == null)
            {
                inputManager = GetComponent<InputManager>();
            }
        }

        private void Start()
        {
            if (defaultBiome != null)
            {
                ApplyBiomeConfig(defaultBiome);
            }
        }

        private void Update()
        {
            HandleInput();
        }

        private void FixedUpdate()
        {
            HandleMovement();
            ApplyGravity();
            HandleSteering();
            isGrounded = IsGrounded();
        }

        public void ApplyBiomeConfig(BiomeConfig config)
        {
            if (config == null)
            {
                return;
            }

            m_MaxSpeed = config.maxSpeed;
            m_ReverseSpeed = config.maxSpeed * 0.5f;
            m_Acceleration = config.acceleration;
            m_Drag = config.drag;
            m_SteeringSensitivity = config.steeringSensitivity;
            m_GravityMultiplier = config.gravityMultiplier;
            m_JumpForce = config.jumpForce;
            m_ExtraLateralDrift = config.extraLateralDrift;

            m_Rb.linearDamping = m_Drag;
        }

        private void HandleInput()
        {
            if (!inputManager)
            {
                m_AccelerateInput = 0f;
                m_BrakeInput = 0f;
                m_SteerInput = 0f;
                m_JumpPressed = false;
                return;
            }

            m_AccelerateInput = Mathf.Clamp01(inputManager.AccelerateInput);
            m_BrakeInput = Mathf.Clamp01(inputManager.BrakeInput);
            m_SteerInput = Mathf.Clamp(inputManager.SteerInput, -1f, 1f);
        }

        private void HandleMovement()
        {
            var planarVelocity = new Vector3(m_Rb.linearVelocity.x, 0f, m_Rb.linearVelocity.z);
            var planarSpeed = planarVelocity.magnitude;
            var currentForward = transform.forward;
            currentForward.y = 0f;
            currentForward.Normalize();

            // Check if moving forward or backward relative to car's orientation
            bool isMovingForward = Vector3.Dot(planarVelocity, currentForward) > 0f;

            // Handle acceleration
            if (m_AccelerateInput > 0.01f)
            {
                // Forward acceleration
                m_DesiredDirection = currentForward;
                m_Rb.AddForce(m_DesiredDirection * (m_Acceleration * m_AccelerateInput), ForceMode.Acceleration);

                // Apply lateral drift when steering
                if (m_ExtraLateralDrift > 0f && Mathf.Abs(m_SteerInput) > 0.01f)
                {
                    var right = Vector3.Cross(Vector3.up, m_DesiredDirection);
                    var driftForce = right * (m_SteerInput * planarSpeed * m_ExtraLateralDrift);
                    m_Rb.AddForce(driftForce, ForceMode.Acceleration);
                }
            }

            // Handle braking and reverse
            if (m_BrakeInput > 0.01f)
            {
                if (isMovingForward && planarSpeed > 0.5f)
                {
                    // Apply brake - slow down the car
                    m_Rb.AddForce(-planarVelocity.normalized * (BrakeForce * m_BrakeInput), ForceMode.Acceleration);
                }
                else
                {
                    // Reverse - move backward
                    var reverseDirection = -currentForward;
                    m_Rb.AddForce(reverseDirection * (m_Acceleration * 0.5f * m_BrakeInput), ForceMode.Acceleration);
                    m_DesiredDirection = reverseDirection;

                    // Clamp reverse speed
                    if (planarSpeed > m_ReverseSpeed)
                    {
                        var clamped = planarVelocity.normalized * m_ReverseSpeed;
                        m_Rb.linearVelocity = new Vector3(clamped.x, m_Rb.linearVelocity.y, clamped.z);
                    }
                }
            }

            // Maintain desired direction for steering when coasting
            if (m_AccelerateInput < 0.01f && m_BrakeInput < 0.01f)
            {
                if (planarVelocity.sqrMagnitude > 0.01f)
                {
                    m_DesiredDirection = planarVelocity.normalized;
                }
                else
                {
                    m_DesiredDirection = currentForward;
                }
            }

            // Jump
            m_JumpPressed = inputManager.ConsumeJumpPressed();
            if (m_JumpPressed && m_JumpForce > 0f && IsGrounded())
            {
                m_Rb.AddForce(Vector3.up * m_JumpForce, ForceMode.VelocityChange);
                Debug.Log("<color=cyan>Jump executed</color>");
            }

            // Clamp forward speed when accelerating
            if (m_AccelerateInput > 0.01f)
            {
                ClampPlanarSpeed();
            }
        }

        private void ApplyGravity()
        {
            var gravityOffset = m_GravityMultiplier - 1f;
            if (Mathf.Abs(gravityOffset) < 0.001f)
            {
                return;
            }

            m_Rb.AddForce(Physics.gravity * gravityOffset, ForceMode.Acceleration);
        }

        private void HandleSteering()
        {
            if (m_SteeringSensitivity <= 0f)
            {
                Debug.Log("Steering disabled: m_SteeringSensitivity is 0");
                return;
            }

            // Only allow steering if there's forward input or the car has sufficient speed
            var planarVelocity = new Vector3(m_Rb.linearVelocity.x, 0f, m_Rb.linearVelocity.z);
            var planarSpeed = planarVelocity.magnitude;

            // Don't steer if no input and speed is too low
            if (m_AccelerateInput < 0.01f && m_BrakeInput < 0.01f && planarSpeed < MinSpeedForSteering)
            {
                return;
            }

            // Get current forward direction
            var currentForward = transform.forward;
            currentForward.y = 0f;
            currentForward.Normalize();

            // Calculate the steering angle per second (degrees)
            float turnSpeed = m_SteeringSensitivity; // Scale up for noticeable turning
            float steerAngle = m_SteerInput * turnSpeed * Time.fixedDeltaTime;

            // Apply steering rotation
            if (Mathf.Abs(steerAngle) > 0.001f)
            {
                Quaternion deltaRotation = Quaternion.Euler(0f, steerAngle, 0f);
                Quaternion newRotation = m_Rb.rotation * deltaRotation;
                m_Rb.MoveRotation(newRotation);
            }
        }

        private void ClampPlanarSpeed()
        {
            if (m_MaxSpeed <= 0f)
            {
                return;
            }

            var planarVelocity = new Vector3(m_Rb.linearVelocity.x, 0f, m_Rb.linearVelocity.z);
            if (planarVelocity.sqrMagnitude <= m_MaxSpeed * m_MaxSpeed)
            {
                return;
            }

            var clamped = planarVelocity.normalized * m_MaxSpeed;
            m_Rb.linearVelocity = new Vector3(clamped.x, m_Rb.linearVelocity.y, clamped.z);
        }

        private bool IsGrounded()
        {
            var origin = transform.position + Vector3.up * 0.05f;
            var distance = groundCheckDistance + 0.05f;
            return Physics.Raycast(origin, Vector3.down, distance, groundMask, QueryTriggerInteraction.Ignore);
        }
    }
}
