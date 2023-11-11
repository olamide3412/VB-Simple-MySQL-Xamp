Imports MySql.Data.MySqlClient

Public Class Form1

    'Dim Connection As New MySqlConnection("server=db4free.net; Port=3306; user=olamide3412; password=Pass@1234; database=my_vb_database; Connection Timeout=100")
    'Dim Connection As New MySqlConnection("server=db4free.net;Port=3306; User ID=olamide3412; password=Pass@1234; database=my_vb_database")
    Dim Connection As New MySqlConnection("server=localhost;  Port=3306; user=root; password=; database=my_vb_database")

    Dim MySqlCmd As New MySqlCommand
    Dim MySqlAdapter As New MySqlDataAdapter
    Dim dr As MySqlDataReader
    Dim Table_Name As String = "student_record"
    Dim Data As Integer
    Dim DT As New DataTable

    Dim IMG_FileNameInput As String
    Dim isImageSelected As Boolean = False
    Dim isEdit As Boolean = False

    Dim regno, fName, address, fClass As String

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'If Connection.State = ConnectionState.Open Then
        '    Connection.Close()
        'End If
        Connection.Open()



        'showRecordWithLoop()
        'showRecordFromDB()
        'showRecordUsingDr()
        'BackgroundWorker1.RunWorkerAsync()

    End Sub


    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If IMG_FileNameInput = "" Then
            MsgBox("Select Image")
            Return

        End If
        Try
            If Connection.State = ConnectionState.Open Then
                Connection.Close()
            End If
            Connection.Open()

        Catch ex As Exception
            MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        Try
            MySqlCmd.CommandType = CommandType.Text
            MySqlCmd.CommandText = "SELECT regNum FROM " & Table_Name & " WHERE regNum LIKE " & txtRegNum.Text
            MySqlAdapter = New MySqlDataAdapter(MySqlCmd.CommandText, Connection)
            DT = New DataTable
            Data = MySqlAdapter.Fill(DT)
            If Data > 0 Then
                MsgBox("Student record already exist")
            Else

                Dim mstream As New System.IO.MemoryStream()
                Dim arrImage() As Byte


                PictureBoxImageInput.Image.Save(mstream, System.Drawing.Imaging.ImageFormat.Jpeg)
                arrImage = mstream.GetBuffer()


                MySqlCmd = New MySqlCommand
                With MySqlCmd
                    .CommandText = "INSERT INTO " & Table_Name & "(regNum, fullName, address, class, image) VALUES (@regNumber, @fullname, @address, @class, @image)"
                    .Connection = Connection
                    .Parameters.AddWithValue("@regNumber", txtRegNum.Text)
                    .Parameters.AddWithValue("@fullname", txtFullName.Text)
                    .Parameters.AddWithValue("@address", txtAddress.Text)
                    .Parameters.AddWithValue("@class", txtClass.Text)
                    .Parameters.AddWithValue("@image", arrImage)
                    .ExecuteNonQuery()

                End With
                IMG_FileNameInput = ""

                Connection.Close()
                showRecordWithLoop()
                MsgBox("New Student record added successfully", MsgBoxStyle.Information)
            End If


        Catch ex As Exception
            MsgBox("Data failed to save !!!" & vbCr & ex.Message, MsgBoxStyle.Critical, "Error Message")
            Connection.Close()
        End Try



    End Sub


    Private Sub BtnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click

        Try

            If Connection.State = ConnectionState.Open Then

                Connection.Close()

            End If

            Connection.Open()

        Catch ex As Exception
            MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try





        MySqlCmd = New MySqlCommand
        MySqlCmd.CommandType = CommandType.Text
        MySqlCmd.CommandText = "SELECT regNum, fullName, address, class, image FROM " & Table_Name & " WHERE regNum = " & txtFind.Text
        MySqlAdapter = New MySqlDataAdapter(MySqlCmd.CommandText, Connection)
        DT = New DataTable
        Data = MySqlAdapter.Fill(DT)

        If Data > 0 Then
            txtRegNum.Text = DT.Rows(0).Item("regNum")
            txtFullName.Text = DT.Rows(0).Item("fullName")
            txtAddress.Text = DT.Rows(0).Item("address")
            txtClass.Text = DT.Rows(0).Item("class")

            Dim ImgArray() As Byte = DT.Rows(0).Item("image")
            Dim lmgStr As New System.IO.MemoryStream(ImgArray)
            PictureBoxImageInput.Image = Image.FromStream(lmgStr)
            PictureBoxImageInput.SizeMode = PictureBoxSizeMode.Zoom
            lmgStr.Close()

        End If

        ' MsgBox("Count " + CStr(Data))


    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Try
            If Connection.State = ConnectionState.Open Then
                Connection.Close()
            End If
            Connection.Open()

        Catch ex As Exception
            MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        Dim mstream As New System.IO.MemoryStream()
        Dim arrImage() As Byte

        If isImageSelected = True Then

            PictureBoxImageInput.Image.Save(mstream, System.Drawing.Imaging.ImageFormat.Jpeg)
            arrImage = mstream.GetBuffer()

        End If


        MySqlCmd = New MySqlCommand
        With MySqlCmd


            If isImageSelected = True Then
                .CommandText = "UPDATE " & Table_Name & " SET  fullName=@fullname, address=@address, class=@class, image=@image WHERE regNum=@regNumber "
                .Parameters.AddWithValue("@image", arrImage)
            Else
                .CommandText = "UPDATE " & Table_Name & " SET  fullName=@fullname, address=@address, class=@class WHERE regNum=@regNumber "
            End If
            .Connection = Connection
            .Parameters.AddWithValue("@regNumber", txtRegNum.Text)
            .Parameters.AddWithValue("@fullname", txtFullName.Text)
            .Parameters.AddWithValue("@address", txtAddress.Text)
            .Parameters.AddWithValue("@class", txtClass.Text)


            .ExecuteNonQuery()
            IMG_FileNameInput = ""
            isImageSelected = False
        End With
        showRecordWithLoop()
        MsgBox("Data updated successfully", MsgBoxStyle.Information, "Information")
    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Try
            If Connection.State = ConnectionState.Open Then
                Connection.Close()
            End If
            Connection.Open()

        Catch ex As Exception
            MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try





        MySqlCmd.CommandType = CommandType.Text
        MySqlCmd.CommandText = "SELECT regNum FROM " & Table_Name & " WHERE regNum = " & txtRegNum.Text
        MySqlAdapter = New MySqlDataAdapter(MySqlCmd.CommandText, Connection)
        DT = New DataTable
        Data = MySqlAdapter.Fill(DT)
        If Data > 0 Then
            MySqlCmd.CommandType = CommandType.Text
            MySqlCmd.CommandText = "DELETE FROM " & Table_Name & " WHERE regNum='" & txtFind.Text & "'"
            MySqlCmd.Connection = Connection
            MySqlCmd.ExecuteNonQuery()
            MsgBox("Record deleted")
        Else
            MsgBox("Record not found")

        End If










    End Sub

    Private Sub PictureBoxImageInput_Click(sender As Object, e As EventArgs) Handles PictureBoxImageInput.Click
        OpenFileDialog1.FileName = ""
        'OpenFileDialog1.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.Desktop
        OpenFileDialog1.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG" ' "JPEG (*.jpeg;*.jpg)|*.jpeg;*.jpg"

        If (OpenFileDialog1.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
            IMG_FileNameInput = OpenFileDialog1.FileName
            PictureBoxImageInput.ImageLocation = IMG_FileNameInput
            isImageSelected = True

        End If
    End Sub



    Private Sub DataGridView1_CellMouseDown(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DataGridView1.CellMouseDown
        Try
            If e.Button = MouseButtons.Left Then
                DataGridView1.CurrentCell = DataGridView1(e.ColumnIndex, e.RowIndex)
                Dim i As Integer
                With DataGridView1
                    If e.RowIndex >= 0 Then
                        i = .CurrentRow.Index
                        'txtFind.Text = .Rows(i).Cells(0).Value.ToString
                        'txtRegNum.Text = .Rows(i).Cells(0).Value.ToString
                        'txtFullName.Text = .Rows(i).Cells(1).Value.ToString
                        'txtAddress.Text = .Rows(i).Cells(2).Value.ToString
                        'txtClass.Text = .Rows(i).Cells(3).Value.ToString
                        isEdit = False
                        LoadImages(.Rows(i).Cells(0).Value.ToString)
                    End If
                End With
            End If
        Catch ex As Exception

        End Try




    End Sub

    Sub LoadImages(ByVal regNo As String)

        Try
            If Connection.State = ConnectionState.Open Then
                Connection.Close()
            End If
            Connection.Open()

        Catch ex As Exception
            MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try



        MySqlCmd.CommandType = CommandType.Text
        MySqlCmd.CommandText = "SELECT image FROM " & Table_Name & " WHERE regNum = " & regNo
        MySqlAdapter = New MySqlDataAdapter(MySqlCmd.CommandText, Connection)
        DT = New DataTable
        Data = MySqlAdapter.Fill(DT)

        If Data > 0 Then
            Dim ImgArray() As Byte = DT.Rows(0).Item("image")
            Dim ImgStr As New System.IO.MemoryStream(ImgArray)
            If isEdit = True Then
                PictureBoxImageInput.Image = Image.FromStream(ImgStr)
            Else
                PictureBoxViewRecord.Image = Image.FromStream(ImgStr)
            End If


            ImgStr.Close()
            Connection.Close()
        End If


    End Sub

    Private  Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        showRecordWithLoop()

    End Sub

    Sub showRecordWithLoop()
        Try
            If Connection.State = ConnectionState.Open Then
                Connection.Close()
            End If
            Connection.Open()

        Catch ex As Exception
            MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        MySqlCmd.CommandType = CommandType.Text
        MySqlCmd.CommandText = "Select regNum, fullName, address, class, image from " & Table_Name
        MySqlAdapter = New MySqlDataAdapter(MySqlCmd.CommandText, Connection)

        DT = New DataTable
        Data = MySqlAdapter.Fill(DT)

        If DataGridView1.RowCount > 0 Then

            DataGridView1.Rows.Clear()

        End If


        Dim var, startval, endval As Integer

        startval = 0 'DataGridView1.TabIndex = 0
        endval = Data - 1

        If Data > 0 Then
            For var = startval To endval
                Dim rNum As Integer = DataGridView1.Rows.Add

                DataGridView1.Rows.Item(rNum).Cells(0).Value = DT.Rows(var).Item("regNum")
                DataGridView1.Rows.Item(rNum).Cells(1).Value = DT.Rows(var).Item("fullName")
                DataGridView1.Rows.Item(rNum).Cells(2).Value = DT.Rows(var).Item("address")
                DataGridView1.Rows.Item(rNum).Cells(3).Value = DT.Rows(var).Item("class")

                'Dim ImgArray() As Byte = DT.Rows(var).Item("image")
                'Dim lmgStr As New System.IO.MemoryStream(ImgArray)
                'DataGridView1.Rows.Item(rNum).Cells(4).Value = Image.FromStream(lmgStr)
                'lmgStr.Close()

            Next

        End If

    End Sub

    Sub showRecordFromDB()
        Try
            If Connection.State = ConnectionState.Open Then
                Connection.Close()
            End If
            Connection.Open()

        Catch ex As Exception
            MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        MySqlCmd.CommandType = CommandType.Text
        MySqlCmd.CommandText = "Select regNum, fullName, address, class, image from " & Table_Name
        MySqlAdapter = New MySqlDataAdapter(MySqlCmd.CommandText, Connection)

        DT = New DataTable




        Data = MySqlAdapter.Fill(DT)
        If Data > 0 Then
            DataGridView1.DataSource = Nothing
            DataGridView1.DataSource = DT
            DataGridView1.ClearSelection()

        End If


    End Sub

    Async Sub showRecordUsingDr()

        Try
            If Connection.State = ConnectionState.Open Then
                Connection.Close()
            End If
            Connection.Open()

        Catch ex As Exception
            MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try
        Dim i As Integer

        MySqlCmd = New MySqlCommand("Select regNum, fullName, address, class, image from student_record", Connection)
         dr =  MySqlCmd.ExecuteReader
        While dr.Read
            i += 1
            DataGridView1.Rows.Add(dr.Item("regNum"), dr.Item("fullName"), dr.Item("address"), dr.Item("class"))
        End While


    End Sub



    Sub showDG()
        'DataGridView1.Rows.Add(dr.Item("regNum"), dr.Item("fullName"), dr.Item("address"), dr.Item("class"))
        Me.BeginInvoke(Sub() DataGridView1.Rows.Add(dr.Item("regNum"), dr.Item("fullName"), dr.Item("address"), dr.Item("class")))

    End Sub



    Sub exAddingDatatoMySqlDb()
        Try
            If Connection.State = ConnectionState.Open Then
                Connection.Close()
            End If
            Connection.Open()

        Catch ex As Exception
            MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        Dim mstream As New System.IO.MemoryStream()
        Dim arrImage() As Byte


        PictureBoxImageInput.Image.Save(mstream, System.Drawing.Imaging.ImageFormat.Jpeg)
        arrImage = mstream.GetBuffer()


        MySqlCmd = New MySqlCommand
        With MySqlCmd
            If IMG_FileNameInput <> "" Then
                .CommandText = "UPDATE " & Table_Name & " SET  fullName=@fullname, address=@address, class=@class, image=@image WHERE regNum=@regNumber "

                .Parameters.AddWithValue("@image", arrImage)
            Else
                .CommandText = "UPDATE " & Table_Name & " SET  fullName=@fullname, address=@address, class=@class WHERE regNum=@regNumber "

            End If
            .Connection = Connection
            .Parameters.AddWithValue("@regNumber", txtRegNum.Text)
            .Parameters.AddWithValue("@fullname", txtFullName.Text)
            .Parameters.AddWithValue("@address", txtAddress.Text)
            .Parameters.AddWithValue("@class", txtClass.Text)


            .ExecuteNonQuery()
            IMG_FileNameInput = ""
        End With
        showRecordWithLoop()
        MsgBox("Data updated successfully", MsgBoxStyle.Information, "Information")
    End Sub

    Private Sub DeleteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem.Click
        If DataGridView1.RowCount = 0 Then
            MsgBox("Cannot delete, table data is empty", MsgBoxStyle.Critical, "Error Message")
            Return
        End If

        If DataGridView1.SelectedRows.Count = 0 Then
            MsgBox("Cannot delete, select the table data to be deleted", MsgBoxStyle.Critical, "Error Message")
            Return
        End If

        If MsgBox("Delete record?", MsgBoxStyle.Question + MsgBoxStyle.OkCancel, "Confirmation") = MsgBoxResult.Cancel Then Return

        Try
            If Connection.State = ConnectionState.Open Then
                Connection.Close()
            End If
            Connection.Open()

        Catch ex As Exception
            MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try



        For Each row As DataGridViewRow In DataGridView1.SelectedRows
            Dim regNum As String = DataGridView1.Item(0, row.Index).Value

            If row.Selected = True Then
                MySqlCmd.CommandType = CommandType.Text
                MySqlCmd.CommandText = "delete from " & Table_Name & " where regNum='" & regNum & "'"
                MySqlCmd.Connection = Connection
                MySqlCmd.ExecuteNonQuery()
            End If
        Next

        showRecordWithLoop()
        PictureBoxImageInput.Image = My.Resources.photo
        PictureBoxViewRecord.Image = Nothing

    End Sub



    Private Sub EditToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditToolStripMenuItem.Click

        Dim row As Integer = DataGridView1.CurrentCell.RowIndex

        txtRegNum.Text = DataGridView1.Item(0, row).Value
        txtFind.Text = DataGridView1.Item(0, row).Value
        txtFullName.Text = DataGridView1.Item(1, row).Value
        txtAddress.Text = DataGridView1.Item(2, row).Value
        txtClass.Text = DataGridView1.Item(3, row).Value

        isEdit = True

        LoadImages(DataGridView1.Item(0, row).Value)


    End Sub





End Class
