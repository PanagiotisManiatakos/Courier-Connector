Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Windows.Forms
Imports Softone

Public Class MassVoucherError

    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick
        If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            Dim str = "SALDOC[AUTOLOCATE=" + DataGridView1.Rows(e.RowIndex).Cells("FINDOC").Value.ToString + "]"
            XXX.ExecS1Command(str, New Form)
        End If
    End Sub

    Private Sub DataGridView1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DataGridView1.CellFormatting
        DataGridView1.Rows(e.RowIndex).HeaderCell.Value = CStr(e.RowIndex + 1)
    End Sub
End Class