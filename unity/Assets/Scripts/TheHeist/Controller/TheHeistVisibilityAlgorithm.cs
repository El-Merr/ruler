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

public class TheHeistAlgorithmV3 : MonoBehaviour {

	// position of the player,guard and objects.
	[SerializeField]
	Vector2 playerPos;
	[SerializeField]
	Vector2 guardPos;
	[SerializeField]
	Vector2 guardOrientation;
    [SerializeField]
    Ray guardDir;
    [SerializeField]
	float coneWidth = 40;
	[SerializeField]
	float coneLength;
    

    float CalcAngle(Vector2 vertice)
    {
        float angle = Vector2.SignedAngle(vertice, Vector2.up);
        if (angle < 0) angle += 360;
        return angle;
    }

    List<Vector2> RadialSweepAlgorithm(Polygon2DWithHoles polygon, List<Vector2> vertices)
    {
        List<Vector2> visible = new List<Vector2>();
        
        foreach (var v in vertices)
        {
            // if angle falls outside 180 vision
            if (CalcAngle(v) > 180) continue;

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

    private bool betweenAngles(float test, float a1, float a2)
    {
        if (a1 < test && test < a2) return true;
        else return true;
    }

    List<Event> GetEvents(Polygon2DWithHoles polygon)
    {
        Vector2 origin = Vector2.zero;
        Vector2 direction = new Vector2(0, -1);
        Vector2 offset = new Vector2(1, 0);
        Ray2D coneFirst = new Ray2D(origin, direction - offset);
        Ray2D coneSecond = new Ray2D(origin, direction + offset);

        List<Event> events = new List<Event>();
        List<LineSegment> segments = polygon.Segments.ToList();

        // create events
        for (int i = 0; i < segments.Count; i++) {
            Vector2 vertex = segments[i].Point1;
            LineSegment prev = (i != 0) ? segments[i - 1] : segments[segments.Count - 1];
            LineSegment next = segments[i];

            events.Add(new Event(vertex, new Segment(prev), new Segment(next)));
        }

        // filter and sort events
        events.Sort();
        var angle_cone1 = CalcAngle(coneFirst.direction);
        var angle_cone2 = CalcAngle(coneSecond.direction);

        foreach (Event e in events) {
            var angle_v = CalcAngle(e.vertex);
            var angle_prev = CalcAngle(e.prevSeg.segment.Point1);
            var angle_next = CalcAngle(e.nextSeg.segment.Point2);

            // if all points fall outside and on the same side of the cone
            if (betweenAngles(angle_v, angle_cone1,  270) && betweenAngles(angle_prev, angle_cone1, 270) && betweenAngles(angle_prev, angle_cone1, 270))
            {
                events.Remove(e);
            }

            if (betweenAngles(angle_v, 90, angle_cone2) && betweenAngles(angle_prev, 90, angle_cone2) && betweenAngles(angle_prev, 90, angle_cone2))
            {
                events.Remove(e);
            }
        }


        return events;
    }

    void PostProcess()
    {

    }

	void Start()
	{
		print("Start");

        //setupLevel(playerPos, guardPos, guardOrientation, coneWidth, coneLength, lines);

        //checkPlayerVisibility(playerPos, guardPos, guardOrientation);

        //List<Line> lines = new List<Line>();
        //lines.Add(new Line(new Vector2(1f, 1f), new Vector2(3f, 1f)));
        //lines.Add(new Line(new Vector2(4f, 1f), new Vector2(5f, 1f)));
        //lines.Add(new Line(new Vector2(-1f, 1f), new Vector2(-3f, 1f)));
        //lines.Add(new Line(new Vector2(-4f, 1f), new Vector2(-6f, 1f)));
        //lines.Add(new Line(new Vector2(-1f, 1f), new Vector2(1f, 1f)));
        //CurrentLevel = lines;


        List<Vector2> Outer = new List<Vector2>() { 
            // default triangle
            new Vector2(0, 4), new Vector2(4, 4), new Vector2(4, 0), new Vector2(0, 0)
        };

        Polygon2DWithHoles polygon = new Polygon2DWithHoles(new Polygon2D(Outer));

        guardPos = new Vector2(1, 1);
        //guardDir = new Ray(guardPos, );
        playerPos = new Vector2(2, 2);

        // shift such that guard is origin of polygon
        polygon.ShiftToOrigin(guardPos);

        List<Event> events = GetEvents(polygon);


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

        print(visiblePoly);

    }

    // Event consists of a point and its adjecent line segments
    public class Event : IComparable<Event>, IEquatable<Event>
    {
        public readonly Vector2 vertex;
        public Segment prevSeg;
        public Segment nextSeg;

        private static readonly VertexComparer vc = new VertexComparer();

        public Event(Vector2 v, Segment prevSeg, Segment nextSeg)
        {
            vertex = v;
            this.prevSeg = prevSeg;
            this.nextSeg = nextSeg;
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

    
