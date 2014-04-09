using System;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;

namespace Vts.Gui.Silverlight.View
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
                CurrentQuota.Text = (isf.Quota/1048576).ToString();
                SpaceUsed.Text = ((isf.Quota - isf.AvailableFreeSpace)/1048576).ToString();
                //SpaceAvailable.Text = (isf.AvailableFreeSpace/1048576).ToString();
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
