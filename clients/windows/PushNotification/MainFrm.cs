
namespace PushNotification 
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.Linq;
	using System.ServiceModel;
	using System.ServiceModel.Description;
	using System.Text;
	using System.Windows.Forms;
	using NotificationService;
	
	public partial class MainFrm : Form {
		INotificationService _service = null;
		
		public MainFrm ()
		{
			InitializeComponent ();

			this.Load += (o, e) => {
				var factory = new ChannelFactory<INotificationService> ("NotificationService");
				factory.Endpoint.Behaviors.Add (new WebHttpBehavior ());
				_service = factory.CreateChannel ();
				_refreshRecipients ();
			};

			btnSend.Click += (s, e) => {
				String message = txtMessage.Text.Trim ();
				if (message.Length == 0) {
					MessageBox.Show (this, "Please specify a message in the text box", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				try {
					KeyValuePair<String, String> recipient = (KeyValuePair<String, String>)this.cmbRecipients.SelectedItem; ;
					_service.Push (message, new[] { recipient.Value });
					MessageBox.Show ("Message was sent successfully!");
					txtMessage.Text = String.Empty;
				} catch (Exception ex) {
					MessageBox.Show (this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			};

			txtMessage.TextChanged += (s, e) => {
				Int32 charsLeft = 165 - txtMessage.TextLength;
				lblCharLength.Text = Convert.ToString (charsLeft);

				if (charsLeft <= 20) {
					if (lblCharLength.ForeColor != Color.Red) {
						lblCharLength.ForeColor = Color.Red;
						lblCharLength.Font = new Font (lblCharLength.Font, FontStyle.Bold);
					}
				} else {
					if (lblCharLength.ForeColor != Color.Green) {
						lblCharLength.ForeColor = Color.Green;
						lblCharLength.Font = new Font (lblCharLength.Font, FontStyle.Regular);
					}
				}

				btnSend.Enabled = charsLeft != 165;
			};

			this.btnRefresh.Click += (o, e) => _refreshRecipients ();
		}


		void _refreshRecipients ()
		{
			this.cmbRecipients.DataSource = null;
			this.cmbRecipients.Items.Clear ();
			
			try {
				var items = _service.GetRecipients ();
				if (items.Count () > 0) {
					this.cmbRecipients.DataSource = new BindingSource (items, null);
					this.cmbRecipients.SelectedIndex = 0;
				}
	
			} catch (Exception) {
				// Do nothing because the service is not running
				MessageBox.Show (this, "Appears that the Push service is not running", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
	}
}
