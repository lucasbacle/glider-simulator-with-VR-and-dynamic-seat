#include "I2Cdev.h"
#include "MPU6050_6Axis_MotionApps20.h"
#include "Wire.h"
#include "main.h"
#include <Arduino.h>
#include <Tone32.h>

MPU6050 mpu;

#define OUTPUT_READABLE_YAWPITCHROLL

#define INTERRUPT_PIN 2
const int RECAL_PIN = 35; // BUTTON PIN 
bool blinkState = false;

// MPU control/status vars
bool dmpReady = false;  // set true if DMP init was successful
uint8_t devStatus;      // return status after each device operation (0 = success, !0 = error)
uint8_t fifoBuffer[64]; // FIFO storage buffer

// orientation/motion vars
VectorFloat gravity;
Quaternion q;
float ypr[3];           // [yaw, pitch, roll]

//-------- INTERRUPT ROUTINE ---------------------
volatile bool mpuInterrupt = false;     // indicates whether MPU interrupt pin has gone high
void dmpDataReady() {
    mpuInterrupt = true;
}

//TODO multithreader cette méthode pour ne pas bloquer le serveur
//TODO mettre en place un timeout
void calibrateGyro() {
    dmpReady = false;
    mpu.setDMPEnabled(false);

    //BIP
    tone(BUZZER_PIN, NOTE_B4, 100, BUZZER_CHANNEL);

    bool gyroCalibrated;
    
    gyroCalibrated = mpu.CalibrateAccel(4, 3);
    gyroCalibrated &= mpu.CalibrateGyro(4, 3);
    //mpu.PrintActiveOffsets();
    
    if(gyroCalibrated) {
        tone(BUZZER_PIN, NOTE_D8, 100, BUZZER_CHANNEL);
        Serial.println("SUCCESS");
    } else {
        tone(BUZZER_PIN, NOTE_B4, 30, BUZZER_CHANNEL);
        Serial.println("FAILED");
    }

    //active le DMP apres calibration
    mpu.setDMPEnabled(true);
    dmpReady = true;
}

void setupGyro() {
    Wire.begin(SDA, SCL, 400000); // rejoindre le bus I2C
    
    //-------- GPIO ---------------------
    pinMode(RECAL_PIN, INPUT);
    
    //-------- MPU ---------------------
    mpu.initialize();
    if(!mpu.testConnection()) {
        Serial.println("ERROR : can't connect to MPU");
    }

    devStatus = mpu.dmpInitialize(); // initialize le DMP (Digital Motion Processor)

    // TODO regarder les effets de l'offset pour la recalibration
    mpu.setXGyroOffset(0);
    mpu.setYGyroOffset(0);
    mpu.setZGyroOffset(0);
    mpu.setZAccelOffset(1788);

    if (devStatus == 0) {
        calibrateGyro();

        //active l'interruption sur la pin
        attachInterrupt(digitalPinToInterrupt(INTERRUPT_PIN), dmpDataReady, RISING);

        dmpReady = true;
    } else {
        Serial.println("ERROR ");
        Serial.print(devStatus);
        Serial.print(" : can't initialize DMP");
    }
}

void loopGyro() {
    if (!dmpReady) return;

    if (mpu.dmpGetCurrentFIFOPacket(fifoBuffer)) {
        mpu.dmpGetQuaternion(&q, fifoBuffer);
        mpu.dmpGetGravity(&gravity, &q);
        mpu.dmpGetYawPitchRoll(ypr, &q, &gravity);
    }

    if(digitalRead(RECAL_PIN) == 1) {
        calibrateGyro();
    }
}

float * readGyroYPR() {
    if(!dmpReady)
        return NULL;

    return ypr;
}