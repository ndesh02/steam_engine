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
      iteration++;
      Vector2 steamForce = new Vector2(-1,0);
     /* if(iteration>=10&&iteration<=20){
        largeRodPipe.AddForce(new Vector2(-200,-67), new Vector2(10,0));
        pipe.AddCentralForce(new Vector2(-10,0));
      // largeRodPipe.AddCentralForce(new Vector2(100,0));
        GD.Print("pipeForce "+pipe.AppliedForce);
        GD.Print("rodForce "+largeRodPipe.AppliedForce);
      }*/
      if(pipe.Position.x < 539){
        pipe.AppliedForce = new Vector2(0,0);
        GD.Print("here");
      }
      else{
        pipe.AddCentralForce(steamForce);
      }
      largeRodPipe.LinearVelocity = pipe.LinearVelocity;
      largeRodPipe.AngularVelocity=2;
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
