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

namespace Tuhvatshin_Autoservice
{
    /// <summary>
    /// Логика взаимодействия для SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        private Service _currentService = new Service();
        public SignUpPage(Service SelectedService)
        {
            InitializeComponent();
            if(SelectedService != null)
                this._currentService = SelectedService;
            DataContext = _currentService;
            var _currentClient = Tuhvatshin_autoservisEntities.GetContext().Client.ToList();
            ComboClient.ItemsSource = _currentClient;
        }
        private ClientService _currentCLientService = new ClientService();
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (ComboClient.SelectedItem == null)
                errors.AppendLine("Укажите ФИО клиента");
            if (StartDate.Text == "")
                errors.AppendLine("Укажите дату услуги");
            if (TBStart.Text == "")
                errors.AppendLine("Укажите время начала услуги");
            if(errors.Length>0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            Client selectedClient = ComboClient.SelectedItem as Client;
            _currentCLientService.ClientID = selectedClient.ID;
            _currentCLientService.ServiceID = _currentService.ID;
            _currentCLientService.StartTime = Convert.ToDateTime(StartDate.Text +" "+ TBStart.Text);
            if (_currentCLientService.ID == 0)
                Tuhvatshin_autoservisEntities.GetContext().ClientService.Add(_currentCLientService);
            try
            {
                Tuhvatshin_autoservisEntities.GetContext().SaveChanges();
                MessageBox.Show("информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void TBStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = TBStart.Text;
            if (s.Length < 3 || !s.Contains(':'))
            {
                TBEnd.Text = "";
                return;
            }

            try
            {
                string[] start = s.Split(':');

                // Проверяем, что обе части существуют и могут быть преобразованы в числа
                if (start.Length != 2 ||
                    !int.TryParse(start[0], out int hours) ||
                    !int.TryParse(start[1], out int minutes))
                {
                    TBEnd.Text = "";
                    return;
                }

                // Проверяем корректность диапазонов времени
                if (hours < 0 ||hours>23 || minutes < 0 || minutes > 59)
        {
                    TBEnd.Text = "";
                    return;
                }

                // Пересчет времени
                int totalMinutes = (int)(hours * 60 + minutes + _currentService.Duration);
                int endHours = totalMinutes / 60;
                int endMinutes = totalMinutes % 60;

                // Корректировка если время переходит через полночь
                endHours %= 24;

                TBEnd.Text = $"{endHours:D2}:{endMinutes:D2}";
            }
            catch
            {
                TBEnd.Text = "";
            }
        }
    }
}
