using System.Collections;
using System.Collections.Generic;

public static class WeightedRandomChooser
{
	public static T ChooseRandom<T>(Dictionary<T, int> weights, System.Random rand)
	// Choose one object at random from a collection of objects.
	{
		List<T> keys = new List<T>(weights.Keys);
		List<Interval> intervals = new List<Interval>();

		intervals.Add(new Interval(0, weights[keys[0]]));

		for(int i = 1; i < keys.Count; i++)
		{
			int s = intervals[i-1].End;
			int e = s + weights[keys[i]];
			intervals.Add(new Interval(s,e));
		}

		int total = intervals[intervals.Count - 1].End;

		int r = rand.Next (0, total);

		int index = 0;
		for(int i = 0; i < intervals.Count; i++)
		{
			if(intervals[i].Contains(r))
			{
				index = i;
				break;
			}
		}

		return keys[index];

	}

	public static List<T> ChooseNRandom<T>(Dictionary<T, int> weights, System.Random rand, int num)
	{
		List<T> objectsToReturn = new List<T>();

		for(int i = 0; i < num; i++)
		{
			objectsToReturn.Add (ChooseRandom(weights, rand));
		}

		return objectsToReturn;
	}

	public static List<T> ChooseNDistinctRandom<T>(Dictionary<T, int> weights, System.Random rand, int num)
	{
		if(num > weights.Keys.Count)
		{
			throw new System.ArgumentException("There are not (num) distinct random objects in this collection.");
		}

		List<T> toReturn = new List<T>();
		T choice = ChooseRandom(weights, rand);
		toReturn.Add(choice);

		if(num == 1)
		{
			return toReturn;
		}
		else
		{
			Dictionary<T, int> newWeights = new Dictionary<T, int>(weights);
			newWeights.Remove(choice);
			toReturn.AddRange(ChooseNDistinctRandom(newWeights, rand, num - 1));
			return toReturn;
		}

	}

}
