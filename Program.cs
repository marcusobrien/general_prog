using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Configuration;

namespace SortingBasics
{
	public static class AppSettings
	{
		public static string ReadSetting(string key)
		{
			string result = "Not Found";
			try
			{
				var appSettings = ConfigurationManager.AppSettings;
				result = appSettings[key];
			}
			catch (ConfigurationErrorsException)
			{
				Console.WriteLine("Error reading app settings");
			}

			return result;
		}
	}

	interface ISortable<T> where T : IComparable
	{
		void SortWithRecursion(IList<T> items,int offset);
		void SortWithoutRecursionv1(IList<T> items);
		void SortWithoutRecursionv2(IList<T> items);
	}

	public sealed class MOBsSuperSort<T> : ISortable<T>  where T : IComparable
	{
		private bool Print { get; }
		private bool PrintToFile = true;
		public static string Filename = "";

		private void ReadSettings()
		{
			PrintToFile = (AppSettings.ReadSetting("PrintToFile") == "true");
			Filename = AppSettings.ReadSetting("Filename");
		}

		public static void DisplayResults()
		{
			Process notePad = new Process();
			notePad.StartInfo.FileName = @"c:\windows\vim.bat";
			notePad.StartInfo.Arguments = MOBsSuperSort<int>.Filename;
			notePad.Start();
		}

		public static void DeleteOldTextFile()
		{
			if (File.Exists(Filename))
				File.Delete(Filename);
		}

		public MOBsSuperSort(bool print)
		{
			Print = print;
			ReadSettings();
		}

		public void PrintItems(string Message, IList<T> SortedItems)
		{
			System.Console.WriteLine(Message);
			System.Console.WriteLine("The list of items.");
			for (int index = 0; index < SortedItems.Count; index++)
				System.Console.Write("[{0}] - {1} ", index, SortedItems[index]);
			System.Console.WriteLine("End of the list of items.");
		}


		public void SortWithRecursion(IList<T> unsortedItems, int offset)
		{
			if (offset == unsortedItems.Count)
				return;
		}

		private void UpdateProgress(int count)
		{
			if (count % 10000 == 0)
				System.Console.WriteLine("");
			if (count % 100 == 0)
				System.Console.Write("o");
		}

		public void SortWithoutRecursionv3(List<T> unsortedItems)
		{
			int count = unsortedItems.Count;

			if (Print)
			{
				PrintItems("v3 Pre Sorted items", unsortedItems);
				PrintListToFile(String.Format("v3 Pre Sorted items : {0}", count), unsortedItems);
			}

			var watch = System.Diagnostics.Stopwatch.StartNew();
			unsortedItems.Sort();
			watch.Stop();

			var elapsedMs = watch.ElapsedMilliseconds;

			PrintTime("v3 ", elapsedMs);
			WriteToFile(String.Format("v3 Post Sorted items : {0} Took : {1} seconds {2} minutes ", count, elapsedMs * 0.001, (elapsedMs * 0.001) / 60));

			if (Print)
			{
				PrintItems("V3 Post Sorted items", unsortedItems);
				PrintListToFile("V3 Post Sorted List", unsortedItems);
			}

			System.Console.WriteLine("v3 Post Sorted items");
		}

		public void SortWithoutRecursionv1(IList<T> unsortedItems)
		{
			int NUM_ITERATIONS = 0;
			int NUM_COMPARES = 0;

			int count = unsortedItems.Count;

			if (Print)
			{
				PrintItems("v1 Pre Sorted items", unsortedItems);
				PrintListToFile(String.Format("v1 Pre Sorted items : {0}", count), unsortedItems);
			}

			var watch = System.Diagnostics.Stopwatch.StartNew();

			//Can not use another list, need to use the same list so it gets smaller !
			if (1 > count)
				return;

			int startPos = 0;

			//Goes through the whole list from the start position n-1 times
			//If the current item is less than the start pos,swap it
			//the other sorter also checks the item at the end of the list aswell
			do
			{
				UpdateProgress(startPos);
				for (int index = startPos + 1; index < count; index++)
				{
					if (unsortedItems[index].CompareTo(unsortedItems[startPos]) < 0)
					{
						T temp = unsortedItems[index];
						unsortedItems[index] = unsortedItems[startPos];
						unsortedItems[startPos] = temp;
					}
					++NUM_COMPARES;
				}
				++startPos;
				++NUM_ITERATIONS;
			} while (startPos < count - 1);

			watch.Stop();
			var elapsedMs = watch.ElapsedMilliseconds;

			PrintTime("v1 ", elapsedMs);
			WriteToFile(String.Format("v1 Post Sorted items : {0} Took : {1} seconds {2} minutes - Did {3} Iterations Did {4} Compares ", count, elapsedMs * 0.001, (elapsedMs * 0.001) / 60, NUM_ITERATIONS, NUM_COMPARES));

			if (Print)
			{
				PrintItems("Post Sorted items", unsortedItems);
				PrintListToFile("V1 Post Sorted List", unsortedItems);
			}

			System.Console.WriteLine(String.Format("v1 Post Sorted items Did {0} iterations in search and {1} Comparisons", NUM_ITERATIONS, NUM_COMPARES));
		}

