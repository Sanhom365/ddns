Imports System.Net
Imports System.Text

Module ModuleMain

	Sub Main()
		Console.WriteLine(Process() & "动态域名 IP 地址更新开始" & vbCrLf)
		Dim domains() As String = My.Settings.domains.Split(",")
		Dim tokens() As String = My.Settings.tokens.Split(",")
		Dim IPv6() As String = GetIP(Dns.GetHostName, domains.Length - 1)
		Console.WriteLine()
		For i As Byte = 0 To domains.Length - 1
			Try
				If IPv6(i).Length > 0 Then
					Dim IPv6old() As String = GetIP(domains(i), 2)
					If IPv6(i) = IPv6old(0) Then
						Console.WriteLine(Process() & "域名 " & domains(i) & " 的 IPv6 地址没有变化，本次更新不执行。")
					Else
						Console.WriteLine(Process() & "正在更新域名 " & domains(i) & " 的 IPv6 地址为：" & IPv6(i))
						UpdateIP(String.Format(My.Settings.param, {domains(i), tokens(i), IPv6(i), IPv6(i)}))
					End If
				End If
			Catch ex As Exception
				Console.WriteLine(Process() & "没有可以与域名 " & domains(i) & " 对应的 IP 地址，放弃更新。")
			End Try
		Next
		Console.WriteLine(vbCrLf & Process() & "本轮更新结束。")
	End Sub

	Function Process() As String
		Return "[" & Now.ToString("yyyy年MM月dd日 HH:mm:ss") & "] "
	End Function

	Function GetIP(host As String, l As Byte) As String()
		Dim ipAddress() As IPAddress = Dns.GetHostAddresses(host)
		Dim IPv6(l) As String
		Dim ip As IPAddress
		Dim i As Byte = 0
		Dim flag As Boolean
		Dim isps() As String = My.Settings.isp.Split(",")
		Dim isp As String
		If Dns.GetHostName = host Then
			Console.WriteLine(Process() & "本机的 IP 地址有：")
		End If
		For Each ip In ipAddress
			If Dns.GetHostName = host Then
				Console.WriteLine(ip.ToString)
			End If
			flag = False
			For Each isp In isps
				If Mid(ip.ToString, 1, 4).ToLower = isp Then
					flag = True
					Exit For
				End If
			Next
			If flag Then
				IPv6(i) = ip.ToString
				i += 1
			End If
		Next
		Return IPv6
	End Function

	Sub UpdateIP(postData As String)
		Dim http As New WebClient
		http.Headers(HttpRequestHeader.ContentType) = "application/x-www-form-urlencoded"
		http.Encoding = Encoding.UTF8
		Dim content As String = RegularExpressions.Regex.Unescape(http.UploadString(My.Settings.URL & postData, ""))
		If content = "addresses updated" Then
			Console.WriteLine(Process() & "更新成功！")
		Else
			Console.WriteLine(Process() & "更新失败。")
		End If
	End Sub
End Module
