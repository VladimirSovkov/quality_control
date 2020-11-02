using System.Collections.Generic;

namespace Car
{
    public class Car
    {
        private bool m_isEngineOn = false;
        private int m_speed = 0;
        private int m_currentGearing = 0;

        private const int MAX_SPEED = 150;
        private const int MAX_GEAR = 5;

        Dictionary<int, List<int>> AllowableSpeedOnGear = new  Dictionary<int, List<int>>
        {
	        {-1, new List<int>{0, 20}},
	        {0, new List<int>{0, 150}},
	        {1, new List<int>{0, 30}},
	        {2, new List<int>{20, 50}},
	        {3, new List<int>{30, 60}},
        	{4, new List<int>{40, 90}},
	        {5, new List<int>{50, 150}},
        }; 

        public enum DrivingDirection
        {
            Forward,
            Back,
            StandStill
        }
        public bool TurnOnEngine()
        {
            m_isEngineOn = true;
            return true;
        }

        public bool TurnOffEngine()
        {
            if (m_currentGearing == 0 && m_speed == 0)
            {
                m_isEngineOn = false;
                return true;
            }

            return false;
        }

        private bool IsExceptionSituationToShiftGear(int gear)
        {
	        return (gear == 0) ||
		        (gear == m_currentGearing) ||
		        (gear == -1 && m_speed == 0) ||
		        (gear == 1 && m_currentGearing == -1 && m_speed == 0);
        }

        public bool SetGear(int gear)
        {
            if (!m_isEngineOn && gear != 0)
            {
                return false;
            }

            if (gear > 1 && m_speed < 0)
            {
                return false;
            }

            if (IsExceptionSituationToShiftGear(gear))
            {
                m_currentGearing = gear;
                return true;
            }

            if (gear > -1 && gear <= MAX_GEAR && m_currentGearing != -1)
            {
                int maxSpeedForGear = AllowableSpeedOnGear[gear][1];
                int minSpeedForGear = AllowableSpeedOnGear[gear][0];
                if (minSpeedForGear <= m_speed && m_speed <= maxSpeedForGear)
                {
                    m_currentGearing = gear;
                    return true;
                }
            }

            return false;
        }

        private bool IsIncorrectSituationToChangeSpeed(int speed, int currentSpeed)
        {
            return (speed > MAX_SPEED || speed < 0) ||
                (speed > currentSpeed && m_currentGearing == 0) ||
                (AllowableSpeedOnGear[m_currentGearing][0] > speed || speed > AllowableSpeedOnGear[m_currentGearing][1]);
        }

        public bool SetSpeed(int speed)
        {
            if (m_isEngineOn == false)
            {
                return false;
            }

            if (IsIncorrectSituationToChangeSpeed(speed, GetSpeed()))
            {
                return false;
            }

            if (m_currentGearing == -1 || m_speed < 0)
            {
                speed *= -1;
            }

            m_speed = speed;
            return true;
        }

        public bool GetIsEngineOn()
        {
            return m_isEngineOn;
        }

        public DrivingDirection GetDrivingDirectionCar()
        {
            if (m_speed > 0)
            {
                return DrivingDirection.Forward;
            }
            else if (m_speed < 0)
            {
                return DrivingDirection.Back;
            }
            else
            {
                return DrivingDirection.StandStill;
            }
        }

        public int GetGear()
        {
            return m_currentGearing;
        }

        public int GetSpeed()
        {
            return m_speed;
        }
    }
}
