using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Text;

namespace AlgorythmsPracticeGraph;

public class Vertex(string name, string mark)
{
    public string Name { get; } = name;
    private string Mark { get; } = mark;


    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;

        var other = (Vertex)obj;
        return Name.Equals(other.Name);
    }


    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }


    public override string ToString()
    {
        return $"({Name}, {Mark})";
    }
}

public class Edge(Vertex? source, Vertex? destination, int weight)
{
    public Vertex? Source { get; } = source;
    public Vertex? Destination { get; } = destination;
    public int Weight { get; set; } = weight;

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;

        var other = (Edge)obj;
        return Source != null && Source.Equals(other.Source) && Destination != null &&
               Destination.Equals(other.Destination);
    }


    public override int GetHashCode()
    {
        return HashCode.Combine(Source, Destination);
    }


    public override string ToString()
    {
        return $"({Source?.Name}, {Destination?.Name}, {Weight})";
    }
}

public class Graph
{
    public List<Vertex> Vertices { get; } = new();
    public int[,]? IncidenceMatrix { get; private set; }


    public void AddVertex(string name, string mark)
    {
        if (Vertices.Any(v => v.Name == name))
            throw new ArgumentException($"Vertex with name '{name}' already exists.");
        Vertices.Add(new Vertex(name, mark));
        RebuildIncidenceMatrix();
    }

    public void AddEdge(string sourceName, string destName, int weight = 1)
    {
        var source = Vertices.Find(v => v.Name == sourceName);
        var dest = Vertices.Find(v => v.Name == destName);

        if (source == null || dest == null) throw new ArgumentException("Source or destination vertex not found.");

        var nextEdgeIndex = IncidenceMatrix?.GetLength(1) ?? 0;
        RebuildIncidenceMatrix(nextEdgeIndex + 1, true);


        if (IncidenceMatrix != null)
        {
            var sourceIndex = Vertices.IndexOf(source);
            var destIndex = Vertices.IndexOf(dest);
            IncidenceMatrix[sourceIndex, nextEdgeIndex] = weight;
            IncidenceMatrix[destIndex, nextEdgeIndex] = -weight;
        }
    }
    

    private void RebuildIncidenceMatrix(int? newEdgeCount = null, bool preserveExistingEdges = false)
    {
        var numVertices = Vertices.Count;
        var numEdges = newEdgeCount ?? (IncidenceMatrix?.GetLength(1) ?? 0);

        var newMatrix = new int[numVertices, numEdges];

        if (preserveExistingEdges && IncidenceMatrix != null)
        {
            var rowsToCopy = IncidenceMatrix.GetLength(0);
            var colsToCopy = IncidenceMatrix.GetLength(1);
            for (var i = 0; i < rowsToCopy; i++)
            for (var j = 0; j < colsToCopy; j++)
                newMatrix[i, j] = IncidenceMatrix[i, j];
        }

        IncidenceMatrix = newMatrix;
    }


    public void PrintGraph()
    {
        if (IncidenceMatrix == null)
        {
            Console.WriteLine("Graph is empty.");
            return;
        }

        var numVertices = IncidenceMatrix.GetLength(0);
        var numEdges = IncidenceMatrix.GetLength(1);

        Console.Write("   ");
        for (var j = 0; j < numEdges; j++) Console.Write($"e{j + 1} ");
        Console.WriteLine();

        for (var i = 0; i < numVertices; i++)
        {
            Console.Write($"{Vertices[i].Name}  ");
            for (var j = 0; j < numEdges; j++) Console.Write($"{IncidenceMatrix[i, j]}  ");
            Console.WriteLine();
        }
    }

    public void PrintGraphDiagram()
    {
        if (IncidenceMatrix == null)
        {
            Console.WriteLine("Graph is empty.");
            return;
        }

        var numVertices = IncidenceMatrix.GetLength(0);
        var numEdges = IncidenceMatrix.GetLength(1);

        var edges = new List<Edge>();

        for (var j = 0; j < numEdges; j++)
        {
            Vertex? source = null;
            Vertex? dest = null;
            var weight = 0;

            for (var i = 0; i < numVertices; i++)
                switch (IncidenceMatrix[i, j])
                {
                    case > 0:
                        weight = IncidenceMatrix[i, j];
                        source = Vertices[i];
                        break;
                    case < 0:
                        dest = Vertices[i];
                        break;
                }

            if (source != null && dest != null) edges.Add(new Edge(source, dest, weight));
        }


        var sb = new StringBuilder();
        foreach (var edge in edges) sb.AppendLine($"{edge.Source?.Name} --({edge.Weight})--> {edge.Destination?.Name}");

        Console.WriteLine(sb.ToString());
    }
    
