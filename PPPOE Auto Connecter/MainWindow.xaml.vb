Imports System.IO
Imports DotRas
Imports System.Net
Imports System.Collections.ObjectModel
Imports System.Windows.Threading
Imports Microsoft.Win32

Class MainWindow
    Private Sub Window_Initialized(sender As Object, e As EventArgs)
        If Not File.Exists(AppContext.BaseDirectory & "\Configuration.ini") Then '检查配置文件是否存在
            File.Create(AppContext.BaseDirectory & "\Configuration.ini")
        End If
        '读取配置
        If Config_Reader("Setting")("Launch") = "True" Then
            set_launch.IsChecked = True
        Else
            set_launch.IsChecked = False
        End If
        If Config_Reader("Setting")("AutoLaunch") = "True" Then
            set_autolaunch.IsChecked = True
            If Not Config_Reader("PPPOE")("Name") = "" Then
                '创建连接
                Dim connectionName As String = Config_Reader("PPPOE")("Name") & " - By PAC"
                Dim username As String = Config_Reader("PPPOE")("Account")
                Dim password As String = Config_Reader("PPPOE")("Pwd")
                Disconnect()
                If Connect(connectionName, username, password, "1") = True Then
                    'MsgBox("拨号成功！")
                    If CheckPPPoEConnectionStatus(connectionName) = True Then
                        text_status.Text = "拨号已连接！" & DateTime.Now
                    Else
                        text_status.Text = "拨号连接失败或还未连接！"
                    End If
                End If
            End If


        Else
            set_autolaunch.IsChecked = False
        End If
        If Not Config_Reader("PPPOE")("Name") = "" Then
            pppoe_name.Text = Config_Reader("PPPOE")("Name")
        End If
        pppoe_account.Text = Config_Reader("PPPOE")("Account")
        pppoe_pwd.Text = Config_Reader("PPPOE")("Pwd")

        '检查
        Dim DispatcherTimer = New Threading.DispatcherTimer()
        AddHandler DispatcherTimer.Tick, AddressOf dispatcherTimer_Tick
        DispatcherTimer.Interval = New TimeSpan(0, 0, 1)
        DispatcherTimer.Start()
    End Sub

    Private Sub dispatcherTimer_Tick(sender As Object, e As EventArgs)
        If CheckPPPoEConnectionStatus(Config_Reader("PPPOE")("Name") & " - By PAC") = True Then
            text_status.Text = "拨号已连接！" & DateTime.Now
        Else
            text_status.Text = "拨号连接失败！"
        End If

    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Try '保存数据
            Config_Writer("PPPOE", "Name", pppoe_name.Text)
            Config_Writer("PPPOE", "Account", pppoe_account.Text)
            Config_Writer("PPPOE", "Pwd", pppoe_pwd.Password)
            'MsgBox("保存成功！", 64, "保存")
        Catch ex As Exception
            MsgBox("保存失败！", 64, "保存")
        End Try

        '创建连接
        Dim connectionName As String = Config_Reader("PPPOE")("Name") & " - By PAC"
        Dim username As String = Config_Reader("PPPOE")("Account")
        Dim password As String = Config_Reader("PPPOE")("Pwd")
        Disconnect()
        If Connect(connectionName, username, password, "1") = True Then
            'MsgBox("拨号成功！")
            If CheckPPPoEConnectionStatus(connectionName) = True Then
                text_status.Text = "拨号已连接！" & DateTime.Now
            Else
                text_status.Text = "拨号连接失败！"
            End If
        End If


    End Sub

    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        Try '保存数据
            Config_Writer("PPPOE", "Name", pppoe_name.Text)
            Config_Writer("PPPOE", "Account", pppoe_account.Text)
            Config_Writer("PPPOE", "Pwd", pppoe_pwd.Password)
            MsgBox("保存成功！", 64, "保存")
        Catch ex As Exception
            MsgBox("保存失败！", 64, "保存")
        End Try

    End Sub

    Private Sub set_launch_Click(sender As Object, e As RoutedEventArgs) Handles set_launch.Click
        If set_launch.IsChecked = True Then
            Config_Writer("Setting", "Launch", "True")
            Dim runKey As RegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", True)

            If runKey.GetValue(Application.ResourceAssembly.GetName().Name) Is Nothing Then
                ' 设置开机自启动
                runKey.SetValue(Application.ResourceAssembly.GetName().Name, System.Reflection.Assembly.GetExecutingAssembly().Location)
            End If
        Else
            Config_Writer("Setting", "Launch", "False")
            Dim runKey As RegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", True)

            If runKey.GetValue(Application.ResourceAssembly.GetName().Name) IsNot Nothing Then
                ' 关闭开机自启动
                runKey.DeleteValue(Application.ResourceAssembly.GetName().Name, False)
            End If
        End If
    End Sub

    Private Sub set_autolaunch_Click(sender As Object, e As RoutedEventArgs) Handles set_autolaunch.Click
        If set_autolaunch.IsChecked = True Then
            Config_Writer("Setting", "AutoLaunch", "True")
        Else
            Config_Writer("Setting", "AutoLaunch", "False")
        End If
    End Sub

    Private Sub Button_Click_2(sender As Object, e As RoutedEventArgs)
        ' 注册表路径
        Dim registryPath As String = "SYSTEM\CurrentControlSet\Services\NlaSvc\Parameters\Internet"

        ' 键名
        Dim keyName As String = "EnableActiveProbing"

        Try
            ' 打开注册表项
            Using key As RegistryKey = Registry.LocalMachine.OpenSubKey(registryPath, True)
                If key IsNot Nothing Then
                    ' 设置键值
                    key.SetValue(keyName, 0)
                    MsgBox("设置成功！请重启电脑！", 64, "修复断连")
                Else
                    ' 注册表路径不存在，处理异常情况
                    MsgBox("设置失败，注册表路径不存在，请尝试手动操作！", 16, "修复断连")
                End If
            End Using

        Catch ex As Exception
            ' 处理异常情况
            MsgBox("设置失败，请尝试手动操作！", 16, "修复断连")
        End Try
    End Sub

    Private Sub Button_Click_3(sender As Object, e As RoutedEventArgs)
        If Not Config_Reader("Url")("Manage") = "" Then
            Dim url As String = Config_Reader("Url")("Manage")
            Process.Start(New ProcessStartInfo With {
            .FileName = url,
            .UseShellExecute = True
        })
        Else
            MsgBox("未配置给钱的地方！")
        End If

    End Sub

    Private Sub Button_Click_4(sender As Object, e As RoutedEventArgs)
        Dim url As String = ""
        Process.Start(New ProcessStartInfo With {
        .FileName = url,
        .UseShellExecute = True
    })
    End Sub
End Class
