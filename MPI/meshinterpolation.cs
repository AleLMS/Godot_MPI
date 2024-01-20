using Godot;
using System.Linq;

public partial class meshinterpolation : Node
{
	[Export]
	public Node3D target, mesh;
	[Export]
	public Node movementSource;
	private Transform3D previous, current;
	[Export]
	public bool UpdateLast = true;

	public override void _Ready()
	{
		FindDepencies();
		previous = target.GlobalTransform;
		current = target.GlobalTransform;

		CallDeferred("MoveNode");
	}
	public override void _Process(double delta)
	{
		UpdatePosition();
	}

	public override void _PhysicsProcess(double delta)
	{
		UpdateTransforms();
	}

	// Move node in the scene hierarchy
	public void MoveNode()
	{
		int tempIndex = Mathf.Max(target.GetIndex(), mesh.GetIndex()) + 1;

		int index = (movementSource == null) ? tempIndex : UpdateLast ? Mathf.Max(tempIndex, movementSource.GetIndex() + 1) : Mathf.Min(tempIndex, movementSource.GetIndex() + 1);

		int maxIndex = GetParent().GetChildCount();
		int clampedIndex = Mathf.Clamp(index, 0, maxIndex);
		GetParent().MoveChild(this, clampedIndex);
	}


	public void UpdateTransforms()
	{
		if (!CheckDepencies())
			return;

		previous = current;
		current = target.GlobalTransform;
	}

	public void UpdatePosition()
	{
		if (!CheckDepencies())
			return;
		mesh.GlobalTransform = previous.InterpolateWith(current, (float)Engine.GetPhysicsInterpolationFraction());
	}

	public bool CheckDepencies()
	{
		if (target == null || mesh == null)
			return false;
		else
			return true;
	}

	public void Teleport()
	{
		if (!CheckDepencies())
			return;

		previous = mesh.GlobalTransform;
		current = mesh.GlobalTransform;
	}

	public MeshInstance3D FindMesh()
	{
		Node[] children = GetParent().GetChildren().ToArray();
		foreach (Node child in children)
		{
			if (child is MeshInstance3D mesh)
				return mesh;
		}
		return null;
	}

	public Node3D FindTarget()
	{
		Node[] children = GetParent().GetChildren().ToArray();
		foreach (Node child in children)
		{
			if (child is PhysicsBody3D body)
				return body;
		}
		return null;
	}

	public void FindDepencies()
	{
		if (mesh == null)
			mesh = FindMesh();
		if (target == null)
			target = FindTarget();
	}
}
