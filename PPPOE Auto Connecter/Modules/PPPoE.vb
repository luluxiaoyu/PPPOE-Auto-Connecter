Imports DotRas
Imports System.Collections.ObjectModel
Imports System.Net

Module PPPoE
    Public Sub Disconnect() '断开连接
        Dim conList As ReadOnlyCollection(Of RasConnection) = RasConnection.GetActiveConnections()

        For Each con As RasConnection In conList
            con.HangUp()
        Next
    End Sub
    Public Function Connect(ByVal PPPOEname As String, ByVal username As String, ByVal password As String, ByRef msg As String) As Boolean '创建连接
        Try
            CreateOrUpdatePPPOE(PPPOEname)

            Using dialer As RasDialer = New RasDialer()
                dialer.EntryName = PPPOEname
                dialer.AllowUseStoredCredentials = True
                dialer.Timeout = 1000
                dialer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers)
                dialer.Credentials = New NetworkCredential(username, password)
                dialer.Dial()
                Return True
            End Using

        Catch re As RasException
            msg = re.ErrorCode & " " + re.Message
            Return False
        End Try
    End Function
    Public Sub CreateOrUpdatePPPOE(ByVal updatePPPOEname As String) '新建/更新pppoe
        Dim dialer As RasDialer = New RasDialer()
        Dim allUsersPhoneBook As RasPhoneBook = New RasPhoneBook()
        Dim path As String = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers)
        allUsersPhoneBook.Open(path)

        If allUsersPhoneBook.Entries.Contains(updatePPPOEname) Then
            allUsersPhoneBook.Entries(updatePPPOEname).PhoneNumber = " "
            allUsersPhoneBook.Entries(updatePPPOEname).Update()
        Else
            Dim adds As String = String.Empty
            Dim readOnlyCollection As ReadOnlyCollection(Of RasDevice) = RasDevice.GetDevices()
            Dim device As RasDevice = RasDevice.GetDevices().Where(Function(o) o.DeviceType = RasDeviceType.PPPoE).First()
            Dim entry As RasEntry = RasEntry.CreateBroadbandEntry(updatePPPOEname, device)
            entry.PhoneNumber = " "
            allUsersPhoneBook.Entries.Add(entry)
        End If
    End Sub



    Function CheckPPPoEConnectionStatus(ByVal pppoeName As String) As Boolean
        Dim conList As ReadOnlyCollection(Of RasConnection) = RasConnection.GetActiveConnections()

        For Each con As RasConnection In conList
            If pppoeName = con.EntryName Then
                If con.GetConnectionStatus.ConnectionState.ToString = "Connected" Then
                    Return True
                Else
                    Return False
                End If
            End If

        Next
    End Function


End Module
