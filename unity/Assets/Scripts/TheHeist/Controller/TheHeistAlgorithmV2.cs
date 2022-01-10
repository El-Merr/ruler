using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheHeistAlgorithmV2 : MonoBehaviour {

	// position of the player,guard and objects.
	[SerializeField]
	Vector2 playerPos { get; set; }
	[SerializeField]
	Vector2 guardPos { get; set; }
	[SerializeField]
	Vector2 guardOrientation { get; set; }
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
		}
		public void translate(Vector2 translation)
		{
			start += translation;
			end += translation;
		}
		public Vector2 start, end;
		public float startInterval = 0f;
		public float endInterval = 0f;
	}

	List<Line> sortOnRotationalSweep()
    {
		//Important to have the lvl and update the guard pos.

		List<Line> tempLines = new List<Line>(lines);
		List<Line> linesToBeAdded = new List<Line>();
		foreach (Line l in tempLines)
		{
			l.translate(guardPos * -1);
		}

		//calculate angle for each line and make sure they are between 0 and 360
		foreach (Line l in tempLines)
		{
			float startAngle = Vector2.SignedAngle(Vector2.up, l.start);
			float endAngle = Vector2.SignedAngle(Vector2.up, l.end);

			if (startAngle < 0)
				startAngle += 360;
			if (endAngle < 0)
				endAngle += 360;

			//Set the interval for the line
			if (startAngle < endAngle)
			{
				l.minInterval = startAngle;
				l.maxInterval = endAngle;
			}
			else
			{
				l.minInterval = endAngle;
				l.maxInterval = startAngle;
			}

			//flip the interval if nessesary
			if (l.maxInterval > l.minInterval + 180)
			{
				float tempMin = l.minInterval;
				l.minInterval = l.maxInterval;
				l.maxInterval = tempMin;
			}

			//create two lines corresponding to a line when the interval contains 0
			if (l.minInterval > l.maxInterval)
			{
				//split the line in two lines
				Line newLine = new Line(l.start, l.end);
				newLine.minInterval = 0;
				newLine.maxInterval = l.maxInterval;
				linesToBeAdded.Add(newLine);
				//update original 
				l.maxInterval = 360;

			}
		}

		//add the aditional intervals of the lines
		tempLines.AddRange(linesToBeAdded);
		//sort on smallest start of interval
		tempLines.Sort((x, y) => x.minInterval.CompareTo(y.minInterval));

	}

	// Use this for initialization
	void Start () {
		CurrentLevel.Add(new Line(new Vector2(1f, 1f), new Vector2(3f, 1f)));
		CurrentLevel.Add(new Line(new Vector2(4f, 1f), new Vector2(5f, 1f)));
		CurrentLevel.Add(new Line(new Vector2(-1f, 1f), new Vector2(-3f, 1f)));
		CurrentLevel.Add(new Line(new Vector2(-4f, 1f), new Vector2(-6f, 1f)));
		CurrentLevel.Add(new Line(new Vector2(-1f, 1f), new Vector2(1f, 1f)));


	}
	

}