		private void PrintTime(string mes, long elapsedMs)
		{
			System.Console.WriteLine(mes + " Time took seconds " + (elapsedMs * 0.001));
			System.Console.WriteLine(mes + " Time took minutes " + ((elapsedMs * 0.001) / 60));
		}

		
		public void WriteSessionIdToFile(Guid id, string message)
		{
			using (System.IO.StreamWriter file = new System.IO.StreamWriter(Filename, true))
			{
				file.WriteLine("\n\n\nThis session id is : " + id.ToString());
				file.WriteLine("This is test type " + message);
			}
		}

		public static void WriteToFile(string message)
		{
			using (System.IO.StreamWriter file = new System.IO.StreamWriter(Filename, true))
			{
				file.WriteLine(message);
			}
			System.Console.WriteLine(message);
		}


		private void PrintListToFile(string message, IList<T> unsortedItems)
		{
			if (PrintToFile == false)
				return;

			using (System.IO.StreamWriter file = new System.IO.StreamWriter(Filename, true))
			{
				int count = 0;
				file.WriteLine(message);
				foreach (var line in unsortedItems)
				{
					file.Write(String.Format(" {0} ", line));
					count++;
				}
				file.WriteLine("  \n");
			}
		}

		public void SortWithoutRecursionv2(IList<T> unsortedItems)
		{
			int count = unsortedItems.Count;

			System.Console.WriteLine("v2 Pre Sorted items : " + count);

			if (Print)
			{
				PrintListToFile("v2 Pre Sorted items : " + count, unsortedItems);
				PrintItems("V2 Pre Sorted items", unsortedItems);
			}

			var watch = System.Diagnostics.Stopwatch.StartNew();
			// the code that you want to measure comes here
			
			//This one uses a backward pointer as well so should be twice as quick
			//Can not use another list, need to use the same list so it gets smaller !

			if (1 > count)
				return;

			//We are choosing pairs, the biggest and smallest. We put then at th left startpos
			//and at the right endpos

			//So we are going to start with the left startPos, and the right endPos
			//We go through the list and if the current item is < item at startPos swap it
			//otherwise if its greater than the item at end pos swap it, at the ed of the run
			//start pos and end pos must move up, as the startpos will have the smallest in the list
			//endpos will have the biggest
			int startPos = 0;
			int endPos = count-1;

			int NUM_ITERATIONS = 0;
			int NUM_COMPARES = 0;

			do
			{
				//Must make sure the number in the index on the left is smaller than the index on the right
				//ie 5,3,5,2,2 index is 0, so left element is 5 and right element is 2, looking at index+1 so value 3
				//so 3 is smaller than 1 but also bigger than 2 - so it will go both places ! Make sure before we start 
				//that the smallest in index left and index right is on the left, and the other one is in index right
				//so before we start we swap 5 and 2, now we have 2,3,5,2,5, now the number we are looking at is
				//still 3, so it goes in neither as it doesnt move

				if (unsortedItems[startPos].CompareTo(unsortedItems[endPos]) > 0)
				{
					T temp = unsortedItems[startPos];
					unsortedItems[startPos] = unsortedItems[endPos];
					unsortedItems[endPos] = temp;
				}

				if (endPos - startPos > 1)
				{
					//we are NOT the last pair, so need to iterate
					UpdateProgress(startPos);

					for (int index = startPos + 1; index < endPos; index++)
					{
						if (unsortedItems[index].CompareTo(unsortedItems[startPos]) < 0)
						{
							T temp = unsortedItems[index];
							unsortedItems[index] = unsortedItems[startPos];
							unsortedItems[startPos] = temp;
							--NUM_COMPARES;
						}
						else if (unsortedItems[index].CompareTo(unsortedItems[endPos]) > 0)
						{
							//Careful if the number is bigger than the one on the right, will the one at the right
							//we swap it with be smaller than the one on the left ie 3,5,6,7,1, so
							//compare 5, its not smaller than 3 so dont swap, its bigger than 1 so swap with 1
							//now the 1 should have been swapped with the 3 ! but we have already swapped 3 and 1
							//so we are ok !
							T temp = unsortedItems[index];
							unsortedItems[index] = unsortedItems[endPos];
							unsortedItems[endPos] = temp;
						}
						NUM_COMPARES += 2;
					}
				}

				startPos++;
				endPos--;
				//You should only have to sort half the list, as each number in the first half will contain the 
				//sorted
				NUM_ITERATIONS++;
			} while (startPos < endPos);


			watch.Stop();
			var elapsedMs = watch.ElapsedMilliseconds;

			PrintTime("v2 ", elapsedMs);
			WriteToFile(String.Format("v2 Post Sorted items : {0} Took : {1} seconds {2} minutes We did {3} ITERATIONS and {4} Compares in V2", count, elapsedMs * 0.001, (elapsedMs * 0.001) / 60, NUM_ITERATIONS, NUM_COMPARES));

			if (Print)
			{
				PrintListToFile("V2 Post sorted items", unsortedItems);
				PrintItems(String.Format("V2 Post sorted items did {0} iterations and {1} compares ", NUM_ITERATIONS, NUM_COMPARES), unsortedItems);
			}
		}
	}

