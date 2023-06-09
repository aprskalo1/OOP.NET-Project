using DAL;

namespace WindowsFormsPart
{
    public partial class FootbalManagerForm : Form
    {
        private static string path = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName).Parent.FullName;
        string favouritePlayersFilePath = Path.Combine(path, "favPlayers.txt");
        string settingPath = Path.Combine(path, "settings.txt");

        IRepo repo = RepoFactory.GetRepo();

        public FootbalManagerForm()
        {
            InitializeComponent();

            lbAllPlayers.AllowDrop = true;
            lbAllPlayers.DragDrop += lbAllPlayers_DragDrop;
            lbAllPlayers.DragEnter += lbAllPlayers_DragEnter;


            lbFavouritePlayers.AllowDrop = true;
            lbFavouritePlayers.DragDrop += lbFavouritePlayers_DragDrop;
            lbFavouritePlayers.DragEnter += lbFavouritePlayers_DragEnter;

            lbAllPlayers.MouseUp += lbAllPlayers_MouseUp;
            lbFavouritePlayers.MouseUp += lbFavouritePlayers_MouseUp;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetInitalSettings();
            SetLanguage();
        }

        private void SetLanguage()
        {
            if (repo.GetLanguage() == "ENG")
            {
                igraciToolStripMenuItem.Text = "Players";
                rangListeToolStripMenuItem.Text = "Rank lists";
                postavkeToolStripMenuItem.Text = "Settings";

                lblOtherPlayers.Text = "Other players:";
                lblFavPlayers.Text = "Favourite players:";
                lblRankPlayers.Text = "Players:";
                lblRangVisitors.Text = "Visitors:";

                btnPlayerDetails.Text = "Player details";
            }
            else
            {
                igraciToolStripMenuItem.Text = "Igraci";
                rangListeToolStripMenuItem.Text = "Rang liste";
                postavkeToolStripMenuItem.Text = "Postavke";

                lblOtherPlayers.Text = "Ostali igraci:";
                lblFavPlayers.Text = "Omiljeni igraci:";
                lblRankPlayers.Text = "Igraci:";
                lblRangVisitors.Text = "Posjetitelji:";

                btnPlayerDetails.Text = "Detalji igraca";
            }
        }

        private void btnAddFavouriteTeam_Click(object sender, EventArgs e)
        {
            IRepo repo = RepoFactory.GetRepo();
            repo.SaveFavouriteTeam(cbTeams.SelectedItem.ToString(), settingPath);
            tlpFavouriteTeam.Visible = false;

            List<Player> playerList = repo.LoadPlayers();
            foreach (var player in playerList)
            {
                clbPlayers.Items.Add(player.GetName());
            }

            tlpFavouritePlayers.Visible = true;
        }

        private void btnAddFavouritePlayers_Click(object sender, EventArgs e)
        {
            List<string> selectedPlayers = new List<string>();
            foreach (string player in clbPlayers.CheckedItems)
            {
                selectedPlayers.Add(player);
            }

            IRepo repo = RepoFactory.GetRepo();
            repo.SaveFavouritePLayers(selectedPlayers, favouritePlayersFilePath);

            tlpFavouritePlayers.Visible = false;
            SetPlayers();
        }

        private void SetPlayers()
        {
            msMainMenu.Visible = true;
            tlpPlayersPanels.Visible = true;
            lblFavPlayers.Visible = true;
            lblOtherPlayers.Visible = true;
            btnPlayerDetails.Visible = true;
            tlpRankLists.Visible = false;
            lblRankPlayers.Visible = false;
            lblRangVisitors.Visible = false;

            List<Player> playerList = repo.LoadPlayers();
            List<string> favouritePlayers = repo.GetFavouritePlayers(favouritePlayersFilePath);

            lbAllPlayers.Items.Clear();
            lbFavouritePlayers.Items.Clear();

            foreach (var player in playerList)
            {
                if (!favouritePlayers.Contains(player.Name))
                {
                    lbAllPlayers.Items.Add(player.GetName());
                }
            }

            foreach (var player in favouritePlayers)
            {
                lbFavouritePlayers.Items.Add(player);
            }
        }

