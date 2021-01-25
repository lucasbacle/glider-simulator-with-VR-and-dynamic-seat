#include <Wire.h>
#include <Arduino.h>
#include "SSD1306.h"

SSD1306  display(0x3C, SDA, SCL);

void setupDisplay()
{
    Serial.begin(115200);
    display.init();

    display.setTextAlignment(TEXT_ALIGN_LEFT);
    display.setFont(ArialMT_Plain_10);
    display.display(); 
}

void clearDisplay() {
    display.clear();
}

void drawString(uint8_t x, uint8_t y, String string) {
    display.drawString(x, y, string);
    display.display();
}