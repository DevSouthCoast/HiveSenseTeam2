using System;
using Gadgeteer.Modules.GHIElectronics;
using Microsoft.SPOT;

namespace HiveSense
{
    public class ButtonSensor
    {
        private Button _buttonSensor;
        private BluetoothController _bluetoothController;

        public ButtonSensor(Button button, BluetoothController bluetoothController)
        {
            _bluetoothController = bluetoothController;
            _buttonSensor = button;

            _buttonSensor.ButtonPressed += ButtonSensorOnButtonPressed;
        }

        private void ButtonSensorOnButtonPressed(Button sender, Button.ButtonState state)
        {
            _bluetoothController.StartPairing();
            Debug.Print("Button Pressed");
        }
    }
}
