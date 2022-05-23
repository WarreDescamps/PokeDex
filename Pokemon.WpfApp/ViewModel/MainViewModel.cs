using Common;
using Microsoft.Win32;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PokeApi;
using PokeApi.Model;
using PokeDex.WpfApp.Model;
using PokeDex.WpfApp.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PokeDex.WpfApp.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private const int MaxPokemon = 151;
        private PokeApiClient _pokeClient;
        private Random _random;
        private Pokemon? _selectedPokemon;
        private CsvManager<PokemonResult> _csvPokemon;
        private CsvManager<PokemonTime> _csvTimeout;
        private IEnumerable<PokemonResult> _pokemonResults;

        public MainViewModel()
        {
            _pokeClient = new PokeApiClient();
            _random = new Random();
            _csvPokemon = new CsvManager<PokemonResult>("./pokemons.csv");
            _csvTimeout = new CsvManager<PokemonTime>("./timeout.csv");

            _ = Initialize();

            SearchCommand = new AsyncCommand<string>(Search);
            GraduateCommand = new Command(Graduate);
            ClearCommand = new Command(Clear);
        }

        private async Task Initialize()
        {
            SelectedTabIndex = 0;
            OnPropertyChanged("SelectedTabIndex");

            _pokemonResults = (await _pokeClient.GetPokemonAsync(MaxPokemon)).Results;

            PokemonListItems = new ObservableCollection<PokemonListItem>();
            for (int i = 1; i <= MaxPokemon; i++)
            {
                var pokemon = await _pokeClient.GetPokemonAsync(i.ToString());
                var pokemonListItem = new PokemonListItem(pokemon, _csvPokemon.Objects.Any(pr => pr.Name == pokemon.Name));
                pokemonListItem.MouseDown += OnMouseDownOnPokemonItem;
                PokemonListItems.Add(pokemonListItem);
                OnPropertyChanged("PokemonListItems");
                OnPropertyChanged("CatchCommand");
            }
        }

        private void OnMouseDownOnPokemonItem(object obj, MouseEventArgs e)
        {
            _ = Search((obj as PokemonListItem)?.Pokemon.Name);

            SelectedTabIndex = 0;
            OnPropertyChanged("SelectedTabIndex");
        }

        private async Task Search(string? name = null)
        {
            Pokemon? pokemon = null;
            if (string.IsNullOrEmpty(name))
            {
                int id = _random.Next(0, MaxPokemon + 1);
                pokemon = await _pokeClient.GetPokemonAsync(id.ToString());
            }
            else do
            {
                var genPokemon = _pokemonResults.FirstOrDefault(p => p.Name.ToLower() == name.ToLower() || p.Url[..^1].Split('/').Last() == name) ??
                        _pokemonResults.FirstOrDefault(p => p.Name.ToLower().Contains(name.ToLower()));
                if (genPokemon != null)
                    pokemon = await _pokeClient.GetPokemonAsync(genPokemon.Name);
                name = name[..^1];
            } while (pokemon == null && name.Length > 0);

            _selectedPokemon = pokemon;
            PokemonItem = new PokemonItem(pokemon, _csvPokemon.Objects.GroupBy(pr => pr.Name).Any(pr => pr.Key == pokemon.Name), _csvPokemon.Objects.Count(pr => pr.Name == _selectedPokemon.Name));
            OnPropertyChanged("PokemonItem");
            OnPropertyChanged("CatchCommand");
        }

        private void Catch()
        {
            var catchAttempts = _csvTimeout.Objects.Where(pt => pt.Name == _selectedPokemon.Name).OrderBy(pt => pt.Time);
            if (catchAttempts.Count() >= 3)
            {
                var timeDif = catchAttempts.Last().Time.Subtract(DateTime.Now).Add(TimeSpan.FromMinutes(3));
                if (timeDif > TimeSpan.Zero)
                {
                    MessageBox.Show($"The Pokemon is exhausted!\r\nYou'll have to wait {timeDif.ToStringX()} for it to recover.");
                    return;
                }
                _csvTimeout.RemoveAll(p => p.Name == _selectedPokemon.Name).Write();
            }
            if (_random.Next(0, 10) <= 6)
            {
                _csvTimeout.AddObject(_selectedPokemon)
                    .Write();
                MessageBox.Show("Oh, no!\r\nThe POKEMON broke free!");
                return;
            }
            _csvPokemon.AddObject(_pokemonResults.First(pr => pr.Name == _selectedPokemon.Name)).Write();
            _csvTimeout.RemoveAll(p => p.Name == _selectedPokemon.Name).Write();

            PokemonListItems.First(pli => pli.Pokemon.Name == _selectedPokemon.Name).Update(true);
            //OnPropertyChanged("PokemonListItems");

            PokemonItem.Update(true, _csvPokemon.Objects.Where(pr => pr.Name == _selectedPokemon.Name).Count());
            OnPropertyChanged("PokemonItem");

            MessageBox.Show($"Gotcha!\r\n{_selectedPokemon.Name.ToUpper()} was caught!");
        }

        private void Graduate()
        {
            if (_csvPokemon.Objects.GroupBy(pr => pr.Name).Count() != MaxPokemon)
            {
                MessageBox.Show($"You have to catch all {MaxPokemon} Pokemons to earn your diploma!\nCome back once your Pokédex is filled up!");
                return;
            }

            var dialog = new SaveFileDialog() { FileName = "Pokemon Diploma", DefaultExt = ".pdf", Filter = "PDF Document (.pdf)|*.pdf" };
            string? path = dialog.ShowDialog() ?? false ? dialog.FileName : null;
            if (path == null)
                return;

            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 20);
            gfx.DrawString("You have earned The Pokemon Diploma!\nThe only people who can have this document are those who have filled their Pokédex!", font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height),
            XStringFormat.Center);
            document.Save(path);
        }

        private void Clear()
        {
            var result = MessageBox.Show("This will clear you whole Pokédex!\nAre you sure you want to do this?", "WARNING!", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                _csvPokemon.RemoveAll().Write();
                _csvTimeout.RemoveAll().Write();

                foreach (var pokemon in PokemonListItems)
                {
                    pokemon.Update(false);
                    //OnPropertyChanged("PokemonListItems");
                }
            }
        }

        public PokemonItem PokemonItem { get; private set; }
        public ObservableCollection<PokemonListItem> PokemonListItems { get; private set; }
        public int SelectedTabIndex { get; set; }

        public Command GraduateCommand { get; }
        public Command ClearCommand { get; }
        public AsyncCommand<string> SearchCommand { get; }
        public CatchCommand CatchCommand => new(Catch, _selectedPokemon, PokemonListItems?.Any(pli => pli.Pokemon.Id >= _selectedPokemon?.Id));

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
