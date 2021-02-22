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
    public bool pipeRight = false;
    public bool limiterRight = false;
    public int pipeY;
    public int limiterY;
    public int largeRodLimiterY;
    public int largeRodPipeY;
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
        pipeY = (int)pipe.Position.y;
        limiterY = (int)limiter.Position.y;
        largeRodLimiterY = (int)largeRodLimiter.Position.y;
        largeRodPipeY = (int)largeRodPipe.Position.y;
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        largeRodLimiter.LinearVelocity = new Vector2(largeRodLimiter.LinearVelocity.x, 0);
        limiter.RotationDegrees = 0;
        pipe.RotationDegrees = 0;
        pipe.AngularVelocity = 0;

        
        wheel.AngularVelocity = smallRodPipe.AngularVelocity;
       // smallRodLimiter.AngularVelocity = -wheel.AngularVelocity;
        smallRodLimiter.LinearVelocity = new Vector2(0,0);
        wheel.LinearVelocity = new Vector2(0,0);
        smallRodPipe.LinearVelocity = new Vector2(0,0);

        limiter.Position = new Vector2(limiter.Position.x, limiterY);
        pipe.Position = new Vector2(pipe.Position.x, pipeY);
        largeRodLimiter.Position = new Vector2(largeRodLimiter.Position.x, largeRodLimiterY);
        largeRodPipe.Position = new Vector2(largeRodPipe.Position.x, largeRodPipeY);


        if(limiter.Position.x <334){
            limiterRight = true;
        }
        if(limiter.Position.x > 390){
            limiterRight = false;
        }
        if(pipe.Position.x <538 && pipeRight==false){
            pipeRight = true;
            pipe.LinearVelocity = new Vector2(0,0);
            pipe.AppliedForce = new Vector2(0,0);
            limiter.LinearVelocity = new Vector2(0,0);
        }
        if(pipe.Position.x > 752 && pipeRight==true){
            pipeRight = false;
            pipe.LinearVelocity = new Vector2(0,0);
            pipe.AppliedForce = new Vector2(0,0);
            limiter.LinearVelocity = new Vector2(0,0);

        }
        if(pipeRight==true){
            pipe.AppliedForce = steamForce;
            pipe.LinearVelocity = new Vector2(pipe.LinearVelocity.x, 0);
        }
        if(pipeRight==false){
            pipe.AppliedForce = -steamForce;
            pipe.LinearVelocity = new Vector2(pipe.LinearVelocity.x, 0);
        }
        if(limiterRight == true){
            limiter.LinearVelocity =  new Vector2(Math.Abs(limiter.LinearVelocity.x), 0);
        }
        if(limiterRight == false){
            limiter.LinearVelocity =  - new Vector2(Math.Abs(limiter.LinearVelocity.x),0);
        }


        
    }
}
