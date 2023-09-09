# PPPOE Auto Connecter

## 简介

因为懒，所以写了这玩意

适用于插网线PPPoE拨号上网的情况（一般是校园网）

可配置开机自动拨号，无须手动连接

## 设置打钱网站的方法

在配置文件Configuration.ini中添加：（一定要加http/https协议头）

 <pre>[Url]
Manage = http://网站`</pre> 

## To Do List

- [ ] 高优先级的开机自启动（目前开机后过一会才会启动）

## 引用的开源库

UI库：WPF-UI https://github.com/lepoco/wpfui

PPPoE库：DotRas https://github.com/DotRas/DotRas

配置文件读写库：iniPaser https://github.com/rickyah/ini-parser