        private void SetInitalSettings()
        {
            tlpFavouriteTeam.Location = new Point(
                (ClientSize.Width - tlpFavouriteTeam.Width) / 2,
                (ClientSize.Height - tlpFavouriteTeam.Height) / 2
            );

            tlpFavouritePlayers.Location = new Point(
                (ClientSize.Width - tlpFavouritePlayers.Width) / 2,
                (ClientSize.Height - tlpFavouritePlayers.Height) / 2
            );
            tlpFavouritePlayers.Visible = false;
            msMainMenu.Visible = false;
            tlpPlayersPanels.Visible = false;
            lblFavPlayers.Visible = false;
            lblOtherPlayers.Visible = false;
            btnPlayerDetails.Visible = false;
            tlpRankLists.Visible = false;
            lblRangVisitors.Visible = false;
            lblRankPlayers.Visible = false;

            var userSettingsForm = new UserSettings();

            if (!File.Exists(settingPath) || new FileInfo(settingPath).Length == 0)
            {
                userSettingsForm.ShowDialog();
                File.Delete(favouritePlayersFilePath);
            }

            List<Team> teamList = repo.LoadTeams();
            foreach (var team in teamList)
            {
                cbTeams.Items.Add(team.FillComboBox());
            }

            if (cbTeams.Items.Count > 0)
            {
                cbTeams.SelectedIndex = 0;
            }

            if (!repo.FavouriteTeamExists(settingPath))
            {
                tlpFavouriteTeam.Visible = false;
                List<Player> playerList = repo.LoadPlayers();
                foreach (var player in playerList)
                {
                    clbPlayers.Items.Add(player.GetName());
                }
            }

            if (File.Exists(favouritePlayersFilePath) && new FileInfo(favouritePlayersFilePath).Length > 0)
            {
                SetPlayers();
            }
            else if (!repo.FavouriteTeamExists(settingPath))
            {
                tlpFavouritePlayers.Visible = true;
            }
        }

        //Drag and drop
        private bool isDragging = false;

        private void lbAllPlayers_MouseDown(object sender, MouseEventArgs e)
        {
            if (lbAllPlayers.Items.Count > 0 && lbAllPlayers.SelectedItem != null)
            {
                isDragging = true;
                lbAllPlayers.DoDragDrop(lbAllPlayers.SelectedItem, DragDropEffects.Move);
                lbFavouritePlayers.SelectedItem = null;
            }
        }

        private void lbFavouritePlayers_DragEnter(object sender, DragEventArgs e)
        {
            if (isDragging && lbAllPlayers.SelectedItem != null)
                e.Effect = DragDropEffects.Move;
        }

        private void lbFavouritePlayers_DragDrop(object sender, DragEventArgs e)
        {
            if (isDragging && lbAllPlayers.SelectedItem != null && lbAllPlayers.SelectedItem.ToString() == e.Data.GetData(typeof(string)).ToString())
            {
                List<string> favouritePlayers = repo.GetFavouritePlayers(favouritePlayersFilePath);
                favouritePlayers.Add(lbAllPlayers.SelectedItem.ToString());
                repo.SaveFavouritePLayers(favouritePlayers, favouritePlayersFilePath);

                lbFavouritePlayers.Items.Add(lbAllPlayers.SelectedItem);
                lbAllPlayers.Items.Remove(lbAllPlayers.SelectedItem);
            }
        }

        private void lbFavouritePlayers_MouseDown(object sender, MouseEventArgs e)
        {
            if (lbFavouritePlayers.Items.Count > 0 && lbFavouritePlayers.SelectedItem != null)
            {
                isDragging = true;
                lbFavouritePlayers.DoDragDrop(lbFavouritePlayers.SelectedItem, DragDropEffects.Move);
                lbAllPlayers.SelectedItem = null;
            }
        }

