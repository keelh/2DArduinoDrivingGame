#include <Wire.h>
#include <LedControl.h>
#include <MaxMatrix.h>
#include <TM1637Display.h>
#include <pitches.h>
#include <SoftwareSerial.h>

#define maxDisplays 5

const int VRx = A0;  // Analog input for joystick X-axis
const int VRy = A1;  // Analog input for joystick Y-axis

#define buttonPin 7  // Pin for the joystick button

unsigned long lastUpdateTime = 0;
const unsigned long DISPLAY_INTERVAL = 1000;

byte Buf7219[7];
const int data = 11;
const int load = 10;
const int clock = 13;

// Define the connections pins
#define CLK 6
#define DIO 5

MaxMatrix m(data, load, clock, maxDisplays);
TM1637Display display = TM1637Display(CLK, DIO);

int latchPin = 3;
int clockPin = 4;
int dataPin = 2;

char string[] = "Welcome to R & Drive!  ";
char string2[] = "GO! ";
char string3[] = "Lap Complete!";
char string4[] = "3... ";
char string5[] = "2... ";
char string6[] = "1... ";

// Define buzzer pin
#define BUZZER 8

unsigned long timerStart = 0;
unsigned long lapStartTime = 0;
bool lapCompleted = false;
int lapCount = 1;
bool timerRunning = false;
bool gameRunning = false;
bool started = false;

int melody[] = {
  NOTE_C5, NOTE_D5, NOTE_E5, NOTE_F5, NOTE_G5, NOTE_A5, NOTE_B5, NOTE_C6
};
int duration = 500;

bool gameStarted = false;

#define GREEN_LED 9
#define RED_LED 12

LedControl lc = LedControl(data, clock, load, 1);
SoftwareSerial mySerial(10, 11);

byte seven_seg_digits[10] = {
  B11111100,  // = 0
  B01100000,  // = 1
  B11011010,  // = 2
  B11110010,  // = 3
  B01100110,  // = 4
  B10110110,  // = 5
  B10111110,  // = 6
  B11100000,  // = 7
  B11111110,  // = 8
  B11100110   // = 9
};

