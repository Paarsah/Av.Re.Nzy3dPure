using System;
using System.Collections.Generic;

namespace Mag3DView.Nzy3dAPI.Plot3D.Builder.Delaunay.Jdt
{
	/// <summary>
	/// <para>
	/// Grid Index is a simple spatial index for fast point/triangle location.
	/// The idea is to divide a predefined geographic extent into equal sized
	/// cell matrix (tiles). Every cell will be associated with a triangle which lies inside.
	/// Therfore, one can easily locate a triangle in close proximity of the required
	/// point by searching from the point's cell triangle. If the triangulation is
	/// more or less uniform and bound in space, this index is very effective,
	/// roughly recuing the searched triangles by square(xCellCount * yCellCount),
	/// as only the triangles inside the cell are searched.
	/// </para>
	/// <para>
	/// The index takes xCellCount * yCellCount capacity. While more cells allow
	/// faster searches, even a small grid is helpfull.
	/// </para>
	/// <para>
	/// This implementation holds the cells in a memory matrix, but such a grid can
	/// be easily mapped to a DB table or file where it is usually used for it's fullest.
	/// </para>
	/// <para>
	/// Note that the index is geographically bound - only the region given in the
	/// c'tor is indexed. Added Triangles outside the indexed region will cause rebuilding of
	/// the whole index. Since triangulation is mostly always used for static raster data,
	/// and usually is never updated outside the initial zone (only refininf existing triangles)
	/// this is never an issue in real life.
	/// </para>
	/// </summary>
	public class GridIndex
	{
		/// <summary> The triangulation of the index </summary>
		private Delaunay_Triangulation indexDelaunay;

		/// <summary> Horizontal geographic size of a cell index </summary>
		private double x_size;

		/// <summary> Vertical  geographic size of a cell index </summary>
		private double y_size;

		/// <summary> The indexed geographic size </summary>
		private BoundingBox indexRegion;

		/// <summary> A division of indexRegion to a cell matrix, where each cell holds a triangle which lies in it </summary>
		private Triangle_dt[,] grid;

		/// <summary>
		/// Constructs a grid index holding the triangles of a delaunay triangulation.
		/// This version uses the bounding box of the triangulation as the region to index.
		/// </summary>
		/// <param name="delaunay">delaunay triangulation to index</param>
		/// <param name="xCellCount">number of grid cells in a row</param>
		/// <param name="yCellCount">number of grid cells in a column</param>
		public GridIndex(Delaunay_Triangulation delaunay, int xCellCount, int yCellCount) : this(delaunay, xCellCount, yCellCount, delaunay.BoundingBox)
		{
		}

		/// <summary>
		/// Constructs a grid index holding the triangles of a delaunay triangulation.
		/// The grid will be made of (xCellCount * yCellCount) cells.
		/// The smaller the cells the less triangles that fall in them, whuch means better
		/// indexing, but also more cells in the index, which mean more storage.
		/// The smaller the indexed region is, the smaller the cells can be and still
		/// maintain the same capacity, but adding geometries outside the initial region
		/// will invalidate the index !
		/// </summary>
		/// <param name="delaunay">delaunay triangulation to index</param>
		/// <param name="xCellCount">number of grid cells in a row</param>
		/// <param name="yCellCount">number of grid cells in a column</param>
		/// <param name="region">geographic region to index</param>
		public GridIndex(Delaunay_Triangulation delaunay, int xCellCount, int yCellCount, BoundingBox region)
		{
			Init(delaunay, xCellCount, yCellCount, region);
		}

		public void Init(Delaunay_Triangulation delaunay, int xCellCount, int yCellCount, BoundingBox region)
		{
			indexDelaunay = delaunay;
			indexRegion = region;
			x_size = region.Width / yCellCount;
			y_size = region.Height / xCellCount;
			// The grid will hold a trinagle for each cell, so a point (x,y) will lie
			// in the cell representing the grid partition of region to a
			//  xCellCount on yCellCount grid
			grid = new Triangle_dt[xCellCount + 1, yCellCount + 1];
			Triangle_dt colStartTriangle = indexDelaunay.Find(MiddleOfCell(0, 0));
			UpdateCellValues(0, 0, xCellCount - 1, yCellCount - 1, colStartTriangle);
		}

		/// <summary>
		/// Finds a triangle near the given point
		/// </summary>
		/// <param name="point">a query point</param>
		/// <returns>a triangle at the same cell of the point</returns>
		public Triangle_dt FindCellTriangleOf(Point_dt point)
		{
			int x_index = Convert.ToInt32((point.X - indexRegion.MinX) / x_size);
			int y_index = Convert.ToInt32((point.Y - indexRegion.MinY) / y_size);
			return grid[x_index, y_index];
		}

