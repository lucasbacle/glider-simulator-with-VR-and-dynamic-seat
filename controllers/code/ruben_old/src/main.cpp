#include <Arduino.h>
#include <ESPAsyncWebServer.h>
#include <SPIFFS.h>
#include <Tone32.h>
#include "gyro.h"
#include "display.h"
#include "server.h"
#include "main.h"

//POUR LE SPIFFS
// SPIFFS is a file system intended for SPI NOR flash devices on embedded targets.
//https://docs.espressif.com/projects/esp-idf/en/latest/esp32/api-reference/storage/spiffs.html
// File system : ou mémoire SPIFFS est un espace de stockage (que l’on pourrait assimiler à une SDcard).
//https://byfeel.info/utilisation-de-la-memoire-spiffs-sur-esp8266/
//1 - ENVOYER LES FICHIERS D'ABORD DANS LA MEMOIRE FLASH
//2 - ENVOYER LE CODE ENSUITE

//Avec l'ARDUINO IDE:
//https://randomnerdtutorials.com/install-esp32-filesystem-uploader-arduino-ide/

//Avec PlatformIO:
//Cliquer sur PlatformIO
//Faire UPLOAD FILE SYSTEM IMAGE

//Ne pas mettre de delai dans la boucle car nous ne voulons pas perturber le serveur
//Il vaut mieux utiliser les fonctions du tyep "blink without delay"

const int potentiometrePin = 34; //POTENTIOMETRE PIN
const int buttonPin = 35; // BUTTON PIN
const int led = 22;       // ONBOARD LED GPIO
int buttonState = 0;      // variable for reading the pushbutton status

void setup()
{

    //--------SERIAL--------------------------------------------------------------------------------------//

    Serial.begin(115200);
    while (!Serial)
    {
        Serial.print(".");
        delay(500);
    }
    Serial.println("");

    //--------GPIO---------------------------------------------------------------------------------------//

    pinMode(led, OUTPUT);               //LED
    digitalWrite(led, LOW);             //LED
    pinMode(potentiometrePin, INPUT);   //POTENTIOMETRE
    pinMode(buttonPin, INPUT);          //BOUTON

    //-------SPIFFS--------------------------------------------------------------------------------------//

    Serial.print("Uploading SPIFFS files:\n");

    if (!SPIFFS.begin()) //SPIFFS est le gestionnaire de fichier qui a accès à la mémoire Flash
    {
        Serial.println("Erreur SPIFFS...");
        return;
    }

    //Est-ce que mes fichiers sont biens détectés dans la mémoire ?
    File root = SPIFFS.open("/");    //ouvre la racine de la mémoire Flash
    File file = root.openNextFile(); //on charge le premier fichier
    while (file)
    { //on entre dans la boucle si il existe un premier fichier
        Serial.print("File:");
        Serial.println(file.name()); //on affiche le nom
        file.close();                //on ferme le fichier
        file = root.openNextFile();  //le fichier est égal au prochain fichier
    }                                //on retourne au début

    //--------DISPLAY------------------------------------------------------------------------------------//
    Serial.print("Setting up display :\n");
    setupDisplay();

    //-------SERVER--------------------------------------------------------------------------------------//
    //ACCES_POINT OR ON_NETWORK
    Serial.print("Launching server :\n");
    if(digitalRead(buttonPin)) {
        setupServer(ACCES_POINT, "INSA VR Access Point", "INSAT");
    } else {
        setupServer(ON_NETWORK, "UwU", "whatsthis");
    }

    //--------GYRO---------------------------------------------------------------------------------------//
    Serial.print("Setting up gyroscope :\n");
    setupGyro();
}

void loop()
{
    loopGyro();

    // if the glider height is under 10, sound the alarm!
    float h = getGliderHeight();
    if(h != -1 && h < 10) {
        tone(BUZZER_PIN, NOTE_B7, 500, BUZZER_CHANNEL);
    }
}