PROGMEM const unsigned char CH[] = {
  3, 8, B0000000, B0000000, B0000000, B0000000, B0000000,      // space
  1, 8, B1011111, B0000000, B0000000, B0000000, B0000000,      // !
  3, 8, B0000011, B0000000, B0000011, B0000000, B0000000,      // "
  5, 8, B0010100, B0111110, B0010100, B0111110, B0010100,      // #
  4, 8, B0100100, B1101010, B0101011, B0010010, B0000000,      // $
  5, 8, B1100011, B0010011, B0001000, B1100100, B1100011,      // %
  5, 8, B0110110, B1001001, B1010110, B0100000, B1010000,      // &
  1, 8, B0000011, B0000000, B0000000, B0000000, B0000000,      // '
  3, 8, B0011100, B0100010, B1000001, B0000000, B0000000,      // (
  3, 8, B1000001, B0100010, B0011100, B0000000, B0000000,      // )
  5, 8, B0101000, B0011000, B0001110, B0011000, B0101000,      // *
  5, 8, B0001000, B0001000, B0111110, B0001000, B0001000,      // +
  2, 8, B10110000, B1110000, B0000000, B0000000, B0000000,     // ,
  4, 8, B0001000, B0001000, B0001000, B0001000, B0000000,      // -
  2, 8, B1100000, B1100000, B0000000, B0000000, B0000000,      // .
  4, 8, B1100000, B0011000, B0000110, B0000001, B0000000,      // /
  4, 8, B0111110, B1000001, B1000001, B0111110, B0000000,      // 0
  3, 8, B1000010, B1111111, B1000000, B0000000, B0000000,      // 1
  4, 8, B1100010, B1010001, B1001001, B1000110, B0000000,      // 2
  4, 8, B0100010, B1000001, B1001001, B0110110, B0000000,      // 3
  4, 8, B0011000, B0010100, B0010010, B1111111, B0000000,      // 4
  4, 8, B0100111, B1000101, B1000101, B0111001, B0000000,      // 5
  4, 8, B0111110, B1001001, B1001001, B0110000, B0000000,      // 6
  4, 8, B1100001, B0010001, B0001001, B0000111, B0000000,      // 7
  4, 8, B0110110, B1001001, B1001001, B0110110, B0000000,      // 8
  4, 8, B0000110, B1001001, B1001001, B0111110, B0000000,      // 9
  2, 8, B01010000, B0000000, B0000000, B0000000, B0000000,     // :
  2, 8, B10000000, B01010000, B0000000, B0000000, B0000000,    // ;
  3, 8, B0010000, B0101000, B1000100, B0000000, B0000000,      // <
  3, 8, B0010100, B0010100, B0010100, B0000000, B0000000,      // =
  3, 8, B1000100, B0101000, B0010000, B0000000, B0000000,      // >
  4, 8, B0000010, B1011001, B0001001, B0000110, B0000000,      // ?
  5, 8, B0111110, B1001001, B1010101, B1011101, B0001110,      // @
  4, 8, B1111110, B0010001, B0010001, B1111110, B0000000,      // A
  4, 8, B1111111, B1001001, B1001001, B0110110, B0000000,      // B
  4, 8, B0111110, B1000001, B1000001, B0100010, B0000000,      // C
  4, 8, B1111111, B1000001, B1000001, B0111110, B0000000,      // D
  4, 8, B1111111, B1001001, B1001001, B1000001, B0000000,      // E
  4, 8, B1111111, B0001001, B0001001, B0000001, B0000000,      // F
  4, 8, B0111110, B1000001, B1001001, B1111010, B0000000,      // G
  4, 8, B1111111, B0001000, B0001000, B1111111, B0000000,      // H
  3, 8, B1000001, B1111111, B1000001, B0000000, B0000000,      // I
  4, 8, B0110000, B1000000, B1000001, B0111111, B0000000,      // J
  4, 8, B1111111, B0001000, B0010100, B1100011, B0000000,      // K
  4, 8, B1111111, B1000000, B1000000, B1000000, B0000000,      // L
  5, 8, B1111111, B0000010, B0001100, B0000010, B1111111,      // M
  5, 8, B1111111, B0000100, B0001000, B0010000, B1111111,      // N
  4, 8, B0111110, B1000001, B1000001, B0111110, B0000000,      // O
  4, 8, B1111111, B0001001, B0001001, B0000110, B0000000,      // P
  4, 8, B0111110, B1000001, B1000001, B10111110, B0000000,     // Q
  4, 8, B1111111, B0001001, B0001001, B1110110, B0000000,      // R
  4, 8, B1000110, B1001001, B1001001, B0110010, B0000000,      // S
  5, 8, B0000001, B0000001, B1111111, B0000001, B0000001,      // T
  4, 8, B0111111, B1000000, B1000000, B0111111, B0000000,      // U
  5, 8, B0001111, B0110000, B1000000, B0110000, B0001111,      // V
  5, 8, B0111111, B1000000, B0111000, B1000000, B0111111,      // W
  5, 8, B1100011, B0010100, B0001000, B0010100, B1100011,      // X
  5, 8, B0000111, B0001000, B1110000, B0001000, B0000111,      // Y
  4, 8, B1100001, B1010001, B1001001, B1000111, B0000000,      // Z
  2, 8, B1111111, B1000001, B0000000, B0000000, B0000000,      // [
  4, 8, B0000001, B0000110, B0011000, B1100000, B0000000,      // backslash
  2, 8, B1000001, B1111111, B0000000, B0000000, B0000000,      // ]
  3, 8, B0000010, B0000001, B0000010, B0000000, B0000000,      // hat
  4, 8, B1000000, B1000000, B1000000, B1000000, B0000000,      // _
  2, 8, B0000001, B0000010, B0000000, B0000000, B0000000,      //
  4, 8, B0100000, B1010100, B1010100, B1111000, B0000000,      // a
  4, 8, B1111111, B1000100, B1000100, B0111000, B0000000,      // b
  4, 8, B0111000, B1000100, B1000100, B0000000, B0000000,      // c // JFM MOD.
  4, 8, B0111000, B1000100, B1000100, B1111111, B0000000,      // d
  4, 8, B0111000, B1010100, B1010100, B0011000, B0000000,      // e
  3, 8, B0000100, B1111110, B0000101, B0000000, B0000000,      // f
  4, 8, B10011000, B10100100, B10100100, B01111000, B0000000,  // g
  4, 8, B1111111, B0000100, B0000100, B1111000, B0000000,      // h
  3, 8, B1000100, B1111101, B1000000, B0000000, B0000000,      // i
  4, 8, B1000000, B10000000, B10000100, B1111101, B0000000,    // j
  4, 8, B1111111, B0010000, B0101000, B1000100, B0000000,      // k
  3, 8, B1000001, B1111111, B1000000, B0000000, B0000000,      // l
  5, 8, B1111100, B0000100, B1111100, B0000100, B1111000,      // m
  4, 8, B1111100, B0000100, B0000100, B1111000, B0000000,      // n
  4, 8, B0111000, B1000100, B1000100, B0111000, B0000000,      // o
  4, 8, B11111100, B0100100, B0100100, B0011000, B0000000,     // p
  4, 8, B0011000, B0100100, B0100100, B11111100, B0000000,     // q
  4, 8, B1111100, B0001000, B0000100, B0000100, B0000000,      // r
  4, 8, B1001000, B1010100, B1010100, B0100100, B0000000,      // s
  3, 8, B0000100, B0111111, B1000100, B0000000, B0000000,      // t
  4, 8, B0111100, B1000000, B1000000, B1111100, B0000000,      // u
  5, 8, B0011100, B0100000, B1000000, B0100000, B0011100,      // v
  5, 8, B0111100, B1000000, B0111100, B1000000, B0111100,      // w
  5, 8, B1000100, B0101000, B0010000, B0101000, B1000100,      // x
  4, 8, B10011100, B10100000, B10100000, B1111100, B0000000,   // y
  3, 8, B1100100, B1010100, B1001100, B0000000, B0000000,      // z
  3, 8, B0001000, B0110110, B1000001, B0000000, B0000000,      // {
  1, 8, B1111111, B0000000, B0000000, B0000000, B0000000,      // |
  3, 8, B1000001, B0110110, B0001000, B0000000, B0000000,      // }
  4, 8, B0001000, B0000100, B0001000, B0000100, B0000000,      // ~
};

