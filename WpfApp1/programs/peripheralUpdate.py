#-*- coding:utf-8 -*-

import sys
import os

import ftpUtil

#
# ftoTool == C# exe
# ftpCtrl == C# FtpClientCtrl class
#

def update_step_1(host):
	'''
	'''
	ftpCtrl.ConnectFTP(host,'makihara','wildgeese')
	ftpCtrl.Rename('Fuji','Fuji_app')
	ftpCtrl.MakeDirectory('Fuji')
	ftpCtrl.UploadDirectory("C:\\Users\\makih\\source\\repos\\update_test\\Fuji",'Fuji')
	ftpCtrl.RemoveDirectory('Fuji')

	#ftpTool.ConsoleOut('step_1')
	#ftpTool.ConsoleOut(host)
	ftpCtrl.Close()
	pass

def update_step_2(host):
	ftpTool.ConsoleOut('step_2')
	pass

def update_step_3(host):
	ftpTool.ConsoleOut('step_3')
	pass