    public void CreateFromAdjacencyMatrix(int[,] adjacencyMatrix, string[] vertexMarks)
    {
        int numVertices = adjacencyMatrix.GetLength(0);

        if (adjacencyMatrix.GetLength(1) != numVertices || vertexMarks.Length != numVertices)
        {
            throw new ArgumentException("Invalid adjacency matrix or vertex marks dimensions.");
        }

        for (int i = 0; i < numVertices; i++)
        {
            AddVertex(((char)('A' + i)).ToString(), vertexMarks[i]);
        }

        int numEdges = 0;
        for (int i = 0; i < numVertices; i++)
        {
            for (int j = 0; j < numVertices; j++)
            {
                if (adjacencyMatrix[i, j] > 0)
                {
                    numEdges++;
                }
            }
        }

        IncidenceMatrix = new int[numVertices, numEdges];
        int edgeIndex = 0;

        for (int i = 0; i < numVertices; i++)
        {
            for (int j = 0; j < numVertices; j++)
            {
                if (adjacencyMatrix[i, j] > 0)
                {
                    IncidenceMatrix[i, edgeIndex] = adjacencyMatrix[i, j];  // Source vertex gets positive value
                    IncidenceMatrix[j, edgeIndex] = -adjacencyMatrix[i, j]; // Destination vertex gets negative value

                    edgeIndex++;
                }
            }
        }
    }
}

public static class GraphExtensions
{
    public static List<List<Vertex>> FindCyclesOfLength(this Graph graph, int cycleLength)
    {
        if (graph.IncidenceMatrix == null) return new List<List<Vertex>>();

        var cycles = new List<List<Vertex>>();
        var currentPath = new List<Vertex>();
        var uniqueCycles = new HashSet<string>();

        foreach (var startVertex in graph.Vertices)
            FindCyclesRecursive(graph, startVertex, currentPath, cycleLength, cycles, uniqueCycles);

        return cycles;
    }

    private static void FindCyclesRecursive(Graph graph, Vertex currentVertex, List<Vertex> currentPath,
        int cycleLength, List<List<Vertex>> cycles, HashSet<string> uniqueCycles)
    {
        currentPath.Add(currentVertex);

        if (currentPath.Count == cycleLength)
        {
            var adjacent = graph.AreVerticesAdjacent(currentPath.Last(), currentPath[0]);
            if (adjacent)
            {
                var canonicalCycle = CanonicalizeCycle(currentPath);
                if (!uniqueCycles.Contains(canonicalCycle))
                {
                    cycles.Add(new List<Vertex>(currentPath));
                    uniqueCycles.Add(canonicalCycle);
                }
            }
        }
        else
        {
            foreach (var edge in graph.GetOutgoingEdges(currentVertex).Where(edge =>
                         !currentPath.Contains(edge.Destination ?? throw new InvalidOperationException()) ||
                         (edge.Destination.Equals(currentPath[0]) && currentPath.Count == cycleLength - 1)))

                FindCyclesRecursive(graph, edge.Destination ?? throw new InvalidOperationException(), currentPath,
                    cycleLength, cycles, uniqueCycles);
        }

        currentPath.RemoveAt(currentPath.Count - 1);
    }


    private static string CanonicalizeCycle(List<Vertex> cycle)
    {
        var cycleNames = cycle.Select(v => v.Name).ToList();
        cycleNames.Sort();
        return string.Join(" ", cycleNames);
    }


    private static bool AreVerticesAdjacent(this Graph graph, Vertex source, Vertex destination)
    {
        if (graph.IncidenceMatrix == null) return false;

        var sourceIndex = graph.Vertices.IndexOf(source);
        var destIndex = graph.Vertices.IndexOf(destination);

        if (sourceIndex == -1 || destIndex == -1) return false;

        for (var j = 0; j < graph.IncidenceMatrix.GetLength(1); j++)
            if (graph.IncidenceMatrix[sourceIndex, j] > 0 && graph.IncidenceMatrix[destIndex, j] < 0)
                return true;

        return false;
    }


    private static List<Edge> GetOutgoingEdges(this Graph graph, Vertex source)
    {
        var outgoingEdges = new List<Edge>();
        if (graph.IncidenceMatrix == null) return outgoingEdges;
        var vertexIndex = graph.Vertices.IndexOf(source);

        if (vertexIndex == -1) return outgoingEdges;


        for (var j = 0; j < graph.IncidenceMatrix.GetLength(1); j++)
            if (graph.IncidenceMatrix[vertexIndex, j] > 0)
                for (var i = 0; i < graph.Vertices.Count; i++)
                    if (graph.IncidenceMatrix[i, j] < 0)
                    {
                        outgoingEdges.Add(new Edge(source, graph.Vertices[i], graph.IncidenceMatrix[vertexIndex, j]));
                        break;
                    }

        return outgoingEdges;
    }

