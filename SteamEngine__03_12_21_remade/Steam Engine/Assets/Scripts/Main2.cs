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
    [Export] NodePath tension1LabelPath;
    [Export] NodePath tension2LabelPath;
    [Export] NodePath thetaLabelPath;
    [Export] NodePath wheelVelLabelPath;
    [Export] NodePath pipeLargeRodJointPath;


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
    public static Label tension1Label;
    public static Label tension2Label;
    public static Label thetaLabel;
    public static Label wheelVelLabel;
    public static PinJoint2D pipeLargeRodJoint;

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
    public static float flowRate = 2;
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
    public static float pipeLargeRodJointY;

    public static float lidInitialHeight;
    public static float lidLifterInitialHeight;
    public static Color white = Color.Color8(255,255,255,255);
    public static Color lightRed = Color.Color8(222,155,155,255);
    public static Color darkRed = Color.Color8(255,0,0,255); 
    public static float maxRotation=0;
    public static int iteration=0;
    public static bool left=false;
    public static bool switching =false;
    public static float sumChange=0;
    public static float length1;
    public static float length2;
    public static float radiusRod;
    public static float radiusWheel;
    public static float alpha;
    public static float beta;
    public static float angularA;
    public static float angularSpeed;
    public static float torque;
    public static float I; //moment of inertia
    public static float theta;
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
        pipeLargeRodJoint = GetNode<PinJoint2D>(pipeLargeRodJointPath) as PinJoint2D;
        
        lid = GetNode<RigidBody2D>(lidPath) as RigidBody2D;
        lidLifter = GetNode<RigidBody2D>(lidLifterPath) as RigidBody2D;
        steamInputRate = GetNode<Label>(steamInputRatePath) as Label;
        tension1Label = GetNode<Label>(tension1LabelPath) as Label;
        tension2Label = GetNode<Label>(tension2LabelPath) as Label;
        thetaLabel = GetNode<Label>(thetaLabelPath) as Label;
        wheelVelLabel = GetNode<Label>(wheelVelLabelPath) as Label;
        
        wheelX = wheel.Position.x;
        wheelY = wheel.Position.y;

        pipeLargeRodJointY = pipeLargeRodJoint.Position.y;

        
        lidInitialHeight = lid.Position.y;
        lidLifterInitialHeight = lidLifter.Position.y;
        

        steamInPipe1.Amount = (int)(flowRate*flowRateMagnifier);
        steamInPipe2.Amount = (int)(flowRate*flowRateMagnifier);
        steamInPipe3.Amount = (int)(flowRate*flowRateMagnifier);

        pipeY = (int)pipe.Position.y;
        limiterY = (int)limiter.Position.y;
        largeRodLimiterY = (int)largeRodLimiter.Position.y;
        largeRodPipeY = (int)largeRodPipe.Position.y;
        restOfV = 262*125;
        
        //initializing steam engine values for physics
        radiusRod = (float)114.0175;
        //length1 = 
        length2 = 458;
        alpha = -smallRodPipe.RotationDegrees;//in degrees
        beta = 0;
        length1 = (float)(Math.Sqrt(Math.Pow(radiusRod,2)+Math.Pow(length2, 2)-2*length2*radiusRod*Math.Cos(alpha*Math.PI/180)));
        radiusWheel = 670;
    }

    public override void _PhysicsProcess(float delta)
    {

        calculatingPressure();
        //doing the steam engine physics calculations
      //  alpha = (float)(180-Math.Acos(((Math.Pow(length1,2)-Math.Pow(radiusRod,2)-Math.Pow(length2,2))/(2*length2*radiusRod))));
        alpha = -smallRodPipe.RotationDegrees;
        torque = (float)(P*Math.Cos(Math.PI/180*beta)*Math.Sin(alpha));
        I = (float)(wheel.Mass*Math.Pow(radiusWheel,2));
        angularA = torque/I;
      //  alpha = (float)(alpha * 180/Math.PI);
        angularSpeed = angularSpeed+angularA*delta*10000;
        theta = (float)(angularSpeed*delta + 1/2*angularA*Math.Pow(delta,2)*180/Math.PI);
        wheel.AngularVelocity = angularSpeed;
       //wheel.AppliedTorque = torque;
        smallRodPipe.AngularVelocity = wheel.AngularVelocity;
        
     /*   GD.Print("alpha: "+alpha);
        GD.Print("torque: "+torque);
        GD.Print("I: "+I);
        GD.Print("angularA: "+angularA);
        GD.Print("angularSpeed: "+angularSpeed);
        GD.Print("theta: "+theta);*/

        stopEngineWeirdness();
        steamForce = new Vector2(P, 0);     
        
      //  wheel.AngularVelocity = smallRodPipe.AngularVelocity;
       


        

        //Code that makes all the pieces move according to the steam force
        smallRodLimiter.AngularVelocity = smallRodPipe.AngularVelocity;
        //code that stops the jank
        stopEngineWeirdness();
        //Governor physics
        //calculating the force to apply to each sphere
        
        Governor.governorPhysics(wheel.AngularVelocity);
        Governor.stopGovernorWeirdness();
        Governor.liftLid(lid, lidLifter, lidInitialHeight, lidLifterInitialHeight);
        

        //colour of particles in tank
        changeSteamColor();
        
        //diagnostic
       
        wheelVelLabel.Text = "Wheel Angular Velocity: "+wheel.AngularVelocity*10;
        steamInputRate.Text = "Steam Input FLow Rate: "+flowRate;
        
    }
    
    public void calculatingPressure(){
        
        //Code that calculates the steam force based on the area and the location of the pipe and limiter
        if(limiter.Position.x <386){
            if(lidInitialHeight - lid.Position.y<25){
                // flowRate+=(float)0.01;
                flowRate = 4;
                if(RN<100)
                    RN = RN +(float)(Math.Pow(2,flowRate));
            }
            else{
                flowRate=(float)0;

            }
            if(LN>0){
                LN = LN -2;
            }
            if(LN<0){
                LN = 0;
            }
        }
        if(limiter.Position.x >=386){
            if(lidInitialHeight - lid.Position.y<25){
                // flowRate+=(float)0.01;
                flowRate = 4;
                if(LN<200)
                    LN=LN + (float)(Math.Pow(2, flowRate));
            }
            else{
                flowRate=(float)0;

            }
            if(RN>200){
                RN = RN -2;
            }
            if(RN<0)
                RN = 0;
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
       // GD.Print(largeRodPipe.LinearVelocity);
        pipe.LinearVelocity = new Vector2(pipe.LinearVelocity.x, 0);
        pipe.Position = new Vector2(pipe.Position.x, pipeY);
        pipe.AngularVelocity = 0;
        pipe.AppliedTorque = 0;
        pipe.RotationDegrees = 0;

       // GD.Print(largeRodPipe.LinearVelocity);
        largeRodPipe.LinearVelocity = new Vector2(largeRodPipe.LinearVelocity.x, 0);
      //  largeRodPipe.Position = new Vector2(largeRodPipe.Position.x, largeRodPipeY);
        
        pipeLargeRodJoint.Position = new Vector2(pipeLargeRodJoint.Position.x, pipeLargeRodJointY);
        //largeRodPipe.Position = new Vector2(largeRodPipe.Position.x, (float)(largeRodPipe.Position.y - 0.1*iteration));
        limiter.LinearVelocity = new Vector2(limiter.LinearVelocity.x, 0);
        limiter.Position = new Vector2(limiter.Position.x, limiterY);

        pipe.ForceUpdateTransform();
        largeRodPipe.ForceUpdateTransform();
        //largeRodPipe.RotationDegrees = largeRodPipe.RotationDegrees - iteration*(float)0.1;
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
