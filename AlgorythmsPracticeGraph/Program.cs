namespace AlgorythmsPracticeGraph;

public static class Program
{
    public static void Main(string[] args)
    {
        //Simple graph
        Console.WriteLine("Simple graph:");
        int[,] simpleAdjacencyMatrix = new int[,]
        { // A  B  C  D  E
            {0, 1, 0, 1, 1}, // A
            {0, 0, 1, 0, 0}, // B
            {1, 0, 0, 0, 0}, // C
            {0, 0, 1, 0, 0}, // D
            {0, 0, 0, 1, 0}  // E
        };

        string[] simpleVertexMarks = { "1", "2", "3", "4", "5" };

        var simpleGraph = new Graph();
        
        simpleGraph.CreateFromAdjacencyMatrix(simpleAdjacencyMatrix, simpleVertexMarks);
        
        simpleGraph.PrintGraphDiagram();
        simpleGraph.PrintGraph();
        simpleGraph.VisualizeGraph("simple_graph.png");
        
        Console.WriteLine("Enter the desired cycle length for simple graph: ");
        if (int.TryParse(Console.ReadLine(), out int simpleCycleLength))
        {
            TestCycles(simpleGraph, simpleCycleLength);
        }
        
        Console.WriteLine("\n");
        
        // Complex graph
        Console.WriteLine("Complex graph:");
        int[,] complexAdjacencyMatrix = new int[,]
        { // A  B  C  D  E  F  G  H
            {0, 3, 0, 0, 0, 0, 7, 0},  // A
            {0, 0, 2, 0, 0, 0, 0, 1},  // B
            {0, 0, 0, 3, 0, 1, 0, 0},  // C
            {0, 21, 0, 0, 2, 0, 0, 0}, // D
            {0, 0, 0, 0, 0, 0, 3, 0},  // E
            {0, 0, 0, 0, 74, 0, 0, 2}, // F
            {9, 0, 1, 0, 0, 0, 0, 0},  // G
            {1, 0, 0, 0, 0, 0, 6, 0}   // H 
        };
        
        string[] complexVertexMarks = { "1", "2", "3", "4", "5", "6", "7", "8" };
        
        var complexGraph = new Graph();
        
        complexGraph.CreateFromAdjacencyMatrix(complexAdjacencyMatrix, complexVertexMarks);
        
        complexGraph.PrintGraphDiagram();
        complexGraph.PrintGraph();
        complexGraph.VisualizeGraph("complex_graph.png");
        
        Console.WriteLine("Enter the desired cycle length for complex graph: ");
        if (int.TryParse(Console.ReadLine(), out int complexCycleLength))
        {
            TestCycles(simpleGraph, complexCycleLength);
        }
           
    }

    private static void TestCycles(Graph graph, int cycleLength)
    {
        var cycles = graph.FindCyclesOfLength(cycleLength);

        if (cycles.Count <= 0) return;
        Console.WriteLine($"Cycles of length {cycleLength}:");

        var uniqueCycles = new HashSet<string>();
        foreach (var cycle in cycles)
        {
            var cycleString = string.Join(" -> ", cycle.Select(v => v.Name));

            if (!uniqueCycles.Contains(cycleString))
            {
                uniqueCycles.Add(cycleString);
                Console.WriteLine(cycleString);
            }
            else
            {
                Console.WriteLine($"No cycles of length {cycleLength} found.");
            }
        }

        Console.WriteLine();
    }
}