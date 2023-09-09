Imports IniParser
Imports System.Reflection
Imports IniParser.Model

Module ConfigModule
    Function Config_Reader() As IniData
        Dim parser = New FileIniDataParser()
        Dim data As IniData = parser.ReadFile(AppContext.BaseDirectory & "\Configuration.ini")
        Return data
    End Function
    Function Config_Writer(group As String, title As String, value As String)
        Dim parser = New FileIniDataParser()
        Dim data As IniData = parser.ReadFile(AppContext.BaseDirectory & "\Configuration.ini")
        data(group)(title) = value
        parser.WriteFile(AppContext.BaseDirectory & "\Configuration.ini", data)
    End Function
End Module
