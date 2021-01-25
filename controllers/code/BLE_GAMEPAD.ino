/*
 * This example turns the ESP32 into a Bluetooth LE gamepad that presses buttons and moves axis
 * 
 * Possible buttons are:
 * BUTTON_1 through to BUTTON_64 
 * 
 * Possible DPAD/HAT switch position values are: 
 * DPAD_CENTERED, DPAD_UP, DPAD_UP_RIGHT, DPAD_RIGHT, DPAD_DOWN_RIGHT, DPAD_DOWN, DPAD_DOWN_LEFT, DPAD_LEFT, DPAD_UP_LEFT
 * (or HAT_CENTERED, HAT_UP etc)
 *
 * bleGamepad.setAxes takes the following int16_t parameters for the Left/Right Thumb X/Y, uint16_t for the Left/Right Triggers plus slider1 and slider2, and hat switch position as above: 
 * (Left Thumb X, Left Thumb Y, Right Thumb X, Right Thumb Y, Left Trigger, Right Trigger, Hat switch positions (hat1, hat2, hat3, hat4));
 */
 
#include <BleGamepad.h> 

BleGamepad bleGamepad;

int JoystickX;
int JoystickY;
int JoystickZ;

int currentButtonState1;
int lastButtonState1;

void setup() 
{
  Serial.begin(115200);
  Serial.println("Starting BLE work!");
  bleGamepad.begin();
}

void loop() 
{
  if(bleGamepad.isConnected()) 
  {

     // Read Joystick
  JoystickX = analogRead(34); // Hall effect sensor connects to this analog pin
  JoystickY = analogRead(35); // Hall effect sensor connects to this analog pin

    // Read Rudder Pedals
  JoystickZ = analogRead(32); // Hall effect sensor connects to this analog pin

    //Display on console 
  Serial.println("X"+(String)JoystickX+" Y"+(String)JoystickY +" Z"+ (String)JoystickZ);
  

  // Read Button
int currentButtonState1 = !digitalRead(4); // Button 1

  if (currentButtonState1 != lastButtonState1)
  {
    Serial.println("LOOP");
    if (currentButtonState1 == 0){
      bleGamepad.press(BUTTON_1);
    }
    else if (currentButtonState1 == 1){
      bleGamepad.release(BUTTON_1);
    }
    
  lastButtonState1 = currentButtonState1;
  
  }

  Serial.println("Button: "+(String)currentButtonState1+ " Old State:"+(String)lastButtonState1);
  
// Map analog reading from 0 ~ 4095 to 32737 ~ -32737 for use as an axis reading
  
  int JoystickXAdjusted = map(JoystickX, 4095, 0, 65454,0 ); //Fix: Unity n'accepte que des valeurs positives
  int JoystickYAdjusted = map(JoystickY, 4095, 200, 65454,0 ); //Fix: on repère une zone morte pour le controlleur en Y entre 0 et 200 (limite mécanique) donc on décale le 0 à 200
  int JoystickZAdjusted = map(JoystickZ, 4095, 0, 65454,0 );

   
// Output Controls
  bleGamepad.setX(JoystickXAdjusted);
  bleGamepad.setY(JoystickYAdjusted);
  bleGamepad.setZ(JoystickZAdjusted);
  
 delay(10);
  }
}
