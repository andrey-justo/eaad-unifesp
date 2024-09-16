using System.Data;

class AndreyJustoSem1 {
    // Helper strings
    private static readonly string RANDOM = "RANDOM";
    private static readonly string ASC = "ASC";
    private static readonly string DESC = "DESC";
    // Size of array
    private static int VALUES_SIZE = 10000;

    private static int MAX_ITERATIONS = 100;
    // Save executions for statistics
    private static Dictionary<string, List<double>> EXECUTIONS_QUICKSORT = new Dictionary<string, List<double>>() {
        { RANDOM, new List<double>() },
        { ASC, new List<double>() },
        { DESC, new List<double>() }
    };
    private static Dictionary<string, List<double>> EXECUTIONS_INTROSORT = new Dictionary<string, List<double>>() {
        { RANDOM, new List<double>() },
        { ASC, new List<double>() },
        { DESC, new List<double>() }
    };

    // If we choose a too large pivot, we receive out of memory, because quick sort will need to run more times
    // and the current approach uses the program stack call, so it needs a lot of memory for those cases
    private static int QUICK_PIVOT_STARTER = VALUES_SIZE - 1; // can use 3 for testing
    
    public static int[] generateRandomValues() {
        Random seed = new Random();
        int[] result = new int[VALUES_SIZE];
        for (int i = 0; i < VALUES_SIZE; i++) {
            result[i] = seed.Next(VALUES_SIZE);
        }

        return result;
    }

    static void Main(string[] args) {
        for (int i = 0; i < MAX_ITERATIONS; i++) {
            int[] randomValues = generateRandomValues();
            int[] orderedValuesAsc = generateRandomValues();
            Array.Sort(orderedValuesAsc);
            int[] orderedValuesDesc = generateRandomValues().OrderByDescending(c => c).ToArray();
            var originalValues = Tuple.Create(randomValues, orderedValuesAsc, orderedValuesDesc);

            // Quicksort
            Execute("Quicksort", originalValues, QuickSort, EXECUTIONS_QUICKSORT);

            // Introsort
            Execute("Introsort", originalValues, Array.Sort, EXECUTIONS_INTROSORT);
        }

        Console.WriteLine($"Average Quicksort: {EXECUTIONS_QUICKSORT[RANDOM].Average()} for random values");
        Console.WriteLine($"Average Introsort: {EXECUTIONS_INTROSORT[RANDOM].Average()} for random values");
        
        Console.WriteLine($"Average Quicksort: {EXECUTIONS_QUICKSORT[ASC].Average()} for asc values");
        Console.WriteLine($"Average Introsort: {EXECUTIONS_INTROSORT[ASC].Average()} for asc values");

        Console.WriteLine($"Average Quicksort: {EXECUTIONS_QUICKSORT[DESC].Average()} for desc values");
        Console.WriteLine($"Average Introsort: {EXECUTIONS_INTROSORT[DESC].Average()} for desc values");

        Console.WriteLine($"Max Quicksort: {EXECUTIONS_QUICKSORT[RANDOM].Max()} for random values");
        Console.WriteLine($"Max Introsort: {EXECUTIONS_INTROSORT[RANDOM].Max()} for random values");
        
        Console.WriteLine($"Max Quicksort: {EXECUTIONS_QUICKSORT[ASC].Max()} for asc values");
        Console.WriteLine($"Max Introsort: {EXECUTIONS_INTROSORT[ASC].Max()} for asc values");

        Console.WriteLine($"Max Quicksort: {EXECUTIONS_QUICKSORT[DESC].Max()} for desc values");
        Console.WriteLine($"Max Introsort: {EXECUTIONS_INTROSORT[DESC].Max()} for desc values");
    }

    static void Execute(string alg, Tuple<int[], int[], int[]> originalValues, Action<int[]> orderingFunc, Dictionary<string, List<double>> executions) {
        Console.WriteLine($"Sorting Random Array with {alg}");
        var clonedRandom = (int[])originalValues.Item1.Clone();
        var randomMeasure = measureInMillis(() => orderingFunc(clonedRandom));
        Console.WriteLine($"Result is ordered={CheckIsOrdered(clonedRandom)}");
        executions[RANDOM].Add(randomMeasure);
        Console.WriteLine($"Sorting Ordered Array Asc with {alg}");
        var ascValues = (int[])originalValues.Item2.Clone();
        var orderedMeasureAsc = measureInMillis(() => orderingFunc(ascValues));
        Console.WriteLine($"Result is ordered={CheckIsOrdered(ascValues)}");
        executions[ASC].Add(orderedMeasureAsc);
        Console.WriteLine($"Sorting Ordered Array Desc with {alg}");
        var descValues = (int[])originalValues.Item3.Clone();
        var orderedMeasureDesc = measureInMillis(() => orderingFunc(descValues));
        Console.WriteLine($"Result is ordered={CheckIsOrdered(descValues)}");
        executions[DESC].Add(orderedMeasureDesc);
    }

    static double measureInMillis(Action orderingFunc) {
        DateTime start = DateTime.Now;
        orderingFunc();        
        DateTime finish = DateTime.Now;
        return (finish - start).TotalMilliseconds;
    }

    // Quick sort - Reference: https://learn.microsoft.com/en-us/answers/questions/1259438/c-sorting-algorithms-implementation
    static void QuickSort(int[] values) {
        QuickSort(values, 0, QUICK_PIVOT_STARTER); 
    }

    static void QuickSort(int[] arr, int left, int right) 
    {
        if (left < right) {

            // pi is partitioning index, arr[p]
            // is now at right place
            int pi = Partition(arr, left, right);

            // Separately sort elements before
            // and after partition index
            QuickSort(arr, left, pi - 1);
            QuickSort(arr, pi + 1, right);
        }
    }

    private static int Partition(int[] arr, int left, int right)
    {
        // Choosing the pivot
        int pivot = arr[right];

        // Index of smaller element and indicates
        // the right position of pivot found so far
        int i = (left - 1);

        for (int j = left; j <= right - 1; j++) {

            // If current element is smaller than the pivot
            if (arr[j] < pivot) {

                // Increment index of smaller element
                i++;
                Swap(arr, i, j);
            }
        }
        Swap(arr, i + 1, right);
        return (i + 1);
    }

    // A utility function to swap two elements
    static void Swap(int[] arr, int i, int j)
    {
        int temp = arr[i];
        arr[i] = arr[j];
        arr[j] = temp;
    }

    static bool CheckIsOrdered(int[] values) {
        if (values.Count() <= 1) {
            return true;
        }

        for (int i = 1; i < values.Length; i++) {
            if (values[i-1] > values[i]) {
                return false;
            }
        }

        return true;
    }

}
