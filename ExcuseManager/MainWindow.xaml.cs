using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;

namespace ExcuseManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Used excuse file path or null if no file is loaded.
        /// </summary>
        private string ExcuseFile = null;
        /// <summary>
        /// Reference to ExcuseList.
        /// </summary>
        private ExcuseList Excuses;
        /// <summary>
        /// Reference to the ExcuseViewModel (MVVM (=MVC) pattern).
        /// </summary>
        private ExcuseViewModel ViewModel;
        /// <summary>
        /// Random number generator for random ecxuse function.
        /// </summary>
        private Random rand;
        
        public MainWindow()
        {
            InitializeComponent();
            //init random number generator
            rand = new Random();
            //create view model instance
            ViewModel = new ExcuseViewModel();
            //register as property changed listener to update stars
            ViewModel.PropertyChanged += StarHandler;
            //init with a readonly default excuse
            ViewModel.SetExcuse(Excuse.DefaultExcuse);
            //set data context to view model
            contentGrid.DataContext = ViewModel;
            
            statusLabel.Content = "No File";
        }

        /// <summary>
        /// Updates the rating stars when the rating is changed.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="args">event args providing property name</param>
        private void StarHandler(object sender, PropertyChangedEventArgs args)
        {
            if(args.PropertyName.Equals("Rating") || args.PropertyName.Equals("Rated"))
            {
                SolidColorBrush stroke = new SolidColorBrush(ViewModel.Rated ? Colors.Black : Colors.Red);
                SolidColorBrush fill = new SolidColorBrush(Colors.Blue);
                SolidColorBrush empty = new SolidColorBrush(Colors.White);

                star1.Stroke = stroke;
                star2.Stroke = stroke;
                star3.Stroke = stroke;
                star4.Stroke = stroke;
                star5.Stroke = stroke;

                star1.Fill = ViewModel.Rating >= 1 ? fill : empty;
                star2.Fill = ViewModel.Rating >= 2 ? fill : empty;
                star3.Fill = ViewModel.Rating >= 3 ? fill : empty;
                star4.Fill = ViewModel.Rating >= 4 ? fill : empty;
                star5.Fill = ViewModel.Rating >= 5 ? fill : empty;
            }
        }

        /// <summary>
        /// Set the current used excuse and update the excuse progress widget.
        /// </summary>
        /// <param name="ex">new excuse to display</param>
        private void UpdateCurrentExcuse(Excuse ex)
        {
            ViewModel.SetExcuse(ex);
            excuseCounter.Maximum = Excuses.Excuses.Count;
            if(Excuses.Excuses.Count > 0)
            {
                int idx = Excuses.Excuses.IndexOf(ex);
                idx++;
                if(idx > 0)
                {
                    excuseCounter.Value = idx;
                }
            }
            else
            {
                excuseCounter.Value = 0;
            }
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Excuses Files (*.ex)|*.ex";
            dialog.Title = "Open Excuse File";
            if(dialog.ShowDialog() == true)
            {
                string name = dialog.FileName;
                Excuses = ExcuseList.Load(name);
                if(Excuses != null && File.Exists(name))
                {
                    ExcuseFile = name;
                    statusLabel.Content = File.GetLastWriteTime(ExcuseFile);
                    if(Excuses.Excuses.Count > 0)
                    { 
                        excuseCounter.Maximum = Excuses.Excuses.Count;
                        excuseCounter.Value = 1;
                        UpdateCurrentExcuse(Excuses.Excuses[0]);
                    }
                }
            }
        }

        private void Create_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Create_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "Excuses Files (*.ex)|*.ex";
            dialog.Title = "Open Excuse File";
            if (dialog.ShowDialog() == true)
            {
                string name = dialog.FileName;

                if(File.Exists(name))
                {
                    try
                    {
                        File.Delete(name);
                    }
                    catch(Exception)
                    {
                        //No handling required.
                    }
                }
                
                Excuses = ExcuseList.Create(name);
                if (Excuses != null && File.Exists(name))
                {
                    ExcuseFile = name;
                    statusLabel.Content = File.GetLastWriteTime(ExcuseFile);
                    if (Excuses.Excuses.Count > 0)
                    {
                        UpdateCurrentExcuse(Excuses.Excuses[0]);
                    }
                }
            }
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ExcuseFile != null;
        }

        private void Save_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if(Excuses != null && ExcuseFile != null)
            {
                Excuses.Save();
            }
        }

        private void Exit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Exit_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Random_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ExcuseFile != null;
        }

        private void Random_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            int randIdx = rand.Next(0, Excuses.Excuses.Count);
            UpdateCurrentExcuse(Excuses.Excuses[randIdx]);
        }

        private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ExcuseFile != null;
        }

        private void New_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Excuse nex = new Excuse();
            Excuses.Excuses.Add(nex);
            UpdateCurrentExcuse(nex);
        }

        private void Next_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ExcuseFile != null && Excuses.Excuses.Count > 0 && !Excuses.Excuses.Last().Equals(ViewModel);
        }

        private void Next_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            int index = Excuses.Excuses.IndexOf(ViewModel.CurrentExcuse);
            index++;
            if (index >= Excuses.Excuses.Count)
            {
                if(Excuses.Excuses.Count == 0)
                {
                    UpdateCurrentExcuse(Excuse.DefaultExcuse);
                }
                else
                {
                    UpdateCurrentExcuse(Excuses.Excuses.First());
                }
            }
            else
            {
                UpdateCurrentExcuse(Excuses.Excuses[index]);
            }
        }

        private void Previous_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ExcuseFile != null && Excuses.Excuses.Count > 0 && !Excuses.Excuses.First().Equals(ViewModel);
        }

        private void Previous_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            int index = Excuses.Excuses.IndexOf(ViewModel.CurrentExcuse);
            index--;
            if(index < 0)
            {
                if (Excuses.Excuses.Count == 0)
                {
                    UpdateCurrentExcuse(Excuse.DefaultExcuse);
                }
                else
                {
                    UpdateCurrentExcuse(Excuses.Excuses.Last());
                }
            }
            else
            {
                UpdateCurrentExcuse(Excuses.Excuses[index]);
            }
        }

        private void Star_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(sender.Equals(star1))
            {
                ViewModel.Rating = 1;
            }
            else if(sender.Equals(star2))
            {
                ViewModel.Rating = 2;
            }
            else if (sender.Equals(star3))
            {
                ViewModel.Rating = 3;
            }
            else if (sender.Equals(star4))
            {
                ViewModel.Rating = 4;
            }
            else if (sender.Equals(star5))
            {
                ViewModel.Rating = 5;
            }
            else
            {
                throw new InvalidValueException("sender not known!");
            }
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    public static class ExcuseCommands
    {
        public static readonly RoutedUICommand Open = new RoutedUICommand(
            "_Open",
            "open",
            typeof(ExcuseCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.O, ModifierKeys.Control)
            }
            );

        public static readonly RoutedUICommand Create = new RoutedUICommand(
            "_Create",
            "create",
            typeof(ExcuseCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.C, ModifierKeys.Control)
            }
            );


        public static readonly RoutedUICommand Save = new RoutedUICommand(
            "_Save",
            "save",
            typeof(ExcuseCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.S, ModifierKeys.Control)
            }
            );

        public static readonly RoutedUICommand Exit = new RoutedUICommand(
            "_Exit",
            "exit",
            typeof(ExcuseCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.X, ModifierKeys.Control)
            }
            );

        public static readonly RoutedUICommand Random = new RoutedUICommand(
            "_Random",
            "random",
            typeof(ExcuseCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.R, ModifierKeys.Control)
            }
            );

        public static readonly RoutedUICommand New = new RoutedUICommand(
            "_New",
            "new",
            typeof(ExcuseCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.N, ModifierKeys.Control)
            }
            );

        public static readonly RoutedUICommand Next = new RoutedUICommand(
            "_Next",
            "next",
            typeof(ExcuseCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.F, ModifierKeys.Control)
            }
            );

        public static readonly RoutedUICommand Previous = new RoutedUICommand(
            "_Previous",
            "previous",
            typeof(ExcuseCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.P, ModifierKeys.Control)
            }
            );
    }
}
