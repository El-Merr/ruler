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
		public float startInterval = 0f;
		public float endInterval = 0f;
	}
	class TreeNode
	{
		public TreeNode(Line line)
		{
			this.line = line;
		}
		public bool isRoot = false;
		public bool isLeaf = false;
		public bool isIntersection = false;
		public TreeNode leftChild;
		public TreeNode rightChild;
		public Line line;
		public float minInterval, maxInterval = -1;
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

	TreeNode generateIntervalTree(List<Line> sortedLines)
	{
		TreeNode root = new TreeNode(sortedLines[0]);
		root.isRoot = true;
		root.minInterval = root.line.startInterval;
		root.maxInterval = root.line.endInterval;
		foreach(Line l in sortedLines)
		{
			//TODO EVENTS HANDLEN
		}


		return root;
	}

	List<Line> sortOnRotationalSweep()
	{
		//translate lines so the guard is at 0,0
		List<Line> sortedLines = new List<Line>(lines);
		foreach(Line l in sortedLines)
		{
			l.translate(guardPos*-1);
		}
		
		//calculate angle for each line
		foreach(Line l in sortedLines)
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
				l.startInterval = l.startAngle;
				l.endInterval = l.endAngle;
			}
			else
			{
				l.startInterval = l.endAngle;
				l.endInterval = l.startAngle;
			}
				
		}

		//sort on smallest angle
		sortedLines.Sort(delegate (Line a, Line b)
		{
			if (a.startInterval <= b.startInterval)
				return 1;
			else
				return 0;
		});

		return sortedLines;
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

		lines.Add(new Line(new Vector2(0f, 1f), new Vector2(1f, 0f)));
		lines.Add(new Line(new Vector2(1f, 0f), new Vector2(0f, -1f)));
		lines.Add(new Line(new Vector2(0f, -1f), new Vector2(-1f, 0f)));
		lines.Add(new Line(new Vector2(-1f, 0f), new Vector2(0f, 1f)));



		setupLevel(playerPos, guardPos, guardOrientation, coneWidth, coneLength, lines);
		foreach(Line l in sortOnRotationalSweep())
		{
			print(l.startInterval);
			print(l.endInterval);
		}
	}

}
