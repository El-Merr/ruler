﻿using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

public class TheHeistAlgorithmV2 : MonoBehaviour
{

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

    enum COLOR { RED, BLACK};
    enum DIRECTION { LEFT, RIGHT};
    enum COMPARE { SMALLER, EQUAL, GREATER, UNKOWN}

    class Line
    {
        public Line(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;
            this.middle = ((end - start) / 2) + end;
        }
        public void translate(Vector2 translation)
        {
            start += translation;
            end += translation;
        }
        public Vector2 start, end, middle;
        public float startDistance, endDistance, shortestDistance, longestDistance, middleDistance;
        public float startInterval, endInterval;
    }

    class Interval : IInterval
    {
        public bool isLeftEndpoint;
        public Endpoint endpoint;
        Line correspondingLine;
        //TODO keep track of intervals in which the endpoint lies.

        public Interval(Endpoint endpoint, Line correspondingLine)
        {
            this.endpoint = endpoint;
            this.correspondingLine = correspondingLine;
        }
        public COMPARE compare(IInterval other)
        {
            Interval interval = other as Interval;

            if (interval == null)
                throw new Exception("Other interval to compare is null");

            if (this.endpoint.pos > interval.endpoint.pos)
                return COMPARE.SMALLER;
            else if (this.endpoint.pos == interval.endpoint.pos)
                return COMPARE.EQUAL;
            else if (this.endpoint.pos < interval.endpoint.pos)
                return COMPARE.GREATER;
            else
                return COMPARE.UNKOWN;
        }

        public string print()
        {
            if (endpoint != null)
                return "pos: " + endpoint.pos + "is left: " + isLeftEndpoint;
            else
                return "No endpoint avaiable";
        }
    }
     interface IInterval
        {
            COMPARE compare(IInterval other);
            string print();
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

   

    class Node
    {
        //comparable data in our case a interval(eindpoint) 
        public IInterval data;

        //node info
        public Node left;
        public Node right;
        public Node parent;
        public bool isLeftChild;
        public COLOR color = COLOR.BLACK;

        //creates a node with empty children
        public Node(IInterval data)
        {
            setData(data);
        }

        //sets the data and create empty children because the node is already a leaf.
        public void setData(IInterval data)
        {
            this.data = data;
            color = COLOR.RED;
            left = new Node();
            right = new Node();
            left.setParent(this);
            right.setParent(this);
            left.isLeftChild = true;
            right.isLeftChild = false;
        }

        public void setParent(Node parent)
        {
            this.parent = parent;
        }

        public void setRight(Node rightchild)
        {
            right = rightchild;
            right.setParent(this);
        }

        public void setLeft(Node leftchild)
        {
            left = leftchild;
            left.setParent(this);
        }

        public string print()
        {
            string msg = "";
            if (data != null)
            {
                if (color == COLOR.RED)
                {
                    msg = "Red - " + data.print();
                }
                else
                {
                    msg = "Black - " + data.print();
                }
            }
            else
                    msg = "No interval available";
            return msg;
        }

        public Node()
        {
            
        }
    }

    class RedBlackTree
    {
        public Node root;

        public  RedBlackTree()
        {
            root = new Node(null);
        }

        public void insertInterval(IInterval interval)
        {
            insertNode(interval, ref root);
            
        }


        public void print()
        {
            printNode(ref root);
        }

        private void printNode(ref Node currentNode)
        {
            string msg;
            if(currentNode!=null && currentNode.left != null && currentNode.right != null)
            {
                msg = "current: " + currentNode.print() + " left: " + currentNode.left.print() + " right: " + currentNode.right.print();
            }
            else
            {
                msg = "leaf";
            }

            Debug.Log(msg);
            if (currentNode.left != null)
                printNode(ref currentNode.left);
            if (currentNode.right != null)
                printNode(ref currentNode.right);
        }
        private void insertNode(IInterval interval, ref Node currentNode)
        {
            if (currentNode.data == null)
            {
                currentNode.setData(interval);
                return;
            }
            else
            {
                COMPARE result = currentNode.data.compare(interval);
                switch (result)
                {
                    case COMPARE.SMALLER:
                        {
                            insertNode(interval, ref currentNode.left);
                        }break;
                    case COMPARE.EQUAL:
                        {
                            insertNode(interval, ref currentNode.left);
                        }break;
                    case COMPARE.GREATER:
                        {
                            insertNode(interval, ref currentNode.right);
                        }
                        break;
                    default:
                        {
                            throw new Exception("Can't compare interval data");
                        }break;
                }
            }

        }

