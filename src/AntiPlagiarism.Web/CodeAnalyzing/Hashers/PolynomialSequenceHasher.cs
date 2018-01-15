using System;
using System.Collections.Generic;

namespace AntiPlagiarism.Web.CodeAnalyzing.Hashers
{
	public class PolynomialSequenceHasher : ISequenceHasher
	{
		private readonly int polynomBase;
		private int currentHash;
		private readonly Queue<int> objectsHashes = new Queue<int>();
		private readonly Dictionary<int, int> valueToSubstractCache;
		private int[] basePowers;

		public PolynomialSequenceHasher(int polynomBase, int defaultCapacity=10000)
		{
			this.polynomBase = polynomBase;
			valueToSubstractCache = new Dictionary<int, int>();
			InitBasePowers(defaultCapacity);
			Reset();
		}

		public void Enqueue(object obj)
		{
			var objectHashCode = obj.GetHashCode();
			currentHash = unchecked(currentHash * polynomBase + objectHashCode);	
			objectsHashes.Enqueue(objectHashCode);
		}

		public void Dequeue()
		{
			if (!valueToSubstractCache.TryGetValue(objectsHashes.Count, out var valueToSubstract))
			{
				valueToSubstract = GetBasePower(objectsHashes.Count - 1);
				valueToSubstractCache[objectsHashes.Count] = valueToSubstract;
			}

			currentHash = unchecked(currentHash - valueToSubstract * objectsHashes.Dequeue());
		}

		public int GetCurrentHash()
		{
			return currentHash;
		}

		public void Reset()
		{
			currentHash = 0;
			objectsHashes.Clear();
		}
		
		private void InitBasePowers(int capacity)
		{
			basePowers = new int[capacity];
			basePowers[0] = 1;
			for (var i = 1; i < capacity; i++)
				basePowers[i] = unchecked(basePowers[i - 1] * polynomBase);
		}
		
		private int GetBasePower(int power)
		{
			var result = 1;
			while (power >= basePowers.Length)
			{
				result = unchecked(result * basePowers[basePowers.Length - 1]);
				power -= basePowers.Length - 1;
			}
			return unchecked(result * basePowers[power]);
		}

	}
}