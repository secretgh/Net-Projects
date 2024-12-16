using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

namespace SimpleNotifications
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Set up NotifyIcon
            NotifyIcon notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Asterisk,
                Visible = true,
                Text = "Notification Scheduler"
            };

            // Set up context menu
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Schedule Notification", null, ScheduleNotification);
            contextMenu.Items.Add("Exit", null, (s, e) => Application.Exit());

            notifyIcon.ContextMenuStrip = contextMenu;

            // Run the application
            Application.Run();
        }

        private static void ScheduleNotification(object sender, EventArgs e)
        {
            // Open scheduling form/dialog
            Form scheduleForm = new Form
            {
                Text = "Schedule Notification",
                Size = new Size(300, 250),
                StartPosition = FormStartPosition.CenterScreen
            };

            Label titleLabel = new Label { Text = "Title:", Location = new Point(10, 20), AutoSize = true };
            TextBox titleTextBox = new TextBox { Location = new Point(80, 20), Width = 180 };

            Label messageLabel = new Label { Text = "Message:", Location = new Point(10, 60), AutoSize = true };
            TextBox messageTextBox = new TextBox { Location = new Point(80, 60), Width = 180 };

            Label dateLabel = new Label { Text = "Date:", Location = new Point(10, 100), AutoSize = true };
            DateTimePicker datePicker = new DateTimePicker { Location = new Point(80, 100), Width = 180, Format = DateTimePickerFormat.Short };

            Label timeLabel = new Label { Text = "Time:", Location = new Point(10, 140), AutoSize = true };
            DateTimePicker timePicker = new DateTimePicker { Location = new Point(80, 140), Width = 180, Format = DateTimePickerFormat.Time, ShowUpDown = true };

            Button scheduleButton = new Button
            {
                Text = "Schedule",
                Location = new Point(80, 180),
                Width = 180
            };

            scheduleButton.Click += (s, args) =>
            {
                string title = titleTextBox.Text;
                string message = messageTextBox.Text;
                DateTime scheduledTime = datePicker.Value.Date + timePicker.Value.TimeOfDay;

                if (scheduledTime <= DateTime.Now)
                {
                    MessageBox.Show("The scheduled time must be in the future.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    ScheduleNotificationTask(title, message, scheduledTime);
                    MessageBox.Show("Notification scheduled!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    scheduleForm.Close();
                }
            };

            scheduleForm.Controls.Add(titleLabel);
            scheduleForm.Controls.Add(titleTextBox);
            scheduleForm.Controls.Add(messageLabel);
            scheduleForm.Controls.Add(messageTextBox);
            scheduleForm.Controls.Add(dateLabel);
            scheduleForm.Controls.Add(datePicker);
            scheduleForm.Controls.Add(timeLabel);
            scheduleForm.Controls.Add(timePicker);
            scheduleForm.Controls.Add(scheduleButton);

            scheduleForm.ShowDialog();
        }

        private static void ScheduleNotificationTask(string title, string message, DateTime scheduledTime)
        {
            double interval = (scheduledTime - DateTime.Now).TotalMilliseconds;

            System.Timers.Timer notificationTimer = new System.Timers.Timer(interval);
            notificationTimer.Elapsed += (s, e) =>
            {
                ShowNotification(title, message);
                notificationTimer.Stop();
                notificationTimer.Dispose();
            };
            notificationTimer.Start();
        }

        private static void ShowNotification(string title, string message)
        {
            NotifyIcon notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Exclamation,
                Visible = true
            };
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipText = message;
            notifyIcon.ShowBalloonTip(3000);
        }
    }
}

