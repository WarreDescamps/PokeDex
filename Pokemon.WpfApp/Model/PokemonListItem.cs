using PokeApi.Model;
using PokeDex.WpfApp.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PokeDex.WpfApp.Model
{
    public class PokemonListItem : Border
    {
        private Image _image;

        public PokemonListItem(Pokemon pokemon, bool isCaught)
        {
            Pokemon = pokemon;

            BorderThickness = new Thickness(2);
            BorderBrush = Brushes.Black;
            Background = Brushes.White;

            var panel = new StackPanel();
            panel.Children.Add(new Label()
            {
                Content = pokemon.Id,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            panel.Children.Add(_image = new Image()
            {
                Source = BitmapConverter.Convert(isCaught ? pokemon.Image : pokemon.Silhouette)
            });
            Child = panel;

            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            BorderBrush = Brushes.Red;
            BorderThickness = new Thickness(4);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            BorderBrush = Brushes.Black;
            BorderThickness = new Thickness(2);
        }

        public void Update(bool isCaught)
        {
            _image.Source = BitmapConverter.Convert(isCaught ? Pokemon.Image : Pokemon.Silhouette);
        }

        public Pokemon Pokemon { get; private set; }
    }
}