	class Program
	{
		private static int NUMS_NUM_NUMS = 10;
		private static bool PrintList = false;
		private static int NUMBER_ITERATIONS = 3;

		private static void ReadSettings()
		{
			NUMS_NUM_NUMS = Convert.ToInt32(AppSettings.ReadSetting("NUMS_NUM_NUMS"));
			PrintList = (AppSettings.ReadSetting("PrintList") == "true");
			NUMBER_ITERATIONS = Convert.ToInt32(AppSettings.ReadSetting("NUMBER_ITERATIONS"));
		}

		private static void FillRandomInts(List<int> MyItemsToSort)
		{
			Random r = new Random();
			for (int index = 0; index < NUMS_NUM_NUMS; index++)
				MyItemsToSort.Add(r.Next(0, 1000));
		}

		private static void FillInts(List<int> MyItemsToSort, bool constant, bool ascending)
		{
			if (constant)
			{
				for (int index = 0; index < NUMS_NUM_NUMS; index++)
				{
					MyItemsToSort.Add(42);
				}
			}
			else if (ascending)
			{
				for (int index = 0; index < NUMS_NUM_NUMS; index++)
				{
					MyItemsToSort.Add(index);
				}
			}
			else
			{
				for (int index = NUMS_NUM_NUMS; index > 0; index--)
				{
					MyItemsToSort.Add(index);
				}
			}
		}

		private static void TestAlgorithmFill(ref List<int> MyItemsToSort)
		{
			MyItemsToSort.Clear();
			MyItemsToSort = new List<int>() { 6, 7, 8, 9, 5 };
			return;
		}

		private static void Fill(List<int> MyItemsToSort, int testT)
		{
			MyItemsToSort.Clear();
			if ((TestTypes)testT == TestTypes.Random)
				FillRandomInts(MyItemsToSort);
			else
			{
				bool Ascending = true;
				bool constantNums = false;

				TestTypes typeOfTest = (TestTypes)testT;

				switch (typeOfTest)
				{
					case TestTypes.Ordered:
						//Nums go up from 1
						break;
					case TestTypes.Reversed:
						//nums go down to 1
						Ascending = false;
						break;
					case TestTypes.Constant:
						//All nums the same
						constantNums = true;
						break;
				}
				FillInts(MyItemsToSort, constantNums, Ascending);
			}
		}

