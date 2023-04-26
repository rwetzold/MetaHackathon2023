using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hackathon.Commands
{
    public class AttacCommand : ICommand
    {

        private readonly TowerBehaviour _attacer;

        private readonly SpaceshipBehaviour _target;

        public AttacCommand(TowerBehaviour attacer, SpaceshipBehaviour target)
        {
            _attacer = attacer;
            _target = target;
        }
        public void Execute()
        {
            _target.ApplyDamage(_attacer.armedAttributes.DamageValue);
        }

    }
}
