<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class CloseVoucher
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
        Me.Button1 = New System.Windows.Forms.Button()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabACS = New System.Windows.Forms.TabPage()
        Me.LinkLabel1 = New System.Windows.Forms.LinkLabel()
        Me.PictureBox3 = New System.Windows.Forms.PictureBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.TabGeniki = New System.Windows.Forms.TabPage()
        Me.PictureBox5 = New System.Windows.Forms.PictureBox()
        Me.LinkLabel2 = New System.Windows.Forms.LinkLabel()
        Me.PictureBox4 = New System.Windows.Forms.PictureBox()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.TabCC = New System.Windows.Forms.TabPage()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.PictureBox6 = New System.Windows.Forms.PictureBox()
        Me.LinkLabel3 = New System.Windows.Forms.LinkLabel()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.TabControl1.SuspendLayout()
        Me.TabACS.SuspendLayout()
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabGeniki.SuspendLayout()
        CType(Me.PictureBox5, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabCC.SuspendLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox6, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(58, 265)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(213, 48)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "Κλείσιμο Voucher ACS"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabACS)
        Me.TabControl1.Controls.Add(Me.TabGeniki)
        Me.TabControl1.Controls.Add(Me.TabCC)
        Me.TabControl1.Location = New System.Drawing.Point(12, 12)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(329, 365)
        Me.TabControl1.TabIndex = 3
        '
        'TabACS
        '
        Me.TabACS.BackColor = System.Drawing.SystemColors.Control
        Me.TabACS.Controls.Add(Me.LinkLabel1)
        Me.TabACS.Controls.Add(Me.PictureBox3)
        Me.TabACS.Controls.Add(Me.PictureBox1)
        Me.TabACS.Controls.Add(Me.Button1)
        Me.TabACS.Location = New System.Drawing.Point(4, 22)
        Me.TabACS.Name = "TabACS"
        Me.TabACS.Padding = New System.Windows.Forms.Padding(3)
        Me.TabACS.Size = New System.Drawing.Size(321, 339)
        Me.TabACS.TabIndex = 0
        Me.TabACS.Text = "ACS"
        '
        'LinkLabel1
        '
        Me.LinkLabel1.AutoSize = True
        Me.LinkLabel1.Location = New System.Drawing.Point(82, 316)
        Me.LinkLabel1.Name = "LinkLabel1"
        Me.LinkLabel1.Size = New System.Drawing.Size(164, 13)
        Me.LinkLabel1.TabIndex = 3
        Me.LinkLabel1.TabStop = True
        Me.LinkLabel1.Text = "Άνοιγμα φακέλου αποθήκευσης"
        Me.LinkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PictureBox3
        '
        Me.PictureBox3.Image = Global.D1_CourierConnector.My.Resources.Resources.partner_logo_1
        Me.PictureBox3.Location = New System.Drawing.Point(58, 203)
        Me.PictureBox3.Name = "PictureBox3"
        Me.PictureBox3.Size = New System.Drawing.Size(213, 56)
        Me.PictureBox3.TabIndex = 2
        Me.PictureBox3.TabStop = False
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.D1_CourierConnector.My.Resources.Resources.Untitled_1
        Me.PictureBox1.Location = New System.Drawing.Point(58, 6)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(213, 191)
        Me.PictureBox1.TabIndex = 1
        Me.PictureBox1.TabStop = False
        '
        'TabGeniki
        '
        Me.TabGeniki.BackColor = System.Drawing.SystemColors.Control
        Me.TabGeniki.Controls.Add(Me.PictureBox5)
        Me.TabGeniki.Controls.Add(Me.LinkLabel2)
        Me.TabGeniki.Controls.Add(Me.PictureBox4)
        Me.TabGeniki.Controls.Add(Me.Button2)
        Me.TabGeniki.Location = New System.Drawing.Point(4, 22)
        Me.TabGeniki.Name = "TabGeniki"
        Me.TabGeniki.Padding = New System.Windows.Forms.Padding(3)
        Me.TabGeniki.Size = New System.Drawing.Size(321, 339)
        Me.TabGeniki.TabIndex = 1
        Me.TabGeniki.Text = "Γενική Ταχ."
        '
        'PictureBox5
        '
        Me.PictureBox5.Image = Global.D1_CourierConnector.My.Resources.Resources.Untitled_1
        Me.PictureBox5.Location = New System.Drawing.Point(58, 6)
        Me.PictureBox5.Name = "PictureBox5"
        Me.PictureBox5.Size = New System.Drawing.Size(213, 191)
        Me.PictureBox5.TabIndex = 6
        Me.PictureBox5.TabStop = False
        '
        'LinkLabel2
        '
        Me.LinkLabel2.AutoSize = True
        Me.LinkLabel2.Location = New System.Drawing.Point(82, 316)
        Me.LinkLabel2.Name = "LinkLabel2"
        Me.LinkLabel2.Size = New System.Drawing.Size(164, 13)
        Me.LinkLabel2.TabIndex = 8
        Me.LinkLabel2.TabStop = True
        Me.LinkLabel2.Text = "Άνοιγμα φακέλου αποθήκευσης"
        Me.LinkLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PictureBox4
        '
        Me.PictureBox4.Image = Global.D1_CourierConnector.My.Resources.Resources.partner_logo_1
        Me.PictureBox4.Location = New System.Drawing.Point(58, 203)
        Me.PictureBox4.Name = "PictureBox4"
        Me.PictureBox4.Size = New System.Drawing.Size(213, 56)
        Me.PictureBox4.TabIndex = 7
        Me.PictureBox4.TabStop = False
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(58, 265)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(213, 48)
        Me.Button2.TabIndex = 3
        Me.Button2.Text = "Κλείσιμο Voucher Γενικής"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'TabCC
        '
        Me.TabCC.BackColor = System.Drawing.SystemColors.Control
        Me.TabCC.Controls.Add(Me.PictureBox2)
        Me.TabCC.Controls.Add(Me.PictureBox6)
        Me.TabCC.Controls.Add(Me.LinkLabel3)
        Me.TabCC.Controls.Add(Me.Button3)
        Me.TabCC.Location = New System.Drawing.Point(4, 22)
        Me.TabCC.Name = "TabCC"
        Me.TabCC.Padding = New System.Windows.Forms.Padding(3)
        Me.TabCC.Size = New System.Drawing.Size(321, 339)
        Me.TabCC.TabIndex = 2
        Me.TabCC.Text = "Courier Center"
        '
        'PictureBox2
        '
        Me.PictureBox2.Image = Global.D1_CourierConnector.My.Resources.Resources.Untitled_1
        Me.PictureBox2.Location = New System.Drawing.Point(58, 6)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(213, 191)
        Me.PictureBox2.TabIndex = 10
        Me.PictureBox2.TabStop = False
        '
        'PictureBox6
        '
        Me.PictureBox6.Image = Global.D1_CourierConnector.My.Resources.Resources.partner_logo_1
        Me.PictureBox6.Location = New System.Drawing.Point(58, 203)
        Me.PictureBox6.Name = "PictureBox6"
        Me.PictureBox6.Size = New System.Drawing.Size(213, 56)
        Me.PictureBox6.TabIndex = 11
        Me.PictureBox6.TabStop = False
        '
        'LinkLabel3
        '
        Me.LinkLabel3.AutoSize = True
        Me.LinkLabel3.Location = New System.Drawing.Point(82, 316)
        Me.LinkLabel3.Name = "LinkLabel3"
        Me.LinkLabel3.Size = New System.Drawing.Size(164, 13)
        Me.LinkLabel3.TabIndex = 9
        Me.LinkLabel3.TabStop = True
        Me.LinkLabel3.Text = "Άνοιγμα φακέλου αποθήκευσης"
        Me.LinkLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(58, 265)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(213, 48)
        Me.Button3.TabIndex = 4
        Me.Button3.Text = "Κλείσιμο Courier Center"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(61, 4)
        '
        'CloseVoucher
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(356, 387)
        Me.Controls.Add(Me.TabControl1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "CloseVoucher"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Form2"
        Me.TabControl1.ResumeLayout(False)
        Me.TabACS.ResumeLayout(False)
        Me.TabACS.PerformLayout()
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabGeniki.ResumeLayout(False)
        Me.TabGeniki.PerformLayout()
        CType(Me.PictureBox5, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabCC.ResumeLayout(False)
        Me.TabCC.PerformLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox6, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Button1 As Windows.Forms.Button
    Friend WithEvents PictureBox1 As Windows.Forms.PictureBox
    Friend WithEvents TabControl1 As Windows.Forms.TabControl
    Friend WithEvents TabACS As Windows.Forms.TabPage
    Friend WithEvents TabGeniki As Windows.Forms.TabPage
    Friend WithEvents Button2 As Windows.Forms.Button
    Friend WithEvents PictureBox3 As Windows.Forms.PictureBox
    Friend WithEvents PictureBox4 As Windows.Forms.PictureBox
    Friend WithEvents PictureBox5 As Windows.Forms.PictureBox
    Friend WithEvents LinkLabel1 As Windows.Forms.LinkLabel
    Friend WithEvents LinkLabel2 As Windows.Forms.LinkLabel
    Friend WithEvents TabCC As Windows.Forms.TabPage
    Friend WithEvents Button3 As Windows.Forms.Button
    Friend WithEvents LinkLabel3 As Windows.Forms.LinkLabel
    Friend WithEvents PictureBox6 As Windows.Forms.PictureBox
    Friend WithEvents PictureBox2 As Windows.Forms.PictureBox
    Friend WithEvents ContextMenuStrip1 As Windows.Forms.ContextMenuStrip
End Class