void setup() {
  Serial.begin(9600);
  mySerial.begin(9600);
  Serial.println("Setup started");

  lc.shutdown(0, false);  // Wake up displays
  lc.setIntensity(0, 3);  // Set brightness level
  lc.clearDisplay(0);     // Clear display

  display.setBrightness(5);
  Serial.println("LC and Display Initialized");

  lc.setDigit(0, 0, lapCount, false);

  pinMode(latchPin, OUTPUT);
  pinMode(clockPin, OUTPUT);
  pinMode(dataPin, OUTPUT);

  // Set LED pins as outputs
  pinMode(RED_LED, OUTPUT);
  pinMode(GREEN_LED, OUTPUT);

  // Initialize buzzer
  pinMode(buttonPin, INPUT_PULLUP);
  pinMode(BUZZER, OUTPUT);

  digitalWrite(GREEN_LED, LOW);
  digitalWrite(RED_LED, LOW);

  sevenSegWrite(lapCount);

  m.init();
  m.setIntensity(3);

  display.clear();
  Serial.println("MaxMatrix Initialized");

  Serial.println("Waiting for start command...  ");

  display.setBrightness(0x0f);
  display.showNumberDecEx(0, 0b01000000, true, 4, 0);
}

void loop() {
  if (Serial.available()) {
    char command = Serial.read();
    while (started == false) {
      if (command == 'S') {
        started = true;
        processCommand();
      }
    }

    if (command == 'L') {
      funcLapCompleted();
    }
    if (command == 'X') {
      stopGame();
    }
  }

  if (timerRunning) {
    // Calculate elapsed time
    unsigned long currentMillis = millis();
    unsigned long elapsedMillis = currentMillis - timerStart;

    // Convert elapsedMillis to minutes and seconds
    int minutes = (elapsedMillis / 60000) % 60;  // Minutes
    int seconds = (elapsedMillis / 1000) % 60;   // Seconds

    // Combine minutes and seconds for display
    int displayTime = minutes * 100 + seconds;

    // Display the time on the TM1637
    display.showNumberDecEx(displayTime, 0b01000000, true, 4, 0);

    // Small delay to prevent rapid updates
  }

  int xPosition = analogRead(VRx);
  int yPosition = analogRead(VRy);

  // Map joystick values to a range of -1 to 1
  float xValue = map(xPosition, 0.02, 1046, -100, 100) / -100.0;
  float yValue = map(yPosition, 0, 1046, -100, 100) / 100.0;

  Serial.print(xValue);
  Serial.print(",");
  Serial.print(yValue);
  Serial.println();

  unsigned long currentMillis = millis();
  if (currentMillis - lastUpdateTime >= DISPLAY_INTERVAL) {
    lastUpdateTime = currentMillis;
    if (lapCompleted) {
      lapCompleted = false;
      m.clear();
      sevenSegWrite(lapCount);
      noTone(BUZZER);
      m.clear();
    }
  }
}

