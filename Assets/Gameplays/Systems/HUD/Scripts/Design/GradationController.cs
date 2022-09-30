using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GradationController : BaseMeshEffect
{
	public Color colorTop    = Color.white;
	public Color colorBottom = Color.white;

	public override void ModifyMesh(VertexHelper vh)
	{
		if ( !IsActive() )
			return;

		List<UIVertex> vertices = new List<UIVertex>();

		vh.GetUIVertexStream(vertices);

		Gradation(vertices);

		vh.Clear();
		vh.AddUIVertexTriangleStream(vertices);
	}

	private void Gradation(List<UIVertex> vertices)
	{
		for (int i = 0; i < vertices.Count; i++)
		{
			UIVertex newVertex= vertices [i];
								
			newVertex.color = (i % 6 == 0 || i % 6 == 1 || i % 6 == 5) ? colorTop : colorBottom;

			vertices [i] = newVertex;
		}
	}
}