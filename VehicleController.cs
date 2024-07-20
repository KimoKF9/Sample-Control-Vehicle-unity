// is controller free 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class VehicleController : MonoBehaviour
{

    [Header("Basic Entries")]
    [Range(-1,1)]
	public float steerInput = 0.0f;
    [Range(0,1)]
	public float throttle;
    [Range(0,1)]
	public float brakeInput = 0.0f;
    [Range(0,1)]
	public float handbrakeInput = 0.0f;
    [Header("Main Entries")]
    //public float Torque;
    public float steering;
    [Header("Full Setup Vehicle")]
    public EngineVehicle Engine;
    public TransmissionVehicle Transmission;
    public SuspensionVehicle Suspension;
    public WC WheelCollider;
	public WT Transform;
    public EffectVehicle Effect;
    
    public void Start(){
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass= CenterOfMass.transform.localPosition;
        engineSound = GetComponent<AudioSource>();
       }
    
    public void Update(){
        Engine_();
        PhysicsWheel();
        DriveType();
        GearControl();
    }
    private void FixedUpdate() {
         Engine_();
    }
        #region Controle Moblie
          //controle
            
        #endregion

        #region PhysicsWheel
      void PhysicsWheel(){
         float maxstreeingAngle = 30f;
            steering = maxstreeingAngle * Input.GetAxis("Horizontal"); // 50 = maxstreeingAngle
            WheelCollider.frontleftWheel.steerAngle = steering;
            WheelCollider.frontRightWheel.steerAngle = steering;
            if(steering >0)
                steerInput=1f;
                    if(steering <0)
                         steerInput=-1f; 
                        if(steering==0)
                              steerInput=0; 
             if(Transmission.speed > 50.0f )
               maxstreeingAngle = maxstreeingAngle - 15f;                       
             //Rotation Wheel
              UpdateWheel(WheelCollider.frontleftWheel, Transform.frontleftTransform);
              UpdateWheel(WheelCollider.frontRightWheel, Transform.frontRightTransform);
              UpdateWheel(WheelCollider.backleftWheel, Transform.backleftTransform);
              UpdateWheel(WheelCollider.backRightWheel,Transform.backRightTransform);
               void UpdateWheel(WheelCollider coll,Transform wheelMesh){
                                Quaternion quat;
                                Vector3 position;
                                coll.GetWorldPose(out position, out quat);
                                wheelMesh.transform.position = position;
                                wheelMesh.transform.rotation = quat;
                             }                                         
            //Brake Wheel
             float maxBrake = 150.0f;
        //bool isBrake = false; Input.GetButton("Jump")
        if(Input.GetButton("Jump")){
                brakeInput=1;
                WheelCollider.frontleftWheel.brakeTorque = maxBrake * 1000 * Time.deltaTime; //1000 maxmotortorque
                WheelCollider.frontRightWheel.brakeTorque = maxBrake * 1000 * Time.deltaTime;
                WheelCollider.backleftWheel.brakeTorque = maxBrake * 1000 * Time.deltaTime;
                WheelCollider.backRightWheel.brakeTorque = maxBrake * 1000 * Time.deltaTime;
                GetComponentInChildren<TrailRenderer>().emitting = true;
                //TrailRenderer.emitting = true;
              //  emitting = true;
                
             } else{
                brakeInput=0;
                WheelCollider.frontleftWheel.brakeTorque = 0 * Time.deltaTime;
                WheelCollider.frontRightWheel.brakeTorque = 0 * Time.deltaTime;
                WheelCollider.backleftWheel.brakeTorque = 0 * Time.deltaTime;
                WheelCollider.backRightWheel.brakeTorque = 0 *Time.deltaTime;
                 GetComponentInChildren<TrailRenderer>().emitting = false;
              }  

        /*if(Input.GetKeyDown(KeyCode.Z)){
                handbrakeInput=1f;
                WheelCollider.backleftWheel.brakeTorque = maxBrake * 1000 * Time.deltaTime;
                WheelCollider.backRightWheel.brakeTorque = maxBrake * 1000 * Time.deltaTime;
             } else{
                handbrakeInput=0;
                WheelCollider.backleftWheel.brakeTorque = 0 * Time.deltaTime;
                WheelCollider.backRightWheel.brakeTorque = 0 *Time.deltaTime;
              }  */ 

        ///Anit Roll Bar System
          WheelHit hit;
        float travelfL = 1.0f;
        float travelfR = 1.0f;
        float travelbL = 1.0f;
        float travelbR =1.0f;
        // fornt
        bool groundedfL = Suspension.wheelfL.GetGroundHit(out hit);
        if(groundedfL){
            travelfL =(-Suspension.wheelfL.transform.InverseTransformPoint (hit.point).y -Suspension.wheelfL.radius) /Suspension.wheelfL.suspensionDistance;
        }
         bool groundedfR = Suspension.WheelfR.GetGroundHit(out hit);
        if(groundedfL){
            travelfR =(-Suspension.WheelfR.transform.InverseTransformPoint (hit.point).y -Suspension.WheelfR.radius) /Suspension.WheelfR.suspensionDistance;
        }
         Suspension.antiRollForce_front =(travelfL - travelfR )* Suspension.AntiRoll ;
        if(groundedfL)
         rb.AddForceAtPosition (Suspension.wheelfL.transform.up * -Suspension.antiRollForce_front, Suspension.wheelfL.transform.position);
         if(groundedfR)
         rb.AddForceAtPosition (Suspension.WheelfR.transform.up * -Suspension.antiRollForce_front, Suspension.WheelfR.transform.position);
         // Back
         bool groundedbL = Suspension.wheelbL.GetGroundHit(out hit);
        if(groundedbL){
            travelbL =(-Suspension.wheelbL.transform.InverseTransformPoint (hit.point).y -Suspension.wheelbL.radius) /Suspension.wheelbL.suspensionDistance;
        }
        bool groundedbR = Suspension.WheelbR.GetGroundHit(out hit);
        if(groundedbL){
            travelbR =(-Suspension.WheelbR.transform.InverseTransformPoint (hit.point).y -Suspension.WheelbR.radius) /Suspension.WheelbR.suspensionDistance;
        }
        Suspension.antiRollForce_back =(travelbL - travelbR )* Suspension.AntiRoll ;
        if(groundedbL)
         rb.AddForceAtPosition (Suspension.wheelbL.transform.up * -Suspension.antiRollForce_back, Suspension.wheelbL.transform.position);
         if(groundedbR)
         rb.AddForceAtPosition (Suspension.WheelbR.transform.up * -Suspension.antiRollForce_back, Suspension.WheelbR.transform.position); 
        }
        #endregion
    #region Engine_Transmission
    void Engine_(){
         //Engine.rpmNeedle.rotation = Quaternion.Euler(0,0,Mathf.Lerp(Engine.miniNeedleRotation,Engine.maxNeedleRotaion,Engine.EngineRPM /Engine.maxRPM_D));
         Engine.rpmImage.fillAmount = Engine.currentRPM * 0.9f/10000f;
        Engine.currentRPM = ((Transmission.speed  * 518) /40) * (Transmission.gearRatios[0] * Transmission.gearRatios[Transmission.currentGear]); 
        /////////
        Engine.rpmDisplay.text = $"RPM: {(int)Engine.currentRPM}";
        Engine.TorqueDisplay.text = $"Torque: {(float)Engine.Torque}";
        Transmission.gearDisplay.text = $"Gear: {(int)Transmission.currentGear}";
        Transmission.speedDisplay.text = $"Speed: {(int)Mathf.RoundToInt(Transmission.speed)}";       
     //   Input.GetKey(KeyCode.W) throttle>=1f
        
        if(Input.GetButton("Vertical")){
                 Engine.currentRPM += Engine.addRPM;// * Time.deltaTime;
                 throttle=1;
        }
        else{
                 Engine.currentRPM -= Engine.RemoveRPM;//* Time.deltaTime;
                 throttle=0;
        }
        
            Engine.currentRPM = Mathf.Clamp(Engine.currentRPM,Engine.minRPM,Engine.maxRPM);
            //Engine Sound
            if(Engine.currentRPM > Engine.idleRPM){
                engineSound.clip = Engine.idleSound;
                engineSound.Play();
                 }
            else if(Engine.currentRPM < Engine.maxRPM * 0.6f){
                 engineSound.clip =Engine.lowSound;
                 engineSound.Play();
                 }
            else if(Engine.currentRPM < Engine.maxRPM * 0.6f){
                //engineSound.clip=Engine.highSound;
               // engineSound.Play();
              //  Engine.isBackfiring =true;
               // engineSound.clip =Engine.backfireSound;
               //engineSound.Play();
               }
               // if(Engine.currentRPM < Engine.maxRPM){
                  //  Engine.isBackfiring =true;
                 //   engineSound.clip =Engine.backfireSound;
               // }if(Engine.currentRPM > Engine.maxRPM)
                    // Engine.isBackfiring =false;

            // pitch sound
             engineSound.pitch = Engine.currentRPM/Engine.maxRPM;
          // engineSound.volume = Mathf.Lerp(0.2f,1f,Engine.currentRPM/Engine.maxRPM);
           
            void RotateCrankshaft(){
              // Rotate the crankshaft based on RPM
            float rotationSpeed = Mathf.Lerp(20f, 360f, Mathf.InverseLerp(Engine.idleRPM, Engine.maxRPM, Engine.currentRPM)) ;//* Time.deltaTime;
            Engine.crankshaft.transform.Rotate(Vector3.forward, rotationSpeed);
                }
        //// calculation Torque
         float currentTorqVoid = 0;
        // if (Input.GetAxis("Vertical") > 0)
            if( throttle>=1f)   
                if (Transmission.currentGear != 0 ) {
                    Engine.Torque = Engine.horsepower * 5252 / Engine.currentRPM;
                        currentTorqVoid =(Transmission.gearRatios[0] * Transmission.gearRatios[Transmission.currentGear]) * Engine.powerEngine.Evaluate(Engine.currentRPM);
                    }
                    if (throttle <=0){
                    Engine.Torque=0;        
                 }
                
        //Revers
        
    
    
         Transmission.speed = rb.velocity.magnitude;// * 50.0f;
        if (Transmission.UnitType == TransmissionVehicle.UnitTypes.KMH)
            Transmission.speed = rb.velocity.magnitude * 3.6f; //is multiplied by a 3.6 kmh
        if (Transmission.UnitType == TransmissionVehicle.UnitTypes.MPH)
            Transmission.speed = rb.velocity.magnitude * 2.2f; //is multiplied by a 3.6 mph
       
       }
            
             #endregion
  
        #region Transmission.
        void Automatic(){
                if(Transmission.currentGear !=-1){
                    if(Engine.currentRPM > 3000 && Transmission.currentGear < Transmission.gearRatios.Length -1)
                        Transmission.currentGear++;
                    if(Engine.currentRPM < 3500f && Transmission.currentGear > 1)
                        Transmission.currentGear--;
                        }
                if(Input.GetKeyDown(KeyCode.Z) && Transmission.speed <=30f)
                    Transmission.currentGear--;
                    }
            void Manual(){
                    if(Input.GetKeyDown(KeyCode.E)){
                        Transmission.currentGear++;
                        }
                    if(Input.GetKeyDown(KeyCode.Q)){
                    Transmission.currentGear--;
                    }
            }   
   

   
   
 
        #endregion

        #region Switch
    void GearControl (){
        switch (Transmission.GearboxType)
        {
            case TransmissionVehicle.GearboxTypes.Automatic:
                Automatic();
                break;
            case TransmissionVehicle.GearboxTypes.Manual:
                Manual();
                break;
        }
      }
    
   
    void DriveType(){
        switch (Transmission.TractionType){
                    case TransmissionVehicle.TractionTypes.AWD:
                        WheelCollider.frontleftWheel.motorTorque =Engine.Torque/2;
                        WheelCollider.frontRightWheel.motorTorque = Engine.Torque/2;
                        WheelCollider.backleftWheel.motorTorque = Engine.Torque/2;
                        WheelCollider.backRightWheel.motorTorque = Engine.Torque/2;
                        break;
                    case TransmissionVehicle.TractionTypes.RWD:
                        WheelCollider.backleftWheel.motorTorque = Engine.Torque;
                        WheelCollider.backRightWheel.motorTorque = Engine.Torque;
                        break;
                    case  TransmissionVehicle.TractionTypes.FWD:
                        WheelCollider.frontleftWheel.motorTorque =  Engine.Torque;
                        WheelCollider.frontRightWheel.motorTorque = Engine.Torque;
                        break;
                    }

       }
       #endregion

    #region 
    [System.Serializable]
    public class EngineVehicle {
        //Engine Setup
        public float idleRPM = 800f;
        public float minRPM = 1000f;
        public float maxRPM = 5000f;
        public float currentRPM;
        public float Torque;
        public float horsepower;
        public float addRPM = 100f;
        public float RemoveRPM = 50f;
        [Header("Boolen")]
        public bool Gas;
        public bool Break;
       // public float maxRPMDecreaseAmount = 500f;
       // public float smoothSoundTransitionSpeed = 5f;
        public bool isBackfiring;
        //public float startDelay = 2f;
        //public bool isEngineRunning = false;
       // public bool oilLightOn = false;
       // public float targetPitch;
        [Header("Texts_dashboard")]
        public TMP_Text rpmDisplay;
        public TMP_Text TorqueDisplay;
        //public TMP_Text rpmText;
       // public TMP_Text gasolineText;
       // public TMP_Text temperatureText;
       // public TMP_Text oilLightText;
       // public TMP_Text driveModeText;
       /* [Header("Buttons_Control")]
        public Button startStopButton;
        public Button gasButton;
        public Button normalModeButton;
        public Button sportModeButton;
        public Slider gasSlider;*/
        [Header("crankshaft")]
        public GameObject crankshaft;
        [Header("Audio_Sounds_Effect")]
        public AudioClip idleSound;
        public AudioClip lowSound;
        public AudioClip highSound;
       // public AudioClip choppingSound;
        public AudioClip backfireSound;
        public ParticleSystem exhaustParticles;
        [Header("Tools G/H")]
        public float gasolineLevel = 1.0f; // 1.0 represents full tank
        public float temperature = 20f; // Initial engine temperature 
        public AnimationCurve powerEngine;
        [Header("Visual RPM")]
        public Image rpmImage;
        //public Transform rpmNeedle;
        //public float maxNeedleRotaion = -928.26f;
        //public float miniNeedleRotation = -688.5f;
        //public float maxRPM_D = 9000.0f;
    }
    [System.Serializable]
    public class TransmissionVehicle {
        public enum GearboxTypes { Automatic,Manual, CVT, DualClutch }
        public enum TransmissionMode { Park, Reverse, Neutral, Drive, Low }
        public enum TractionTypes{FWD,RWD,AWD}
        public enum UnitTypes{KMH,MPH}
        public GearboxTypes GearboxType;
        public TransmissionMode TransmissionModes;
        public TractionTypes TractionType;
        public UnitTypes UnitType;
        public float[] gearRatios = { 0f, 3.5f, 2.5f, 1.8f, 1.0f, 0.5f };
        public float finalDriveRatio = 3.0f;
        public float timeToShift = 1.0f;
        public AudioClip shiftSound;
        public TMP_Text gearDisplay;
        public TMP_Text speedDisplay;
        public Toggle speedUnitToggle;
        public Toggle gearToggle;
        public Button modeToggleButton;
        public Slider clutchPedal;
        public PedalType clutchPedalType = PedalType.Button; // Assuming Button is the default
        public float clutchEngageThreshold = 0.5f;
        public int currentGear = 0;
        public bool isShifting = false;
        public float speed;
        public float speedMultiplier = 2.23694f; // 1 m/s = 2.23694 mph
        public enum PedalType { Button, Slider }
       
        //////////////////
        //public int CurrentGear;
       // public float[] gearRatios;
        //public float[] gearRatiosRevers;
      //  [Header("Speed")]
        //public float speed;
        //public TMP_Text Gear;
        //public TMP_Text Speed;

    }
    [System.Serializable]
    public class WC{
         [Header("WheelCollider")]
        public WheelCollider frontleftWheel;
        public WheelCollider frontRightWheel;
        public WheelCollider backleftWheel;
        public WheelCollider backRightWheel;
    }
    [System.Serializable]
    public class WT{
        [Header("Transform")]
        public Transform frontleftTransform;
        public Transform frontRightTransform;
        public Transform backleftTransform;
        public Transform backRightTransform;
    }
    [System.Serializable]
    public class SuspensionVehicle{
        [Header("Anti Roll Bar")]
        public WheelCollider wheelfL;
        public WheelCollider WheelfR;
        public WheelCollider wheelbL;
        public WheelCollider WheelbR;
        public float AntiRoll=5000f;
        public float antiRollForce_front;
        public float antiRollForce_back;
         [Header("Suspension")]
        public float SuspensionValue;
    }
    [System.Serializable]
    public class EffectVehicle{
        public TrailRenderer[] TireMarks;
      
    }
    #endregion
    [Header("Center of Mass")]
    public GameObject CenterOfMass;
    //private var
    private Rigidbody rb;
    private AudioSource engineSound;
}
