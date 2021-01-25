#include <Arduino.h>
#include <ESPAsyncWebServer.h>
#include <SPIFFS.h>
#include <limits>

#include "gyro.h"
#include "display.h"

const int capteurluminosite = 34; //POTENTIOMETRE PIN
const int buttonPin = 35; // BUTTON PIN
AsyncWebServer server(80);

float gliderHeight = -1;

void setupServer(uint8_t mode, char * SSID, char * PWD) {
    IPAddress IP;

    if(mode == ACCES_POINT) {
        Serial.print("Mise en place du point d'acces");
        drawString(0,0, "Mise en place du point d'acces");

        WiFi.softAP(SSID, PWD);

        IP = WiFi.softAPIP();
    } else {
        Serial.print("Tentative de connexion...");
        drawString(0,0, "Tentative de connexion...");

        WiFi.persistent(false);
        WiFi.begin(SSID, PWD);

        while (WiFi.status() != WL_CONNECTED)
        {
        Serial.print(".");
        delay(100);
        }

        IP = WiFi.localIP();
    }    

    Serial.println("\n");
    Serial.print("Adresse IP: ");
    Serial.println(IP);

    clearDisplay();

    char buf[512];
    strcpy(buf,"SSID : ");
    strcat(buf,SSID);
    drawString(0,0, buf);

    strcpy(buf,"PWD : ");
    strcat(buf,PWD);
    drawString(0,10, buf);

    drawString(0,30, "IP : " + IP.toString());

    server.begin();

    //-------REPONSE AUX REQUETES --------------------------------------------------------------------------------------//

    //1 - quand quelqu'un accède à la racine du serveur web
    //2 - je crée une requête AsyncWebServerRequest pour le serveur asynchrone (la requête est un pointeur)
    //3 - request->send(SPIFFS, "/index.html", "text/html"); avec -> car request est un pointeur
    //        => j'envoie la requête avec send
    //        => en spécifiant que les fichiers sont dans le SPIFFS
    //        => je spécifie qu'en atteignant la racine, le fichier à envoyer est le fichier index.html
    //        => et je spécifie que c'est du text/html
    
    // LES REQUETES SPIFFS NE MARCHENT PAS EN MODE AP
    if(mode != ACCES_POINT) {
        server.on("/", HTTP_GET, [](AsyncWebServerRequest *request) {
            request->send(SPIFFS, "/index.html", "text/html");
        });

        server.on("/w3.css", HTTP_GET, [](AsyncWebServerRequest *request) {
            request->send(SPIFFS, "/w3.css", "text/css");
        });

        server.on("/script.js", HTTP_GET, [](AsyncWebServerRequest *request) {
            request->send(SPIFFS, "/script.js", "text/javascript");
        });

        server.on("/jquery-3.4.1.min.js", HTTP_GET, [](AsyncWebServerRequest *request) {
            request->send(SPIFFS, "/jquery-3.4.1.min.js", "text/javascript");
        });
    }

    server.on("/lirePotentiometre", HTTP_GET, [](AsyncWebServerRequest *request) {
        int val = analogRead(capteurluminosite);
        String luminosite = String((map(val, 0, 4095, 0, 360)));
        request->send(200, "text/plain", luminosite); //code 200 pour dire que c'est bon !
    });

    server.on("/lireButton", HTTP_GET, [](AsyncWebServerRequest *request) {
        String etatButton = String(digitalRead(buttonPin));
        request->send(200, "text/plain", etatButton); //code 200 pour dire que c'est bon !
    });

    server.on("/lireGyro", HTTP_GET, [](AsyncWebServerRequest *request) {
        float *ypr = readGyroYPR();
        if(ypr == NULL) {
            request->send(200, "text/plain", "-1");
        } else {
            String gyroValue = "(" + String(ypr[0]) + "," + String(ypr[1]) + "," + String(ypr[2]) + ")";
            request->send(200, "text/plain", gyroValue);
        }
    });

    server.on("/calibrerGyro", HTTP_GET, [](AsyncWebServerRequest *request) {
        calibrateGyro();
    });

    server.on("/gliderHeight", HTTP_GET, [](AsyncWebServerRequest *request) {
        AsyncWebParameter* p = request->getParam(0);
    
        if(p->name() == "h") {
            String h = p->value();
            gliderHeight = h.toFloat();
        } else {
            gliderHeight = -1;
        }     

        Serial.println(gliderHeight);

        request->send(200, "text/plain", "message received");           
    });
    
    server.begin();
    Serial.println("Serveur web actif!"); //Affiche dans le serial moniteur
}

int getGliderHeight() {
    return gliderHeight;
}