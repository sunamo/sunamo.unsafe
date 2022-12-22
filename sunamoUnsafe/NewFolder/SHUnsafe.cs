//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading;

//public class SHUnsafe
//{
//	/// <summary>
//	/// For best effect is needed compila whole app as x64
//	/// Not only *.unsafe due to "bad image format" exception during compile
//	/// </summary>
//	/// <param name="input"></param>
//	/// <param name="oldValue"></param>
//	/// <param name="newValues"></param>
//	/// <returns></returns>
//	public static unsafe StringBuilder ReplaceUnsafeUnmanaged(StringBuilder input, IList< string> oldValue, IList< string> newValues)
//	{
//		for (int i = 0; i < oldValue.Count; i++)
//		{
//			input = ReplaceUnsafeUnmanaged(input, oldValue[i], newValues[i]);
//		}

//		return input;
//	}

//	/// <summary>
//	/// For best effect is needed compila whole app as x64
//	/// Not only *.unsafe due to "bad image format" exception during compile
//	/// </summary>
//	/// <param name="input"></param>
//	/// <param name="oldValue"></param>
//	/// <param name="newValues"></param>
//	/// <returns></returns>
//	public static unsafe StringBuilder ReplaceUnsafeUnmanaged(StringBuilder input, string oldValue, params string[] newValues)
//	{
//		using (var fastReplacer = new FastReplacer(input, oldValue))
//		{
//			var replaceLength = oldValue.Length;
//			var inputLength = input.Length;

//			var replacedDataLength = fastReplacer.FoundIndexes * replaceLength;

//			var maximumParallelWork = newValues.Length > Environment.ProcessorCount
//				? Environment.ProcessorCount
//				: newValues.Length;

//			var partitions = Partitioner.Create(0, newValues.Length).GetPartitions(maximumParallelWork);
//			var threads = new Thread[partitions.Count];

//			var biggestIndex = 0;
//			for (var k = 1; k < newValues.Length; k++)
//			{
//				if (newValues[biggestIndex].Length < newValues[k].Length)
//					biggestIndex = k;
//			}

//			var biggestStackSize = inputLength - replacedDataLength + fastReplacer.FoundIndexes * newValues[biggestIndex].Length;

//			for (var i = 0; i < partitions.Count; i++)
//			{
//				var partition = partitions[i];
//				partition.MoveNext();

//				threads[i] = new Thread(() =>
//				{
//					var range = partition.Current;

//					var maxLength = newValues[range.Item1].Length;
//					for (var j = range.Item1 + 1; j < range.Item2; j++)
//						maxLength = Math.Max(newValues[j].Length, maxLength);

//					var outputLength = inputLength - replacedDataLength + fastReplacer.FoundIndexes * maxLength;
//					char* outputPtr = stackalloc char[(outputLength + 1)];

//					for (var d = range.Item1; d < range.Item2; d++)
//					{
//						fastReplacer.Replace(outputPtr, outputLength, newValues[d]);
//						input.Clear();
//						input.Append(new string(outputPtr));
//						//Console.WriteLine(new string(outputPtr));
//					}
//				}, biggestStackSize + 1000 * 1000 * 1000);

//				threads[i].Start();
//			}

//			for (var k = 0; k < threads.Length; k++)
//				threads[k].Join();

//			return input;
//		}
//	}
//}