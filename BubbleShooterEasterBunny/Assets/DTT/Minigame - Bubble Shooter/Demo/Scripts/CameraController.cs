using UnityEngine;

namespace DTT.BubbleShooter.Demo
{

    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float m_minAspect = 1.77778f;
        [SerializeField] private float m_maxAspect = 2.77778f;

        [SerializeField] private float m_targetAspect = 1.77778f;
        public float TargetAspect => m_targetAspect;

        //Minimum orthographics size in relation to m_targetSize applied when the aspect ratio equals to m_maxAspect
        [SerializeField] private float m_minSize = 6.33f;
        [SerializeField] private float m_targetSize = 8;
        public float TargetSize => m_targetSize;

        private float m_width;
        private float m_height;

        private Camera m_camera;

        private void Start()
        {
            m_camera = GetComponent<Camera>();
            UpdateSize();
        }


        private void UpdateSize()
        {
            //I can't explain why, but it works when I change all m_minAspect, m_maxAspect, and m_targetAspect to 16/9 (1.77778f)
            if (m_width != Screen.width || m_height != Screen.height)
            {
                m_width = Screen.width;
                m_height = Screen.height;

                float aspect = m_height / m_width;

                //Clamp to minimum aspect ratio so we don't go mad with scaling in landscape
                aspect = Mathf.Max(m_minAspect, aspect);

                //Scale for the current aspect ratio
                m_camera.orthographicSize = m_targetSize * (aspect / m_targetAspect) * Mathf.Lerp(1, m_minSize / m_targetSize, Mathf.InverseLerp(m_minAspect, m_maxAspect, aspect));

                Vector3 pos = transform.position;
                //Apply offset to camera based on difference
                //pos.y = m_camera.orthographicSize - m_targetSize;

                //Due to the change of how grid is generate, this pos.y may need set to 0
                pos.y = 0;
                transform.position = pos;
            }
        }

        private void Update()
        {
            UpdateSize();
        }
    }
}