		class g
		{
			int gg;

			public g(int hgh)
			{
				gg = hgh;
			}

			public void print()
			{
				System.Console.Write("The g class:" + gg);
			}
			public g()
			{
				System.Console.Write("Hello");
			}
		}

		static private void arrayStuff()
		{
			g ggg = new g(444444);

			//Shows that constructors for items in the array, dont get called when the array is created,
			//but for value types you can directly set a value after the array is created
			int[] myints2 = new int[10];
			myints2[5] = 99;

			//For reference types the const is not called either, but you CAN 
			//use the [] indexor to set a value as the memory has been set up
			g[] myaf = new g[1000];
			myaf[10] = ggg;

			myaf[10].print();

			//But the constructor has not been called on items in the array so the references still refer to null
			//so calling anything on them will null exception
			myaf[11].print();

			const int mysize = 50;
			
			int[] myints = new int[mysize];
			myints[20] = 99;

			List<g> mylagaa = new List<g>(mysize) { new g(3), new g(88) };

			List<g> mylag = new List<g>(mysize);
			//mylag[44] = ggg;

			List<int> myla = new List<int>(mysize);
			myla[44] = 99;

			List<int> myl = new List<int>();
			myl[44] = 99;
		}

		static void Main(string[] args)
		{
			ReadSettings();
			//JustTestThisSoftware();
			
			MOBsSuperSort<int>.DeleteOldTextFile();

			for (int count = 0; count < NUMBER_ITERATIONS; count++)
			{
				System.Console.WriteLine(String.Format("About to test. Iteration : {0}" , (count+1)));
				DoIt();
			}

			MOBsSuperSort<int>.DisplayResults();

			System.Console.WriteLine("Press any key to quit");
			System.Console.ReadKey();
		}

		private enum TestTypes
		{
			Random,
			Ordered,
			Reversed,
			Constant
		}

		static void TestList(List<int> MyItemsToSort, String searchType)
		{
			//Now test to see if it worked
			List<int> PerfectList = new List<int>(MyItemsToSort);
			PerfectList.Sort();

			IEnumerable<Tuple<int, int>> bothList = PerfectList.Zip(MyItemsToSort, Tuple.Create<int, int>);

			foreach (var f in bothList)
			{
				if (f.Item1 != f.Item2)
				{
					MOBsSuperSort<int>.WriteToFile(String.Format("The list {2} was not sorted properly ! Found the value {0} and it should have been a {1}", f.Item1, f.Item2, searchType));
					return;
				}
			}

			MOBsSuperSort<int>.WriteToFile("The " + searchType + " list was sorted properly ! ");
		}

		static TestTypes  SetTestType()
		{
			return TestTypes.Reversed;
		}

		static void JustTestThisSoftware()
		{
			List<int> MyItemsToSort = new List<int>(NUMS_NUM_NUMS);
			TestAlgorithmFill(ref MyItemsToSort);

			MOBsSuperSort<int> sorting = new MOBsSuperSort<int>(false);
			sorting.SortWithoutRecursionv2(MyItemsToSort);
			TestList(MyItemsToSort, "Search V1");
		}

		static void DoIt()
		{
			Guid id = Guid.NewGuid();

			List<int> MyItemsToSort = new List<int>(NUMS_NUM_NUMS);

			foreach (var suit in Enum.GetValues(typeof(TestTypes)))
			{
				//var suit = SetTestType();
				System.Console.WriteLine(String.Format("Testing with sort type : {0}", suit));
				MOBsSuperSort<int> sorting = new MOBsSuperSort<int>(PrintList);
				sorting.WriteSessionIdToFile(id, suit.ToString());

				//v1 sort test
				Fill(MyItemsToSort, (int)suit);				
				sorting.SortWithoutRecursionv1(MyItemsToSort);
				TestList(MyItemsToSort, "Search V1");

				//Now test v2
				Fill(MyItemsToSort, (int)suit);
				sorting.SortWithoutRecursionv2(MyItemsToSort);
				TestList(MyItemsToSort, "Search V2");

				//Now test the built in one
				Fill(MyItemsToSort, (int)suit);
				sorting.SortWithoutRecursionv3(MyItemsToSort);
				TestList(MyItemsToSort, "Search V3");
			}
		}
	}
}
