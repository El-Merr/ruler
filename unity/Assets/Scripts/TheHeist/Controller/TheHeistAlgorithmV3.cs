using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util.DataStructures.BST;
using System;
using Util.Geometry;


public class TheHeistAlgorithmV3 : MonoBehaviour {

    // position of the player,guard and objects.
    [SerializeField]
    Vector2 playerPos;
    [SerializeField]
    Vector2 guardPos;
    [SerializeField]
    Vector2 guardOrientation;
    [SerializeField]
    float coneWidth;
    [SerializeField]
    float coneLength;
    [SerializeField]
    static List<Line> CurrentLevel;

    class Line
    {
        public Line(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;
            this.middle = ((end - start) / 2) + end;
            this.segment = new LineSegment(start, end);
        }
        public void translate(Vector2 translation)
        {
            start += translation;
            end += translation;
        }
        public Vector2 start, end, middle;
        public float startDistance, endDistance, shortestDistance, longestDistance, middleDistance;
        public float startInterval, endInterval;
        public LineSegment segment;
    }

    class Endpoint
    {
        public float pos;
        public bool isLeft;

        public Endpoint(float pos, bool isLeft)
        {
            this.pos = pos;
            this.isLeft = isLeft;
        }
    }

    class IntervalPoint : IComparable<IntervalPoint>,IEquatable<IntervalPoint>
    {
        public bool isLeft;
        public float endpoint;
        Line correspondingLine;
        List<IntervalPoint> knownIntervals;
        //TODO keep track of intervals in which the endpoint lies.

        public IntervalPoint(Line correspondingLine, bool isLeft)
        {
            this.correspondingLine = correspondingLine;
            this.isLeft = isLeft;
            if (isLeft)
                endpoint = correspondingLine.startInterval;
            else
                endpoint = correspondingLine.endInterval;
        }

        public int CompareTo(IntervalPoint t)
        {
            if (this.endpoint > t.endpoint)
                return 1;
            else if (this.endpoint < t.endpoint)
                return -1;
            else 
                return 0;
        }

        public bool Equals(IntervalPoint t)
        {
            if (this.correspondingLine.start == t.correspondingLine.start && this.correspondingLine.end == t.correspondingLine.end)
                return true;
            else
                return false;
        }

        public void updateKnownIntervals()
        {
            //todo 
        }


        //public COMPARE compare(IInterval other)
        //{
        //    Interval interval = other as Interval;

        //    if (interval == null)
        //        throw new Exception("Other interval to compare is null");

        //    if (this.endpoint.pos > interval.endpoint.pos)
        //        return COMPARE.SMALLER;
        //    else if (this.endpoint.pos == interval.endpoint.pos)
        //        return COMPARE.EQUAL;
        //    else if (this.endpoint.pos < interval.endpoint.pos)
        //        return COMPARE.GREATER;
        //    else
        //        return COMPARE.UNKOWN;
    }


    void calculateDistanceAndAngle(List<Line> currentLevel, Vector2 guardPos, out List<Line> sortedShortestDistance, out List<Line> sortedLongestDistance
            , out List<Line> sortedMiddleDistance, out List<Line> sortedStartInterval, out List<Line> sortedEndInterval)
    {
        //Translate level
        List<Line> tempLines = new List<Line>(currentLevel);
        foreach (Line l in tempLines)
            l.translate(guardPos * -1);

        //calculate angle for each line and make sure they are between 0 and 360
        List<Line> linesToBeAdded = new List<Line>();
        foreach (Line l in tempLines)
        {
            float startAngle = Vector2.SignedAngle(Vector2.up, l.start);
            float endAngle = Vector2.SignedAngle(Vector2.up, l.end);

            if (startAngle < 0)
                startAngle += 360;
            if (endAngle < 0)
                endAngle += 360;

            //Get the rotational interval from the line
            if (startAngle < endAngle)
            {
                l.startInterval = startAngle;
                l.endInterval = endAngle;
            }
            else
            {
                l.startInterval = endAngle;
                l.endInterval = startAngle;
            }

            //flip the interval if nessesary
            if (l.endInterval > l.startInterval + 180)
            {
                float tempStartInterval = l.startInterval;
                l.startInterval = l.endInterval;
                l.endInterval = tempStartInterval;
            }

            //Split a line in two when the interval contains 0
            if (l.startInterval > l.endInterval)
            {
                //split the line in two lines
                Line newLine = new Line(l.start, l.end);
                newLine.startInterval = 0;
                newLine.endInterval = l.endInterval;
                linesToBeAdded.Add(newLine);
                //update original 
                l.endInterval = 360;
            }
        }

        //add the aditional intervals of the lines
        tempLines.AddRange(linesToBeAdded);

        //get distance form the start, end and middle points and keep track of the shorest one.
        foreach (Line l in tempLines)
        {
            l.startDistance = l.start.magnitude;
            l.endDistance = l.end.magnitude;
            l.middleDistance = l.middle.magnitude;

            if (l.startDistance < l.endDistance)
            {
                l.shortestDistance = l.startDistance;
                l.longestDistance = l.endDistance;
            }
            else
            {
                l.shortestDistance = l.endDistance;
                l.longestDistance = l.startDistance;
            }
        }

        //create and sort lists
        sortedShortestDistance = new List<Line>(tempLines);
        sortedLongestDistance = new List<Line>(tempLines);
        sortedMiddleDistance = new List<Line>(tempLines);
        sortedStartInterval = new List<Line>(tempLines);
        sortedEndInterval = new List<Line>(tempLines);
        //we assume this sort is in nLog(n)
        sortedShortestDistance.Sort((x, y) => x.shortestDistance.CompareTo(y.shortestDistance));
        sortedLongestDistance.Sort((x, y) => x.longestDistance.CompareTo(y.longestDistance));
        sortedMiddleDistance.Sort((x, y) => x.middleDistance.CompareTo(y.middleDistance));
        sortedStartInterval.Sort((x, y) => x.startInterval.CompareTo(y.startInterval));
        sortedEndInterval.Sort((x, y) => x.endInterval.CompareTo(y.endInterval));
    }


    // Use this for initialization
    void Start()
    {
        List<Line> lines = new List<Line>();
        lines.Add(new Line(new Vector2(1f, 1f), new Vector2(3f, 1f)));
        lines.Add(new Line(new Vector2(4f, 1f), new Vector2(5f, 1f)));
        lines.Add(new Line(new Vector2(-1f, 1f), new Vector2(-3f, 1f)));
        lines.Add(new Line(new Vector2(-4f, 1f), new Vector2(-6f, 1f)));
        lines.Add(new Line(new Vector2(-1f, 1f), new Vector2(1f, 1f)));
        CurrentLevel = lines;

        //checkVisibility(Vector2.zero, Vector2.zero, Vector2.zero);
    }
}
