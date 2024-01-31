// Define the pin where I've connected the temperature sensor
const int sensorPin = A0;

// Define the pin where my LED is connected
const int ledPin = 9;

// I've set my threshold temperature in Celsius
const int thresholdTemp = 25;

// I'll use this flag to indicate if the temperature exceeds the threshold
volatile bool temperatureExceeded = false;

void setup()
{
    // I'm setting the LED pin as an output
    pinMode(ledPin, OUTPUT);

    // I'm starting serial communication at a rate of 9600 bits per second
    Serial.begin(9600);

    // I'm configuring Timer1 for an interrupt every 1 second
    noInterrupts(); // I'm temporarily disabling interrupts
    TCCR1A = 0;     // Resetting Timer1 control register A
    TCCR1B = 0;     // Resetting Timer1 control register B
    TCNT1 = 0;      // Initializing Timer1 counter value to 0

    // Setting Timer1 to CTC (Clear Timer on Compare Match) mode
    TCCR1B |= (1 << WGM12);

    // Setting prescaler to 1024 for Timer1
    TCCR1B |= (1 << CS12) | (1 << CS10);

    // Setting compare match register to generate an interrupt every 1 second
    OCR1A = 15624; // = 16e6 / (1024 * 1) - 1

    // Enabling Timer1 compare match interrupt
    TIMSK1 |= (1 << OCIE1A);

    interrupts(); // I'm enabling interrupts now
}

void loop()
{
    // My main program logic will be handled inside the interrupt service routine
}

// This is my Interrupt Service Routine (ISR) for Timer1 compare match A
ISR(TIMER1_COMPA_vect) {
    // I'm reading the value from my temperature sensor
    int sensorValue = analogRead(sensorPin);

    // I'm converting the sensor value to voltage
    float voltage = sensorValue * (5.0 / 1023.0);

    // I'm converting voltage to temperature in Celsius
    float temperatureC = (voltage - 0.5) * 100;

    // I'm printing the temperature in Celsius to the serial monitor
    Serial.print("Temperature: ");
    Serial.print(temperatureC);
    Serial.println(" degrees Celsius");

    // I'm checking if the temperature is above my threshold
    temperatureExceeded = (temperatureC > thresholdTemp);

    // I'm turning on or off the LED based on temperature exceeding the threshold
    digitalWrite(ledPin, temperatureExceeded ? HIGH : LOW);

    // I'm printing a message indicating LED status
    Serial.println(temperatureExceeded ? "Turning LED ON" : "Turning LED OFF");
}
