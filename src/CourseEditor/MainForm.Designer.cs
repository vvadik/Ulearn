namespace CourseEditor
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.fileSystemWatcher = new System.IO.FileSystemWatcher();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openCourseDirecotryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openInCourseInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
			this.TocTree = new System.Windows.Forms.TreeView();
			this.slideLayout = new System.Windows.Forms.TableLayoutPanel();
			this.titleTextbox = new System.Windows.Forms.TextBox();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).BeginInit();
			this.menuStrip.SuspendLayout();
			this.mainLayout.SuspendLayout();
			this.slideLayout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// openFileDialog
			// 
			this.openFileDialog.FileName = "openFileDialog";
			// 
			// fileSystemWatcher
			// 
			this.fileSystemWatcher.EnableRaisingEvents = true;
			this.fileSystemWatcher.SynchronizingObject = this;
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(1501, 24);
			this.menuStrip.TabIndex = 0;
			this.menuStrip.Text = "menuStrip";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openCourseDirecotryToolStripMenuItem,
            this.openInCourseInBrowserToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// openCourseDirecotryToolStripMenuItem
			// 
			this.openCourseDirecotryToolStripMenuItem.Name = "openCourseDirecotryToolStripMenuItem";
			this.openCourseDirecotryToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
			this.openCourseDirecotryToolStripMenuItem.Text = "Open course direcotry";
			this.openCourseDirecotryToolStripMenuItem.Click += new System.EventHandler(this.openCourseDirecotryToolStripMenuItem_Click);
			// 
			// openInCourseInBrowserToolStripMenuItem
			// 
			this.openInCourseInBrowserToolStripMenuItem.Name = "openInCourseInBrowserToolStripMenuItem";
			this.openInCourseInBrowserToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
			this.openInCourseInBrowserToolStripMenuItem.Text = "Open in course in browser";
			// 
			// mainLayout
			// 
			this.mainLayout.ColumnCount = 3;
			this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1F));
			this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
			this.mainLayout.Controls.Add(this.TocTree, 0, 0);
			this.mainLayout.Controls.Add(this.slideLayout, 1, 0);
			this.mainLayout.Controls.Add(this.pictureBox, 2, 0);
			this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainLayout.Location = new System.Drawing.Point(0, 24);
			this.mainLayout.Name = "mainLayout";
			this.mainLayout.RowCount = 1;
			this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 83.33334F));
			this.mainLayout.Size = new System.Drawing.Size(1501, 523);
			this.mainLayout.TabIndex = 1;
			// 
			// TocTree
			// 
			this.TocTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.TocTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TocTree.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.TocTree.Location = new System.Drawing.Point(0, 0);
			this.TocTree.Margin = new System.Windows.Forms.Padding(0);
			this.TocTree.Name = "TocTree";
			this.TocTree.Scrollable = false;
			this.TocTree.Size = new System.Drawing.Size(300, 523);
			this.TocTree.TabIndex = 0;
			this.TocTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TocTree_AfterSelect);
			// 
			// slideLayout
			// 
			this.slideLayout.AutoScroll = true;
			this.slideLayout.AutoSize = true;
			this.slideLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.slideLayout.ColumnCount = 1;
			this.slideLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.slideLayout.Controls.Add(this.titleTextbox, 0, 0);
			this.slideLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.slideLayout.Location = new System.Drawing.Point(303, 3);
			this.slideLayout.Name = "slideLayout";
			this.slideLayout.RowCount = 1;
			this.slideLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.slideLayout.Size = new System.Drawing.Size(1, 517);
			this.slideLayout.TabIndex = 2;
			// 
			// titleTextbox
			// 
			this.titleTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.titleTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.titleTextbox.Location = new System.Drawing.Point(3, 3);
			this.titleTextbox.Name = "titleTextbox";
			this.titleTextbox.Size = new System.Drawing.Size(100, 29);
			this.titleTextbox.TabIndex = 0;
			// 
			// pictureBox
			// 
			this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox.Location = new System.Drawing.Point(304, 3);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(1194, 517);
			this.pictureBox.TabIndex = 3;
			this.pictureBox.TabStop = false;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1501, 547);
			this.Controls.Add(this.mainLayout);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
			this.Name = "MainForm";
			this.Text = "Course Editor";
			((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).EndInit();
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.mainLayout.ResumeLayout(false);
			this.mainLayout.PerformLayout();
			this.slideLayout.ResumeLayout(false);
			this.slideLayout.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.IO.FileSystemWatcher fileSystemWatcher;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openCourseDirecotryToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openInCourseInBrowserToolStripMenuItem;
		private System.Windows.Forms.TableLayoutPanel mainLayout;
		private System.Windows.Forms.TreeView TocTree;
		private System.Windows.Forms.TableLayoutPanel slideLayout;
		private System.Windows.Forms.TextBox titleTextbox;
		private System.Windows.Forms.PictureBox pictureBox;
	}
}

