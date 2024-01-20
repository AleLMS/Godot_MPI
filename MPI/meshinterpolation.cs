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

	public void MoveNode()
	{
		int a;
		int index;
		switch (UpdateLast)
		{
			case true:
				a = Mathf.Max(target.GetIndex(), mesh.GetIndex()) + 1;
				if (movementSource != null)
					index = Mathf.Max(a, movementSource.GetIndex() + 1);
				else
					index = a;
				break;
			case false:
				a = Mathf.Min(target.GetIndex(), mesh.GetIndex()) + 1;
				if (movementSource != null)
					index = Mathf.Min(a, movementSource.GetIndex() + 1);
				else
					index = a;
				break;
		}

		int maxIndex = GetParent().GetChildCount();
		int clampedIndex = Mathf.Clamp(index, 0, maxIndex);
		GetParent().MoveChild(this, clampedIndex);
	}

	public override void _Process(double delta)
	{
		UpdatePosition();
	}

	public override void _PhysicsProcess(double delta)
	{
		UpdateTransforms();
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
