using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrimeNumbers_GUI
{
    public partial class MainForm : Form
    {

        private CancellationTokenSource cancellationTokenSource;

        public MainForm()
        {
            InitializeComponent();
        }
        
        private async void startButton_Click(object sender, EventArgs e) {

            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            // Find all prime numbers starting between the first and last numbers
            int firstNum = Convert.ToInt32(startNumTextBox.Text);
            int lastNum = Convert.ToInt32(endNumTextBox.Text);

            numbersTextBox.Clear();

            // Prevent user from messing with certain controls while job is running
            progressBar1.Minimum = firstNum;
            progressBar1.Maximum = lastNum;
            progressBar1.Visible = true;
            cancelButton.Enabled = true;
            pauseButton.Enabled = true;
            startButton.Enabled = false;
            startNumTextBox.Enabled = false;
            endNumTextBox.Enabled = false;

            UseWaitCursor = true;

            // See which numbers are factors and append them to the numbers text box
            await Task.Run(() => {
                for (int i = firstNum; i <= lastNum; i++) {
                    if (token.IsCancellationRequested)
                        break;

                    if (IsPrime(i)) {
                        AddNumberToTextBox(i);
                    }
                }
            });

            // Let the user know we did something even if no prime nums were found
            if (numbersTextBox.TextLength == 0)
            {
                numbersTextBox.Text = "None.";
            }

            UseWaitCursor = false;

            // Reset the form
            startNumTextBox.Enabled = true;
            endNumTextBox.Enabled = true;
            progressBar1.Value = progressBar1.Minimum;
            progressBar1.Visible = false;
            cancelButton.Enabled = false;
            pauseButton.Enabled = false;
            startButton.Enabled = true;
        }

        private bool IsPrime(int num)
        {
            if (num < 2)
                return false;

            // Look for a number that evenly divides the num
            for (int i = 2; i <= num / 2; i++)
                if (num % i == 0)
                    return false;

            // No divisors means the number is prime
            return true;
        }

        private void AddNumberToTextBox(int num) {
            try {
                Invoke((Action)delegate () {
                    numbersTextBox.AppendText(num + "\n");
                    progressBar1.Value = num;
                });
            } catch (ObjectDisposedException) {
                // The form was closed before the thread completed.
            }
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            // Pause or resume the current job
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            // Cancel the work done in the for loop
            cancellationTokenSource.Cancel();
        }
    }
}
