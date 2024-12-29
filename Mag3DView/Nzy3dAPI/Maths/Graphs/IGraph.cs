using System.Collections.Generic;

namespace Mag3DView.Nzy3dAPI.Maths.Graphs
{
	public interface IGraph<V, E>
	{
		void AddVertex(V vertex);

		void AddEdge(E edge, V v1, V v2);

		V GetVertex(int i);

		V GetRandomVertex();

		List<V> GetVertices();

		List<E> GetEdges();

		V GetEdgeStartVertex(E e);

		V GetEdgeStopVertex(E e);
	}
}
