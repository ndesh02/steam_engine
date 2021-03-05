using Godot;
using System;

public class Governor : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export] NodePath collarPath;
    [Export] NodePath leftUpperRodPath;
    [Export] NodePath leftLowerRodPath;
    [Export] NodePath leftSpherePath;
    [Export] NodePath rightUpperRodPath;
    [Export] NodePath rightLowerRodPath;
    [Export] NodePath rightSpherePath;
    [Export] NodePath centerRodPath;
    [Export] NodePath governorLowerLeftJointPath;
    [Export] NodePath governorLowerRightJointPath;

    public static RigidBody2D rightSphere;
    public static RigidBody2D leftSphere;
    public static RigidBody2D rightUpperRod;
    public static RigidBody2D leftUpperRod;
    public static RigidBody2D rightLowerRod;
    public static RigidBody2D leftLowerRod;
    public static RigidBody2D collar;
    public static RigidBody2D centerRod;
    public static RigidBody2D lid;
    public static RigidBody2D lidLifter;
    public static PinJoint2D governorLowerLeftJoint;
    public static PinJoint2D governorLowerRightJoint;
    public static float governorLowerLeftJointInitialY;
    public static float governorLowerRightJointInitialY;
     public static float changeInCollarHeight = 0;
    public static float initialCollarHeight;
    public static float collarX;
    public static float force;
    public static float radius;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        rightUpperRod = GetNode<RigidBody2D>(rightUpperRodPath) as RigidBody2D;
        rightLowerRod = GetNode<RigidBody2D>(rightLowerRodPath) as RigidBody2D;
        leftUpperRod = GetNode<RigidBody2D>(leftUpperRodPath) as RigidBody2D;
        leftLowerRod = GetNode<RigidBody2D>(leftLowerRodPath) as RigidBody2D;
        rightSphere = GetNode<RigidBody2D>(rightSpherePath) as RigidBody2D;
        leftSphere = GetNode<RigidBody2D>(leftSpherePath) as RigidBody2D;
        collar = GetNode<RigidBody2D>(collarPath) as RigidBody2D;
        centerRod = GetNode<RigidBody2D>(centerRodPath) as RigidBody2D;
        governorLowerLeftJoint = GetNode<PinJoint2D>(governorLowerLeftJointPath) as PinJoint2D;
        governorLowerRightJoint = GetNode<PinJoint2D>(governorLowerRightJointPath) as PinJoint2D;
        initialCollarHeight = collar.Position.y;
        collarX = collar.Position.x;
        governorLowerLeftJointInitialY = governorLowerLeftJoint.Position.y;
        governorLowerRightJointInitialY = governorLowerRightJoint.Position.y;
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
    public static void stopGovernorWeirdness(){
        collar.AppliedForce = new Vector2(0, collar.AppliedForce.y);
        collar.RotationDegrees = 0;
        collar.AngularVelocity = 0;
        collar.LinearVelocity = new Vector2(0, collar.LinearVelocity.y);
        collar.AppliedTorque = 0;
        collar.Position = new Vector2(collarX, collar.Position.y);
        changeInCollarHeight = collar.Position.y - initialCollarHeight;
        liftLid();

        governorLowerLeftJoint.Position = new Vector2(governorLowerLeftJoint.Position.x, governorLowerLeftJointInitialY + changeInCollarHeight);
        governorLowerRightJoint.Position = new Vector2(governorLowerRightJoint.Position.x, governorLowerRightJointInitialY + changeInCollarHeight);

        leftLowerRod.Position = governorLowerLeftJoint.Position;
        rightLowerRod.Position = governorLowerRightJoint.Position;
    }
    public static void governorPhysics(){
        radius = Math.Abs(collar.Position.x - rightSphere.Position.x);
        //this is the line that calculates the force
        force = (float)((rightSphere.Weight/9.81)*radius*Main2.wheel.AngularVelocity);
        leftSphere.AppliedForce = new Vector2(0,-force);
        rightSphere.AppliedForce = new Vector2(0, -force);
    }
    public static void liftLid(){
        changeInCollarHeight = collar.Position.y - initialCollarHeight;
        Main2.lid.Position = new Vector2(Main2.lid.Position.x, Main2.lidHeight + changeInCollarHeight);
        Main2.lidLifter.Position = new Vector2(Main2.lidLifter.Position.x, Main2.lidLifterHeight + changeInCollarHeight);
    }
    
}
