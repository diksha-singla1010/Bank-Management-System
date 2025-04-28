' ATM Management System (Full Version with SQL Backend)

' Import necessary namespaces
Imports System.Data.SqlClient

' Connection String (adjust Data Source, Initial Catalog according to your setup)
Public Module DBConnection
    Public con As New SqlConnection("Data Source=YOUR_SERVER;Initial Catalog=ATM_DB;Integrated Security=True")
End Module

' Splash.vb
Public Class Splash
    Private Sub Splash_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Start()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Stop()
        Dim loginForm As New Login
        loginForm.Show()
        Me.Hide()
    End Sub
End Class

' Login.vb
Public Class Login
    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Try
            con.Open()
            Dim cmd As New SqlCommand("SELECT * FROM AccountTbl WHERE AccNum=@acc AND PIN=@pin", con)
            cmd.Parameters.AddWithValue("@acc", txtAccNum.Text)
            cmd.Parameters.AddWithValue("@pin", txtPin.Text)
            Dim da As New SqlDataAdapter(cmd)
            Dim dt As New DataTable()
            da.Fill(dt)

            If dt.Rows.Count > 0 Then
                MainForm.currentAccount = txtAccNum.Text
                Dim mainForm As New MainForm
                mainForm.Show()
                Me.Hide()
            Else
                MessageBox.Show("Invalid Account Number or PIN")
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            con.Close()
        End Try
    End Sub
End Class

' Registration.vb
Public Class Registration
    Private Sub btnRegister_Click(sender As Object, e As EventArgs) Handles btnRegister.Click
        Try
            con.Open()
            Dim cmd As New SqlCommand("INSERT INTO AccountTbl (AccNum, Name, Pin, Balance) VALUES (@acc, @name, @pin, @balance)", con)
            cmd.Parameters.AddWithValue("@acc", txtAccNum.Text)
            cmd.Parameters.AddWithValue("@name", txtName.Text)
            cmd.Parameters.AddWithValue("@pin", txtPin.Text)
            cmd.Parameters.AddWithValue("@balance", 0)
            cmd.ExecuteNonQuery()
            MessageBox.Show("Registration Successful!")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            con.Close()
        End Try
    End Sub
End Class

' MainForm.vb
Public Class MainForm
    Public Shared currentAccount As String

    Private Sub btnDeposit_Click(sender As Object, e As EventArgs) Handles btnDeposit.Click
        Dim depositForm As New Deposit
        depositForm.Show()
    End Sub

    Private Sub btnWithdraw_Click(sender As Object, e As EventArgs) Handles btnWithdraw.Click
        Dim withdrawForm As New Withdrawal
        withdrawForm.Show()
    End Sub

    Private Sub btnBalance_Click(sender As Object, e As EventArgs) Handles btnBalance.Click
        Dim balanceForm As New Balance
        balanceForm.Show()
    End Sub

    Private Sub btnChangePin_Click(sender As Object, e As EventArgs) Handles btnChangePin.Click
        Dim changePinForm As New ChangePin
        changePinForm.Show()
    End Sub
End Class

' Deposit.vb
Public Class Deposit
    Private Sub btnDeposit_Click(sender As Object, e As EventArgs) Handles btnDeposit.Click
        Try
            con.Open()
            Dim cmd As New SqlCommand("UPDATE AccountTbl SET Balance = Balance + @amt WHERE AccNum=@acc", con)
            cmd.Parameters.AddWithValue("@amt", CInt(txtAmount.Text))
            cmd.Parameters.AddWithValue("@acc", MainForm.currentAccount)
            cmd.ExecuteNonQuery()
            MessageBox.Show("Amount Deposited Successfully!")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            con.Close()
        End Try
    End Sub
End Class

' Withdrawal.vb
Public Class Withdrawal
    Private Sub btnWithdraw_Click(sender As Object, e As EventArgs) Handles btnWithdraw.Click
        Try
            con.Open()
            Dim checkBalanceCmd As New SqlCommand("SELECT Balance FROM AccountTbl WHERE AccNum=@acc", con)
            checkBalanceCmd.Parameters.AddWithValue("@acc", MainForm.currentAccount)
            Dim balance As Integer = CInt(checkBalanceCmd.ExecuteScalar())

            If balance >= CInt(txtAmount.Text) Then
                Dim withdrawCmd As New SqlCommand("UPDATE AccountTbl SET Balance = Balance - @amt WHERE AccNum=@acc", con)
                withdrawCmd.Parameters.AddWithValue("@amt", CInt(txtAmount.Text))
                withdrawCmd.Parameters.AddWithValue("@acc", MainForm.currentAccount)
                withdrawCmd.ExecuteNonQuery()
                MessageBox.Show("Amount Withdrawn Successfully!")
            Else
                MessageBox.Show("Insufficient Balance!")
            End If

            Me.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            con.Close()
        End Try
    End Sub
End Class

' Balance.vb
Public Class Balance
    Private Sub Balance_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            con.Open()
            Dim cmd As New SqlCommand("SELECT Balance FROM AccountTbl WHERE AccNum=@acc", con)
            cmd.Parameters.AddWithValue("@acc", MainForm.currentAccount)
            lblBalance.Text = "$" & cmd.ExecuteScalar().ToString()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            con.Close()
        End Try
    End Sub
End Class

' ChangePin.vb
Public Class ChangePin
    Private Sub btnChange_Click(sender As Object, e As EventArgs) Handles btnChange.Click
        Try
            con.Open()
            Dim cmd As New SqlCommand("UPDATE AccountTbl SET Pin=@newpin WHERE AccNum=@acc", con)
            cmd.Parameters.AddWithValue("@newpin", txtNewPin.Text)
            cmd.Parameters.AddWithValue("@acc", MainForm.currentAccount)
            cmd.ExecuteNonQuery()
            MessageBox.Show("PIN Changed Successfully!")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            con.Close()
        End Try
    End Sub
End Class

' MiniStatement.vb
Public Class MiniStatement
    Private Sub MiniStatement_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            con.Open()
            Dim cmd As New SqlCommand("SELECT TOP 5 * FROM TransactionTbl WHERE AccNum=@acc ORDER BY Tdate DESC", con)
            cmd.Parameters.AddWithValue("@acc", MainForm.currentAccount)
            Dim reader As SqlDataReader = cmd.ExecuteReader()

            While reader.Read()
                lstMiniStatement.Items.Add(reader("Type").ToString() & " $" & reader("Amount").ToString())
            End While
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            con.Close()
        End Try
    End Sub
End Class
