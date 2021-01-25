#include <Arduino.h>

#define ACCES_POINT 0
#define ON_NETWORK 1

void setupServer(uint8_t mode, char * SSID, char * PWD);
int getGliderHeight();