using System;
using Car;
using Xunit;

namespace carTest
{
    public class CreationOfDefaultValues
    {
        [Fact]
        public void DefaultValues()
        {
            Car.Car car = new Car.Car();

            Assert.Equal(0, car.GetGear());
            Assert.False(car.GetIsEngineOn());
            Assert.Equal(0, car.GetSpeed());
        }
    }

    public class StartingEngine
    {
        [Fact]
        public void FirstCorrectEngineStart()
        {
            Car.Car car = new Car.Car();
            car.TurnOnEngine();

            Assert.True(car.GetIsEngineOn());
        }
    }

    public class SwitchinGear
    {
        [Fact]
        public void CorrectSwitching()
        {
            Car.Car car = new Car.Car();

            car.TurnOnEngine();
            Assert.True(car.SetGear(1));
            Assert.Equal(1, car.GetGear());
            
            car.SetSpeed(20);
            Assert.True(car.SetGear(2));
            Assert.Equal(2, car.GetGear());

            car.SetSpeed(45);
            Assert.True(car.SetGear(3));
            Assert.Equal(3, car.GetGear());

            Assert.True(car.SetGear(50));
            Assert.Equal(4, car.GetGear());

            car.SetSpeed(80);
            Assert.True(car.SetGear(5));
            Assert.Equal(5, car.GetGear());

            car.SetSpeed(50);
            Assert.True(car.SetGear(2));
            car.SetSpeed(20);
            Assert.True(car.SetGear(1));
            car.SetSpeed(0);
            car.SetGear(-1);
        }

        [Fact]
        public void OnMuffledEngine()
        {
            Car.Car car = new Car.Car();

            for (int i = -1; i <= 5; i++)
            {
                car.SetGear(i);
                Assert.Equal(0, car.GetGear());
            }
        }


    }
}
