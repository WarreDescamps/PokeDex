using PokeApi.Model;
using System;

namespace PokeDex.WpfApp.ViewModel.Commands
{
    public class CatchCommand : Command
    {
        private Pokemon? _pokemon;
        private bool? _canExecute;

        public CatchCommand(Action action, Pokemon? pokemon, bool? canExecute) : base(action)
        {
            _pokemon = pokemon;
            _canExecute = canExecute;
        }

        public override bool CanExecute(object? parameter)
        {
            if (_pokemon != null && (_canExecute ?? false))
                return true;
            return false;
        }
    }
}
