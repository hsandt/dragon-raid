using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestScript_MeleeAttack
    {
        private static void SimulateSetInputValue(InputDevice inputDevice, string inputPath, float inputValue)
        {
            InputEventPtr eventPtr;
            using (StateEvent.From(inputDevice, out eventPtr))
            {
                float currentInputValue = ((InputControl<float>) inputDevice[inputPath]).ReadValue();
                if (currentInputValue != inputValue)
                {
                    inputDevice[inputPath].WriteValueIntoEvent(inputValue, eventPtr);
                    InputSystem.QueueEvent(eventPtr);
                }
                else
                {
                    Debug.LogWarningFormat("Trying to set input value of {0} to {1}, but it is already so.",
                        inputPath, inputValue);
                }
            }
        }
        
        private static void SimulatePressInput(InputDevice inputDevice, string inputPath)
        {
            SimulateSetInputValue(inputDevice, inputPath, 1f);
        }

        private static void SimulateReleaseInput(InputDevice inputDevice, string inputPath)
        {
            SimulateSetInputValue(inputDevice, inputPath, 0f);
        }
        
        private static IEnumerator SimulateShortPressInput(InputDevice inputDevice, string inputPath)
        {
            SimulatePressInput(inputDevice, inputPath);
            
            // WaitForFixedUpdate is more reliable than null and can be stacked, but due to some event lag
            // we prefer adding several to make sure key is correctly released
            for (int i = 0; i < 2; i++)
            {
                yield return new WaitForFixedUpdate();
            }
            
            SimulateReleaseInput(inputDevice, inputPath);
            
            for (int i = 0; i < 2; i++)
            {
                yield return new WaitForFixedUpdate();
            }
        }
        
        [OneTimeSetUp]
        public void LoadScene()
        {
            SceneManager.LoadScene("Scenes/Level_00");
        }

        [TearDown]
        public void CleanupInputSimulation()
        {
            // To avoid getting sticky simulated key at the end of a test,
            // that would make the next test (or even normal Play) start with a simulated key pressed and
            // prevent recognising the next same key press, we must cleanup all simulated key presses.
            // I don't know of a function that would reset the whole input state unfortunately, so we have
            // to do this for every key potentially simulated.
            // That said, you should use ShortPress or manually Release your key inputs at the end of each test
            // to be safe. To avoid releasing again for nothing, we only Release if key is still pressed.
            
            var keyboardDevice = InputSystem.GetDevice<Keyboard>();
            if (keyboardDevice != null)
            {
                if (keyboardDevice.cKey.isPressed)
                {
                    SimulateReleaseInput(keyboardDevice, "c");
                }
            }
        }
        
        // Simulation test with direct player input manipulation
        // This is simply to avoid making a custom assembly for game scripts and referencing them for now,
        // as this would be required to manually call the Melee Attack method on the Dragon
        [UnityTest]
        public IEnumerator TestScript_MeleeAttack_CancelAtTheLastFrame()
        {
            // Inspired by https://forum.unity.com/threads/simulating-input-via-code.397499/
            // Adapted to get a precise device type
            
            var keyboardDevice = InputSystem.GetDevice<Keyboard>();
            if (keyboardDevice == null)
            {
                Assert.Inconclusive("No keyboard device found, cannot test player input");
            }
            
            // wait a little time to give developer a chance to see the first interesting action
            yield return new WaitForSeconds(1f);
                        
            // we cannot yield return null in the middle of using(), as it would cause resource to be deallocated
            // and runtime error:
            // "System.ObjectDisposedException : The Unity.Collections.NativeArray`1[System.Byte] has been deallocated, it is not allowed to access it"
            // however we must wait at least one frame before releasing the attack input, else it won't be pressed at all
            // so, for each event, open a unique using scope and change the input state
            
            yield return SimulateShortPressInput(keyboardDevice, "c");

            for (int i = 0; i < 6; ++i)
            {
                yield return new WaitForFixedUpdate();
            }
            
            yield return SimulateShortPressInput(keyboardDevice, "c");

            for (int i = 0; i < 7; ++i)
            {
                yield return new WaitForFixedUpdate();
            }

            yield return SimulateShortPressInput(keyboardDevice, "c");

            for (int i = 0; i < 8; ++i)
            {
                yield return new WaitForFixedUpdate();
            }

            yield return SimulateShortPressInput(keyboardDevice, "c");

            for (int i = 0; i < 9; ++i)
            {
                yield return new WaitForFixedUpdate();
            }

            yield return SimulateShortPressInput(keyboardDevice, "c");

            for (int i = 0; i < 10; ++i)
            {
                yield return new WaitForFixedUpdate();
            }

            yield return SimulateShortPressInput(keyboardDevice, "c");

            for (int i = 0; i < 11; ++i)
            {
                yield return new WaitForFixedUpdate();
            }

            yield return SimulateShortPressInput(keyboardDevice, "c");

            for (int i = 0; i < 12; ++i)
            {
                yield return new WaitForFixedUpdate();
            }

            yield return SimulateShortPressInput(keyboardDevice, "c");

            for (int i = 0; i < 13; ++i)
            {
                yield return new WaitForFixedUpdate();
            }

            yield return SimulateShortPressInput(keyboardDevice, "c");

            for (int i = 0; i < 14; ++i)
            {
                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
