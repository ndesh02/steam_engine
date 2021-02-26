using Godot;
using System;

public class Main2 : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    //Nodepaths
    [Export] NodePath pipePath;
    [Export] NodePath largeRodPipePath;
    [Export] NodePath largeRodLimiterPath;
    [Export] NodePath smallRodPipePath;
    [Export] NodePath smallRodLimiterPath;
    [Export] NodePath wheelPath;
    [Export] NodePath limiterPath;
    [Export] NodePath steamInTankPath;
    [Export] NodePath steamInPipePath1;
    [Export] NodePath steamInPipePath2;
    [Export] NodePath steamInPipePath3;
    [Export] NodePath collarPath;
    [Export] NodePath leftUpperRodPath;
    [Export] NodePath leftLowerRodPath;
    [Export] NodePath leftSpherePath;
    [Export] NodePath rightUpperRodPath;
    [Export] NodePath rightLowerRodPath;
    [Export] NodePath rightSpherePath;
    [Export] NodePath centerRodPath;


    //Instance all the objects for the engine
    public RigidBody2D pipe;
    public RigidBody2D largeRodPipe;
    public RigidBody2D smallRodPipe;
    public RigidBody2D largeRodLimiter;
    public RigidBody2D smallRodLimiter;
    public RigidBody2D wheel;
    public RigidBody2D limiter;
    public Particles2D steamInTank;
    public Particles2D steamInPipe1;
    public Particles2D steamInPipe2;
    public Particles2D steamInPipe3;
    public ParticlesMaterial tankSteam;

    //Instancing all of the governor objects
    public RigidBody2D rightSphere;
    public RigidBody2D leftSphere;
    public RigidBody2D rightUpperRod;
    public RigidBody2D leftUpperRod;
    public RigidBody2D rightLowerRod;
    public RigidBody2D leftLowerRod;
    public RigidBody2D collar;
    public RigidBody2D centerRod;

    //Steam values
    Vector2 steamForce = new Vector2(30,0);
    //PV = NRT

    //pressure
    public float P;
    //volume
    public float V;
    //amount
    public float RN;
    public float LN;
    public float flowRate = 1;
    public float flowRateMagnifier = 10;
    public float exitRate;
    //ideal gas constant
    public float R = (float)8.314;
    //temperature in kelvin
    public float T = 298;
    //Breaking up the tank into subsections (width, height)
    public float pipeSectionL;
    public float pipeSectionR;
    public float restOfV;

    //Direction and position variables of some of the objects
    public bool pipeRight = false;
    public int pipeY;
    public int limiterY;
    public int largeRodLimiterY;
    public int largeRodPipeY;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //Setting the objects to the correct nodepath
        pipe = GetNode<RigidBody2D>(pipePath) as RigidBody2D;
        largeRodPipe = GetNode<RigidBody2D>(largeRodPipePath) as RigidBody2D;
        smallRodPipe = GetNode<RigidBody2D>(smallRodPipePath) as RigidBody2D;
        largeRodLimiter = GetNode<RigidBody2D>(largeRodLimiterPath) as RigidBody2D;
        smallRodLimiter = GetNode<RigidBody2D>(smallRodLimiterPath) as RigidBody2D;
        wheel = GetNode<RigidBody2D>(wheelPath) as RigidBody2D;
        limiter = GetNode<RigidBody2D>(limiterPath) as RigidBody2D;
        steamInTank = GetNode<Particles2D>(steamInTankPath) as Particles2D;
        steamInPipe1 = GetNode<Particles2D>(steamInPipePath1) as Particles2D;
        steamInPipe2 = GetNode<Particles2D>(steamInPipePath2) as Particles2D;
        steamInPipe3 = GetNode<Particles2D>(steamInPipePath3) as Particles2D;
        tankSteam = ResourceLoader.Load("res://Assets/Scenes/particlesTank.tres") as ParticlesMaterial;
        rightUpperRod = GetNode<RigidBody2D>(rightUpperRodPath) as RigidBody2D;
        rightLowerRod = GetNode<RigidBody2D>(rightLowerRodPath) as RigidBody2D;
        leftUpperRod = GetNode<RigidBody2D>(leftUpperRodPath) as RigidBody2D;
        leftLowerRod = GetNode<RigidBody2D>(leftLowerRodPath) as RigidBody2D;
        rightSphere = GetNode<RigidBody2D>(rightSpherePath) as RigidBody2D;
        leftSphere = GetNode<RigidBody2D>(leftSpherePath) as RigidBody2D;
        collar = GetNode<RigidBody2D>(collarPath) as RigidBody2D;
        centerRod = GetNode<RigidBody2D>(centerRodPath) as RigidBody2D;
        

        steamInPipe1.Amount = (int)(flowRate*flowRateMagnifier);
        steamInPipe2.Amount = (int)(flowRate*flowRateMagnifier);
        steamInPipe3.Amount = (int)(flowRate*flowRateMagnifier);

        pipeY = (int)pipe.Position.y;
        limiterY = (int)limiter.Position.y;
        largeRodLimiterY = (int)largeRodLimiter.Position.y;
        largeRodPipeY = (int)largeRodPipe.Position.y;
        restOfV = 262*125;
        
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {

        //Code that calculates the steam force based on the area and the location of the pipe and limiter
        if(limiter.Position.x <386){
            RN = RN +(float) Math.Pow(2,flowRate);
            LN=0;
        }
        if(limiter.Position.x >=386){
            LN=LN + (float) Math.Pow(2, flowRate);
            RN=0;
        }

        //p = nrt/v
        //volume calculations
        pipeSectionL = (760 - pipe.Position.x)*125;
        pipeSectionR = (pipe.Position.x - 520)*125;

        if(RN-LN>0){
            V = restOfV +pipeSectionR;
        }
        if(RN-LN<0){
            V = restOfV + pipeSectionL;
        }
        P = (RN-LN)*R*T/V;
        GD.Print(P);
        steamForce = new Vector2(P, 0);

        if(P>0){
            
        }
        if(P<0){

        }

        //Code that makes all the pieces move according to the steam force
        smallRodLimiter.AngularVelocity = smallRodPipe.AngularVelocity;
       /* if(pipe.Position.x <535 && pipeRight==false){
            pipeRight = true;
            pipe.LinearVelocity = new Vector2(0,0);
            pipe.AppliedForce = new Vector2(0,0);
            limiter.LinearVelocity = new Vector2(0,0);
            iteration++;

        }
        if(pipe.Position.x > 750 && pipeRight==true){
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
        }*/
        pipe.AppliedForce = steamForce;
        pipe.LinearVelocity = new Vector2(pipe.LinearVelocity.x, 0);
        
        largeRodLimiter.LinearVelocity = new Vector2(largeRodLimiter.LinearVelocity.x, 0);
        limiter.RotationDegrees = 0;
        pipe.RotationDegrees = 0;
        pipe.AngularVelocity = 0;
        
        wheel.AngularVelocity = smallRodPipe.AngularVelocity;
        smallRodLimiter.LinearVelocity = new Vector2(0,0);
        wheel.LinearVelocity = new Vector2(0,0);
        smallRodPipe.LinearVelocity = new Vector2(0,0);

        limiter.Position = new Vector2(limiter.Position.x, limiterY);
        pipe.Position = new Vector2(pipe.Position.x, pipeY);
        largeRodLimiter.Position = new Vector2(largeRodLimiter.Position.x, largeRodLimiterY);
        largeRodPipe.Position = new Vector2(largeRodPipe.Position.x, largeRodPipeY);

        //Governor physics


    }
    public void _on_HSlider_value_changed(float value){
        flowRate = value;
        steamInPipe1.Amount = (int)(value*flowRateMagnifier);
        steamInPipe2.Amount = (int)(value*flowRateMagnifier);
        steamInPipe3.Amount = (int)(value*flowRateMagnifier);
    }
}
