using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace Vape_Assistant
{
    /// <summary>
    /// Interaction logic for ucCalendar.xaml
    /// </summary>
    public partial class ucCalendar : UserControl
    {
        int[] Days = new int[31];
        string[] monthNames;
        List<int> years = new List<int>();

        public ucCalendar()
        {
            InitializeComponent();
            initializeLists();
            LoadMonthsCombos();
            LoadYearCombo();
            initalizeDaysArray();
            LoadDaysCombo();
        }

        public void initializeLists()
        {
            monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames; //DateTimeFormatInfo.CurrentInfo.MonthNames;
            years = Enumerable.Range(DateTime.Now.Year - 60, 100).ToList();
        }

        public void initalizeDaysArray()
        {
            if (cmbMonths.SelectedIndex > 11) { return; }
            int month = DateTimeFormatInfo.CurrentInfo.MonthNames.ToList().IndexOf(cmbMonths.SelectedValue.ToString()) + 1;
            Days = new int[DateTime.DaysInMonth(Convert.ToInt32(cmbYear.SelectedValue), month)];
            for (int i = 0; i < Days.Count(); i++)
            {
                Days[i] = i + 1;
            }
        }

        public void LoadDaysCombo()
        {
            cmbDays.ItemsSource = Days;
            cmbDays.SelectedValue = DateTime.Now.Day;
        }

        public void LoadYearCombo()
        {
            cmbYear.ItemsSource = years;
            cmbYear.SelectedValue = DateTime.Now.Year;
        }

        public void LoadMonthsCombos()
        {
            cmbMonths.ItemsSource = monthNames.Take(12).ToList();
            cmbMonths.SelectedValue = DateTimeFormatInfo.CurrentInfo.GetMonthName(DateTime.Now.Month);
        }

        private void cmbMonths_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbMonths.SelectedIndex > 11)
            {
                cmbMonths.SelectedIndex = 11;
            }
            if (cmbMonths.SelectedValue != null)
            {
                initalizeDaysArray();
                LoadDaysCombo();
                if (cmbMonths.SelectedIndex == 1 && cmbDays.SelectedIndex >= 27 || cmbMonths.SelectedIndex != 1 && cmbDays.SelectedIndex >= 29)
                {
                    cmbDays.SelectedIndex = -1;
                }
            }
        }

        private void cmbYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbYear.SelectedValue != null)
            {
                initalizeDaysArray();
                LoadDaysCombo();
                if (cmbDays.SelectedIndex >= 28) { 
                cmbDays.SelectedIndex = -1;
                }
            }
        }
    }
}

