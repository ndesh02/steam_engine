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

    [Export] NodePath lidPath;
    [Export] NodePath lidLifterPath;
    [Export] NodePath steamInputRatePath;


    //Instance all the objects for the engine
    public static RigidBody2D pipe;
    public static RigidBody2D largeRodPipe;
    public static RigidBody2D smallRodPipe;
    public static RigidBody2D largeRodLimiter;
    public static RigidBody2D smallRodLimiter;
    public static RigidBody2D wheel;
    public static RigidBody2D limiter;
    public static Particles2D steamInTank;
    public static Particles2D steamInPipe1;
    public static Particles2D steamInPipe2;
    public static Particles2D steamInPipe3;
    public static ParticlesMaterial tankSteam;

    //Instancing all of the governor objects
    public static RigidBody2D lid;
    public static RigidBody2D lidLifter;
    public static Label steamInputRate;


    //Steam values
    Vector2 steamForce = new Vector2(0,0);
    //PV = NRT

    //pressure
    public static float P;
    //volume
    public static float V;
    //amount
    public static float RN;
    public static float LN;
    public static float flowRate = 1;
    public static float flowRateMagnifier = 10;
    //ideal gas constant
    public static float R = (float)8.314;
    //temperature in kelvin
    public static float T = 298;
    //Breaking up the tank into subsections (width, height)
    public static float pipeSectionL;
    public static float pipeSectionR;
    public static float restOfV;
    //Direction and position variables of some of the objects
    public static int pipeY;
    public static int limiterY;
    public static int largeRodLimiterY;
    public static int largeRodPipeY;
    public static float wheelY;
    public static float wheelX;



    public static float lidHeight;
    public static float lidLifterHeight;
    public static Color white = Color.Color8(255,255,255,255);
    public static Color lightRed = Color.Color8(222,155,155,255);
    public static Color darkRed = Color.Color8(255,0,0,255); 
    public static float maxRotation=0;
    public static int iteration=0;
    public static bool left=false;
    public static bool switching =false;
    public static float sumChange=0;

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
        
        lid = GetNode<RigidBody2D>(lidPath) as RigidBody2D;
        lidLifter = GetNode<RigidBody2D>(lidLifterPath) as RigidBody2D;
        steamInputRate = GetNode<Label>(steamInputRatePath) as Label;
       
        
        wheelX = wheel.Position.x;
        wheelY = wheel.Position.y;
        
        lidHeight = lid.Position.y;
        lidLifterHeight = lidLifter.Position.y;
        

        steamInPipe1.Amount = (int)(flowRate*flowRateMagnifier);
        steamInPipe2.Amount = (int)(flowRate*flowRateMagnifier);
        steamInPipe3.Amount = (int)(flowRate*flowRateMagnifier);

        pipeY = (int)pipe.Position.y;
        limiterY = (int)limiter.Position.y;
        largeRodLimiterY = (int)largeRodLimiter.Position.y;
        largeRodPipeY = (int)largeRodPipe.Position.y;
        restOfV = 262*125;
        
        
    }

    public override void _PhysicsProcess(float delta)
    {
    //    GD.Print(lidHeight - lid.Position.y);
        calculatingPressure();
        steamForce = new Vector2(P, 0);

        if(left==true && switching==true){
            iteration++;
            GD.Print(iteration);
            switching = false;
            sumChange=0;
        }
        //Code that makes all the pieces move according to the steam force
        smallRodLimiter.AngularVelocity = smallRodPipe.AngularVelocity;
        //code that stops the jank
        stopEngineWeirdness();
        
        //Governor physics
        //calculating the force to apply to each sphere
        Governor.governorPhysics();
        
        //code to keep the governor stable since can't put axis locks on 2d joints
        Governor.stopGovernorWeirdness();

        //colour of particles in tank
        changeSteamColor();
        
        //diagnostic
        if(largeRodPipe.RotationDegrees > maxRotation){
           // maxRotation = largeRodPipe.RotationDegrees;
            sumChange +=maxRotation - largeRodPipe.RotationDegrees;
            maxRotation = largeRodPipe.RotationDegrees;
            GD.Print(sumChange);
        }

        steamInputRate.Text = "Steam Input FLow Rate: "+flowRate;
        
    }
    
    public void calculatingPressure(){
        
        //Code that calculates the steam force based on the area and the location of the pipe and limiter
        if(limiter.Position.x <386){
            if(lidHeight - lid.Position.y<48){
                flowRate+=(float)0.01;
                RN = RN +(float)(Math.Pow(2,flowRate));
            }
            else{
                flowRate = (float)0.1;
            }
            LN=0;
        }
        if(limiter.Position.x >=386){
            if(lidHeight - lid.Position.y<48){
                flowRate+=(float)0.01;
                LN=LN + (float)(Math.Pow(2, flowRate));
            }
            else{
                flowRate = (float)0.1;
            }   
            RN=0;
        }
        //p = nrt/v
        //volume calculations
        pipeSectionL = (760 - pipe.Position.x)*125;
        pipeSectionR = (pipe.Position.x - 520)*125;
        
        if(RN-LN>0){
            V = restOfV +pipeSectionR;
            if(left==false){
                switching = true;
                left = true;
            }
            
        }
        if(RN-LN<0){
            V = restOfV + pipeSectionL;
            if(left==true){
                switching = true;
                left = false;
            }
        }
        P = (RN-LN)*R*T/V;
    }
    
    
    
    
    public void stopEngineWeirdness(){
        pipe.AppliedForce = steamForce;
        pipe.LinearVelocity = new Vector2(pipe.LinearVelocity.x, 0);
        
        largeRodLimiter.LinearVelocity = new Vector2(largeRodLimiter.LinearVelocity.x, 0);
        largeRodPipe.LinearVelocity = new Vector2(largeRodPipe.LinearVelocity.x, 0);

        limiter.RotationDegrees = 0;
        pipe.RotationDegrees = 0;
        
        pipe.AngularVelocity = 0;
        
        wheel.AngularVelocity = smallRodPipe.AngularVelocity;
        smallRodLimiter.LinearVelocity = new Vector2(0,0);
        wheel.LinearVelocity = new Vector2(0,0);
        smallRodPipe.LinearVelocity = new Vector2(0,0);
        wheel.Position = new Vector2(wheelX, wheelY);
        limiter.Position = new Vector2(limiter.Position.x, limiterY);
        pipe.Position = new Vector2(pipe.Position.x, pipeY);

        largeRodLimiter.Position = new Vector2(largeRodLimiter.Position.x, largeRodLimiterY);
        largeRodPipe.Position = new Vector2(largeRodPipe.Position.x, largeRodPipeY);
        largeRodPipe.AppliedTorque = 0;
        largeRodPipe.RotationDegrees = largeRodPipe.RotationDegrees - iteration*(float)0.128;
    }
    public void changeSteamColor(){
        if(Math.Abs(P)<7){
          tankSteam.Color = white;
        }
        else if(Math.Abs(P)<12){
          tankSteam.Color = lightRed;
        }
        else{
          tankSteam.Color = darkRed;
        }
        
    }
    public void _on_HSlider_value_changed(float value){
        flowRate = value;
        steamInPipe1.Amount = (int)(value*flowRateMagnifier);
        steamInPipe2.Amount = (int)(value*flowRateMagnifier);
        steamInPipe3.Amount = (int)(value*flowRateMagnifier);
    }
}
