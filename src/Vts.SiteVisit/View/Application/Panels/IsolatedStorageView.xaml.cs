using System;
using System.Windows;
using System.Windows.Controls;
using System.IO.IsolatedStorage;

namespace Vts.SiteVisit.View
{
    public partial class IsolatedStorageView : UserControl
    {
        public IsolatedStorageView()
        {
            InitializeComponent();

            SetStorageData();
        }

        private void SetStorageData()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                SpacedUsed.Text = "Current Spaced Used = " + (isf.Quota - isf.AvailableFreeSpace).ToString() + " bytes";
                SpaceAvaiable.Text = "Current Space Available = " + isf.AvailableFreeSpace.ToString() + " bytes";
                CurrentQuota.Text = "Current Quota = " + isf.Quota.ToString() + " bytes";
            }
        }

        private void IncreaseStorage(long spaceRequest)
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                long newSpace = isf.Quota + spaceRequest * 1048576; // 1MB = 1048576 bytes
                try
                {
                    if (true == isf.IncreaseQuotaTo(newSpace))
                    {
                        Results.Text = "Quota successfully increased.";
                    }
                    else
                    {
                        Results.Text = "Quota increase was unsuccessful.";
                    }
                }
                catch (Exception e)
                {
                    Results.Text = "An error occured: " + e.Message;
                }
                SetStorageData();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                long spaceRequest = Convert.ToInt64(SpaceRequest.Text);
                IncreaseStorage(spaceRequest);
            }
            catch
            { // User put bad data in text box }
            }
        }
    }
}
