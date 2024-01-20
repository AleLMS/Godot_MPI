#if TOOLS
using Godot;
using System;

[Tool]
public partial class mpiplugin : EditorPlugin
{
	public override void _EnterTree()
	{
		// Initialization of the plugin goes here.
		var script = GD.Load<Script>("res://addons/meshphysicsinterpolation/meshinterpolation.cs");
		var texture = GD.Load<Texture2D>("res://addons/meshphysicsinterpolation/icon2.png");
		AddCustomType("Mesh Interpolation", "Node", script, texture);
	}

	public override void _ExitTree()
	{
		// Clean-up of the plugin goes here.
		RemoveCustomType("Mesh Interpolation");
	}
}
#endif
