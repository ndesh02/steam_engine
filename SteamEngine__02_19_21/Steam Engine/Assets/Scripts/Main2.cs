using Godot;
using System;

public class Main2 : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export] NodePath pipePath;
    [Export] NodePath largeRodPipePath;
    [Export] NodePath largeRodLimiterPath;
    [Export] NodePath smallRodPipePath;
    [Export] NodePath smallRodLimiterPath;
    [Export] NodePath wheelPath;
    [Export] NodePath limiterPath;
    [Export] NodePath steamInTankPath;

    public RigidBody2D pipe;
    public RigidBody2D largeRodPipe;
    public RigidBody2D smallRodPipe;
    public RigidBody2D largeRodLimiter;
    public RigidBody2D smallRodLimiter;
    public RigidBody2D wheel;
    public RigidBody2D limiter;
    public Particles2D steamInTank;
    Vector2 steamForce = new Vector2(30,0);
    public bool right = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        pipe = GetNode<RigidBody2D>(pipePath) as RigidBody2D;
        largeRodPipe = GetNode<RigidBody2D>(largeRodPipePath) as RigidBody2D;
        smallRodPipe = GetNode<RigidBody2D>(smallRodPipePath) as RigidBody2D;
        largeRodLimiter = GetNode<RigidBody2D>(largeRodLimiterPath) as RigidBody2D;
        smallRodLimiter = GetNode<RigidBody2D>(smallRodLimiterPath) as RigidBody2D;
        wheel = GetNode<RigidBody2D>(wheelPath) as RigidBody2D;
        limiter = GetNode<RigidBody2D>(limiterPath) as RigidBody2D;
        steamInTank = GetNode<Particles2D>(steamInTankPath) as Particles2D;
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        float pipeXSpeed = 0;

        if(pipe.Position.x <539 && right==false){
            right = true;
            pipe.LinearVelocity = new Vector2(0,0);
            pipe.AppliedForce = new Vector2(0,0);
            limiter.LinearVelocity = new Vector2(0,0);
            GD.Print(smallRodLimiter.RotationDegrees);
        }
        if(pipe.Position.x > 750 && right==true){
            right = false;
            pipe.LinearVelocity = new Vector2(0,0);
            pipe.AppliedForce = new Vector2(0,0);
            limiter.LinearVelocity = new Vector2(0,0);
            GD.Print(smallRodLimiter.RotationDegrees);

        }
        if(right==true){
            pipe.AppliedForce = steamForce;

            pipe.AngularVelocity = 0;
            pipeXSpeed = pipe.LinearVelocity.x;
            pipe.LinearVelocity = new Vector2(pipeXSpeed, 0);

            limiter.AngularVelocity = 0;
            limiter.Position = new Vector2(limiter.Position.x, 621);
            limiter.RotationDegrees = 0;
          //  smallRodLimiter.AngularVelocity = smallRodPipe.AngularVelocity;
         //   limiter.LinearVelocity = new Vector2(limiter.LinearVelocity.x,0);
            smallRodLimiter.RotationDegrees = smallRodPipe.RotationDegrees - 90;
           // limiter.LinearVelocity = - new Vector2((float)pipe.LinearVelocity.x,0);
        }
        if(right==false){
            pipe.AppliedForce = -steamForce;

            pipe.AngularVelocity = 0;
            pipeXSpeed = pipe.LinearVelocity.x;
            pipe.LinearVelocity = new Vector2(pipeXSpeed, 0);

            limiter.AngularVelocity = 0;
            limiter.Position = new Vector2(limiter.Position.x, 621);
            limiter.RotationDegrees = 0;
       //     smallRodLimiter.AngularVelocity = smallRodPipe.AngularVelocity;
            smallRodLimiter.RotationDegrees = smallRodPipe.RotationDegrees - 90;
            limiter.LinearVelocity = new Vector2(limiter.LinearVelocity.x,0);
           // limiter.LinearVelocity = - new Vector2((float)pipe.LinearVelocity.x,0);
        }

        
    }
}
