# ddns<br>
使用本软件可以在 Windows 系统中更新一个或多个动态域名的公网 IPv6 地址。<br>
有多条公网线路的情况，内网主机可以获取多个公网 IPv6 地址，本软件可以把多个动态域名，按先后顺序绑定该主机的 IPv6 地址。<br>
动态域名地址可以少于等于该主机可以获取到的公网 IPv6 地址，若动态域名个数多于公网 IPv6 地址个数，多出的域名地址则无法绑定公网 IPv6 地址。


2024-05-28更新说明
--
加入了支持 IPv6 前缀，默认为 IPv6 地址的前 64 位部分。

--

2023-06-20更新说明
--
优化了部分代码。

已经增加了域名与 ISP 对应的匹配功能。你可以在 ddns.exe.config 设置文件中进行如下配置：
1. 在设置中的 "domains" 字段填入两个域名，例：example1.dynv6.net,example2.dynv6.net。
2. 在设置中的 "isp" 字段填入两个前缀，例：2401,2402。
配置完成后，当获取到本机的IPv6地址时，example1.dynv6.net 将会使用以 2401 为前缀的IPv6地址，而example2.dynv6.net 将会使用以 2402 为前缀的IPv6地址。
当然，你也可以为多个地址分配相同的ISP前缀。
同时增加了可选使用后缀功能，可以为域名指定本机后缀，记得把“suffix_switch”设置为“true”。

**ddns.exe.config 文件配置示例：**
![image](https://github.com/Sanhom365/ddns/assets/58111416/61939bbb-858e-45c3-a8a9-ee3f1e85c54a)