void processCommand() {
  gameStarted = true;
  gameRunning = true;
  Serial.println("Game started");

  m.clear();
  m.shiftLeft(false, true);
  Serial.println("Displaying welcome message");
  printStringWithShift(string, 70);
  m.clear();
  delay(1000);

  m.clear();
  m.shiftLeft(false, true);
  tone(BUZZER, 1000);
  digitalWrite(RED_LED, HIGH);
  printStringWithShift(string4, 50);
  m.clear();
  noTone(BUZZER);
  digitalWrite(RED_LED, LOW);
  delay(500);

  m.shiftLeft(false, true);
  tone(BUZZER, 1000);
  digitalWrite(RED_LED, HIGH);
  printStringWithShift(string5, 50);
  m.clear();
  noTone(BUZZER);
  digitalWrite(RED_LED, LOW);
  delay(500);

  m.shiftLeft(false, true);
  tone(BUZZER, 1000);
  digitalWrite(RED_LED, HIGH);
  printStringWithShift(string6, 50);
  m.clear();
  noTone(BUZZER);
  digitalWrite(RED_LED, LOW);
  delay(500);

  m.shiftLeft(false, true);
  tone(BUZZER, 1500);
  digitalWrite(GREEN_LED, HIGH);
  Serial.println("Displaying GO message");
  printStringWithShift(string2, 70);
  noTone(BUZZER);
  m.clear();

  Serial.println("Timer started");
  timerStart = millis();
  timerRunning = true;

  lapStartTime = millis();

  Serial.print("Current Lap: ");
  Serial.println(lapCount);
}

void funcLapCompleted() {
  lapCount++;
  flashGreenLED(); 
  if (lapCount > 3) {
    lapCount = 0;
    stopGame();
  }

  lapCompleted = true;
  sevenSegWrite(lapCount);
}

void flashRedLED() {
  for (int i = 0; i < 3; ++i) {
    digitalWrite(RED_LED, HIGH);
    tone(BUZZER, 1000);
    delay(500);
    noTone(BUZZER);
    digitalWrite(RED_LED, LOW);
    delay(500);
  }
}

void flashGreenLED() {
  for (int i = 0; i < 1; ++i) {
    digitalWrite(GREEN_LED, HIGH);
    tone(BUZZER, 1500);
    delay(500);
    noTone(BUZZER);
    digitalWrite(GREEN_LED, LOW);
    delay(500);
    digitalWrite(GREEN_LED, HIGH);
  }
}

// Display a number on the digital segment display
void sevenSegWrite(byte digit) {
  // Set the latchPin to low potential before sending data
  digitalWrite(latchPin, LOW);

  // The original data (bit pattern)
  shiftOut(dataPin, clockPin, LSBFIRST, seven_seg_digits[digit]);

  // Set the latchPin to high potential to latch the data
  digitalWrite(latchPin, HIGH);
}

void printCharWithShift(char c, int shift_speed) {
  if (c < 32) return;
  c -= 32;
  memcpy_P(Buf7219, CH + 7 * c, 7);
  m.writeSprite(maxDisplays * 8, 0, Buf7219);
  m.setColumn(maxDisplays * 8 + Buf7219[0], 0);

  for (int i = 0; i <= Buf7219[0]; i++) {
    delay(shift_speed);
    m.shiftLeft(false, false);
  }
}

void printStringWithShift(char* s, int shift_speed) {
  while (*s != 0) {
    printCharWithShift(*s, shift_speed);
    s++;
  }
}

void stopGame() {
  timerRunning = false;
  gameRunning = false;
  digitalWrite(GREEN_LED, LOW);
  digitalWrite(RED_LED, LOW);
  noTone(BUZZER);
  Serial.println("Game stopped");
  m.clear();
  display.clear();  // Clear the TM1637 display
}