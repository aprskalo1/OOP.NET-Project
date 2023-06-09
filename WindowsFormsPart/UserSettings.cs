﻿using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsPart
{
    public partial class UserSettings : Form
    {
        private static string path = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName).Parent.FullName;
        string settingPath = Path.Combine(path, "settings.txt");
        IRepo repo = RepoFactory.GetRepo();

        public UserSettings()
        {
            InitializeComponent();
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = false;
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            if (cbChooseLanguage.SelectedIndex == -1 || cbChooseWorldCup.SelectedIndex == -1)
            {
                MessageBox.Show("Molimo unesite sve i ispravne parametre", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string language = cbChooseLanguage.SelectedItem.ToString();
                string worldCupType = cbChooseWorldCup.SelectedItem.ToString();

                repo.SaveSettings(language, worldCupType, settingPath);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Pogreska pri spremanju podataka: {ex.Message}");
            }
        }

        private void UserSettings_Load(object sender, EventArgs e)
        {
            cbChooseLanguage.SelectedIndex = 0;
            cbChooseWorldCup.SelectedIndex = 0;
        }
    }
}
