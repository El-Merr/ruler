﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheHeistAlgorithm : MonoBehaviour {

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
	List<Line> OriginalLines;

	static TreeNode root = null;
	enum INTERVAL_LOCATION {LEFT,RIGHT,PARTIALLY_LEFT, PARTIALLY_RIGHT, ENCLOSED, ENCLOSING, UNKOWN};


	//class to keep track of line properties
	class Line
	{
		public Line(Vector2 start, Vector2 end)
		{
			this.start = start;
			this.end = end;
		}
		public void translate(Vector2 translation)
		{
			start += translation;
			end += translation;
		}
		public Vector2 start, end;
		public float startAngle = 0f;
		public float endAngle = 0f;
		public float minInterval = 0f;
		public float maxInterval = 0f;
	}
	class TreeNode
	{
		public TreeNode(Line line)
		{
			this.line = line;
			this.minInterval = line.minInterval;
			this.maxInterval = line.maxInterval;
		}
		public bool isRoot = false;
		public bool isLeaf = false;
		public bool hasIntersection = false;
		public TreeNode leftChild = null;
		public TreeNode rightChild = null;
		public Line line;
		public float minInterval, maxInterval;
	}

	private void updatePlayerPos(Vector2 newPos)
	{
		playerPos = newPos;
	}

	private void updateGuardPos(Vector2 newPos)
	{
		guardPos = newPos;
	}

	private void updateGuardOrientation(Vector2 newOrientation)
	{
		guardOrientation = newOrientation;
	}

	bool checkPlayerVisibility(Vector2 playerPos, Vector2 guardPos, Vector2 guardOrientation)
	{
		updatePlayerPos(playerPos);
		updateGuardPos(guardPos);
		updateGuardOrientation(guardOrientation);

		List<Line> sortedLines = sortOnRotationalSweep(OriginalLines);
		generateIntervalTree(sortedLines);




		return true;
	}

	static void generateIntervalTree(List<Line> sortedLines)
	{
		root = new TreeNode(sortedLines[0]);
		root.isRoot = true;

		foreach(Line l in sortedLines)
		{
			insertLineIntoTree(l, root);
		}
	}

	static void insertLineIntoTree(Line l, TreeNode node)
	{
		INTERVAL_LOCATION il = compareIntervals(l.minInterval,l.maxInterval, node.minInterval, node.maxInterval);
		switch (il)
		{
			case INTERVAL_LOCATION.LEFT:
				{
					if (node.leftChild == null)
						node.leftChild = new TreeNode(l);
					else
						insertLineIntoTree(l, node.leftChild);
				}	break;
			case INTERVAL_LOCATION.RIGHT:
				{
					if (node.rightChild == null)
						node.rightChild = new TreeNode(l);
					else
						insertLineIntoTree(l, node.rightChild);
				}	break;
			case INTERVAL_LOCATION.PARTIALLY_LEFT:
				{
					//check for intersection or if one of the two is visible.


				}	break;
			case INTERVAL_LOCATION.PARTIALLY_RIGHT:
				{
					//check for intersection or if one of the two is visible.

				}
				break;
			case INTERVAL_LOCATION.ENCLOSED:
				{
					//check which one is visible and check for intersection.
					print("In enclosed");
				} break;
			case INTERVAL_LOCATION.ENCLOSING:
				{
					//check which one is visible and check for intersection.
					print("In enclosing");
				} break;

			default: print("Can't insert"); break;
		}

	}

	TreeNode balanceTree(TreeNode root)
	{


		return root;
	}

	//Check if a lies left/right/in etc of b.
	static INTERVAL_LOCATION compareIntervals(float a_min, float a_max, float b_min, float b_max)
	{
		//check if we have a  special case because one interval crosses 0.
		bool aSpecial = false;
		bool bSpecial = false;

		if (a_min > a_max)
			aSpecial = true;
		if (b_min > b_max)
			bSpecial = true;

		if(!aSpecial && !bSpecial)
		{
			if (a_min < b_min && a_max < b_min)
				return INTERVAL_LOCATION.LEFT;
			if (a_min > b_max && a_max > b_max)
				return INTERVAL_LOCATION.RIGHT;
			if (a_min <= b_min && a_max > b_min && a_max < b_max)
				return INTERVAL_LOCATION.PARTIALLY_LEFT;
			if (a_min > b_min && a_min <= b_max && a_max > b_max)
				return INTERVAL_LOCATION.PARTIALLY_RIGHT;
			if (a_min >= b_min && a_min <= b_max && a_max >= b_min && a_max <= b_max)
				return INTERVAL_LOCATION.ENCLOSED;
			if (a_min < b_min && a_max > b_max)
				return INTERVAL_LOCATION.ENCLOSING;
			else
			{
				Debug.LogError("Unknown interval comparison");
				return INTERVAL_LOCATION.UNKOWN;
			}				
		}

		Debug.LogError("Unknown interval comparison");
		return INTERVAL_LOCATION.UNKOWN;
	}


	List<Line> sortOnRotationalSweep(List<Line> lines)
	{
		//translate lines so the guard is at 0,0
		List<Line> tempLines = new List<Line>(lines);
		List<Line> linesToBeAdded = new List<Line>();
		foreach(Line l in tempLines)
		{
			l.translate(guardPos*-1);
		}
		
		//calculate angle for each line and normalize them to be between 0 and 360
		foreach(Line l in tempLines)
		{
			l.startAngle = Vector2.SignedAngle(Vector2.up, l.start);
			l.endAngle = Vector2.SignedAngle(Vector2.up, l.end);
			if (l.startAngle < 0)
				l.startAngle += 360;
			if (l.endAngle < 0)
				l.endAngle += 360;
			
			//Set the interval for the line
			if (l.startAngle < l.endAngle)
			{
				l.minInterval = l.startAngle;
				l.maxInterval = l.endAngle;
			}
			else
			{
				l.minInterval = l.endAngle;
				l.maxInterval = l.startAngle;
			}

			//flip the interval if nessesary
			if(l.maxInterval > l.minInterval + 180)
			{
				float tempMin = l.minInterval;
				l.minInterval = l.maxInterval;
				l.maxInterval = tempMin;
			}

			//create two intervals corresponding to a line when the interval contains 0
			if (l.minInterval > l.maxInterval)
			{
				//add a new interval for the line
				Line newLine = new Line(l.start, l.end);
				newLine.startAngle = l.startAngle;
				newLine.endAngle = l.endAngle;
				newLine.minInterval = 0;
				newLine.maxInterval = l.maxInterval;
				linesToBeAdded.Add(newLine);
				//update original interval
				l.maxInterval = 360;
			}
		}

		//add the aditional intervals of the lines
		tempLines.AddRange(linesToBeAdded);
		//sort on smallest start of interval
		tempLines.Sort((x, y) => x.minInterval.CompareTo(y.minInterval));

		//print intervals
		foreach (Line l in tempLines)
		{
			print("Line");
			print("min: " +l.minInterval);
			print("max: " +l.maxInterval);
		}

		return tempLines;
	}

	void setupLevel(Vector2 playerPos, Vector2 guardPos, Vector2 guardOrientation, float coneWidth, float coneLength, List<Line> lines)
	{
		updatePlayerPos(playerPos);
		updateGuardPos(guardPos);
		updateGuardOrientation(guardOrientation);
		this.coneWidth = coneWidth;
		this.coneLength = coneLength;
		this.OriginalLines = lines;
	}

	void Start()
	{
		print("Hello World");
		List<Line> lines = new List<Line>();
		//lines.Add(new Line(new Vector2(1f, 0f), new Vector2(1f, 1f)));
		//lines.Add(new Line(new Vector2(1f, 1f), new Vector2(1f, -1f)));
		//lines.Add(new Line(new Vector2(1f, -1f), new Vector2(-1f, -1f)));
		//lines.Add(new Line(new Vector2(-1f, -1f), new Vector2(-1f, 1f)));

		//lines.Add(new Line(new Vector2(0f, 1f), new Vector2(1f, 0f)));
		//lines.Add(new Line(new Vector2(1f, 0f), new Vector2(0f, -1f)));
		//lines.Add(new Line(new Vector2(0f, -1f), new Vector2(-1f, 0f)));
		//lines.Add(new Line(new Vector2(-1f, 0f), new Vector2(0f, 1f)));

		//lines.Add(new Line(new Vector2(0f, 2f), new Vector2(2f, 0f)));
		//lines.Add(new Line(new Vector2(1.9f, 0f), new Vector2(0f, -1.9f)));
		//lines.Add(new Line(new Vector2(0f, -1.8f), new Vector2(-1.8f, 0f)));
		//lines.Add(new Line(new Vector2(-1.7f, 0f), new Vector2(0f, 1.7f)));

		lines.Add(new Line(new Vector2(1f, 1f), new Vector2(3f, 1f)));
		lines.Add(new Line(new Vector2(4f, 1f), new Vector2(5f, 1f)));
		lines.Add(new Line(new Vector2(-1f, 1f), new Vector2(-3f, 1f)));
		lines.Add(new Line(new Vector2(-4f, 1f), new Vector2(-6f, 1f)));
		lines.Add(new Line(new Vector2(-1f, 1f), new Vector2(1f, 1f)));


		setupLevel(playerPos, guardPos, guardOrientation, coneWidth, coneLength, lines);

		checkPlayerVisibility(playerPos, guardPos, guardOrientation);

		//foreach(Line l in sortOnRotationalSweep())
		//{
		//	print(l.minInterval);
		//	print(l.maxInterval);
		//}
	}

}