    public static void VisualizeGraph(this Graph graph, string filename = "graph.png")
    {
        var graphSize = graph.Vertices.Count * 100;
        if (graphSize < 1280) graphSize = 1280;


        using var bitmap = new Bitmap(graphSize, graphSize);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.White);


        var vertexPositions = new Dictionary<Vertex, Point>();
        double angle = 0;
        var angleStep = 2 * Math.PI / graph.Vertices.Count;


        foreach (var vertex in graph.Vertices)
        {
            var x = (int)(graphSize / 2 + graphSize / 2.5 * Math.Cos(angle));
            var y = (int)(graphSize / 2 + graphSize / 2.5 * Math.Sin(angle));

            vertexPositions[vertex] = new Point(x, y);
            angle += angleStep;
        }


        foreach (var edge in graph.Edges())
        {
            if (edge.Source == null ||
                !vertexPositions.TryGetValue(edge.Source, out var sourcePos) ||
                edge.Destination == null ||
                !vertexPositions.TryGetValue(edge.Destination, out var destPos)) continue;

            using var pen = new Pen(Color.Black, 3);
            pen.CustomEndCap = new AdjustableArrowCap(12, 10, true);


            var direction = new PointF(destPos.X - sourcePos.X, destPos.Y - sourcePos.Y);
            var distance = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
            direction.X /= distance;
            direction.Y /= distance;


            const int offset = 30;
            var arrowEndPoint = new PointF(
                destPos.X - direction.X * offset,
                destPos.Y - direction.Y * offset
            );

            var shortenedDest = new PointF(
                arrowEndPoint.X - direction.X * 12,
                arrowEndPoint.Y - direction.Y * 12
            );
            graphics.DrawLine(pen, sourcePos, shortenedDest);


            if (edge.Weight != 0)
            {
                using var font = new Font(SystemFonts.DefaultFont.FontFamily, 20, FontStyle.Bold);
                using var brush = new SolidBrush(Color.Red);
                var weightString = edge.Weight.ToString();
                var textSize = graphics.MeasureString(weightString, font);


                angle = Math.Atan2(shortenedDest.Y - sourcePos.Y, shortenedDest.X - sourcePos.X);


                var offsetX = (float)(15 * Math.Sin(angle));
                var offsetY = (float)(-15 * Math.Cos(angle));


                var textLocation = new PointF(
                    (sourcePos.X + shortenedDest.X) / 2 + offsetX - textSize.Width / 2,
                    (sourcePos.Y + shortenedDest.Y) / 2 + offsetY - textSize.Height / 2
                );
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                graphics.DrawString(weightString, font, brush, textLocation);
            }
        }


        var vertexSize = 80;
        foreach (var vertex in graph.Vertices)
            if (vertexPositions.TryGetValue(vertex, out var position))
            {
                var rect = new Rectangle(position.X - vertexSize / 2, position.Y - vertexSize / 2, vertexSize,
                    vertexSize);
                graphics.FillEllipse(Brushes.LightBlue, rect);
                graphics.DrawEllipse(Pens.Black, rect);

                using var font = new Font(SystemFonts.DefaultFont.FontFamily, 20);
                using var format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                using var brush = new SolidBrush(Color.Black);
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                graphics.DrawString(vertex.Name, font, brush, rect, format);

                ;
            }


        bitmap.Save(filename, ImageFormat.Png);
        Console.WriteLine($"Graph visualized and saved to {filename}");
    }

    public static List<Edge> Edges(this Graph graph)
    {
        var edges = new List<Edge>();
        if (graph.IncidenceMatrix != null)
        {
            var numVertices = graph.IncidenceMatrix.GetLength(0);
            var numEdges = graph.IncidenceMatrix.GetLength(1);

            for (var j = 0; j < numEdges; j++)
            {
                Vertex? source = null;
                Vertex? dest = null;
                var weight = 0;

                for (var i = 0; i < numVertices; i++)
                {
                    if (graph.IncidenceMatrix[i, j] > 0)
                    {
                        weight = graph.IncidenceMatrix[i, j];
                        source = graph.Vertices[i];
                    }
                    else if (graph.IncidenceMatrix[i, j] < 0)
                    {
                        dest = graph.Vertices[i];
                    }
                }
                if (source != null && dest != null)
                {
                    edges.Add(new Edge(source, dest, weight));
                }
           
            }
        }
        return edges;
    }
}