        private void balance(ref Node currentNode)
        {
            if (currentNode.parent == null) //root
            {
                currentNode.color = COLOR.BLACK;
                return;
            }
            else if(currentNode.parent.color == COLOR.BLACK)  //parent is black
            {
                return;
            }
            else if(currentNode.parent.color == COLOR.RED) // parent is red
            {
                if(currentNode.parent.parent != null) //grandparent exists
                {
                    if(currentNode.parent.isLeftChild) // parent is a left child
                    {
                        if(currentNode.parent.parent.right != null) // rightsibling exists
                        {
                            if(currentNode.parent.parent.right.color == COLOR.RED) // right sibling is red
                            {
                                currentNode.parent.parent.right.color = COLOR.BLACK;
                                currentNode.parent.color = COLOR.BLACK;
                            }
                            else //right sibling is black
                            {
                                //check black or null rotation an recolour
                                //rotate and recolour
                            }
                        }
                        else // right sibling is null
                        {
                            //rotate and recolour
                        }
                    }
                    else // parent is a right child
                    {
                        if(currentNode.parent.parent.left!= null) // left sibling exits
                        {
                            if(currentNode.parent.parent.left.color == COLOR.RED)//left sibling is red
                            {
                                currentNode.parent.parent.left.color = COLOR.BLACK;
                                currentNode.parent.color = COLOR.BLACK;
                            }
                            else //left sibling is black
                            {
                                //rotate and recolour
                            }
                        }
                        else // left sibling is null
                        {
                            //rotate and recolour
                        }
                    }
                }
                else //grand parent doesn't exist
                {
                    //rotation an recolour
                }
            }

        }

        private void rotate(bool leftRotation, ref Node currentNode)
        {
            if(leftRotation)
            {
                Node parentCurrentNode = currentNode.parent;


            }
            else
            {
                
            }


        }
    }

    

    public bool checkVisibility(Vector2 playerPos, Vector2 guardPos, Vector2 guardOrientation)
    {
        this.playerPos = playerPos;
        this.guardPos = guardPos;
        this.guardOrientation = guardOrientation;

        List<Line> sortedShortestDistance = new List<Line>();
        List<Line> sortedLongestDistance = new List<Line>();
        List<Line> sortedMiddletDistance = new List<Line>();
        List<Line> sortedStartInterval = new List<Line>();
        List<Line> sortedEndInterval = new List<Line>();

        //Get the lines sorted on distances and angles after translating the current level by the guard pos.
        calculateDistanceAndAngle(CurrentLevel, guardPos, out sortedShortestDistance, out sortedLongestDistance, out sortedMiddletDistance, out sortedStartInterval, out sortedEndInterval);

        //create inteval tree
        RedBlackTree tree = new RedBlackTree();
        foreach(Line l in sortedMiddletDistance)
        {
            tree.insertInterval(new Interval(new Endpoint(l.startInterval,true), l));
            tree.insertInterval(new Interval(new Endpoint(l.endInterval, false), l));
        }

        tree.print();

        return true;
    }

    void diskSweep()
    {
        //Important to have the lvl and the guard pos set!!
        //List<Line> linesWithDistance = calculateDistance(CurrentLevel, guardPos);
        //Queue<HeistEvent> status = getEvents(linesWithDistance);
        //return status;
    }

   void getEvents(List<Line> sortedLines)
    {
        //List<HeistEvent> events = new List<HeistEvent>();
        //foreach(Line l in sortedLines)
        //{
        //    events.Add(new HeistEvent(l, true));
        //    events.Add(new HeistEvent(l, false));
        //}
       
        ////sort on event with smallest distance
        //events.Sort((x, y) => x.distance.CompareTo(y.distance));
        //Queue<HeistEvent> status = new Queue<HeistEvent>();
        //foreach(HeistEvent he in events)
        //{
        //    status.Enqueue(he);
        //}
        //return status;
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

        checkVisibility(Vector2.zero, Vector2.zero, Vector2.zero);
    }


}