        private void lbAllPlayers_DragEnter(object sender, DragEventArgs e)
        {
            if (isDragging && lbFavouritePlayers.SelectedItem != null)
                e.Effect = DragDropEffects.Move;
        }

        private void lbAllPlayers_DragDrop(object sender, DragEventArgs e)
        {
            if (isDragging && lbFavouritePlayers.SelectedItem != null && lbFavouritePlayers.SelectedItem.ToString() == e.Data.GetData(typeof(string)).ToString())
            {
                List<string> favouritePlayers = repo.GetFavouritePlayers(favouritePlayersFilePath);
                favouritePlayers.Remove(lbFavouritePlayers.SelectedItem.ToString());
                repo.SaveFavouritePLayers(favouritePlayers, favouritePlayersFilePath);

                lbAllPlayers.Items.Add(lbFavouritePlayers.SelectedItem);
                lbFavouritePlayers.Items.Remove(lbFavouritePlayers.SelectedItem);
            }
        }

        private void lbAllPlayers_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private void lbFavouritePlayers_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        //Show player details
        private void btnPlayerDetails_Click(object sender, EventArgs e)
        {
            List<Player> playerList = repo.LoadPlayers();
            Player selectedPlayer = null;

            if (lbAllPlayers.SelectedItem != null)
            {
                foreach (var player in playerList)
                {
                    if (player.Name == lbAllPlayers.SelectedItem.ToString())
                    {
                        selectedPlayer = player;
                        break;
                    }
                }

                PlayerForm playerDetails = new PlayerForm();
                playerDetails.CurrentPlayer = selectedPlayer;
                playerDetails.ShowDialog();
            }

            if (lbFavouritePlayers.SelectedItem != null)
            {
                foreach (var player in playerList)
                {
                    if (player.Name == lbFavouritePlayers.SelectedItem.ToString())
                    {
                        selectedPlayer = player;
                        break;
                    }
                }

                PlayerForm playerDetails = new PlayerForm();
                playerDetails.CurrentPlayer = selectedPlayer;
                playerDetails.ShowDialog();
            }
        }

        private void rangListeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbPlayerRankList.Items.Clear(); 
            lbVisitorRankList.Items.Clear();

            tlpPlayersPanels.Visible = false;
            lblOtherPlayers.Visible = false;
            lblFavPlayers.Visible = false;
            btnPlayerDetails.Visible = false;
            tlpRankLists.Visible = true;
            lblRangVisitors.Visible = true;
            lblRankPlayers.Visible = true;

            var playerStats = repo.GetPlayerEventData()
                .Where(p => p.EventType == "goal" || p.EventType == "yellow-card")
                .GroupBy(p => p.PlayerName)
                .Select(g => new
                {
                    PlayerName = g.Key,
                    Goals = g.Count(p => p.EventType == "goal"),
                    YellowCards = g.Count(p => p.EventType == "yellow-card")
                })
                .OrderByDescending(p => p.Goals)
                .ToList();

            foreach (var playerStat in playerStats)
            {

                if (playerStats.Count > 0 && playerStats.Count != lbPlayerRankList.Items.Count)
                {
                    lbPlayerRankList.Items.Add($"{playerStat.PlayerName}: Goals - {playerStat.Goals}, Yellow Cards - {playerStat.YellowCards}");
                }
            }

            var visitorStats = repo.GetVisitorData();
            visitorStats = visitorStats.OrderByDescending(v => v.VisitorNumber).ToList();

            foreach (var visitorStat in visitorStats)
            {
                if (visitorStats.Count > 0 && visitorStats.Count != lbVisitorRankList.Items.Count)
                {
                    lbVisitorRankList.Items.Add(visitorStat.GetVisitorInfo());
                }
            }
        }

        private void igraciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetPlayers();
        }

        private void postavkeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clbPlayers.Items.Clear();
            cbTeams.Items.Clear();
            File.Delete(settingPath);
            tlpFavouriteTeam.Visible = true;
            SetInitalSettings();
            SetLanguage();
        }
    }
}