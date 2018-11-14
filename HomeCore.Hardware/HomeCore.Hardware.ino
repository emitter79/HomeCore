/*	
	------------------------------------------
        Written by David P. Smith - 2018
        (david.p.smith.ke5dev@gmail.com)
	------------------------------------------
*/

#include <stdint.h>
#include <arduino.h>
#include "LED.h"

const uint8_t DC1 = 17;
const uint8_t ETB = 23;
const uint8_t CMD_BASE = 69;
const uint8_t CMD_VALUES = CMD_BASE + 1;
const uint8_t CHANNELS = 64;
const uint8_t UNICAST = 63;

uint8_t values[CHANNELS];
uint8_t currentByte = 0;
uint8_t currentChannel = 0;
uint8_t currentValue = 0;
uint8_t phase = 0;

void setup() 
{
	// initialize the channel value lookup array 
	for(uint8_t x = 0; x < CHANNELS; x++) { values[x] = 0; }
	// open the serial port
	Serial.begin(115200);
	// set the PWM channels to their highest frequency
	setPwmFrequency(3, 1);  // 31KHz
	setPwmFrequency(5, 1);  // 62KHz
	setPwmFrequency(6, 1);  // 62KHz
	// create a "warm" 5500K white
	RGBV initialColor;
	initialColor.r = 255;
	initialColor.g = 173;
	initialColor.b = 63;
	// fade to the initial color
	fadeColorValue(initialColor, 255, 75);
}

void loop() { }

void serialEvent() 
{
	// read all incomming bytes
	while (Serial.available()) {
		// look for command codes in the incomming byte stream  
		currentByte = (uint8_t)Serial.read();
		switch(currentByte) {
			case DC1: // start byte
				phase = 1;
				break;
			case ETB: // execute byte
				if (currentChannel > CMD_BASE) {
					executeCommand(currentChannel, currentValue);
				} else if (currentChannel == UNICAST) {
					RGBV w;
					w.r = currentByte;
					w.g = currentByte;
					w.b = currentByte;
					setColorValue(w);
				} else {
					setChannelValue(currentChannel, currentValue);
				}
				break;
			default:
				if (phase == 1) {
					// accumulate channel
					currentChannel = currentByte;
					phase++;
				} else if (phase == 2) {
					// accumulate value
					currentValue = currentByte;
					phase++;
				}
				break;
		}
	}
}

void executeCommand(uint8_t command, uint8_t option) 
{
	switch(command) {
		case CMD_VALUES:
			Serial.write(values[5]);
			Serial.write(values[6]);
			Serial.write(values[3]);
			Serial.write(13);
			Serial.flush();
			break;
		}
}

void setChannelValue(uint8_t channel, uint8_t value)
{
  values[channel] = value;
  if ((channel >= 2 && channel <= 13) || (channel >= 44 && channel <= 46)) {
    // it's an analog pin, so write the PWM value
    analogWrite(channel, value);
  } else {
    // its a digital pin, so set the mode and write the value
    pinMode(channel, OUTPUT);
    digitalWrite(channel, (value > 0));
  }
}

void setColorValue(RGBV color) 
{
  setChannelValue(3, color.r);
  setChannelValue(5, color.g);
  setChannelValue(6, color.b);
}

void setPwmFrequency(int pin, int divisor) 
{
	uint8_t mode;
	if(pin == 5 || pin == 6 || pin == 9 || pin == 10) {
		switch(divisor) {
			case 1: mode = 0x01; break;
			case 8: mode = 0x02; break;
			case 64: mode = 0x03; break;
			case 256: mode = 0x04; break;
			case 1024: mode = 0x05; break;
			default: return;
		}
		if(pin == 5 || pin == 6) {
			TCCR0B = TCCR0B & 0b11111000 | mode;
		} else {
			TCCR1B = TCCR1B & 0b11111000 | mode;
		}
	} else if (pin == 3 || pin == 11) {
		switch(divisor) {
			case 1: mode = 0x01; break;
			case 8: mode = 0x02; break;
			case 32: mode = 0x03; break;
			case 64: mode = 0x04; break;
			case 128: mode = 0x05; break;
			case 256: mode = 0x06; break;
			case 1024: mode = 0x07; break;
			default: return;
		}
		TCCR2B = TCCR2B & 0b11111000 | mode;
	}
}

void fadeColorValue(RGBV color, float steps, int step_delay)
{
	float rx = color.r / steps;
	float rg = color.g / steps;
	float rb = color.b / steps;
	int r = 0,g = 0,b = 0;
	for(int x = 1; x <= steps; x++) {
		r = rx * x;
		g = rg * x;
		b = rb * x;
		setChannelValue(3, r);
		setChannelValue(5, g);
		setChannelValue(6, b);
		delay(step_delay);
	}
}

void fadeWhite(uint8_t target, uint8_t start) 
{
	for(int x = start; x <= target; x++) {
		setChannelValue(5, x);
		setChannelValue(6, x);
		setChannelValue(3, x);
		delay(500);
	}
}

/* EOF */