//using System;
//using System.Runtime.InteropServices;
//using System.Text;

//public unsafe class FastReplacer : IDisposable
//{
//	private readonly char* _oldValue;
//	private readonly int _oldValueLength;

//	private readonly bool* _fastTable;
//	private readonly char* _input;

//	public int FoundIndexes;

//	public FastReplacer(StringBuilder input, string oldValue)
//	{
//		var inputLength = input.Length;
//		_oldValueLength = oldValue.Length;

//		_oldValue = (char*)Marshal.AllocHGlobal((_oldValueLength + 1) * sizeof(char));
//		_input = (char*)Marshal.AllocHGlobal((inputLength + 1) * sizeof(char));
//		_fastTable = (bool*)Marshal.AllocHGlobal((inputLength) * sizeof(bool));

//		// There is no way to convert StringBuilder do char*, must get first string
//		fixed (char* inputSrc = input.ToString(), oldValueSrc = oldValue)
//		{
//			Copy(inputSrc, _input);
//			Copy(oldValueSrc, _oldValue);
//		}

//		BuildMatchedIndexes();
//	}

//	public string result = null;

//	public void Replace(char* outputPtr, int outputLength, string newValue)
//	{
//		var newValueLength = newValue.Length;

//		char* inputPtr = _input;
//		bool* fastTablePtr = _fastTable;

//		fixed (char* newValuePtr = newValue)
//		{
//			while (*inputPtr != 0)
//			{
//				if (*fastTablePtr)
//				{
//					Copy(newValuePtr, outputPtr);
//					outputLength -= newValueLength;
//					outputPtr += newValueLength;

//					inputPtr += _oldValueLength;
//					fastTablePtr += _oldValueLength;
//					continue;
//				}

//				*outputPtr++ = *inputPtr++;
//				fastTablePtr++;
//			}
//		}

//		*outputPtr = '\0';
		
//		// here it can't be, result string could be always empty
//		//result = new string(outputPtr);
//		//if (result != string.Empty)
//		//{

//		//}
//	}

//	public void Dispose()
//	{
//		if (_fastTable != null) Marshal.FreeHGlobal(new IntPtr(_fastTable));
//		if (_input != null) Marshal.FreeHGlobal(new IntPtr(_input));
//		if (_oldValue != null) Marshal.FreeHGlobal(new IntPtr(_oldValue));
//	}

//	private void Copy(char* sourcePtr,
//					  char* targetPtr)
//	{
//		while ((*targetPtr++ = *sourcePtr++) != 0) ;
//	}

//	/// <summary>
//	/// KMP?!
//	/// </summary>
//	private void BuildMatchedIndexes()
//	{
//		var sourcePtr = _input;
//		var fastTablePtr = _fastTable;

//		var i = 0;

//		while (sourcePtr[i] != 0)
//		{
//			fastTablePtr[i] = false;

//			if (sourcePtr[i] == _oldValue[0])
//			{
//				var tempSourcePtr = &sourcePtr[i];
//				var tempOldValuePtr = _oldValue;
//				var isMatch = true;

//				while (isMatch &&
//					   *tempSourcePtr != 0 &&
//					   *tempOldValuePtr != 0)
//				{
//					if (*tempSourcePtr != *tempOldValuePtr)
//						isMatch = false;

//					tempSourcePtr++;
//					tempOldValuePtr++;
//				}

//				if (isMatch)
//				{
//					fastTablePtr[i] = true;
//					i += _oldValueLength;

//					FoundIndexes++;
//					continue;
//				}
//			}

//			i++;
//		}
//	}
//}