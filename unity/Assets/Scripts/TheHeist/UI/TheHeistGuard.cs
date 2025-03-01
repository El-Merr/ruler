﻿namespace TheHeist
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Util.Geometry.Polygon;

    public class TheHeistGuard : MonoBehaviour
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
                //m_controller.UpdateVision(this);
            }
        }

        public int[] path;

        public Vector2 ori;
       
           

        /// <summary>
        /// Holds the visibility polygon.
        /// </summary>
        public Polygon2D VisionPoly { get; set; }

        void Awake()
        {

            m_controller = FindObjectOfType<TheHeistController>();

            // initialize the vision polygon
            GameObject go = Instantiate(m_visionAreaPrefab, new Vector3(0, 0, -1.5f), Quaternion.identity) as GameObject;
            VisionAreaMesh = go.GetComponent<TheHeistIsland>();

            //m_controller.UpdateVision(this);
        }

        void OnDestroy()
        {
            if (VisionAreaMesh != null)
            {
                Destroy(VisionAreaMesh.gameObject);
            }
        }

        //void OnMouseDown()
        //{
        //    m_controller.SelectLighthouse(this);
        //}
    }
}