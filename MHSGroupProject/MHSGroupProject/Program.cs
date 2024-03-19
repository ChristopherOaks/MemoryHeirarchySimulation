namespace MHSGroupProject
{
    class MemoryHierarchySimulator
 {
     static Dictionary<int, int> pageTable = new Dictionary<int, int>();
     static LinkedList<int> lruQueue = new LinkedList<int>();
     static Queue<int> fifoQueue = new Queue<int>();

     static int totalHits = 0;
     static int totalMisses = 0;
     static int readAccesses = 0;
     static int writeAccesses = 0;

     static void Main()
     {
         Console.WriteLine("Memory Hierarchy Simulator");

         // Read configuration or set default values

         Console.WriteLine("Choose algorithm to simulate:");
         Console.WriteLine("1. Optimal Greedy Algorithm");
         Console.WriteLine("2. Optimal FIFO Algorithm");
         Console.WriteLine("3. Optimal LRU Algorithm");

         int choice = int.Parse(Console.ReadLine());


         switch (choice)
         {
             case 1:
                 OptimalGreedyAlgorithm();
                 DisplaySummaryStatistics();
                 break;
             case 2:
                 //OptimalFIFOAlgorithm();
                 DisplaySummaryStatistics();
                 break;
             case 3:
                 //OptimalLRUAlgorithm(); 
                 DisplaySummaryStatistics();
                 break;
             default:
                 Console.WriteLine("Invalid Number Entered...");
                 break;
         }


     }


     static void ProcessMemoryTrace(string line, Func<int, bool> algorithm)
     {
         string[] parts = line.Split(':');
         char accessType = parts[0][0];
         int virtualAddress = Convert.ToInt32(parts[1], 16);

         int virtualPageNumber = virtualAddress >> 12;
         int pageOffset = virtualAddress & 0xFFF;

         bool isHit = algorithm(virtualPageNumber);

         Console.WriteLine($"{virtualAddress:X8} {virtualPageNumber:X6} {pageOffset:X4} {pageTable[virtualPageNumber]:X4}");

         UpdateStatistics(isHit, accessType);
     }

     static void OptimalGreedyAlgorithm()
     {
         Console.WriteLine("Simulating Optimal Greedy Algorithm");

         // Read memory traces from a file
         string[] lines = File.ReadAllLines(@"C:\Users\poiso\Downloads\trunc_12.dat");

         foreach (string line in lines)
         {
             ProcessMemoryTrace(line, OptimalGreedyAlgorithm);
         }
     }

     static bool OptimalGreedyAlgorithm(int virtualPageNumber)
     {
         if (pageTable.ContainsKey(virtualPageNumber))
         {
             // Existing page, it's a hit
             return true;
         }
         else
         {
             // Page fault - update page table
             pageTable[virtualPageNumber] = pageTable.Count;
             return false;
         }
     }

     static bool OptimalFIFOAlgorithm(int virtualPageNumber)
     {
         bool isHit = pageTable.ContainsKey(virtualPageNumber);

         if (!isHit)
         {
             // Page fault - update page table and FIFO queue
             pageTable[virtualPageNumber] = pageTable.Count;
             fifoQueue.Enqueue(virtualPageNumber);
         }

         return isHit;
     }

     static bool OptimalLRUAlgorithm(int virtualPageNumber)
     {
         bool isHit = lruQueue.Contains(virtualPageNumber);

         if (!isHit)
         {
             // Page fault - update page table and LRU queue
             pageTable[virtualPageNumber] = pageTable.Count;
             lruQueue.RemoveLast(); // Remove the least recently used page
             lruQueue.AddFirst(virtualPageNumber);
         }
         else
         {
             // Update the order in the LRU queue for hits
             lruQueue.Remove(virtualPageNumber);
             lruQueue.AddFirst(virtualPageNumber);
         }

         return isHit;
     }

     static void UpdateStatistics(bool isHit, char accessType)
     {
         if (isHit)
             totalHits++;
         else
             totalMisses++;

         if (accessType == 'R')
             readAccesses++;
         else if (accessType == 'W')
             writeAccesses++;
     }

     static void DisplaySummaryStatistics()
     {
         Console.WriteLine("\nSummary Statistics:");
         Console.WriteLine($"Total Hits: {totalHits}");
         Console.WriteLine($"Total Misses: {totalMisses}");
         Console.WriteLine($"Hit Ratio: {totalHits / (double)totalMisses}");
         Console.WriteLine($"Number of Read Accesses: {readAccesses}");
         Console.WriteLine($"Number of Write Accesses: {writeAccesses}");
         Console.WriteLine($"Read/Write Ratio: {readAccesses / (double)writeAccesses}");
         Console.WriteLine($"Total Number of Memory References: {readAccesses + writeAccesses}");
     }
}
