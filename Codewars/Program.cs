using System;
using System.Collections.Generic;
using System.Linq;

namespace Codewars
{
    class Program
    {
        static void Main(string[] args)
        {
            // int[][] queues =
            // {
            //     new int[0], // G
            //     new int[]{0}, // 1
            //     new int[0], // 2
            //     new int[0], // 3
            //     new int[]{2}, // 4
            //     new int[]{3}, // 5
            //     new int[0], // 6
            // };
            // var a = TheLift(queues, 5);
            // foreach (var i in a)
            // {
            //     Console.Write(i);
            // }
            //
            // Console.WriteLine();

            var queues = new []
            {
                new int[]{}, // G
                new int[]{2}, // 1
                new int[]{3,3,3}, // 2
                new int[]{1}, // 3
                new int[]{}, // 4
                new int[]{}, // 5
                new int[]{}, // 6
            };
            var a = TheLift(queues, 1);
        }

        public static int[] TheLift(int[][] queues, int capacity)
        {
            var lift = new Lift(queues, capacity);
            return lift.Operate();
        }
    }

    public enum CurrentDirection
    {
        Up = 0,
        Down = 1
    }

    public class Lift
    {
        public Lift(int[][] queues, int capacity)
        {
            _queues = queues;
            _capacity = capacity;
        }

        private int[][] _queues;
        private int _capacity;

        private CurrentDirection _currentDirection = CurrentDirection.Up;

        private int _maxFloor => _queues.Length - 1;
        private int _minFloor = 0;
        private int _numberOfPeople => _currentPeople.Count;
        private bool _isEmpty => _numberOfPeople == 0;

        private List<int> _currentPeople = new List<int>(100);
        private List<int> _floorLog = new List<int>(new []{0});

        private int _currentFloor = 0;

        private void Enter()
        {
            var numberOfPeople = _numberOfPeople;
            for (int i = 0; i < _capacity - numberOfPeople; i++)
            {
                var currentQueue = _queues[_currentFloor];

                if (_currentDirection == CurrentDirection.Up
                    ? !currentQueue.Any(u => u > _currentFloor)
                    : !currentQueue.Any(u => u < _currentFloor))
                    return;

                var personToEnter = _currentDirection == CurrentDirection.Up
                    ? currentQueue.First(u => u > _currentFloor)
                    : currentQueue.First(u => u < _currentFloor);
            
                _currentPeople.Add(personToEnter);

                int indexOfPersonInQueue = Array.IndexOf(currentQueue, personToEnter);
                _queues[_currentFloor] = currentQueue.Where((_, index) => index != indexOfPersonInQueue).ToArray();
            }
        }

        /// <summary>
        /// number of people exited
        /// </summary>
        /// <returns></returns>
        private void Exit()
        {
            if (_currentPeople.Any(u => u == _currentFloor))
                _currentPeople = _currentPeople.Where(u => u != _currentFloor).ToList();
        }

        public int[] Operate()
        {
            while (!(_queues.All(u => u == null || u.Length == 0) && _numberOfPeople == 0))
            {
                if (!NeedToStop())
                {
                    MoveNextFloor();

                    continue;
                }

                if (_floorLog.Last() != _currentFloor)
                    _floorLog.Add(_currentFloor);
                
                Exit();

                Enter();

                MoveNextFloor();
            }
            
            if (_floorLog.Last() != 0)
                _floorLog.Add(0);

            return _floorLog.ToArray();
        }

        private void ChangeDirection()
            => _currentDirection = _currentDirection == CurrentDirection.Down 
                ? CurrentDirection.Up 
                : CurrentDirection.Down;

        private bool NeedToStop()
        {
            var currentQueue = _queues[_currentFloor];
            return _currentDirection == CurrentDirection.Up 
                ? _currentPeople.Any(u => u == _currentFloor) || currentQueue.Any(u => u > _currentFloor)
                : _currentPeople.Any(u => u == _currentFloor) || currentQueue.Any(u => u < _currentFloor);
        }

        private void MoveNextFloor()
        {
            if (_currentDirection == CurrentDirection.Down)
            {
                if (_currentFloor - 1 < _minFloor)
                {
                    ChangeDirection();
                    return;
                }

                --_currentFloor;
            }

            if (_currentDirection == CurrentDirection.Up)
            {
                if (_currentFloor + 1 > _maxFloor)
                {
                    ChangeDirection();
                    return;
                }

                ++_currentFloor;
            }
        }
    }
}