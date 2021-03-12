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
    public static float tension1;
    public static float tension2;
    public static float theta;
    public static float length;
    public static float g = (float)9.81;
    public static float changeLidHeight;
    public static float thetaPredicted;

    public float angularSpeed;
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
        collar.Position = new Vector2(collar.Position.x, collar.Position.y-50);
        initialCollarHeight = collar.Position.y;
        collarX = collar.Position.x;
        governorLowerLeftJointInitialY = governorLowerLeftJoint.Position.y;
        governorLowerRightJointInitialY = governorLowerRightJoint.Position.y;
        theta = 90+rightUpperRod.RotationDegrees; //check to see if this is correct
     //   GD.Print(theta);
 
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
    public static void stopGovernorWeirdness(){
        /*collar.AppliedForce = new Vector2(0, collar.AppliedForce.y);
        collar.RotationDegrees = 0;
        collar.AngularVelocity = 0;
        collar.LinearVelocity = new Vector2(0, collar.LinearVelocity.y);
        collar.AppliedTorque = 0;
        collar.Position = new Vector2(collarX, collar.Position.y);
        changeInCollarHeight = collar.Position.y - initialCollarHeight;*/
        //this is dependant on your system - your lids and lidLifter in the main scene or script
        

        governorLowerLeftJoint.Position = new Vector2(governorLowerLeftJoint.Position.x, governorLowerLeftJointInitialY + changeInCollarHeight-50);
        governorLowerRightJoint.Position = new Vector2(governorLowerRightJoint.Position.x, governorLowerRightJointInitialY + changeInCollarHeight-50);

        leftLowerRod.Position = governorLowerLeftJoint.Position;
        rightLowerRod.Position = governorLowerRightJoint.Position;
    }
    public static void governorPhysics(float wheelAngularVelocity){
        radius = Math.Abs(centerRod.Position.x - rightSphere.Position.x);
        //physics calculations go here
        tension2 = (float)(collar.Mass*g/(Math.Sin(theta*Math.PI/180)));
        tension1 = (1+(rightSphere.Mass/collar.Mass))*tension2;
        thetaPredicted = (float)(Math.Asin(collar.Mass*g/tension2)*180/Math.PI);
        Main2.tension1Label.Text = "Tension 1: "+Math.Round(tension1);
        Main2.tension2Label.Text = "Tension 2: "+Math.Round(tension2);
        Main2.thetaLabel.Text = "Theta: "+Math.Round(thetaPredicted);
        theta = 90+rightUpperRod.RotationDegrees;
    //    GD.Print(theta+" "+thetaPredicted);

        changeInCollarHeight = (float)(((rightSphere.Mass + collar.Mass)/rightSphere.Mass)*g/Math.Pow(wheelAngularVelocity,2))*5;

        //now move everything in accordance to the theta value

        if(changeInCollarHeight>60){
            changeInCollarHeight=60;
        }
        //GD.Print(initialCollarHeight+changeInCollarHeight+" "+wheelAngularVelocity);
        collar.Position = new Vector2(collar.Position.x, initialCollarHeight+changeInCollarHeight);

        //this is the line that calculates the force
       /* force = (float)((rightSphere.Mass)*radius*wheelAngularVelocity);
        leftSphere.AppliedForce = new Vector2(0,-force);
        rightSphere.AppliedForce = new Vector2(0, -force);*/
    }
    public static void liftLid(RigidBody2D lid, RigidBody2D lidLifter, float initialLidHeight, float initialLidLifterHeight){
        changeLidHeight = collar.Position.y - initialCollarHeight;
        //change these lid objects in accordance with your own steam supply
        lid.Position = new Vector2(lid.Position.x, initialLidHeight + changeLidHeight-50);
        lidLifter.Position = new Vector2(lidLifter.Position.x, initialLidLifterHeight + changeLidHeight-50);
    }
    
}
