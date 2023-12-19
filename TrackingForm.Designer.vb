<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TrackingForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TrackingForm))
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.actionDate = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.action = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.actionLocation = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.actionNotes = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NotifyIcon1 = New System.Windows.Forms.NotifyIcon(Me.components)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToDeleteRows = False
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.actionDate, Me.action, Me.actionLocation, Me.actionNotes})
        Me.DataGridView1.Location = New System.Drawing.Point(12, 12)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.DataGridView1.Size = New System.Drawing.Size(776, 426)
        Me.DataGridView1.TabIndex = 0
        '
        'actionDate
        '
        Me.actionDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.actionDate.HeaderText = "Ημ/νία"
        Me.actionDate.Name = "actionDate"
        Me.actionDate.ReadOnly = True
        '
        'action
        '
        Me.action.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.action.HeaderText = "Ενέργεια"
        Me.action.Name = "action"
        Me.action.ReadOnly = True
        '
        'actionLocation
        '
        Me.actionLocation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.actionLocation.HeaderText = "Τοποθεσία"
        Me.actionLocation.Name = "actionLocation"
        Me.actionLocation.ReadOnly = True
        '
        'actionNotes
        '
        Me.actionNotes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.actionNotes.HeaderText = "Παρατηρήσεις"
        Me.actionNotes.Name = "actionNotes"
        Me.actionNotes.ReadOnly = True
        '
        'NotifyIcon1
        '
        Me.NotifyIcon1.Icon = CType(resources.GetObject("NotifyIcon1.Icon"), System.Drawing.Icon)
        Me.NotifyIcon1.Text = "Day1 Courier"
        Me.NotifyIcon1.Visible = True
        '
        'TrackingForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.DataGridView1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(816, 489)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(816, 489)
        Me.Name = "TrackingForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Dayone Courier Tracking Info"
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents DataGridView1 As Windows.Forms.DataGridView
    Friend WithEvents actionDate As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents action As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents actionLocation As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents actionNotes As Windows.Forms.DataGridViewTextBoxColumn
    Public WithEvents NotifyIcon1 As Windows.Forms.NotifyIcon
End Class
