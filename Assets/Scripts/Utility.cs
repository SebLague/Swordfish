using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility {

    static System.Random prng;

    static Utility()
    {
        prng = new System.Random();
    }

    public static void Shuffle<T>(T[] array)
	{
		for (int i = array.Length; i > 1; i--)
		{
			// Pick random element to swap.
            int j = prng.Next(i); // 0 <= j <= i-1
									// Swap.
			T tmp = array[j];
			array[j] = array[i - 1];
			array[i - 1] = tmp;
		}
	}
}
