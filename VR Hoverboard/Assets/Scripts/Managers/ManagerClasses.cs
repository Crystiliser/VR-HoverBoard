using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates { MainMenu, GamePlay, GameOver, SceneTransition };
public enum GameModes { Continuous, Cursed, Free, GameModesSize }

public class ManagerClasses : MonoBehaviour
{
    public class RoundTimer
    {
        float timeLeft;
        float timeInLevel;
        bool timersPaused = false;

        public RoundTimer(float sTime = 5.0f) { timeLeft = sTime; }
        public void PauseTimer(bool paused) { timersPaused = paused; }
        public void IncreaseTimeLeft(float iTime) { if (!timersPaused) timeLeft += iTime; }
        public float TimeLeft { get { return timeLeft; } set { timeLeft = value; } }
        public float TimeInLevel { get { return timeInLevel; } set { timeInLevel = value; } }
        public void UpdateTimers()
        {
            if (!timersPaused)
            {
                if (timeLeft - Time.deltaTime > 0f)
                    timeLeft -= Time.deltaTime;
                else
                    timeLeft = 0f;

                timeInLevel += Time.deltaTime;
            }
        }
    }

    public class GameState
    {
        public GameStates currentState;
        public GameState() { currentState = GameStates.MainMenu; }
    }

    public class GameMode
    {
        public GameModes currentMode;

        public GameMode() { currentMode = GameModes.Continuous; }

        public void NextMode()
        {
            if (currentMode + 1 >= GameModes.GameModesSize)
                currentMode = 0;
            else
                ++currentMode;
        }

        public void PreviousMode()
        {
            if (currentMode - 1 < 0)
                currentMode = GameModes.GameModesSize - 1;
            else
                --currentMode;
        }
    }

    [System.Serializable]
    public class PlayerMovementVariables
    {
        [Header("Speed Values")]
        public float maxSpeed = 10f;
        public float restingSpeed = 5f;
        public float minSpeed = 2f;

        [Header("Acceleration Values")]
        public float downwardAcceleration = 30f;
        public float restingAcceleration = 30f;
        public float upwardAcceleration = 30f;
        [Tooltip("The amount to interpolate on every fixed update.")]
        [Range(0.001f, 1f)] public float momentum = 0.1f;

        [Header("Sensitivities")]
        [Tooltip("Pitch has no effect on gyro sensitivities.")]
        [Range(0.5f, 5f)] public float pitchSensitivity = 3f;
        [Range(0.5f, 5f)] public float yawSensitivity = 3f;

        [Header("Angles")]
        [Range(10f, 75f)] public float maxDescendAngle = 30f;
        [Tooltip("The threshold at which the object returns to Resting Speed.")]
        [Range(0f, 20f)] public float restingThreshold = 10f;
        [Range(10f, 75f)] public float maxAscendAngle = 30f;

        [Header("Rigid Body Values")]
        [Range(0f, 10f)] public float bounceModifier = 1f;
        public float mass = 1f;
        public float drag = 1f;
        [Tooltip("Angular drag only affects movement after colliding with an object.")]
        public float angularDrag = 5f;

        public PlayerMovementVariables() { }
        public PlayerMovementVariables
		(float dAccel, float rAccel, float uAccel, float mmntm, 
			float maxSpd, float restSpd, float minSpd, 
			float pitchSens, float yawSens, float maxDAng, float RAng, float maxAAngle, 
			float bMod, float ms, float drg, float angDrg)
        {
            downwardAcceleration = dAccel;
            restingAcceleration = rAccel;
            upwardAcceleration = uAccel;
            momentum = mmntm;

            maxSpeed = maxSpd;
            restingSpeed = restSpd;
            minSpeed = minSpd;

            pitchSensitivity = pitchSens;
            yawSensitivity = yawSens;
            maxDescendAngle = maxDAng;
            restingThreshold = RAng;
            maxAscendAngle = maxAAngle;

            bounceModifier = bMod;
            mass = ms;
            drag = drg;
            angularDrag = angDrg;
        }
    }

}
