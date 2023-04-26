using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hackathon.Commands
{
    public class HitPlayerCommand : ICommand
    {
        private SpaceshipBehaviour _spaceship;
        private PlayerBehaviour _playerBehaviour;

        public HitPlayerCommand(SpaceshipBehaviour spaceship, PlayerBehaviour player)
        {
            _spaceship = spaceship;
            _playerBehaviour = player;
        }


        public void Execute()
        {
            _playerBehaviour.ApplyDamage(_spaceship.currentHealth);
        }
    }
}
