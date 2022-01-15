namespace TheHeist
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Util.Geometry;
    using Util.Geometry.Polygon;
    using Util.DataStructures.BST;
    using Util.DataStructures.Queue;
    using Util.Math;
    using System.Linq;
    using System;
    using Util.Algorithms.Polygon;

    public class TheHeistVisibilityAlgorithm : MonoBehaviour
    {

        // position of the player,guard and objects.
        [SerializeField]
        Vector2 playerPos;
        [SerializeField]
        Vector2 guardPos;
        [SerializeField]
        Vector2 guardOrientation;
        [SerializeField]
        Ray guardDir;
        //   [SerializeField]
        //float coneWidth = 40;
        //[SerializeField]
        //float coneLength;

        private static Vector2 origin = Vector2.zero;
        private static float coneWidth = 30;

        private Cone cone;

        // constructor for the class
        public Polygon2D VisionCone(Polygon2DWithHoles polygon, Vector2 guardPos, Vector2 playerPos, Vector2 guardOrientation)
        {
            // set values
            this.playerPos = playerPos;
            this.guardPos = guardPos;
            this.guardOrientation = (guardOrientation.Equals(origin)) ? Vector2.left : guardOrientation;

            // this.cone = new Cone(origin, guardOrientation, coneWidth);

            // shift such that guard is origin of polygon
            polygon.ShiftToOrigin(guardPos);

            //// rotate
            //List<Vector2> rotatedVertices = new List<Vector2>();
            //var angle = MathUtil.Angle(guardOrientation, origin, Vector2.right);

            //foreach (var v in polygon.Outside.Vertices)
            //{
            //    var rv = MathUtil.Rotate(v, angle);
            //    rotatedVertices.Add(rv);
            //}

            //Polygon2DWithHoles rotatedPoly = new Polygon2DWithHoles(new Polygon2D(rotatedVertices));

            // create and filter the events
            //List<Event> events = GetEvents(rotatedPoly);


            //// create sweepline
            //Ray2D sweepline = new Ray2D(origin, Rotate(cone.ray.direction, -90));

            //// init status and list for resulting polygon
            //AATree<Segment> status = InitStatusTree(events, sweepline); //new AATree<StatusItem>();

            //List<Vector2> result = HandleEvents(events, status, sweepline);


            // return the visiblity polygon
            // polygon.ShiftToOrigin(-guardPos);
            return polygon.Outside; // new Polygon2D(result);
        }

        // returns angle between v and cone direction in degrees
        public float CalcAngle(Vector2 vertice)
        {
            float angle = Vector2.SignedAngle(vertice, cone.ray.direction);
            if (angle < 0) angle += 360;
            return angle;
        }

        private bool betweenAngles(float test, float a1, float a2)
        {
            if (a1 < test && test < a2) return true;
            else return true;
        }

        public static Vector2 Rotate(Vector2 v, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }

        List<Vector2> RadialSweepAlgorithm(Polygon2DWithHoles polygon, List<Vector2> vertices)
        {
            List<Vector2> visible = new List<Vector2>();

            foreach (var v in vertices)
            {
                // if angle falls outside 180 vision
                // if (CalcAngle(v) > 180) continue;

                visible.Add(v);
            }

            return visible;

            // for every vertice
            // check if point / line segment lies behind current range

            // if completely begin
            // ignore

            // if fully visible and does not intersect current range
            // add entire line segment

            // if fully visible but intersects current range
            // add entire line segment and update current ovelapping range

            // if partially visible but behind
            // add visible part

            // if partially visible but in front
            // add visible part and update current range



        }

        private List<Event> CreateEvent(List<LineSegment> segments, bool isHole)
        {
            List<Event> e = new List<Event>();
            for (int i = 0; i < segments.Count; i++)
            {
                Vector2 vertex = segments[i].Point1;
                LineSegment prev = (i != 0) ? segments[i - 1] : segments[segments.Count - 1];
                LineSegment next = segments[i];

                e.Add(new Event(vertex, new Segment(prev), new Segment(next), isHole, cone.ray.direction));
            }
            return e;
        }

        private List<Event> GetEvents(Polygon2DWithHoles polygon)
        {
            // Vector2 origin = Vector2.zero;
            // Vector2 direction = new Vector2(0, -1);
            // Vector2 offset = new Vector2(1, 0);
            // Ray2D coneFirst = new Ray2D(origin, direction - offset);
            // Ray2D coneSecond = new Ray2D(origin, direction + offset);

            List<Event> events = new List<Event>();
            List<LineSegment> segmentsOutside = polygon.Outside.Segments.ToList();
            ICollection<Polygon2D> polyHoles = polygon.Holes;



            // create outside events
            events.AddRange(CreateEvent(segmentsOutside, false));

            // add hole events
            foreach (var hole in polyHoles)
            {
                List<LineSegment> segments = hole.Segments.ToList();
                events.AddRange(CreateEvent(segments, true));
            }

            // filter and sort events
            //events.Sort();
            //var angle_cone1 = CalcAngle(cone.start.direction);
            //var angle_cone2 = CalcAngle(cone.end.direction);

            //foreach (Event e in events) {
            //    var angle_v = CalcAngle(e.vertex);
            //    var angle_prev = CalcAngle(e.prevSeg.segment.Point1);
            //    var angle_next = CalcAngle(e.nextSeg.segment.Point2);

            //    // if all points fall outside and on the same side of the cone
            //    if (betweenAngles(angle_v, angle_cone1,  270) && betweenAngles(angle_prev, angle_cone1, 270) && betweenAngles(angle_prev, angle_cone1, 270))
            //    {
            //        events.Remove(e);
            //    }

            //    if (betweenAngles(angle_v, 90, angle_cone2) && betweenAngles(angle_prev, 90, angle_cone2) && betweenAngles(angle_prev, 90, angle_cone2))
            //    {
            //        events.Remove(e);
            //    }
            //}


            return events;
        }

        // Initializes the status structure with all Segments intersecting the sweepline
        private AATree<Segment> InitStatusTree(List<Event> events, Ray2D sweepline)
        {
            AATree<Segment> status = new AATree<Segment>();

            // if the event's previous segment intersects the sweepline, add the segment to the status
            foreach (var e in events)
            {
                var intersection = e.prevSeg.segment.Intersect(sweepline);
                if (intersection.HasValue) status.Insert(e.prevSeg);
            }

            return status;
        }

        private List<Vector2> HandleEvents(List<Event> events, AATree<Segment> status, Ray2D sweepline)
        {
            // List of vertices for the resulting visibility polygon
            List<Vector2> result = new List<Vector2>();

            Segment topSeg;
            status.FindMin(out topSeg);

            //

            for (var i = 0; i < events.Count; i++)
            {
                Event e = events[i];

                // new sweepling
                sweepline = new Ray2D(origin, e.vertex);

                // check if sweepline falls outside cone
                // handle final event

                // handle previous segment
                // if (status.Contains(e.prevSeg)) 

                // handle next segment

            }

            return result;
        }

        /**
        void Start()
        {
            print("Start");


            List<Vector2> Outer = new List<Vector2>() { 
                // default triangle
                new Vector2(0, 4), new Vector2(4, 4), new Vector2(4, 0), new Vector2(0, 0)
            };

            Polygon2DWithHoles polygon = new Polygon2DWithHoles(new Polygon2D(Outer));

            guardPos = new Vector2(1, 1);
            //guardDir = new Ray(guardPos, );
            playerPos = new Vector2(2, 2);
            guardOrientation = new Vector2(0, 1);

          //  Polygon2D visiblePoly = VisionCone(polygon, guardPos, playerPos, guardOrientation);

            //foreach (var v in polygon.Vertices)
            //{
            //    MathUtil.Rotate(v, (Math.PI / 180) * 90);
            //}


            //// ICollection<Vector2> vertices = polygon.Vertices;
            //List<Vector2> vertices = new List<Vector2>(); 

            //foreach (var v in polygon.Vertices)
            //{
            //    vertices.Add(MathUtil.Rotate(v, (Math.PI / 180) * 90));
            //}

            //Polygon2DWithHoles originPoly = new Polygon2DWithHoles(new Polygon2D(vertices));

            //List<Vector2> verticesList = vertices.ToList();
            //verticesList.Sort((a, b) => CalcAngle(a).CompareTo(CalcAngle(b)));

            //List<Vector2> visible = RadialSweepAlgorithm(originPoly, verticesList);
            
            //foreach (var v in visible)
            //{
            //    MathUtil.Rotate(v, (Math.PI / 180) * -90);
            //}
            //Polygon2D visiblePoly = new Polygon2D(visible);

            //visiblePoly.ShiftToOrigin(-guardPos);

          //  print(visiblePoly);

        }
        **/

        private class Cone
        {
            public Ray2D ray;
            public Ray2D start;
            public Ray2D end;

            public Cone(Vector2 origin, Vector2 direction, float width)
            {
                this.ray = new Ray2D(origin, direction);

                Vector2 vecStart = Rotate(direction, width);
                Vector2 vecEnd = Rotate(direction, -width);

                this.start = new Ray2D(origin, vecStart);
                this.end = new Ray2D(origin, vecEnd);
            }

            public Cone(Ray2D direction, float width)
            {
                this.ray = direction;

                Vector2 vecStart = Rotate(ray.direction, width);
                Vector2 vecEnd = Rotate(ray.direction, -width);

                this.start = new Ray2D(origin, vecStart);
                this.end = new Ray2D(origin, vecEnd);
            }


        }

        // Event consists of a point and its adjecent line segments
        public class Event : IComparable<Event>, IEquatable<Event>
        {
            public readonly Vector2 vertex;
            // public float degrees;
            public float radians;
            public Segment prevSeg;
            public Segment nextSeg;
            public bool isHole;

            private static readonly VertexComparer vc = new VertexComparer();

            public Event(Vector2 vertex, Segment prevSeg, Segment nextSeg, bool isHole, Vector2 coneDirection)
            {
                this.vertex = vertex;
                this.prevSeg = prevSeg;
                this.nextSeg = nextSeg;
                this.isHole = isHole;

                // this.degrees = CalcAngle(Vector2.zero);
                this.radians = (float)MathUtil.Angle(vertex, origin, coneDirection);
            }

            public int CompareTo(Event otherEvent)
            {
                return vc.Compare(vertex, otherEvent.vertex);
            }

            public bool Equals(Event otherEvent)
            {
                return vertex.Equals(otherEvent.vertex);
            }
        }

        public class Segment : IComparable<Segment>, IEquatable<Segment>
        {
            public readonly Vector2 vertex;
            public LineSegment segment;

            private static readonly VertexComparer vc = new VertexComparer();

            public Segment(Vector2 p1, Vector2 p2)
            {
                segment = new LineSegment(p1, p2);
            }

            public Segment(LineSegment linesegment)
            {
                segment = linesegment;
            }

            public bool Equals(Segment otherSeg)
            {
                return segment.Equals(otherSeg.segment);
            }

            public int CompareTo(Segment otherSeg)
            {
                return 1;
            }
        }
    }
}

    
