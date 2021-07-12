using Newtonsoft.Json;
using SongScout.Helpers;
using SongScout.LibrespotModels;
using SpotifyAPI.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SongScout.Helpers.LibrespotHelper;
using static SpotifyAPI.Web.SearchRequest;
using System.Runtime.InteropServices;
using Bunifu.Framework.UI;
using javax.naming.ldap;
using Bunifu.UI.WinForms;

namespace SongScout
{
    public partial class MainForm : Form
    {
        public string token;
        public List<FullArtist> artistResults;
        public string selectedArtistId;
        public string selectedArtistName;
        public bool isDataAvailable = false;

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
           int nLeftRect,     // x-coordinate of upper-left corner
           int nTopRect,      // y-coordinate of upper-left corner
           int nRightRect,    // x-coordinate of lower-right corner
           int nBottomRect,   // y-coordinate of lower-right corner
           int nWidthEllipse, // height of ellipse
           int nHeightEllipse // width of ellipse
        );

        public MainForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            SpotifyHelper search = new SpotifyHelper();
            await search.GetToken();
            token = search.OauthToken;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            this.MainPages.PageIndex = 0;
        }

        private void OverviewButton_Click(object sender, EventArgs e)
        {
            this.MainPages.PageIndex = 1;
        }

        private void SinglesButton_Click(object sender, EventArgs e)
        {
            this.MainPages.PageIndex = 2;
        }

        private void AlbumsButton_Click(object sender, EventArgs e)
        {
            this.MainPages.PageIndex = 3;
        }

        private void CompilationsButton_Click(object sender, EventArgs e)
        {
            this.MainPages.PageIndex = 4;
        }

        private void ChartsButton_Click(object sender, EventArgs e)
        {
            this.MainPages.PageIndex = 5;
        }

        private async void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Cursor = Cursors.WaitCursor;
                SearchCoverPanel.Visible = true;

                var spotifyClient = new SpotifyClient(token);
                var artistSearch = await spotifyClient.Search.Item(new SearchRequest(Types.Artist, SearchTextBox.Text));
                artistResults = artistSearch.Artists.Items;
                LibrespotHelper librespotHelper = new LibrespotHelper();

                var cardLocX = 1;
                var cardLocY = 1;

                for (int i = 0; i < artistResults.Count; i++)
                {
                    var artistInfo = librespotHelper.GetArtistInsights(artistResults[i].Id);
                    BunifuPictureBox ArtistPictureBox = new BunifuPictureBox()
                    {
                        ImageLocation = artistInfo.Data.MainImageUrl,
                        Location = new Point(14, 16),
                        Size = new Size(100, 100)
                    };

                    BunifuCards ArtistResultCard = new BunifuCards
                    {
                        BackColor = Color.FromArgb(23, 23, 23),
                        BorderStyle = BorderStyle.None,
                        BorderRadius = 20,
                        BottomSahddow = true,
                        color = System.Drawing.Color.Transparent,
                        LeftSahddow = true,
                        RightSahddow = true,
                        ShadowDepth = 0,
                        TabIndex = 3,
                        Cursor = Cursors.Hand,
                        Size = new Size(128, 174),
                        Location = new Point(cardLocX, cardLocY),
                    };

                    BunifuCustomLabel ArtistNameLabel = new BunifuCustomLabel
                    {
                        Text = artistInfo.Data.Name,
                        ForeColor = Color.White,
                        Font = new System.Drawing.Font("Montserrat", 8.99999F, System.Drawing.FontStyle.Bold),
                        Location = new Point(12, 136),
                        Size = new Size(105, 17),
                        TextAlign = ContentAlignment.MiddleCenter,
                        AutoEllipsis = true
                    };

                    BunifuCustomLabel ArtistIdLabel = new BunifuCustomLabel
                    {
                        Text = Convert.ToString(artistResults[i].Id),
                        Visible = false
                    };

                    List<BunifuLabel> mostStreamedName = new List<BunifuLabel>
                    {
                        MostStreamed1NameLabel,
                        MostStreamed2NameLabel,
                        MostStreamed3NameLabel,
                        MostStreamed4NameLabel,
                        MostStreamed5NameLabel
                    };

                    List<BunifuLabel> mostStreamedPlaycount = new List<BunifuLabel>
                    {
                        MostStreamed1CountLabel,
                        MostStreamed2CountLabel,
                        MostStreamed3CountLabel,
                        MostStreamed4CountLabel,
                        MostStreamed5CountLabel
                    };

                    for (int j = 0; j < 5; j++)
                    {
                        mostStreamedName[j].Text = "";
                        mostStreamedPlaycount[j].Text = "";
                    }

                    ResultsPanel.Controls.Add(ArtistResultCard);
                    ArtistResultCard.Controls.Add(ArtistPictureBox);
                    ArtistResultCard.Controls.Add(ArtistNameLabel);

                    ArtistNameLabel.MouseLeave += delegate { ArtistResultCard.BackColor = Color.FromArgb(23, 23, 23); };
                    ArtistPictureBox.MouseLeave += delegate { ArtistResultCard.BackColor = Color.FromArgb(23, 23, 23); };
                    ArtistResultCard.MouseLeave += delegate { ArtistResultCard.BackColor = Color.FromArgb(23, 23, 23); };
                    ArtistNameLabel.MouseHover += delegate { ArtistResultCard.BackColor = Color.FromArgb(30, 30, 30); };
                    ArtistPictureBox.MouseHover += delegate { ArtistResultCard.BackColor = Color.FromArgb(30, 30, 30); };
                    ArtistResultCard.MouseHover += delegate { ArtistResultCard.BackColor = Color.FromArgb(30, 30, 30); };
                    
                    void clickEvent(object s, EventArgs a)
                    {
                        LoadingCoverPanel.Visible = true;
                        OverviewButton.Focus();
                        Cursor = Cursors.WaitCursor;
                        SearchCoverPanel.Visible = false;
                        OverviewButton.Visible = true;
                        // ArtistResultCoverPanel.Visible = true;
                        SinglesButton.Visible = true;
                        AlbumsButton.Visible = true;
                        CompilationsButton.Visible = true;
                        ChartsButton.Visible = true;

                        selectedArtistId = ArtistIdLabel.Text;
                        OverviewArtistPictureBox.ImageLocation = artistInfo.Data.MainImageUrl;

                        LibrespotHelper librespot = new LibrespotHelper();
                        var artistInsights = librespot.GetArtistInsights(selectedArtistId);
                        var selectedArtistInfo = librespot.GetArtistInfo(selectedArtistId);
                        var spotify = new SpotifyClient(token);
                        var artist = spotify.Artists.Get(selectedArtistId);

                        selectedArtistName = artist.Result.Name;
                        var artistImageUrl = artistInsights.Data.MainImageUrl;
                        var monthlyListeners = artistInsights.Data.MonthlyListeners;
                        var followers = artist.Result.Followers.Total;
                        var popularityScore = artist.Result.Popularity;
                        var globalPosition = artistInsights.Data.GlobalChartPosition;
                        var totalStreams = librespot.GetAllTimeStreams(selectedArtistId, token);

                        var totalSingles = 0;
                        var totalAlbums = 0;
                        var totalCompilations = 0;

                        if (selectedArtistInfo.Data.Releases.Singles.Releases != null)
                            totalSingles = selectedArtistInfo.Data.Releases.Singles.Releases.Count;

                        if (selectedArtistInfo.Data.Releases.Albums.Releases != null)
                            totalAlbums = selectedArtistInfo.Data.Releases.Albums.Releases.Count;

                        if (selectedArtistInfo.Data.Releases.Compilations.Releases != null)
                            totalCompilations = selectedArtistInfo.Data.Releases.Compilations.Releases.Count;

                        if (globalPosition == 0)
                        {
                            GlobalRankLabel.Text = "—";
                        }
                        else
                        {
                            GlobalRankLabel.Text = globalPosition.ToString();
                        }

                        librespot.OrderTracks(librespot.totalTracks, token);

                        for (int j = 0; j < librespot.mostStreamedTracksName.Count; j++)
                        {
                            mostStreamedName[j].Text = librespot.mostStreamedTracksName[j];
                            mostStreamedPlaycount[j].Text = librespot.mostStreamedTracksPlaycount[j].ToString("#,###");
                        }

                        SelectedArtistIdTextbox.Text = selectedArtistId;
                        OverviewArtistNameLabel.Text = selectedArtistName;
                        TotalStreamsLabel.Text = totalStreams.ToString("#,###");
                        PopularityProgressBar.Value = popularityScore;
                        MonthlyListenersLabel.Text = monthlyListeners.ToString("#,###");
                        FollowersLabel.Text = followers.ToString("#,###");
                        SinglesNumLabel.Text = totalSingles.ToString();
                        AlbumsNumLabel.Text = totalAlbums.ToString();
                        CompilationsNumLabel.Text = totalCompilations.ToString();
                        TotalTracksLabel.Text = librespot.totalTracks.Count.ToString();
                        Tracks1BLabel.Text = librespot.tracksWith1B.ToString();
                        Tracks100MLabel.Text = librespot.tracksWith100M.ToString();
                        Tracks10MLabel.Text = librespot.tracksWith10M.ToString();
                        Tracks1MLabel.Text = librespot.tracksWith1M.ToString();

                        MainPages.PageIndex = 1;
                        Cursor = Cursors.Default;
                        isDataAvailable = true;
                        LoadingCoverPanel.Visible = false;
                    }

                    ArtistNameLabel.Click += clickEvent;
                    ArtistPictureBox.Click += clickEvent;
                    ArtistResultCard.Click += clickEvent;

                    if (i >= 5 && i % 5 == 0)
                    {
                        cardLocY += 197;
                        cardLocX = 1;
                    }
                    else
                    {
                        cardLocX += 147;
                    }
                }

                SearchPageScrollBar.Visible = true;
                SearchCoverPanel.Visible = false;
                Cursor = Cursors.Default;
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            SearchCoverPanel.Visible = true;
            ResultsPanel.Controls.Clear();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            ExitConfirmationForm exitForm = new ExitConfirmationForm();
            exitForm.Show();
        }
    }
}
