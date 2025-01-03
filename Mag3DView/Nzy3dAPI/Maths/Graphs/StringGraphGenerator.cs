namespace Mag3DView.Nzy3dAPI.Maths.Graphs
{
	public class StringGraphGenerator
	{
		public static IGraph<string, string> getGraph(int nodes, int edges)
		{
			var graph = new DefaultGraph<string, string>();

			for (int i = 0; i <= nodes - 1; i++)
			{
				graph.AddVertex("vertex " + i);
			}

			for (int i = 0; i <= edges - 1; i++)
			{
				string v1 = graph.GetRandomVertex();
				string v2 = graph.GetRandomVertex();
				graph.AddEdge("edge " + v1 + v2, v1, v2);
			}
			return graph;
		}

		/*
		public static DefaultGraphLayout2d<string> getRandomLayout(IGraph<string, string> graph, double size)
		{
			DefaultGraphLayout2d<string> layout = new DefaultGraphLayout2d<string>();
			Random rng = new Random();
			foreach (string v in graph.getVertices()) {
				double x = rng.NextDouble() * size - size / 2;
				double y = rng.NextDouble() * size - size / 2;
				layout.values().Add( .VertexPosition[v] = new Coord2d(x, y);
			}
			return layout;
		}
        */
	}
}

//=======================================================
//Service provided by Telerik (www.telerik.com)
//Conversion powered by NRefactory.
//Twitter: @telerik
//Facebook: facebook.com/telerik
//=======================================================
