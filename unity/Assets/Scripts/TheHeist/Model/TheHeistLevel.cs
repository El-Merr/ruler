﻿namespace TheHeist
{
    using General.Model;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Util.Geometry.Polygon;

    /// <summary>
    /// Acts as a data container for an art gallery level.
    /// Stores the level polygon (2D polygon without holes) and the max lighthouse parameter.
    /// </summary>
    [CreateAssetMenu(fileName = "thLevelNew", menuName = "Levels/The Heist Level")]
    public class TheHeistLevel : ScriptableObject
    {
        [Header("Level Parameters")]
        public int MaxNumberOfLighthouses = 1;

        [Header("Polygon")]
        public List<Vector2> Outer = new List<Vector2>() { 
            // default triangle
            new Vector2(1, 1), new Vector2(2, 0), new Vector2(0, 0)
        };

        public List<Vector2> CheckPoints = new List<Vector2>() {
            // default point inside triangle
            new Vector2(1, 0.5f)
        };

        public List<Vector2Array> Holes = new List<Vector2Array>();

        public Polygon2DWithHoles Polygon
        {
            get { return new Polygon2DWithHoles(new Polygon2D(Outer), Holes.Select(h => new Polygon2D(h.Points))); }
        }

        [Header("Guards")]
        public List<Vector2> Guards = new List<Vector2>();
        [Header("Player")]
        public List<Vector2> Player = new List<Vector2>();
        [Header("Goal")]
        public List<Vector2> Goal = new List<Vector2>();


    }
}
