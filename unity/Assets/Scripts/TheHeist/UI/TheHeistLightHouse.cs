﻿namespace TheHeist
{
    using UnityEngine;
    using Util.Geometry.Polygon;

    /// <summary>
    /// Represents the lighthouse object in the game.
    /// Holds its position as well as the corresponding visibility polygon.
    /// Handles user clicks and drags.
    /// </summary>
    public class TheHeistLightHouse : MonoBehaviour
    {
        // stores a prefab object for the vision polygon
        [SerializeField]
        private GameObject m_visionAreaPrefab;

        private TheHeistController m_controller;

        /// <summary>
        /// Mesh variable of the art gallery.
        /// </summary>
        public TheHeistIsland VisionAreaMesh { get; set; }

        /// <summary>
        /// Stores lighthouse position. Updates vision after a change in position.
        /// </summary>
        public Vector3 Pos
        {
            get
            {
                return gameObject.transform.position;
            }
            set
            {
                gameObject.transform.position = value;

                // update vision polygon
               // m_controller.UpdateVision(this);
            }
        }

        /// <summary>
        /// Holds the visibility polygon.
        /// </summary>
        public Polygon2D VisionPoly { get; set; }

        // Use this for initialization
        void Awake()
        {
            m_controller = FindObjectOfType<TheHeistController>();

            // initialize the vision polygon
            GameObject go = Instantiate(m_visionAreaPrefab, new Vector3(0, 0, -1.5f), Quaternion.identity) as GameObject;
            VisionAreaMesh = go.GetComponent<TheHeistIsland>();

           // m_controller.UpdateVision(this);
        }

        void OnDestroy()
        {
            if (VisionAreaMesh != null)
            {
                Destroy(VisionAreaMesh.gameObject);
            }
        }
    }
}
