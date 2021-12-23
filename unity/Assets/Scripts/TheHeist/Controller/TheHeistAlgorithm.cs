using System.Collections;
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
	List<Line> lines;

	static TreeNode root = null;
	enum INTERVAL_LOCATION {LEFT,RIGHT,PARTIALLY_LEFT, PARTIALLY_RIGHT, ENCLOSED, OVERLAPPING, DONTKNOW, SAME};


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

		List<Line> sortedLines = sortOnRotationalSweep();
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
		INTERVAL_LOCATION il = checkInterval(l, node.minInterval, node.maxInterval);
		switch (il)
		{
			case INTERVAL_LOCATION.SAME:
				{
					if (l.start == node.line.start && l.end == node.line.end)
						break;//do nothing it is the same line
					else
						break; //we need to check
				} break;
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



				}	break;

			default: print("Can't insert"); break;
		}

	}

	TreeNode balanceTree(TreeNode root)
	{


		return root;
	}

	static INTERVAL_LOCATION checkInterval(Line l, float minInterval, float maxInterval)
	{
		if (l.minInterval == minInterval && l.maxInterval == maxInterval)
			return INTERVAL_LOCATION.SAME;

		if(l.minInterval < minInterval)
		{
			if (l.maxInterval < minInterval)
				return INTERVAL_LOCATION.LEFT;
			else if (l.maxInterval < maxInterval)
				return INTERVAL_LOCATION.PARTIALLY_LEFT;
		}
		if(l.minInterval >= minInterval && l.minInterval <= maxInterval)
		{
			if (l.maxInterval <= maxInterval)
				return INTERVAL_LOCATION.ENCLOSED;
			else if (l.maxInterval > maxInterval)
				return INTERVAL_LOCATION.PARTIALLY_RIGHT;
		}
		if(l.minInterval > maxInterval && l.maxInterval > maxInterval)
		{
			return INTERVAL_LOCATION.RIGHT;
		}
		if(l.minInterval < minInterval && l.maxInterval > maxInterval)
		{
			return INTERVAL_LOCATION.OVERLAPPING;
		}

		return INTERVAL_LOCATION.DONTKNOW;
	}


	List<Line> sortOnRotationalSweep()
	{
		//translate lines so the guard is at 0,0
		List<Line> tempLines = new List<Line>(lines);
		foreach(Line l in tempLines)
		{
			l.translate(guardPos*-1);
		}
		
		//calculate angle for each line
		foreach(Line l in tempLines)
		{
			l.startAngle = Vector2.SignedAngle(Vector2.up, l.start);
			l.endAngle = Vector2.SignedAngle(Vector2.up, l.end);
			if (l.startAngle < 0)
				l.startAngle += 360;
			if (l.endAngle < 0)
				l.endAngle += 360;

			//may be buggy because the interval can lie left or right of the guard. (definitely buggy)
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
			if(l.maxInterval > l.minInterval + 180)
			{
				float temp = l.minInterval;
				l.minInterval = l.maxInterval;
				l.maxInterval = temp;
			}
		}

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
		this.lines = lines;
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


		setupLevel(playerPos, guardPos, guardOrientation, coneWidth, coneLength, lines);

		checkPlayerVisibility(playerPos, guardPos, guardOrientation);

		//foreach(Line l in sortOnRotationalSweep())
		//{
		//	print(l.minInterval);
		//	print(l.maxInterval);
		//}
	}

}
