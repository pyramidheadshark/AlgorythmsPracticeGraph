namespace AlgorythmsPracticeGraph;

public static class Program
{
    public static void Main(string[] args)
    {
        var complexGraph = new Graph();
        complexGraph.AddVertex("A", "1");
        complexGraph.AddVertex("B", "2");
        complexGraph.AddVertex("C", "3");
        complexGraph.AddVertex("D", "4");
        complexGraph.AddVertex("E", "5");
        complexGraph.AddVertex("F", "6");


        complexGraph.AddEdge("A", "B", 4);
        complexGraph.AddEdge("B", "C", 2);
        complexGraph.AddEdge("C", "A", 7);
        complexGraph.AddEdge("A", "D");
        complexGraph.AddEdge("D", "C", 3);
        complexGraph.AddEdge("B", "E");
        complexGraph.AddEdge("E", "F");
        complexGraph.AddEdge("F", "B", 6);
        complexGraph.AddEdge("D", "F", 8);
        complexGraph.AddEdge("E", "D", 4);

        complexGraph.PrintGraphDiagram();

        for (var cycleLength = 3; cycleLength <= 50; cycleLength++) TestCycles(complexGraph, cycleLength);

        complexGraph.VisualizeGraph("complex_graph.png");
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
        }

        Console.WriteLine();
    }
}