Imports System.Net
Imports System.Text
Imports System.Net.Http

Module ModuleMain
	Dim isps() As String = My.Settings.isp.Split(CChar(","))
	Dim domains() As String = My.Settings.domains.Split(CChar(","))

	Sub Main()
		Console.WriteLine(Process() & "动态域名 IP 地址更新开始" & vbCrLf)
		Dim tokens() As String = My.Settings.tokens.Split(CChar(","))
		Dim IPv6() As String = GetIP(Dns.GetHostName)
		Dim IPv4(tokens.Length - 1) As String
		Dim used(domains.Length - 1) As String
		Dim client As New HttpClient()
		Dim response As String = client.GetStringAsync("https://4.ipw.cn").GetAwaiter().GetResult()
		Console.WriteLine()
		For i As Integer = 0 To domains.Length - 1
			'处理每个域名
			For j As Integer = 0 To IPv6.Length - 1
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
			IPv4(i) = GetIP(domains(i), 0, 4)(0)
			If response.Length > 0 Then
				UpdateIP(domains(i), tokens(i), response, IPv4(i), 4)
			End If
		Next
		Console.WriteLine($"{vbCrLf}{Process()}本轮更新结束。")
	End Sub

	Function Process() As String
		Return "[" & Now.ToString("yyyy年MM月dd日 HH:mm:ss") & "] "
	End Function

	Function GetIP(ByVal host As String, Optional ByVal l As Integer = 0, Optional ByVal v As Byte = 6) As String()
		Dim ipAddr() As IPAddress = Dns.GetHostAddresses(host)
		If l = 0 Then
			l = ipAddr.Length - 1
		End If
		Dim ipRecode(l) As String
		Dim ip As IPAddress
		Dim i As Integer = 0
		Dim flag As Boolean
		Dim isp As String
		If Dns.GetHostName = host Then
			Console.WriteLine(Process() & "本机的 IP 地址有：")
		End If
		' 遍历 ipAddress 中的所有 ip 地址
		For Each ip In ipAddr
			' 如果主机名为本机名
			If Dns.GetHostName = host Then
				' 显示当前 ip 地址
				Console.WriteLine(ip.ToString)
			End If
			flag = False
			If v = 6 Then
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
			ElseIf ip.AddressFamily = Sockets.AddressFamily.InterNetwork Then
				flag = True
			End If
			If flag AndAlso i <= l Then
				' 把属于 isp 的 ip 地址存在 ipRecode 数组里
				ipRecode(i) = ip.ToString.ToLower
				i += 1
			End If
		Next
		Return ipRecode
	End Function

	Sub UpdateIP(ByVal domain As String, ByVal token As String, ByVal newip As String, ByVal oldIP As String, Optional ByVal v As Byte = 6)
		If newip = oldIP Then
			' 如果新老 IPv6 地址一致，就显示没变化，不执行
			Console.WriteLine($"{Process()}域名 {domain} 的 IPv{v} 地址没有变化，本次更新不执行。")
		Else
			Console.WriteLine($"{Process()}正在更新域名 {domain} 的 IPv{v} 地址为：{newip}")
			Dim postData As String
			If v = 6 Then
				postData = String.Format(My.Settings.param, {domain, token, newip, newip.Substring(0, newip.LastIndexOf(":") + 1)})
			Else
				postData = String.Format(My.Settings.param4, {domain, token, newip})
			End If
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
