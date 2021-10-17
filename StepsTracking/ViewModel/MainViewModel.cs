using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using StepsTracking.Common;
using StepsTracking.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace StepsTracking.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public List<UserDay> UsersByDay { get; set; }
        public ObservableCollection<User> Users { get; set; }
        public User SelectedUser
        {
            get
            {
                return selectedUser;
            }
            set
            {
                selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }
        public string[] Labels { get; set; }
        public ICommand SaveDataCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private User selectedUser;

        public MainViewModel()
        {
            this.Users = new ObservableCollection<User>();
            this.UsersByDay = new List<UserDay>();

            this.Labels = Enumerable.Range(1, 30).ToList().ConvertAll(i => i.ToString()).ToArray();

            this.SaveDataCommand = new RelayCommand(new Action(SaveData));

            DeserializeJsonFiles();

            InitializeData();
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void SaveData()
		{
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON file (*.json) | *.json";
            if (saveFileDialog.ShowDialog() == true)
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;

                using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, SelectedUser);
                }
                MessageBox.Show("Data saved.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void InitializeData()
        {
            UsersByDay.ForEach(user =>
            {
                var matchingUser = Users.Where(x => x.FullName.Contains(user.User)).FirstOrDefault();
                if (matchingUser == null)
                {
                    var average = Convert.ToInt32(UsersByDay.Where(x => x.User == user.User).Select(x => x.Steps).Average());
                    var bestResult = UsersByDay.Where(x => x.User == user.User).Max(y => y.Steps);
                    var worstResult = UsersByDay.Where(x => x.User == user.User).Min(y => y.Steps);

                    Users.Add(new User()
                    {
                        FullName = user.User,
                        AverageSteps = average,
                        BestResult = bestResult,
                        WorstResult = worstResult,
                        Title = user.User,
                        Values = new ChartValues<int>(UsersByDay.Where(x => x.User == user.User).Select(x => x.Steps)),
                        Mapper = Mappers.Xy<int>()
                                .X((item, index) => index)
                                .Y(item => item)
                                .Fill(x => GetPointColor(x, bestResult, worstResult))
                                .Stroke(x => x == bestResult || x == worstResult ? Brushes.Navy : null),
                        Color = bestResult > average * 1.2 || worstResult < average * 0.8 ? Brushes.Red : Brushes.Black,
                        Days = UsersByDay.Where(x => x.User == user.User).ToList()
                    });
                }
            });

            this.SelectedUser = Users.FirstOrDefault();
        }

        private void DeserializeJsonFiles()
        {
            for (int i = 1; i < 31; i++)
            {
                var json = File.ReadAllText($"Data\\day{i}.json");
                var userItems = JsonConvert.DeserializeObject<UserDay[]>(json);
                userItems.Populate(i);

                UsersByDay.AddRange(userItems);
            }
        }

        private SolidColorBrush GetPointColor(int x, int bestResult, int worstResult)
        {
            if (x == bestResult)
            {
                return Brushes.Green;
            }
            else if (x == worstResult)
            {
                return Brushes.Red;
            }
            else
            {
                return null;
            }
        }
    }
}
