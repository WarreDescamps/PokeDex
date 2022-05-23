using Common;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using PokeApi.Model;
using PokeDex.WpfApp.ViewModel;
using PokeDex.WpfApp.ViewModel.Commands;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PokeDex.WpfApp.Model
{
    public class PokemonItem : Grid
    {
        private readonly Dictionary<string, string> TypeColors = new()
        {
            { "grass", "#3e9709" },
            { "fire", "#f67f0b" },
            { "water", "#0a7abc" },
            { "normal", "#ccc9aa" },
            { "flying", "#5eb9b2" },
            { "bug", "#bddd6e" },
            { "poison", "#a819d7" },
            { "electric", "#fffa24" },
            { "ground", "#e1d158" },
            { "fighting", "#e81319" },
            { "psychic", "#ec0e63" },
            { "rock", "#776a3e" },
            { "ice", "#1995a1" },
            { "ghost", "#8e55a4" },
            { "dragon", "#8a55fd" },
            { "dark", "#5f4632" },
            { "steel", "#7b8e8a" },
            { "fairy", "#ffa0c2" },
        };
        private const double TextSize = 20;
        private readonly BrushConverter _brushConverter;

        private Image _image;
        private Label _nCaught;
        private Label _name;
        private Border _type1;
        private Border _type2;
        private CartesianChart _chart;
        private Label _height;
        private Label _weight;
        private TextBlock _description;

        public PokemonItem() { }

        public PokemonItem(Pokemon pokemon, bool isCaught, int nCaught)
        {
            Pokemon = pokemon;
            Name = pokemon.Name;
            _brushConverter = new BrushConverter();

            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.5, GridUnitType.Star) });
            RowDefinitions.Add(new RowDefinition()); 
            RowDefinitions.Add(new RowDefinition());
            RowDefinitions.Add(new RowDefinition());
            RowDefinitions.Add(new RowDefinition());

            _image = new Image
            {
                Source = BitmapConverter.Convert(isCaught ? pokemon.Image : pokemon.Silhouette)
            };
            var image = new Border
            {
                CornerRadius = new CornerRadius(19),
                BorderBrush = (Brush)_brushConverter.ConvertFromString("#c6b362"),
                BorderThickness = new Thickness(4),
                Child = new Border
                {
                    CornerRadius = new CornerRadius(14),
                    BorderBrush = (Brush)_brushConverter.ConvertFromString("#fffed4"),
                    BorderThickness = new Thickness(4),
                    Child = new Border
                    {
                        CornerRadius = new CornerRadius(10),
                        BorderBrush = (Brush)_brushConverter.ConvertFromString("#afce9d"),
                        BorderThickness = new Thickness(4),
                        Background = 
                        new LinearGradientBrush(
                            new GradientStopCollection(
                                new List<GradientStop>
                                {
                                    new(Brushes.White.Color, 0),
                                    new(Brushes.White.Color, 0.5),
                                    new((Color)ColorConverter.ConvertFromString("#f9f7ff"), 0.5),
                                    new((Color)ColorConverter.ConvertFromString("#f9f7ff"), 1),
                                }))
                        {
                            StartPoint = new Point(1, 0),
                            EndPoint = new Point(1, 1),
                            SpreadMethod = GradientSpreadMethod.Repeat,
                            RelativeTransform = new ScaleTransform(0.12, 0.12)
                        },
                        Child = _image
                    }
                }
            };
            SetColumn(image, 0);
            SetRow(image, 0);
            SetRowSpan(image, 4);
            Children.Add(image);

            _name = new Label
            {
                Content = isCaught ? $"{pokemon.Id} {pokemon.Name.FirstToUpper()}\n{pokemon.Genus}" : $"{Censor(pokemon.Id.ToString(), pokemon.Name)}\n{Censor(pokemon.Genus.Split(' '))}",
                FontSize = TextSize,
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            SetColumn(_name, 0);
            _nCaught = new Label
            {
                Content = isCaught ? $"Caught {nCaught} time{(nCaught > 1 ? "s" : "")}" : "",
                FontSize = TextSize,
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            SetColumn(_nCaught, 1);
            var nameGrid = new Grid();
            nameGrid.ColumnDefinitions.Add(new());
            nameGrid.ColumnDefinitions.Add(new());
            nameGrid.Children.Add(_name);
            nameGrid.Children.Add(_nCaught);
            var name = new Border
            {
                CornerRadius = new CornerRadius(10),
                Background = (Brush)_brushConverter.ConvertFromString("#ffffc6"),
                Child = nameGrid
            };
            SetColumn(name, 1);
            SetColumnSpan(name, 2);
            SetRow(name, 0);
            Children.Add(name);

            var typePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            typePanel.Children.Add(new Label
            {
                Content = "Types:  ",
                FontSize = TextSize,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10),
            });
            typePanel.Children.Add(_type1 = new Border
            {
                Child = new Label
                {
                    Content = isCaught ? pokemon.Types.First().Name.ToUpper() : Censor(pokemon.Types.First().Name.ToUpper()),
                    FontSize = TextSize,
                    FontWeight = FontWeights.SemiBold,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, -5, 5, -5),
                },
                Background = isCaught ? (Brush)_brushConverter.ConvertFromString(TypeColors[pokemon.Types.First().Name]) : Brushes.White,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(3),
                CornerRadius = new CornerRadius(3),
                VerticalAlignment = VerticalAlignment.Center,
            });
            if (pokemon.Types.Count() > 1)
            {
                typePanel.Children.Add(_type2 = new Border
                {
                    Child = new Label
                    {
                        Content = isCaught ? pokemon.Types.Last().Name.ToUpper() : Censor(pokemon.Types.Last().Name.ToUpper()),
                        FontSize = TextSize,
                        FontWeight = FontWeights.SemiBold,
                        Margin = new Thickness(5, -5, 5, -5),
                    },
                    Background = isCaught ? (Brush)_brushConverter.ConvertFromString(TypeColors[pokemon.Types.Last().Name]) : Brushes.White,
                    BorderBrush = Brushes.Black,
                    Margin = new Thickness(5, 0, 0, 0),
                    BorderThickness = new Thickness(3),
                    CornerRadius = new CornerRadius(3),
                    VerticalAlignment = VerticalAlignment.Center,
                });
            }
            SetColumn(typePanel, 1);
            SetColumnSpan(typePanel, 2);
            SetRow(typePanel, 1);
            Children.Add(typePanel);

            _height = new Label
            {
                Content = "HT:  " + (isCaught ? pokemon.Height : Censor(pokemon.Height)),
                FontSize = TextSize,
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Center
            };
            SetColumn(_height, 1);
            SetRow(_height, 2);
            Children.Add(_height);

            _weight = new Label
            {
                Content = "WT:  " + (isCaught ? pokemon.Weight : Censor(pokemon.Weight)),
                FontSize = TextSize,
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Center
            };
            SetColumn(_weight, 1);
            SetRow(_weight, 3);
            Children.Add(_weight);

            var chartGrid = new Grid
            {
                Margin = new Thickness(0, 0, 10, 5)
            };
            chartGrid.Children.Add(_chart = new CartesianChart
            {
                Series = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Values = isCaught ? new ChartValues<int>(pokemon.Stats.Select(s => s.Base)) : new ChartValues<int>(new int[]{ 128, 128, 128,128, 128, 128 }),
                        DataLabels = true,
                        LabelPoint = p => isCaught ?  p.Y.ToString() : "?",
                    }
                },
                AxisX = new AxesCollection
                {
                    new()
                    {
                        Labels = pokemon.Stats.Select(s => StatName(s.Name)).ToList(),
                    }
                },
                AxisY = new AxesCollection
                {
                    new()
                    {
                        Title = "Base Stat",
                        MaxValue = 255,
                        MinValue = 1
                    }
                },
                BorderBrush = (Brush)_brushConverter.ConvertFromString("#ffffc6"),
                BorderThickness = new Thickness(2)
            });
            chartGrid.Children.Add(new Button
            {
                Content = "Save",
                Height = 20,
                Width = 35,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Command = new Command(SaveStats),
                Margin = new Thickness(5)
            });
            SetColumn(chartGrid, 2);
            SetRow(chartGrid, 2);
            SetRowSpan(chartGrid, 2);
            Children.Add(chartGrid);

            _description = new TextBlock
            {
                Text = isCaught ? pokemon.FlavorText : Censor(pokemon.FlavorText.Split(' ')),
                TextWrapping = TextWrapping.Wrap,
                FontSize = TextSize,
                Background = Brushes.White,
                Padding = new Thickness(10, 0, 10, 0)
            };
            var description = new Grid();
            description.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            description.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8, GridUnitType.Star) });
            description.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            var sideBorderLeft = new Border
            {
                CornerRadius = new CornerRadius(10),
                Background = (Brush)_brushConverter.ConvertFromString("#6c93cf"),
                Margin = new Thickness(20, 10, 20, 10)
            };
            SetColumn(sideBorderLeft, 0);
            description.Children.Add(sideBorderLeft);
            var textBorder = new Border
            {
                CornerRadius = new CornerRadius(10),
                Background = (Brush)_brushConverter.ConvertFromString("#86adf9"),
                Margin = new Thickness(0, 5, 0, 5),
                Child = _description
            };
            SetColumn(textBorder, 1);
            description.Children.Add(textBorder);
            var sideBorderRight = new Border
            {
                CornerRadius = new CornerRadius(10),
                Background = (Brush)_brushConverter.ConvertFromString("#6c93cf"),
                Margin = new Thickness(20, 10, 20, 10)
            };
            SetColumn(sideBorderRight, 2);
            description.Children.Add(sideBorderRight);

            var desctriptionBorder = new Border
            {
                CornerRadius = new CornerRadius(5),
                Background = (Brush)_brushConverter.ConvertFromString("#86adf9"),
                Child = description
            };
            SetColumn(desctriptionBorder, 0);
            SetColumnSpan(desctriptionBorder, 3);
            SetRow(desctriptionBorder, 4);


            Children.Add(desctriptionBorder);
        }

        private string Censor(string? text)
        {
            return new string('?', text?.Length ?? 0);
        }

        private string Censor(params string[] strings)
        {
            string text = string.Empty;
            foreach (string str in strings)
            {
                text += Censor(str) + " ";
            }
            return text.TrimEnd();
        }

        private string StatName(string name)
        {
            if (name.Contains('-'))
            {
                var parts = name.Split('-');
                if (parts.Last().Contains("att"))
                    name = "Sp. Atk";
                else
                    name = "Sp. Def";
            }
            else if (name == "hp")
                name = name.ToUpper();
            else
                name = name.FirstToUpper();
            return name;
        }

        private void SaveStats()
        {
            var dialog = new SaveFileDialog { FileName = $"{Pokemon.Name.Replace(".", "").Replace(' ', '_')}_stats", DefaultExt = ".png", Filter = "Image (.png)|*.png" };
            string? path = dialog.ShowDialog() ?? false ? dialog.FileName : null;
            if (path == null)
                return;
            var bitmap = new RenderTargetBitmap((int)_chart.ActualWidth, (int)_chart.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(_chart);

            var encoder = new PngBitmapEncoder();
            var frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);

            using (var f = new FileStream(dialog.FileName, FileMode.OpenOrCreate))
            {
                encoder.Save(f);
            }
        }

        public void Update(bool isCaught, int nCaught)
        {
            if (!isCaught)
                return;

            _image.Source = BitmapConverter.Convert(Pokemon.Image);

            _name.Content = $"{Pokemon.Id} {Pokemon.Name.FirstToUpper()}\n{Pokemon.Genus}";

            _nCaught.Content = $"Caught {nCaught} time{(nCaught > 1 ? "s" : "")}";

            (_type1.Child as Label).Content = Pokemon.Types.First().Name.ToUpper();
            _type1.Background = (Brush)_brushConverter.ConvertFromString(TypeColors[Pokemon.Types.First().Name]);

            if (_type2 is not null)
            {
                (_type2.Child as Label).Content = Pokemon.Types.Last().Name.ToUpper();
                _type2.Background = (Brush)_brushConverter.ConvertFromString(TypeColors[Pokemon.Types.Last().Name]);
            }

            _height.Content = "HT:  " + Pokemon.Height;

            _weight.Content = "WT:  " + Pokemon.Weight;

            _chart.Series.First().Values = new ChartValues<int>(Pokemon.Stats.Select(s => s.Base));
            _chart.Series.First().LabelPoint = p => p.Y.ToString();

            _description.Text = Pokemon.FlavorText;
        } 

        public Pokemon Pokemon { get; }

        public string Name { get; }
    }
}