		/// <summary>
		/// Updates the grid index to reflect changes to the triangulation. Note that added
		/// triangles outside the indexed region will force to recompute the whole index
		/// with the enlarged region
		/// </summary>
		/// <param name="updatedTriangles">Changed triangles of the triangulation. This may be added triangles,
		/// removed triangles or both. All that matter is that they cover the
		/// changed area.
		/// </param>
		public void UpdateIndex(IEnumerator<Triangle_dt> updatedTriangles)
		{
			// Gather the bounding box of the updated area
			BoundingBox updatedRegion = new BoundingBox();
			while (updatedTriangles.Current != null)
			{
				updatedRegion = updatedRegion.UnionWith(updatedTriangles.Current.BoundingBox);
				updatedTriangles.MoveNext();
			}

			if (updatedRegion.IsNull) return;

			// No update...
			// Bad news - the updated region lies outside the indexed region.
			// The whole index must be recalculated
			if (!indexRegion.Contains(updatedRegion))
			{
				Init(indexDelaunay, Convert.ToInt32(indexRegion.Width / x_size), Convert.ToInt32(indexRegion.Height / y_size), indexRegion.UnionWith(updatedRegion));
			}
			else
			{
				// Find the cell region to be updated
				Point_dt minInvalidCell = GetCellOf(updatedRegion.MinPoint);
				Point_dt maxInvalidCell = GetCellOf(updatedRegion.MaxPoint);

				// And update it with fresh triangles
				Triangle_dt adjacentValidTriangle = FindValidTriangle(minInvalidCell);
				UpdateCellValues((int)minInvalidCell.X, (int)minInvalidCell.Y, (int)maxInvalidCell.X, (int)maxInvalidCell.Y, adjacentValidTriangle);
			}
		}

		/// <summary>
		/// Go over each grid cell and locate a triangle in it to be the cell's
		/// starting search triangle. Since we only pass between adjacent cells
		/// we can search from the last triangle found and not from the start.
		/// Add triangles for each column cells
		/// </summary>
		/// <param name="startXCell"></param>
		/// <param name="startYCell"></param>
		/// <param name="lastXCell"></param>
		/// <param name="lastYCell"></param>
		/// <param name="startTriangle"></param>
		private void UpdateCellValues(int startXCell, int startYCell, int lastXCell, int lastYCell, Triangle_dt startTriangle)
		{
			for (int i = startXCell; i <= lastXCell; i++)
			{
				// Find a triangle at the begining of the current column
				startTriangle = indexDelaunay.Find(MiddleOfCell(i, startYCell), startTriangle);
				grid[i, startYCell] = startTriangle;
				Triangle_dt prevRowTriangle = startTriangle;

				// Add triangles for the next row cells
				for (int j = startYCell + 1; j <= lastYCell; j++)
				{
					grid[i, j] = indexDelaunay.Find(MiddleOfCell(i, j), prevRowTriangle);
					prevRowTriangle = grid[i, j];
				}
			}
		}

		/// <summary>
		/// Finds a valid (existing) trinagle adjacent to a given invalid cell
		/// </summary>
		/// <param name="minInvalidCell">minimum bounding box invalid cell</param>
		/// <returns>a valid triangle adjacent to the invalid cell</returns>
		private Triangle_dt FindValidTriangle(Point_dt minInvalidCell)
		{
			// If the invalid cell is the minimal one in the grid we are forced to search the
			// triangulation for a trinagle at that location
			if (minInvalidCell.X == 0 && minInvalidCell.Y == 0)
			{
				return indexDelaunay.Find(MiddleOfCell((int)minInvalidCell.X, (int)minInvalidCell.Y), null);
			}
			else
			{
				// Otherwise we can take an adjacent cell triangle that is still valid
				return grid[(int)Math.Min(0, minInvalidCell.X), (int)Math.Min(0, minInvalidCell.Y)];
			}
		}

		/// <summary>
		/// Locates the grid cell point covering the given coordinat
		/// </summary>
		/// <param name="coordinate">World coordinate to locate</param>
		/// <returns>Cell covering the coordinate</returns>
		private Point_dt GetCellOf(Point_dt coordinate)
		{
			int x_cell = Convert.ToInt32((coordinate.X - indexRegion.MinX) / x_size);
			int y_cell = Convert.ToInt32((coordinate.Y - indexRegion.MinY) / y_size);
			return new Point_dt(x_cell, y_cell);
		}

		/// <summary>
		/// Create a point at the center of a cell
		/// </summary>
		/// <param name="x_index">Horizontal cell index</param>
		/// <param name="y_index">Vertical cell index</param>
		/// <returns>Point at the center of the cell at (x_index, y_index)</returns>
		private Point_dt MiddleOfCell(int x_index, int y_index)
		{
			double middleXCell = indexRegion.MinX + x_index * x_size + x_size / 2;
			double middleYCell = indexRegion.MinY + y_index * y_size + y_size / 2;
			return new Point_dt(middleXCell, middleYCell);
		}
	}
}
