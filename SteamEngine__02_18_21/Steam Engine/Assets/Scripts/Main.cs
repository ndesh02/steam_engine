using Godot;
using System;

public class Main : Node2D
{
//[Export] NodePath tankPath;
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
public int iteration=0;

public bool right = false;



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

//  Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _PhysicsProcess(float delta)
  {
      
      Vector2 steamForce = new Vector2(-1,0);
        if(pipe.Position.x > 750 && right==true){
          right = false;
          pipe.LinearVelocity = new Vector2(0,0);
          pipe.AppliedForce = new Vector2(0,0);
          largeRodPipe.AngularVelocity = 0;
          largeRodPipe.LinearVelocity = new Vector2(0,0);
          smallRodPipe.AngularVelocity = 0;
          limiter.LinearVelocity = new Vector2(0,0);
          largeRodLimiter.LinearVelocity = new Vector2(0,0);
          largeRodLimiter.AngularVelocity = 0;
          smallRodLimiter.AngularVelocity = 0;
        }
        if(pipe.Position.x < 539 && right==false){
          right = true;
          pipe.LinearVelocity = new Vector2(0,0);
          pipe.AppliedForce = new Vector2(0,0);
          largeRodPipe.AngularVelocity = 0;
          largeRodPipe.LinearVelocity = new Vector2(0,0);
          smallRodPipe.AngularVelocity = 0;
          limiter.LinearVelocity = new Vector2(0,0);
          largeRodLimiter.LinearVelocity = new Vector2(0,0);
          largeRodLimiter.AngularVelocity = 0;
          smallRodLimiter.AngularVelocity = 0;
          iteration++;
        }
        if(right==false){
          pipe.AddCentralForce(steamForce);
          largeRodPipe.LinearVelocity = pipe.LinearVelocity;
          if(largeRodPipe.Position.x > 826){
            largeRodPipe.AngularVelocity = -(float)0.2;
          }else{
            largeRodPipe.AngularVelocity = (float)0.2;
          }
          largeRodPipe.RotationDegrees = largeRodPipe.RotationDegrees +((float)0.38255)*(iteration-1);
          smallRodPipe.AngularVelocity = -(float)0.0082*Math.Abs(pipe.LinearVelocity.x)-(float)0.53;
          limiter.LinearVelocity = -pipe.LinearVelocity;
          largeRodLimiter.LinearVelocity = limiter.LinearVelocity;
         /* largeRodLimiter.AngularVelocity = -(float)0.1;
          smallRodLimiter.AngularVelocity = (float)0.4;*/
        }
        if(right==true){
          pipe.AddCentralForce(-steamForce);
          largeRodPipe.LinearVelocity = pipe.LinearVelocity;
          if(largeRodPipe.Position.x > 826){
            largeRodPipe.AngularVelocity = -(float)0.2;
          }else{
            largeRodPipe.AngularVelocity = (float)0.2;
          }
          largeRodPipe.RotationDegrees = largeRodPipe.RotationDegrees +((float)0.38255)*(iteration-1);
          smallRodPipe.AngularVelocity = -(float)0.0082*Math.Abs(pipe.LinearVelocity.x)-(float)0.53;
          limiter.LinearVelocity = -pipe.LinearVelocity;
          largeRodLimiter.LinearVelocity = limiter.LinearVelocity;
         /* largeRodLimiter.AngularVelocity = (float)0.1;
          smallRodLimiter.AngularVelocity = -(float)0.4;*/
        }

      
      
  }

  public void testing(){
    largeRodPipe.AddForce(new Vector2(-200,-67), new Vector2(10,0));
    pipe.AddCentralForce(new Vector2(-10,0));
  // largeRodPipe.AddCentralForce(new Vector2(100,0));
    GD.Print("pipeForce "+pipe.AppliedForce);
    GD.Print("rodForce "+largeRodPipe.AppliedForce);
  }
  public void _on_Timer_timeout(){
  //  testing();
  }
}
