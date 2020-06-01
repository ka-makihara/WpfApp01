#-*- coding:utf-8 -*-

import sys
import os

import ftpUtil

#
# ftoTool == C# exe
#

def update_step_1(host):
	'''
	'''
	ftp = ftpUtil.connectFTP(host)
	#ftpUtil.rename(ftp,'Fuji','Fuji_app')
	ftpUtil.mkdir(ftp,'Fuji')
	ftpUtil.upload(ftp,"C:\\Users\\ka.makihara\\Desktop\\update_test\\Fuji",'Fuji')

	#ftpTool.ConsoleOut('step_1')
	#ftpTool.ConsoleOut(host)
	ftp.close()
	pass

def update_step_2(host):
	ftpTool.ConsoleOut('step_2')
	pass

def update_step_3(host):
	ftpTool.ConsoleOut('step_3')
	pass
