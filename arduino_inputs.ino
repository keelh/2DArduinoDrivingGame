const int VRx = A0;  // Analog input for joystick X-axis
const int VRy = A1;  // Analog input for joystick Y-axis

void setup() {
    Serial.begin(9600);
}

void loop() {
    int xPosition = analogRead(VRx);
    int yPosition = analogRead(VRy);

    // Map joystick values to a range of -1 to 1
    float xValue = map(xPosition, 0, 1023, -100, 100) / -100.0;
    float yValue = map(yPosition, 0, 1023, -100, 100) / 100.0;

    Serial.print(xValue);
    Serial.print(",");
    Serial.println(yValue);

    delay(10);  // Adjust delay as needed
}
