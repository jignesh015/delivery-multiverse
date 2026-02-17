using UnityEngine;

namespace DeliveryMultiverse
{
    [ExecuteInEditMode]
    public class SkyboxTextureScroller : MonoBehaviour
    {
        [Header("Skybox Material")]
        [Tooltip("The skybox material to scroll. If null, will use RenderSettings skybox.")]
        public Material skyboxMaterial;

        [Header("Scroll Settings")]
        [Tooltip("Enable/disable texture scrolling")]
        public bool enableScrolling = true;

        [Header("Front Texture (+Z)")]
        public bool scrollFront = true;
        public Vector2 frontScrollSpeed = new Vector2(0.01f, 0f);

        [Header("Back Texture (-Z)")]
        public bool scrollBack = true;
        public Vector2 backScrollSpeed = new Vector2(0.01f, 0f);

        [Header("Left Texture (+X)")]
        public bool scrollLeft = true;
        public Vector2 leftScrollSpeed = new Vector2(0.01f, 0f);

        [Header("Right Texture (-X)")]
        public bool scrollRight = true;
        public Vector2 rightScrollSpeed = new Vector2(0.01f, 0f);

        [Header("Up Texture (+Y)")]
        public bool scrollUp = true;
        public Vector2 upScrollSpeed = new Vector2(0.01f, 0f);

        [Header("Down Texture (-Y)")]
        public bool scrollDown = true;
        public Vector2 downScrollSpeed = new Vector2(0.01f, 0f);

        [Header("Advanced Settings")]
        [Tooltip("Reset offsets when they reach 1.0 to prevent floating point precision issues")]
        public bool wrapOffsets = true;

        [Tooltip("Use unscaled time (ignores Time.timeScale)")]
        public bool useUnscaledTime = false;

        // Private variables to store current offsets
        private Vector2 frontOffset = Vector2.zero;
        private Vector2 backOffset = Vector2.zero;
        private Vector2 leftOffset = Vector2.zero;
        private Vector2 rightOffset = Vector2.zero;
        private Vector2 upOffset = Vector2.zero;
        private Vector2 downOffset = Vector2.zero;

        // Shader property names
        private readonly string frontTexProperty = "_FrontTex";
        private readonly string backTexProperty = "_BackTex";
        private readonly string leftTexProperty = "_LeftTex";
        private readonly string rightTexProperty = "_RightTex";
        private readonly string upTexProperty = "_UpTex";
        private readonly string downTexProperty = "_DownTex";

        void Start()
        {
            // If no material is assigned, try to get the skybox from RenderSettings
            if (!skyboxMaterial)
            {
                skyboxMaterial = RenderSettings.skybox;
            }

            // Initialize offsets from current material values
            if (skyboxMaterial)
            {
                InitializeOffsets();
            }
        }

        void InitializeOffsets()
        {
            if (!skyboxMaterial) return;

            // Get current texture offsets from the material
            frontOffset = skyboxMaterial.GetTextureOffset(frontTexProperty);
            backOffset = skyboxMaterial.GetTextureOffset(backTexProperty);
            leftOffset = skyboxMaterial.GetTextureOffset(leftTexProperty);
            rightOffset = skyboxMaterial.GetTextureOffset(rightTexProperty);
            upOffset = skyboxMaterial.GetTextureOffset(upTexProperty);
            downOffset = skyboxMaterial.GetTextureOffset(downTexProperty);
        }

        void Update()
        {
            if (!Application.isPlaying) return;
            if (!enableScrolling || !skyboxMaterial) return;

            float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            // Update each texture offset based on its scroll speed
            if (scrollFront)
            {
                frontOffset += frontScrollSpeed * deltaTime;
                if (wrapOffsets) frontOffset = WrapOffset(frontOffset);
                skyboxMaterial.SetTextureOffset(frontTexProperty, frontOffset);
            }

            if (scrollBack)
            {
                backOffset += backScrollSpeed * deltaTime;
                if (wrapOffsets) backOffset = WrapOffset(backOffset);
                skyboxMaterial.SetTextureOffset(backTexProperty, backOffset);
            }

            if (scrollLeft)
            {
                leftOffset += leftScrollSpeed * deltaTime;
                if (wrapOffsets) leftOffset = WrapOffset(leftOffset);
                skyboxMaterial.SetTextureOffset(leftTexProperty, leftOffset);
            }

            if (scrollRight)
            {
                rightOffset += rightScrollSpeed * deltaTime;
                if (wrapOffsets) rightOffset = WrapOffset(rightOffset);
                skyboxMaterial.SetTextureOffset(rightTexProperty, rightOffset);
            }

            if (scrollUp)
            {
                upOffset += upScrollSpeed * deltaTime;
                if (wrapOffsets) upOffset = WrapOffset(upOffset);
                skyboxMaterial.SetTextureOffset(upTexProperty, upOffset);
            }

            if (scrollDown)
            {
                downOffset += downScrollSpeed * deltaTime;
                if (wrapOffsets) downOffset = WrapOffset(downOffset);
                skyboxMaterial.SetTextureOffset(downTexProperty, downOffset);
            }
        }

        Vector2 WrapOffset(Vector2 offset)
        {
            // Wrap offset values to stay between 0 and 1
            offset.x = offset.x % 1.0f;
            offset.y = offset.y % 1.0f;

            // Handle negative values
            if (offset.x < 0) offset.x += 1.0f;
            if (offset.y < 0) offset.y += 1.0f;

            return offset;
        }

        // Public methods for runtime control
        public void ResetOffsets()
        {
            frontOffset = Vector2.zero;
            backOffset = Vector2.zero;
            leftOffset = Vector2.zero;
            rightOffset = Vector2.zero;
            upOffset = Vector2.zero;
            downOffset = Vector2.zero;

            if (skyboxMaterial != null)
            {
                skyboxMaterial.SetTextureOffset(frontTexProperty, frontOffset);
                skyboxMaterial.SetTextureOffset(backTexProperty, backOffset);
                skyboxMaterial.SetTextureOffset(leftTexProperty, leftOffset);
                skyboxMaterial.SetTextureOffset(rightTexProperty, rightOffset);
                skyboxMaterial.SetTextureOffset(upTexProperty, upOffset);
                skyboxMaterial.SetTextureOffset(downTexProperty, downOffset);
            }
        }

        public void SetAllScrollSpeeds(Vector2 speed)
        {
            frontScrollSpeed = speed;
            backScrollSpeed = speed;
            leftScrollSpeed = speed;
            rightScrollSpeed = speed;
            upScrollSpeed = speed;
            downScrollSpeed = speed;
        }

        public void EnableAllFaces(bool enable)
        {
            scrollFront = enable;
            scrollBack = enable;
            scrollLeft = enable;
            scrollRight = enable;
            scrollUp = enable;
            scrollDown = enable;
        }

        void OnValidate()
        {
            // Re-initialize offsets if material changes in inspector
            if (skyboxMaterial && Application.isPlaying)
            {
                InitializeOffsets();
            }
        }

        void OnDestroy()
        {
            ResetOffsets();
        }
    }
}
