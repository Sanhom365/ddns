Imports System.Net
Imports System.Text

Module ModuleMain
	Dim isps() As String = My.Settings.isp.Split(",")

	Sub Main()
		Console.WriteLine(Process() & "动态域名 IP 地址更新开始" & vbCrLf)
		Dim domains() As String = My.Settings.domains.Split(",")
		Dim tokens() As String = My.Settings.tokens.Split(",")
		Dim IPv6() As String = GetIP(Dns.GetHostName)
		Dim used(domains.Length - 1) As String
		Console.WriteLine()
		For i As Byte = 0 To domains.Length - 1
			'处理每个域名
			For j As Byte = 0 To IPv6.Length - 1
				' 如果第i个 IP 地址字符串存在
				If IPv6(j) IsNot Nothing AndAlso Not Array.Exists(used, Function(str) str = IPv6(j)) Then
					' 定义更新前的 IP 地址字符串数组，并获取某个域名的 IPv6 地址
					Dim IPv6old() As String = GetIP(domains(i), 2)
					If isps.Length < i Then
						If IPv6(j).StartsWith(isps(i)) Then
							UpdateIP(domains(i), tokens(i), IPv6(j), IPv6old(0))
							used(i) = IPv6(j)
							Exit For
						End If
					Else
						UpdateIP(domains(i), tokens(i), IPv6(j), IPv6old(0))
						used(i) = IPv6(j)
						Exit For
					End If
				End If
			Next
		Next
		Console.WriteLine(vbCrLf & Process() & "本轮更新结束。")
	End Sub

	Function Process() As String
		Return "[" & Now.ToString("yyyy年MM月dd日 HH:mm:ss") & "] "
	End Function

	Function GetIP(host As String, Optional l As Byte = 0) As String()
		Dim ipAddress() As IPAddress = Dns.GetHostAddresses(host)
		If l = 0 Then
			l = CByte(ipAddress.Length - 1)
		End If
		Dim IPv6(l) As String
		Dim ip As IPAddress
		Dim i As Byte = 0
		Dim flag As Boolean
		Dim isp As String
		If Dns.GetHostName = host Then
			Console.WriteLine(Process() & "本机的 IP 地址有：")
		End If
		' 遍历 ipAddress 中的所有 ip 地址
		For Each ip In ipAddress
			' 如果主机名为本机名
			If Dns.GetHostName = host Then
				' 显示当前 ip 地址
				Console.WriteLine(ip.ToString)
			End If
			flag = False
			' 遍历所有的 isp
			For Each isp In isps
				' 如果 ip 地址属于 ips，把标记设为真
				If ip.ToString.ToLower.StartsWith(isp.ToLower) Then
					If My.Settings.suffix_switch Then
						'判断后缀是否与预设的一致，是的才返回
						If ip.ToString.ToLower.EndsWith(My.Settings.suffix) Then
							flag = True
							Exit For
						End If
					Else
						flag = True
						Exit For
					End If
				End If
			Next
			If flag AndAlso i <= l Then
				' 把属于 isp 的 ip 地址存在 IPv6 数组里
				IPv6(i) = ip.ToString.ToLower
				i += 1
			End If
		Next
		Return IPv6
	End Function

	Sub UpdateIP(domain As String, token As String, newip As String, oldIP As String)
		If newip = oldIP Then
			' 如果新老 IPv6 地址一致，就显示没变化，不执行
			Console.WriteLine(Process() & "域名 " & domain & " 的 IPv6 地址没有变化，本次更新不执行。")
		Else
			Console.WriteLine(Process() & "正在更新域名 " & domain & " 的 IPv6 地址为：" & newip)
			Dim postData As String = String.Format(My.Settings.param, {domain, token, newip, newip.Substring(0, newip.LastIndexOf(":") + 1)})
			Try
				Dim http As New WebClient
				http.Headers(HttpRequestHeader.ContentType) = "application/x-www-form-urlencoded"
				http.Encoding = Encoding.UTF8
				Dim content As String = RegularExpressions.Regex.Unescape(http.UploadString(My.Settings.URL & postData, ""))
				If content = "addresses updated" Then
					Console.WriteLine(Process() & "更新成功！")
				Else
					Console.WriteLine(Process() & "更新失败。")
				End If
			Catch ex As Exception
				' 处理错误响应
			End Try
		End If
	End Sub
End Module
