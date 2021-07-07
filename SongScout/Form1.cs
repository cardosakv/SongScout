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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SongScout.Helpers.LibrespotHelper;
using static SpotifyAPI.Web.SearchRequest;

namespace SongScout
{
    public partial class Form1 : Form
    {
        public string token;
        public List<FullArtist> artistResults;

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            SpotifyHelper search = new SpotifyHelper();
            await search.GetToken();
            token = search.OauthToken;
        }

        public async void SearchButton_Click(object sender, EventArgs e)
        {
            ResultsListBox.Items.Clear();
            var spotifyClient = new SpotifyClient(token);
            var artistSearch = await spotifyClient.Search.Item(new SearchRequest(Types.Artist, SearchTextBox.Text));
            artistResults = artistSearch.Artists.Items;

            for (int i = 0; i < artistResults.Count; i++)
            {
                ResultsListBox.Items.Add(artistResults[i].Name);
            }
        }

        public void ResultsListBox_Click(object sender, EventArgs e)
        {
            var selectedArtist = artistResults[ResultsListBox.SelectedIndex];
            var selectedArtistID = selectedArtist.Id;

            LibrespotHelper librespotHelper = new LibrespotHelper();
            var artistInsights = librespotHelper.GetArtistInsights(selectedArtistID);
            var artistInfo = librespotHelper.GetArtistInfo(selectedArtistID);
            var spotify = new SpotifyClient(token);
            var artist = spotify.Artists.Get(selectedArtistID);

            var artistImageUrl = artistInsights.Data.MainImageUrl;
            var artistName = selectedArtist.Name;
            var monthlyListeners = artistInsights.Data.MonthlyListeners;
            var followers = artist.Result.Followers.Total;
            var popularityScore = artist.Result.Popularity;
            var globalPosition = artistInsights.Data.GlobalChartPosition;
            var totalStreams = librespotHelper.GetAllTimeStreams(selectedArtistID, token);

            var totalSingles = 0;
            var totalAlbums = 0;
            var totalCompilations = 0;

            if (artistInfo.Data.Releases.Singles.Releases != null)
                totalSingles = artistInfo.Data.Releases.Singles.Releases.Count;

            if (artistInfo.Data.Releases.Albums.Releases != null)
                totalAlbums = artistInfo.Data.Releases.Albums.Releases.Count;

            if (artistInfo.Data.Releases.Compilations.Releases != null)
                totalCompilations = artistInfo.Data.Releases.Compilations.Releases.Count;

            ArtistPictureBox.ImageLocation = artistImageUrl;
            NameLabel.Text = artistName;
            ListenersLabel.Text = monthlyListeners.ToString("#,###");
            FollowersLabel.Text = followers.ToString("#,###");
            PopularityLabel.Text = popularityScore.ToString();
            GlobalPositionLabel.Text = globalPosition.ToString();
            TotalSinglesLabel.Text = totalSingles.ToString();
            TotalAlbumsLabel.Text = totalAlbums.ToString();
            TotalCompilationsLabel.Text = totalCompilations.ToString();
            AlltimeStreamsLabel.Text = totalStreams.ToString("#,###");
            TotalTracksLabel.Text = librespotHelper.totalTracks.Count.ToString("#,###");
            Tracks1BLabel.Text = librespotHelper.tracksWith1B.ToString("#,###");
            Tracks100MLabel.Text = librespotHelper.tracksWith100M.ToString("#,###");
            Tracks10MLabel.Text = librespotHelper.tracksWith10M.ToString("#,###");
            Tracks1MLabel.Text = librespotHelper.tracksWith1M.ToString("#,###");
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            LibrespotHelper test = new LibrespotHelper();
            MessageBox.Show(Convert.ToString(test.GetArtistInfo("4npEfmQ6YuiwW1GpUmaq3F").Data.Releases.AppearsOn.Releases.Count));
        }
    }
}
